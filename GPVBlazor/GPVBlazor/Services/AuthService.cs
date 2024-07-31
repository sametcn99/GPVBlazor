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
    }
}
