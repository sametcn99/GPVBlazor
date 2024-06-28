using GPVBlazor.Models;
using GPVBlazor.Services.Interfaces;
using System.Text.Json;

namespace GPVBlazor.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        //private readonly AuthenticationHeaderValue _authHeader = new("Bearer", "TOKEN");

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<User?> FetchUserProfile(string username)
        {
            var userRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/users/{username}");
            userRequest.Headers.Add("User-Agent", "BlazorApp");
            //userRequest.Headers.Authorization = _authHeader;

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
            //userRequest.Headers.Authorization = _authHeader;

            var response = await _httpClient.SendAsync(userRequest);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var users = JsonSerializer.Deserialize<UserSearchResult>(content);
                return users;
            }
            else
            {
                return new UserSearchResult(); // Return an empty list instead of null to avoid null reference returns
            }
        }


        public async Task<List<Repository>> FetchUserRepositories(string username, int page = 1)
        {
            var repos = new List<Repository>();
            while (true)
            {
                var reposRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/users/{username}/repos?per_page=100&page={page}");
                reposRequest.Headers.Add("User-Agent", "BlazorApp");
                //reposRequest.Headers.Authorization = _authHeader;

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
