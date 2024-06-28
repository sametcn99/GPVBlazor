using GPVBlazor.Models;
using GPVBlazor.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GPVBlazor.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationHeaderValue _authHeader = new("Bearer", "TOKEN");

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<User?> FetchUserProfile(string username)
        {
            var userRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/users/{username}");
            userRequest.Headers.Add("User-Agent", "BlazorApp");
            userRequest.Headers.Authorization = _authHeader;

            var response = await _httpClient.SendAsync(userRequest);
            return response.IsSuccessStatusCode
                ? JsonSerializer.Deserialize<User>(await response.Content.ReadAsStringAsync())
                : null;
        }

        public async Task<List<Repository>> FetchUserRepositories(string username, int page = 1)
        {
            var repos = new List<Repository>();
            while (true)
            {
                var reposRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/users/{username}/repos?per_page=100&page={page}");
                reposRequest.Headers.Add("User-Agent", "BlazorApp");
                reposRequest.Headers.Authorization = _authHeader;

                var reposResponse = await _httpClient.SendAsync(reposRequest);
                if (!reposResponse.IsSuccessStatusCode) break;

                var pageRepositories = JsonSerializer.Deserialize<List<Repository>>(await reposResponse.Content.ReadAsStringAsync());
                if (pageRepositories == null || pageRepositories.Count == 0) break;

                repos.AddRange(pageRepositories);
                page++;
            }
            return repos;
        }
    }

}
