using System.Net.Http.Headers;
using System.Text.Json;
using GPVBlazor.Services.Interfaces;

namespace GPVBlazor.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public string? CurrentAccessToken { get; private set; }
        public Models.User? CurrentUser { get; private set; }
        public event Action? OnAuthStateChanged;

        private string? ClientId => _configuration["ClientId"] ?? _configuration["GitHub:ClientId"];
        private string? ClientSecret => _configuration["ClientSecret"] ?? _configuration["GitHub:ClientSecret"];
        private string? RedirectUri => _configuration["RedirectUri"] ?? _configuration["GitHub:RedirectUri"];

        public AuthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public void Login(string token)
        {
            CurrentAccessToken = token;
            OnAuthStateChanged?.Invoke();
            _ = FetchCurrentUserAsync(token);
        }

        public void Logout()
        {
            CurrentAccessToken = null;
            CurrentUser = null;
            OnAuthStateChanged?.Invoke();
        }

        public string GetGitHubLoginUrl()
        {
            // Scopes: public_repo, read:user, user:email, gist as suggested in the modal
            var scopes = "public_repo,read:user,user:email,gist";

            return $"https://github.com/login/oauth/authorize?client_id={ClientId}&redirect_uri={RedirectUri}&scope={scopes}";
        }

        public async Task<Models.User?> FetchCurrentUserAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
            request.Headers.Add("User-Agent", "BlazorApp");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<Models.User>(content);
                if (user != null)
                {
                    CurrentUser = user;
                    OnAuthStateChanged?.Invoke();
                    return user;
                }
            }
            return null;
        }

        public async Task<Models.AuthTokenResponse?> GetAccessTokenFromCodeAsync(string code)
        {
            var tokenReq = new HttpRequestMessage(
                HttpMethod.Post,
                "https://github.com/login/oauth/access_token"
            );
            tokenReq.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var parameters = new Dictionary<string, string>
            {
                { "client_id", ClientId ?? "" },
                { "client_secret", ClientSecret ?? "" },
                { "code", code },
                { "redirect_uri", RedirectUri ?? "" },
            };

            tokenReq.Content = new FormUrlEncodedContent(parameters);

            try
            {
                var response = await _httpClient.SendAsync(tokenReq);
                if (!response.IsSuccessStatusCode)
                    return null;

                using var stream = await response.Content.ReadAsStreamAsync();
                var tokenResponse = await JsonSerializer.DeserializeAsync<Models.AuthTokenResponse>(
                    stream
                );

                return tokenResponse;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Models.AuthTokenResponse?> RefreshAccessTokenAsync(string refreshToken)
        {
            var tokenReq = new HttpRequestMessage(
                HttpMethod.Post,
                "https://github.com/login/oauth/access_token"
            );
            tokenReq.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var parameters = new Dictionary<string, string>
            {
                { "client_id", ClientId ?? "" },
                { "client_secret", ClientSecret ?? "" },
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            };

            tokenReq.Content = new FormUrlEncodedContent(parameters);

            try
            {
                var response = await _httpClient.SendAsync(tokenReq);
                if (!response.IsSuccessStatusCode)
                    return null;

                using var stream = await response.Content.ReadAsStreamAsync();
                var tokenResponse = await JsonSerializer.DeserializeAsync<Models.AuthTokenResponse>(
                    stream
                );

                return tokenResponse;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            var rateReq = new HttpRequestMessage(
                HttpMethod.Get,
                "https://api.github.com/rate_limit"
            );
            rateReq.Headers.Add("User-Agent", "BlazorApp");
            if (token is not null)
            {
                var authHeader = new AuthenticationHeaderValue("Bearer", token);
                rateReq.Headers.Authorization = authHeader;
            }
            var response = await _httpClient.SendAsync(rateReq);
            return response.IsSuccessStatusCode;
        }

        public async Task<Models.RateLimitInfo?> GetRateLimitAsync(string? token)
        {
            var rateReq = new HttpRequestMessage(
                HttpMethod.Get,
                "https://api.github.com/rate_limit"
            );
            rateReq.Headers.Add("User-Agent", "BlazorApp");
            if (!string.IsNullOrWhiteSpace(token))
            {
                rateReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.SendAsync(rateReq);
            if (!response.IsSuccessStatusCode)
                return null;

            using var stream = await response.Content.ReadAsStreamAsync();
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            // GitHub returns an object with 'resources' property containing core/search
            var root = await System.Text.Json.JsonSerializer.DeserializeAsync<RateRoot>(
                stream,
                options
            );
            if (root?.Resources is null)
                return null;

            return new Models.RateLimitInfo
            {
                Resources = new Models.RateLimitResources
                {
                    Core = new Models.RateResource
                    {
                        Limit = root.Resources.Core.Limit,
                        Remaining = root.Resources.Core.Remaining,
                        Reset = root.Resources.Core.Reset,
                    },
                    Search = new Models.RateResource
                    {
                        Limit = root.Resources.Search.Limit,
                        Remaining = root.Resources.Search.Remaining,
                        Reset = root.Resources.Search.Reset,
                    },
                },
            };
        }

        private class RateRoot
        {
            public RateResources? Resources { get; set; }
        }

        private class RateResources
        {
            public RateResourceInfo Core { get; set; } = new RateResourceInfo();
            public RateResourceInfo Search { get; set; } = new RateResourceInfo();
        }

        private class RateResourceInfo
        {
            public int Limit { get; set; }
            public int Remaining { get; set; }
            public long Reset { get; set; }
        }
    }
}
