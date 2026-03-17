using System.Net.Http.Headers;
using System.Text.Json;

using GPVBlazor.Models;
using GPVBlazor.Services.Interfaces;

namespace GPVBlazor.Services
{
    public class ContactService : IContactService
    {
        private readonly HttpClient _httpClient;
        private const int PageSize = 100;

        public ContactService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<T>> FetchModalData<T>(string username, string endpoint, string token, int page = 1)
        {
            var collection = new List<T>();
            while (true)
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/users/{username}/{endpoint}?per_page={PageSize}&page={page}");
                    request.Headers.Add("User-Agent", "BlazorApp");
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        var authHeader = new AuthenticationHeaderValue("Bearer", token);
                        request.Headers.Authorization = authHeader;
                    }

                    var response = await _httpClient.SendAsync(request);
                    if (!response.IsSuccessStatusCode)
                    {
                        break;
                    }

                    var pageItems = JsonSerializer.Deserialize<List<T>>(await response.Content.ReadAsStringAsync());
                    if (pageItems is null || pageItems.Count is 0)
                    {
                        break;
                    }

                    collection.AddRange(pageItems);
                    if (pageItems.Count < PageSize)
                    {
                        break;
                    }

                    page++;
                }
                catch
                {
                    break;
                }
            }

            return collection;
        }

        public async Task<(List<User> Followers, List<User> Following)> FetchNetworkData(string username, string token)
        {
            var followersTask = FetchModalData<User>(username, "followers", token);
            var followingTask = FetchModalData<User>(username, "following", token);

            await Task.WhenAll(followersTask, followingTask);

            return (await followersTask, await followingTask);
        }
    }
}
