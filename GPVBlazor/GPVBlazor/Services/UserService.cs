using GPVBlazor.Models;
using GPVBlazor.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GPVBlazor.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<User?> FetchUserProfile(string username, string token)
        {
            var userRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/users/{username}");
            userRequest.Headers.Add("User-Agent", "BlazorApp");
            if (token is not null)
            {
                var authHeader = new AuthenticationHeaderValue("Bearer", token);
                userRequest.Headers.Authorization = authHeader;
            }
            var response = await _httpClient.SendAsync(userRequest);
            return response.IsSuccessStatusCode
                ? JsonSerializer.Deserialize<User>(await response.Content.ReadAsStringAsync())
                : null;
        }

        public async Task<UserSearchResult> SearchUsers(string inputValue)
        {
            var url = $"https://api.github.com/search/users?q={inputValue}";
            var userRequest = new HttpRequestMessage(HttpMethod.Get, url);
            userRequest.Headers.Add("User-Agent", "BlazorApp");
            var response = await _httpClient.SendAsync(userRequest);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var users = JsonSerializer.Deserialize<UserSearchResult>(content);
                if (users is not null) return users;
                else return new UserSearchResult();
            }
            else return new UserSearchResult();
        }


        public async Task<List<Repository>> FetchUserRepositories(string username, string token, int page = 1)
        {
            var repos = new List<Repository>();
            while (true)
            {
                var reposRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/users/{username}/repos?per_page=100&page={page}");
                reposRequest.Headers.Add("User-Agent", "BlazorApp");
                if (token is not null)
                {
                    var authHeader = new AuthenticationHeaderValue("Bearer", token);
                    reposRequest.Headers.Authorization = authHeader;
                }
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