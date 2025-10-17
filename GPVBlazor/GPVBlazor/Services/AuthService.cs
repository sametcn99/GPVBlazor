using GPVBlazor.Services.Interfaces;
using System.Net.Http.Headers;

namespace GPVBlazor.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
