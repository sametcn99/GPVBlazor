using GPVBlazor.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GPVBlazor.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public string? CurrentAccessToken { get; private set; }
        public event Action? OnAuthStateChanged;

        public AuthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public void Login(string token)
        {
            CurrentAccessToken = token;
            OnAuthStateChanged?.Invoke();
        }

        public void Logout()
        {
            CurrentAccessToken = null;
            OnAuthStateChanged?.Invoke();
        }

        public string GetGitHubLoginUrl()
        {
            var clientId = _configuration["GitHub:ClientId"];
            var redirectUri = _configuration["GitHub:RedirectUri"];
            // Scopes: public_repo, read:user, user:email, gist as suggested in the modal
            var scopes = "public_repo,read:user,user:email,gist";
            
            return $"https://github.com/login/oauth/authorize?client_id={clientId}&redirect_uri={redirectUri}&scope={scopes}";
        }

        public async Task<string?> GetAccessTokenFromCodeAsync(string code)
        {
            var tokenReq = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
            tokenReq.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            var parameters = new Dictionary<string, string>
            {
                { "client_id", _configuration["GitHub:ClientId"] ?? "" },
                { "client_secret", _configuration["GitHub:ClientSecret"] ?? "" },
                { "code", code },
                { "redirect_uri", _configuration["GitHub:RedirectUri"] ?? "" }
            };

            tokenReq.Content = new FormUrlEncodedContent(parameters);

            try 
            {
                var response = await _httpClient.SendAsync(tokenReq);
                if (!response.IsSuccessStatusCode) return null;

                using var stream = await response.Content.ReadAsStreamAsync();
                var tokenResponse = await JsonSerializer.DeserializeAsync<OAuthTokenResponse>(stream);
                
                return tokenResponse?.AccessToken;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            var rateReq = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/rate_limit");
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
            var rateReq = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/rate_limit");
            rateReq.Headers.Add("User-Agent", "BlazorApp");
            if (!string.IsNullOrWhiteSpace(token))
            {
                rateReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.SendAsync(rateReq);
            if (!response.IsSuccessStatusCode) return null;

            using var stream = await response.Content.ReadAsStreamAsync();
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // GitHub returns an object with 'resources' property containing core/search
            var root = await System.Text.Json.JsonSerializer.DeserializeAsync<RateRoot>(stream, options);
            if (root?.Resources is null) return null;

            return new Models.RateLimitInfo
            {
                Resources = new Models.RateLimitResources
                {
                    Core = new Models.RateResource
                    {
                        Limit = root.Resources.Core.Limit,
                        Remaining = root.Resources.Core.Remaining,
                        Reset = root.Resources.Core.Reset
                    },
                    Search = new Models.RateResource
                    {
                        Limit = root.Resources.Search.Limit,
                        Remaining = root.Resources.Search.Remaining,
                        Reset = root.Resources.Search.Reset
                    }
                }
            };
        }

        private class OAuthTokenResponse
        {
            [System.Text.Json.Serialization.JsonPropertyName("access_token")]
            public string? AccessToken { get; set; }
            
            [System.Text.Json.Serialization.JsonPropertyName("token_type")]
            public string? TokenType { get; set; }
            
            [System.Text.Json.Serialization.JsonPropertyName("scope")]
            public string? Scope { get; set; }
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
