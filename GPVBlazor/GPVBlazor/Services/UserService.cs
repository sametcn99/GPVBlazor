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


        public async Task<List<Repository>> FetchUserRepositories(string username, string token, int count, int page = 1)
        {
            var repos = new List<Repository>();
            var tasks = new List<Task>();

            // Calculate the number of pages to fetch based on the count
            var pages = (int)Math.Ceiling(count / 100.0);

            // Create tasks for each page request
            var pageTasks = Enumerable.Range(page, pages).Select(async currentPage =>
            {
                var reposRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/users/{username}/repos?per_page=100&page={currentPage}");
                reposRequest.Headers.Add("User-Agent", "BlazorApp");
                if (!string.IsNullOrEmpty(token))
                {
                    var authHeader = new AuthenticationHeaderValue("Bearer", token);
                    reposRequest.Headers.Authorization = authHeader;
                }

                var reposResponse = await _httpClient.SendAsync(reposRequest).ConfigureAwait(false);
                if (!reposResponse.IsSuccessStatusCode) return new List<Repository>();

                var pageRepositories = JsonSerializer.Deserialize<List<Repository>>(await reposResponse.Content.ReadAsStringAsync().ConfigureAwait(false));
                return pageRepositories ?? new List<Repository>();
            }).ToList();

            // Wait for all page requests to complete
            var allRepos = await Task.WhenAll(pageTasks).ConfigureAwait(false);

            // Flatten the results
            repos.AddRange(allRepos.SelectMany(r => r));

            // Fetch README content in parallel
            var readmeTasks = repos
                .Where(repo => repo.Name != null && !string.IsNullOrEmpty(token))
                .Select(repo => FetchReadmeInfo(username, repo.Name, token).ContinueWith(readmeTask =>
                {
                    var readmeInfo = readmeTask.Result;
                    if (readmeInfo != null)
                    {
                        repo.Readme = readmeInfo;
                    }
                })).ToList();

            await Task.WhenAll(readmeTasks).ConfigureAwait(false);

            return repos;
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

        public async Task<string> FetchReadmeText(string downloadUrl, string token)
        {
            var readmeTextRequest = new HttpRequestMessage(HttpMethod.Get, downloadUrl);
            readmeTextRequest.Headers.Add("User-Agent", "BlazorApp");
            if (token is not null)
            {
                var authHeader = new AuthenticationHeaderValue("Bearer", token);
                readmeTextRequest.Headers.Authorization = authHeader;
            }
            var readmeTextResponse = await _httpClient.SendAsync(readmeTextRequest);
            if (readmeTextResponse.IsSuccessStatusCode)
            {
                return await readmeTextResponse.Content.ReadAsStringAsync();
            }
            return string.Empty;
        }
    }
}