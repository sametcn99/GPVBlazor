using GPVBlazor.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

public class ContactService : IContactService
{
    private readonly HttpClient _httpClient;

    public ContactService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<T>> FetchModalData<T>(string username, string endpoint, string token, int page = 1)
    {
        var collection = new List<T>();
        while (true)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/users/{username}/{endpoint}?per_page=100&page={page}");
            request.Headers.Add("User-Agent", "BlazorApp");
            if (token is not null)
            {
                var authHeader = new AuthenticationHeaderValue("Bearer", token);
                request.Headers.Authorization = authHeader;
            }
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) break;
            var pageItems = JsonSerializer.Deserialize<List<T>>(await response.Content.ReadAsStringAsync());
            if (pageItems is null || pageItems.Count is 0) break;

            collection.AddRange(pageItems);
            page++;
        }
        return collection;
    }
}
