using GPVBlazor.Models;
using GPVBlazor.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using System.Text;
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

        public async Task<List<Gist>> FetchUserGists(string username, string token, int count, int page = 1)
        {
            string cacheKey = $"UserGists-{username}";
            if (_memoryCache.TryGetValue(cacheKey, out List<Gist>? cachedGists)) return cachedGists ?? new List<Gist>();

            var gists = new List<Gist>();

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var graphQLGists = await FetchUserGistsGraphQL(username, token, count);
                    if (graphQLGists != null)
                    {
                        _memoryCache.Set(cacheKey, graphQLGists, TimeSpan.FromHours(1));
                        return graphQLGists;
                    }
                }
                catch
                {
                    // Fallback to REST if GraphQL fails
                }
            }

            var pages = (int)Math.Ceiling(count / 100.0);

            var pageTasks = Enumerable.Range(page, pages).Select(async currentPage =>
            {
                try
                {
                    var gistsRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/users/{username}/gists?per_page=100&page={currentPage}");
                    gistsRequest.Headers.Add("User-Agent", "BlazorApp");
                    if (!string.IsNullOrEmpty(token))
                    {
                        var authHeader = new AuthenticationHeaderValue("Bearer", token);
                        gistsRequest.Headers.Authorization = authHeader;
                    }

                    var gistsResponse = await _httpClient.SendAsync(gistsRequest);
                    if (!gistsResponse.IsSuccessStatusCode) return new List<Gist>();

                    var pageGists = JsonSerializer.Deserialize<List<Gist>>(await gistsResponse.Content.ReadAsStringAsync());
                    return pageGists ?? new List<Gist>();
                }
                catch
                {
                    return new List<Gist>();
                }
            });

            var allGists = await Task.WhenAll(pageTasks);
            gists.AddRange(allGists.SelectMany(g => g));

            _memoryCache.Set(cacheKey, gists, TimeSpan.FromHours(1));
            return gists;
        }

        private async Task<List<Gist>?> FetchUserGistsGraphQL(string username, string token, int count)
        {
            var gists = new List<Gist>();
            string? cursor = null;
            bool hasNextPage = true;

            while (hasNextPage && gists.Count < count)
            {
                var query = new
                {
                    query = @"
                    query ($username: String!, $cursor: String) {
                        user(login: $username) {
                            gists(first: 100, after: $cursor, orderBy: {field: CREATED_AT, direction: DESC}) {
                                pageInfo {
                                    endCursor
                                    hasNextPage
                                }
                                nodes {
                                    id
                                    description
                                    url
                                    isPublic
                                    createdAt
                                    updatedAt
                                    stargazerCount
                                    comments {
                                        totalCount
                                    }
                                    files {
                                        name
                                        language {
                                            name
                                        }
                                    }
                                }
                            }
                        }
                    }",
                    variables = new { username, cursor }
                };

                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.github.com/graphql");
                request.Headers.Add("User-Agent", "BlazorApp");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Content = new StringContent(JsonSerializer.Serialize(query), Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);

                if (doc.RootElement.TryGetProperty("errors", out _)) return null;

                if (!doc.RootElement.TryGetProperty("data", out var data) ||
                    !data.TryGetProperty("user", out var user) ||
                    !user.TryGetProperty("gists", out var gistsData)) return null;

                var nodes = gistsData.GetProperty("nodes");

                foreach (var node in nodes.EnumerateArray())
                {
                    var gist = new Gist
                    {
                        Id = node.GetProperty("id").GetString(),
                        Description = node.GetProperty("description").GetString(),
                        HtmlUrl = node.GetProperty("url").GetString(),
                        Public = node.GetProperty("isPublic").GetBoolean(),
                        CreatedAt = node.GetProperty("createdAt").GetDateTime(),
                        UpdatedAt = node.GetProperty("updatedAt").GetDateTime(),
                        StargazersCount = node.GetProperty("stargazerCount").GetInt32(),
                        Comments = node.GetProperty("comments").GetProperty("totalCount").GetInt32(),
                        Files = new Dictionary<string, GistFile>()
                    };

                    if (node.TryGetProperty("files", out var filesElement))
                    {
                        foreach (var file in filesElement.EnumerateArray())
                        {
                            var filename = file.GetProperty("name").GetString();
                            var language = file.GetProperty("language").ValueKind == JsonValueKind.Null
                                ? null
                                : file.GetProperty("language").GetProperty("name").GetString();

                            if (filename != null)
                            {
                                gist.Files[filename] = new GistFile
                                {
                                    Filename = filename,
                                    Language = language
                                };
                            }
                        }
                    }
                    gists.Add(gist);
                }

                var pageInfo = gistsData.GetProperty("pageInfo");
                hasNextPage = pageInfo.GetProperty("hasNextPage").GetBoolean();
                if (hasNextPage)
                {
                    cursor = pageInfo.GetProperty("endCursor").GetString();
                }
            }

            return gists;
        }

        public async Task<List<Repository>> FetchReadmes(string username, string token, List<Repository> repositories)
        {
            var readmeTasks = repositories.Select(async repo =>
            {
                try
                {
                    var readmeRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/repos/{username}/{repo.Name}/readme");
                    readmeRequest.Headers.Add("User-Agent", "BlazorApp");
                    if (!string.IsNullOrEmpty(token))
                    {
                        var authHeader = new AuthenticationHeaderValue("Bearer", token);
                        readmeRequest.Headers.Authorization = authHeader;
                    }

                    var readmeResponse = await _httpClient.SendAsync(readmeRequest);
                    if (!readmeResponse.IsSuccessStatusCode) return repo;

                    var readmeContent = await readmeResponse.Content.ReadAsStringAsync();
                    var readme = JsonSerializer.Deserialize<Readme>(readmeContent);
                    if (readme is not null && readme.Content is not null)
                    {
                        var decodedBytes = Convert.FromBase64String(readme.Content);
                        readme.Content = System.Text.Encoding.UTF8.GetString(decodedBytes);
                        readme.Content = Markdig.Markdown.ToHtml(readme.Content);
                        repo.Readme = readme;
                    }
                    else
                    {
                        repo.Readme = null;
                    }
                    return repo;
                }
                catch
                {
                    // Log the error or handle it as needed
                    return repo;
                }
            });

            var results = await Task.WhenAll(readmeTasks);
            return results.ToList();
        }

        public async Task<StarHistory> FetchStarHistory(string owner, string repo, string token)
        {
            string cacheKey = $"StarHistory-{owner}-{repo}";
            if (_memoryCache.TryGetValue(cacheKey, out StarHistory? cachedHistory))
                return cachedHistory ?? new StarHistory();

            try
            {
                var starHistory = new StarHistory();
                var stargazers = new List<StarHistoryPoint>();
                int page = 1;
                const int perPage = 100;

                // Fetch stargazers with timestamps (this requires a different endpoint)
                while (true)
                {
                    var stargazersRequest = new HttpRequestMessage(HttpMethod.Get,
                        $"https://api.github.com/repos/{owner}/{repo}/stargazers?per_page={perPage}&page={page}");
                    stargazersRequest.Headers.Add("User-Agent", "BlazorApp");
                    stargazersRequest.Headers.Add("Accept", "application/vnd.github.v3.star+json");

                    if (!string.IsNullOrEmpty(token))
                    {
                        var authHeader = new AuthenticationHeaderValue("Bearer", token);
                        stargazersRequest.Headers.Authorization = authHeader;
                    }

                    var response = await _httpClient.SendAsync(stargazersRequest);
                    if (!response.IsSuccessStatusCode) break;

                    var content = await response.Content.ReadAsStringAsync();
                    var pageStargazers = JsonSerializer.Deserialize<List<StarHistoryPoint>>(content);

                    if (pageStargazers == null || pageStargazers.Count == 0) break;

                    stargazers.AddRange(pageStargazers);

                    // If we got less than perPage results, we're done
                    if (pageStargazers.Count < perPage) break;

                    page++;

                    // Limit to prevent too many API calls (adjust as needed)
                    if (page > 10) break;
                }

                starHistory.Points = stargazers.OrderBy(s => s.StarredAt).ToList();

                // Group by month for chart display
                var monthlyData = starHistory.Points
                    .GroupBy(s => new { s.StarredAt.Year, s.StarredAt.Month })
                    .Select(g => new
                    {
                        Date = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Date)
                    .ToDictionary(x => x.Date, x => x.Count);

                starHistory.MonthlyData = monthlyData;

                // Cache for 6 hours
                _memoryCache.Set(cacheKey, starHistory, TimeSpan.FromHours(6));
                return starHistory;
            }
            catch (Exception ex)
            {
                // Log error and return empty history
                Console.WriteLine($"Error fetching star history: {ex.Message}");
                return new StarHistory();
            }
        }

        public async Task<ContributionResponse?> FetchUserContributions(string username)
        {
            string cacheKey = $"UserContributions-{username}";
            if (_memoryCache.TryGetValue(cacheKey, out ContributionResponse? cachedContributions))
                return cachedContributions;

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://github-contributions-api.jogruber.de/v4/{username}?y=last");
                // No auth needed for this public API
                
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var contributions = JsonSerializer.Deserialize<ContributionResponse>(content);
                    
                    if (contributions is not null)
                    {
                        _memoryCache.Set(cacheKey, contributions, TimeSpan.FromHours(24));
                        return contributions;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Organization>> FetchUserOrganizations(string username, string token)
        {
            string cacheKey = $"UserOrganizations-{username}";
            if (_memoryCache.TryGetValue(cacheKey, out List<Organization>? cachedOrgs)) return cachedOrgs ?? new List<Organization>();

            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/users/{username}/orgs");
            request.Headers.Add("User-Agent", "BlazorApp");
            if (!string.IsNullOrEmpty(token))
            {
                var authHeader = new AuthenticationHeaderValue("Bearer", token);
                request.Headers.Authorization = authHeader;
            }

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var orgs = JsonSerializer.Deserialize<List<Organization>>(content);
                if (orgs is not null)
                {
                    _memoryCache.Set(cacheKey, orgs, TimeSpan.FromHours(1));
                    return orgs;
                }
            }
            return new List<Organization>();
        }
    }
}
