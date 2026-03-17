using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using GPVBlazor.Models;
using GPVBlazor.Services.Interfaces;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace GPVBlazor.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IGitHubAuthSessionStore _sessionStore;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly AuthSecurityOptions _securityOptions;

        public string? CurrentAccessToken { get; private set; }
        public string? CurrentAuthSource { get; private set; }
        public User? CurrentUser { get; private set; }
        public event Action? OnAuthStateChanged;

        private string? ClientId => _configuration["ClientId"] ?? _configuration["GitHub:ClientId"];
        private string? ClientSecret => _configuration["ClientSecret"] ?? _configuration["GitHub:ClientSecret"];
        private string? RedirectUri => _configuration["RedirectUri"] ?? _configuration["GitHub:RedirectUri"];

        public AuthService(
            HttpClient httpClient,
            IConfiguration configuration,
            IGitHubAuthSessionStore sessionStore,
            AuthenticationStateProvider authenticationStateProvider,
            IOptions<AuthSecurityOptions> securityOptions)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _sessionStore = sessionStore;
            _authenticationStateProvider = authenticationStateProvider;
            _securityOptions = securityOptions.Value;
        }

        public string GetGitHubLoginUrl(HttpContext httpContext)
        {
            var scopes = "public_repo,read:user,user:email,gist";
            var redirectUri = GetOAuthCallbackUri(httpContext);
            var state = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));

            httpContext.Response.Cookies.Append(
                _securityOptions.OAuthStateCookieName,
                state,
                new CookieOptions
                {
                    HttpOnly = true,
                    IsEssential = true,
                    MaxAge = TimeSpan.FromMinutes(_securityOptions.OAuthStateTtlMinutes),
                    SameSite = _securityOptions.OAuthStateCookieSameSite,
                    Secure = ShouldUseSecureCookies(_securityOptions.OAuthStateCookieSecurePolicy, httpContext),
                }
            );

            return $"https://github.com/login/oauth/authorize?client_id={ClientId}&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope={Uri.EscapeDataString(scopes)}&state={Uri.EscapeDataString(state)}";
        }

        public async Task<bool> InitializeFromSessionAsync(CancellationToken cancellationToken = default)
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var session = await ResolveSessionAsync(authState.User, cancellationToken);
            if (session == null)
            {
                ClearLocalState();
                return false;
            }

            var currentUser = await FetchCurrentUserAsync(session.AccessToken, cancellationToken);

            CurrentAccessToken = session.AccessToken;
            CurrentAuthSource = session.AuthSource;
            CurrentUser = currentUser ?? BuildCachedUser(session);
            OnAuthStateChanged?.Invoke();
            return true;
        }

        public async Task<string?> GetActiveAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return await GetActiveAccessTokenAsync(authState.User, cancellationToken);
        }

        public async Task<string?> GetActiveAccessTokenAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
        {
            var session = await ResolveSessionAsync(principal, cancellationToken);
            return session?.AccessToken;
        }

        public async Task<User?> GetCurrentUserAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
        {
            var session = await ResolveSessionAsync(principal, cancellationToken);
            if (session == null)
            {
                ClearLocalState();
                return null;
            }

            var currentUser = await FetchCurrentUserAsync(session.AccessToken, cancellationToken);
            if (currentUser != null)
            {
                session.UserLogin = currentUser.Login;
                session.UserAvatarUrl = currentUser.AvatarUrl;
                await _sessionStore.SetAsync(session, cancellationToken);
            }

            CurrentAccessToken = session.AccessToken;
            CurrentAuthSource = session.AuthSource;
            CurrentUser = currentUser ?? BuildCachedUser(session);
            return CurrentUser;
        }

        public async Task<User?> FetchCurrentUserAsync(string token, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
            request.Headers.Add("User-Agent", "BlazorApp");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            return await JsonSerializer.DeserializeAsync<User>(stream, cancellationToken: cancellationToken);
        }

        public async Task<bool> IsTokenValidAsync(string token, CancellationToken cancellationToken = default)
        {
            var rateReq = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/rate_limit");
            rateReq.Headers.Add("User-Agent", "BlazorApp");
            rateReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(rateReq, cancellationToken);
            return response.IsSuccessStatusCode;
        }

        public async Task<RateLimitInfo?> GetRateLimitAsync(string? token = null, ClaimsPrincipal? principal = null, CancellationToken cancellationToken = default)
        {
            token ??= principal == null
                ? await GetActiveAccessTokenAsync(cancellationToken)
                : await GetActiveAccessTokenAsync(principal, cancellationToken);

            var rateReq = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/rate_limit");
            rateReq.Headers.Add("User-Agent", "BlazorApp");
            if (!string.IsNullOrWhiteSpace(token))
            {
                rateReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.SendAsync(rateReq, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var root = await JsonSerializer.DeserializeAsync<RateRoot>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                cancellationToken
            );

            if (root?.Resources is null)
            {
                return null;
            }

            return new RateLimitInfo
            {
                Resources = new RateLimitResources
                {
                    Core = new RateResource
                    {
                        Limit = root.Resources.Core.Limit,
                        Remaining = root.Resources.Core.Remaining,
                        Reset = root.Resources.Core.Reset,
                    },
                    Search = new RateResource
                    {
                        Limit = root.Resources.Search.Limit,
                        Remaining = root.Resources.Search.Remaining,
                        Reset = root.Resources.Search.Reset,
                    },
                },
            };
        }

        public async Task<AuthTokenResponse?> GetAccessTokenFromCodeAsync(string code, string redirectUri, CancellationToken cancellationToken = default)
        {
            var tokenReq = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
            tokenReq.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            tokenReq.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "client_id", ClientId ?? string.Empty },
                    { "client_secret", ClientSecret ?? string.Empty },
                    { "code", code },
                    { "redirect_uri", redirectUri },
                }
            );

            try
            {
                var response = await _httpClient.SendAsync(tokenReq, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                return await JsonSerializer.DeserializeAsync<AuthTokenResponse>(stream, cancellationToken: cancellationToken);
            }
            catch
            {
                return null;
            }
        }

        public async Task<AuthTokenResponse?> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var tokenReq = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
            tokenReq.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            tokenReq.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "client_id", ClientId ?? string.Empty },
                    { "client_secret", ClientSecret ?? string.Empty },
                    { "grant_type", "refresh_token" },
                    { "refresh_token", refreshToken },
                }
            );

            try
            {
                var response = await _httpClient.SendAsync(tokenReq, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                return await JsonSerializer.DeserializeAsync<AuthTokenResponse>(stream, cancellationToken: cancellationToken);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> SignInWithGitHubCodeAsync(HttpContext httpContext, string code, string? state, CancellationToken cancellationToken = default)
        {
            if (!ValidateOAuthState(httpContext, state))
            {
                return false;
            }

            var redirectUri = GetOAuthCallbackUri(httpContext);
            var tokenResponse = await GetAccessTokenFromCodeAsync(code, redirectUri, cancellationToken);
            if (tokenResponse == null || string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
            {
                return false;
            }

            var session = await CreateSessionAsync(tokenResponse, "github", cancellationToken);
            if (session == null)
            {
                return false;
            }

            await SignInAsync(httpContext, session, cancellationToken);
            return true;
        }

        public async Task<bool> SignInWithPersonalAccessTokenAsync(HttpContext httpContext, string token, string authSource = "token", CancellationToken cancellationToken = default)
        {
            if (!await IsTokenValidAsync(token, cancellationToken))
            {
                return false;
            }

            var user = await FetchCurrentUserAsync(token, cancellationToken);
            if (user == null)
            {
                return false;
            }

            var session = new GitHubAuthSession
            {
                SessionId = Guid.NewGuid().ToString("N"),
                AccessToken = token,
                AuthSource = authSource,
                UserLogin = user.Login,
                UserAvatarUrl = user.AvatarUrl,
            };

            await _sessionStore.SetAsync(session, cancellationToken);
            await SignInAsync(httpContext, session, cancellationToken);
            return true;
        }

        public async Task SignOutAsync(HttpContext httpContext, ClaimsPrincipal principal, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var sessionId = principal.FindFirstValue(GitHubAuthenticationDefaults.SessionIdClaimType);
            if (!string.IsNullOrWhiteSpace(sessionId))
            {
                await _sessionStore.RemoveAsync(sessionId, cancellationToken);
            }

            await httpContext.SignOutAsync(GitHubAuthenticationDefaults.AuthenticationScheme);
            ClearLocalState();
        }

        private bool ValidateOAuthState(HttpContext httpContext, string? state)
        {
            var expectedState = httpContext.Request.Cookies[_securityOptions.OAuthStateCookieName];

            httpContext.Response.Cookies.Delete(
                _securityOptions.OAuthStateCookieName,
                new CookieOptions
                {
                    HttpOnly = true,
                    IsEssential = true,
                    SameSite = _securityOptions.OAuthStateCookieSameSite,
                    Secure = ShouldUseSecureCookies(_securityOptions.OAuthStateCookieSecurePolicy, httpContext),
                }
            );

            if (string.IsNullOrWhiteSpace(state) || string.IsNullOrWhiteSpace(expectedState))
            {
                return false;
            }

            var providedBytes = Encoding.UTF8.GetBytes(state);
            var expectedBytes = Encoding.UTF8.GetBytes(expectedState);

            return providedBytes.Length == expectedBytes.Length
                && CryptographicOperations.FixedTimeEquals(providedBytes, expectedBytes);
        }

        private async Task<GitHubAuthSession?> ResolveSessionAsync(ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            if (principal?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var sessionId = principal.FindFirstValue(GitHubAuthenticationDefaults.SessionIdClaimType);
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return null;
            }

            var session = await _sessionStore.GetAsync(sessionId, cancellationToken);
            if (session == null)
            {
                return null;
            }

            if (session.AccessTokenExpiresAt.HasValue && session.AccessTokenExpiresAt <= DateTimeOffset.UtcNow.AddMinutes(5))
            {
                if (string.IsNullOrWhiteSpace(session.RefreshToken) || (session.RefreshTokenExpiresAt.HasValue && session.RefreshTokenExpiresAt <= DateTimeOffset.UtcNow))
                {
                    await _sessionStore.RemoveAsync(sessionId, cancellationToken);
                    return null;
                }

                var refreshed = await RefreshAccessTokenAsync(session.RefreshToken, cancellationToken);
                if (refreshed == null || string.IsNullOrWhiteSpace(refreshed.AccessToken))
                {
                    await _sessionStore.RemoveAsync(sessionId, cancellationToken);
                    return null;
                }

                session.AccessToken = refreshed.AccessToken;
                session.AccessTokenExpiresAt = refreshed.ExpiresIn > 0
                    ? DateTimeOffset.UtcNow.AddSeconds(refreshed.ExpiresIn)
                    : session.AccessTokenExpiresAt;

                if (!string.IsNullOrWhiteSpace(refreshed.RefreshToken))
                {
                    session.RefreshToken = refreshed.RefreshToken;
                }

                session.RefreshTokenExpiresAt = refreshed.RefreshTokenExpiresIn > 0
                    ? DateTimeOffset.UtcNow.AddSeconds(refreshed.RefreshTokenExpiresIn)
                    : session.RefreshTokenExpiresAt;

                await _sessionStore.SetAsync(session, cancellationToken);
            }

            return session;
        }

        private async Task<GitHubAuthSession?> CreateSessionAsync(AuthTokenResponse tokenResponse, string authSource, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
            {
                return null;
            }

            var currentUser = await FetchCurrentUserAsync(tokenResponse.AccessToken, cancellationToken);
            if (currentUser == null)
            {
                return null;
            }

            var session = new GitHubAuthSession
            {
                SessionId = Guid.NewGuid().ToString("N"),
                AccessToken = tokenResponse.AccessToken,
                AccessTokenExpiresAt = tokenResponse.ExpiresIn > 0
                    ? DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
                    : null,
                RefreshToken = tokenResponse.RefreshToken,
                RefreshTokenExpiresAt = tokenResponse.RefreshTokenExpiresIn > 0
                    ? DateTimeOffset.UtcNow.AddSeconds(tokenResponse.RefreshTokenExpiresIn)
                    : null,
                AuthSource = authSource,
                UserLogin = currentUser.Login,
                UserAvatarUrl = currentUser.AvatarUrl,
            };

            await _sessionStore.SetAsync(session, cancellationToken);
            return session;
        }

        private async Task SignInAsync(HttpContext httpContext, GitHubAuthSession session, CancellationToken cancellationToken)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, session.UserLogin ?? session.SessionId),
                new(GitHubAuthenticationDefaults.SessionIdClaimType, session.SessionId),
                new(GitHubAuthenticationDefaults.AuthSourceClaimType, session.AuthSource),
            };

            if (!string.IsNullOrWhiteSpace(session.UserLogin))
            {
                claims.Add(new Claim(ClaimTypes.Name, session.UserLogin));
            }

            var principal = new ClaimsPrincipal(
                new ClaimsIdentity(claims, GitHubAuthenticationDefaults.AuthenticationScheme)
            );

            var expiresAt = session.RefreshTokenExpiresAt
                ?? session.AccessTokenExpiresAt
                ?? DateTimeOffset.UtcNow.AddDays(7);

            await httpContext.SignInAsync(
                GitHubAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                    ExpiresUtc = expiresAt,
                }
            );

            cancellationToken.ThrowIfCancellationRequested();
        }

        private string GetOAuthCallbackUri(HttpContext? httpContext = null)
        {
            if (httpContext != null)
            {
                var request = httpContext.Request;
                var origin = $"{request.Scheme}://{request.Host.ToUriComponent()}{request.PathBase.ToUriComponent()}";
                return BuildCallbackUri(origin);
            }

            return BuildCallbackUri(RedirectUri ?? string.Empty);
        }

        private static string BuildCallbackUri(string baseUri)
        {
            if (baseUri.Contains("/api/auth/github-callback", StringComparison.OrdinalIgnoreCase))
            {
                return baseUri;
            }

            if (baseUri.Contains("/github-callback", StringComparison.OrdinalIgnoreCase))
            {
                return baseUri.Replace(
                    "/github-callback",
                    "/api/auth/github-callback",
                    StringComparison.OrdinalIgnoreCase
                );
            }

            if (baseUri.EndsWith("/", StringComparison.Ordinal))
            {
                return $"{baseUri}api/auth/github-callback";
            }

            return $"{baseUri}/api/auth/github-callback";
        }

        private static bool ShouldUseSecureCookies(CookieSecurePolicy policy, HttpContext httpContext)
        {
            return policy switch
            {
                CookieSecurePolicy.Always => true,
                CookieSecurePolicy.None => false,
                _ => httpContext.Request.IsHttps,
            };
        }

        private User? BuildCachedUser(GitHubAuthSession session)
        {
            if (string.IsNullOrWhiteSpace(session.UserLogin) && string.IsNullOrWhiteSpace(session.UserAvatarUrl))
            {
                return null;
            }

            return new User
            {
                Login = session.UserLogin,
                AvatarUrl = session.UserAvatarUrl,
            };
        }

        private void ClearLocalState()
        {
            CurrentAccessToken = null;
            CurrentAuthSource = null;
            CurrentUser = null;
            OnAuthStateChanged?.Invoke();
        }

        private class RateRoot
        {
            public RateResources? Resources { get; set; }
        }

        private class RateResources
        {
            public RateResourceInfo Core { get; set; } = new();
            public RateResourceInfo Search { get; set; } = new();
        }

        private class RateResourceInfo
        {
            public int Limit { get; set; }
            public int Remaining { get; set; }
            public long Reset { get; set; }
        }
    }
}
