using GPVBlazor.Models;
using GPVBlazor.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GPVBlazor.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;

        public UserService(HttpClient httpClient, IMemoryCache memoryCache)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
        }

        public async Task<User?> FetchUserProfile(string username, string token)
        {
            string cacheKey = $"UserProfile-{username}";
            if (_memoryCache.TryGetValue(cacheKey, out User? cachedUser)) return cachedUser;

            var userRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/users/{username}");
            userRequest.Headers.Add("User-Agent", "BlazorApp");
            if (token is not null)
            {
                var authHeader = new AuthenticationHeaderValue("Bearer", token);
                userRequest.Headers.Authorization = authHeader;
            }
            var response = await _httpClient.SendAsync(userRequest);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<User>(content);
                if (user is not null)
                {
                    _memoryCache.Set(cacheKey, user, TimeSpan.FromHours(1));
                    return user;
                }
                else return null;
            }
            else return null;
        }

        public async Task<UserSearchResult> SearchUsers(string inputValue)
        {
            string cacheKey = $"SearchUsers-{inputValue}";
            if (_memoryCache.TryGetValue(cacheKey, out UserSearchResult? cachedUsers)) if (cachedUsers is not null) return cachedUsers;
            var url = $"https://api.github.com/search/users?q={inputValue}";
            var userRequest = new HttpRequestMessage(HttpMethod.Get, url);
            userRequest.Headers.Add("User-Agent", "BlazorApp");
            var response = await _httpClient.SendAsync(userRequest);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var users = JsonSerializer.Deserialize<UserSearchResult>(content);
                if (users is not null)
                {
                    _memoryCache.Set(cacheKey, users, TimeSpan.FromDays(1));
                    return users;
                }
                else return new UserSearchResult();
            }
            else return new UserSearchResult();
        }

        public async Task<List<Repository>> FetchUserRepositories(string username, string token, int count, int page = 1)
        {
            // Define a unique cache key for this request
            string cacheKey = $"UserRepositories-{username}";

            // Attempt to get the repository list from cache
            if (_memoryCache.TryGetValue(cacheKey, out List<Repository>? cachedRepos)) return cachedRepos ?? new List<Repository>();

            var repos = new List<Repository>();

            // Calculate the number of pages to fetch based on the count
            var pages = (int)Math.Ceiling(count / 100.0);

            // Create tasks for each page request
            var pageTasks = Enumerable.Range(page, pages).Select(async currentPage =>
            {
                try
                {
                    var reposRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/users/{username}/repos?per_page=100&page={currentPage}");
                    reposRequest.Headers.Add("User-Agent", "BlazorApp");
                    if (!string.IsNullOrEmpty(token))
                    {
                        var authHeader = new AuthenticationHeaderValue("Bearer", token);
                        reposRequest.Headers.Authorization = authHeader;
                    }

                    var reposResponse = await _httpClient.SendAsync(reposRequest);
                    if (!reposResponse.IsSuccessStatusCode) return new List<Repository>();

                    var pageRepositories = JsonSerializer.Deserialize<List<Repository>>(await reposResponse.Content.ReadAsStringAsync());
                    return pageRepositories ?? new List<Repository>();
                }
                catch
                {
                    // Log the error or handle it as needed
                    return new List<Repository>();
                }
            });

            // Wait for all page requests to complete
            var allRepos = await Task.WhenAll(pageTasks);

            // Flatten the results
            repos.AddRange(allRepos.SelectMany(r => r));

            // Cache the fetched repositories
            _memoryCache.Set(cacheKey, repos, TimeSpan.FromHours(1));
            return repos;
        }

        public async Task<List<Repository>> FetchReadmes(string username, string token, List<Repository> repositories)
        {
            var readmeTasks = repositories
                .Where(repo => repo.Name != null && !string.IsNullOrEmpty(token))
                .Select(async repo =>
                {
                    var readmeInfo = await FetchReadmeInfo(username, repo.Name!, token);
                    if (readmeInfo != null) repo.Readme = readmeInfo;
                });

            await Task.WhenAll(readmeTasks);

            return repositories;
        }

        public async Task<Readme?> FetchReadmeInfo(string username, string repoName, string token)
        {
            var readmeRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/repos/{username}/{repoName}/readme");
            readmeRequest.Headers.Add("User-Agent", "BlazorApp");
            if (token is not null)
            {
                var authHeader = new AuthenticationHeaderValue("Bearer", token);
                readmeRequest.Headers.Authorization = authHeader;
            }
            var readmeResponse = await _httpClient.SendAsync(readmeRequest);
            if (readmeResponse.IsSuccessStatusCode)
            {
                var readmeContent = await readmeResponse.Content.ReadAsStringAsync();
                var readmeInfo = JsonSerializer.Deserialize<Readme>(readmeContent);
                var readmeText = readmeInfo?.Content?.ToString();
                if (readmeInfo != null && !string.IsNullOrEmpty(readmeText))
                {
                    var decodedBytes = Convert.FromBase64String(readmeText);
                    readmeInfo.Content = System.Text.Encoding.UTF8.GetString(decodedBytes);
                    readmeInfo.Content = Markdig.Markdown.ToHtml(readmeInfo.Content);
                }
                return readmeInfo;
            }
            return null;
        }
    }
}
