using GPVBlazor.Services.Interfaces;
using System.Text.Json;

public class ContactService : IContactService
{
    private readonly HttpClient _httpClient;
    //private readonly AuthenticationHeaderValue _authHeader = new("Bearer", "TOKEN");

    public ContactService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<T>> FetchModalData<T>(string username, string endpoint, int page = 1)
    {
        var collection = new List<T>();

        while (true)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/users/{username}/{endpoint}?per_page=100&page={page}");
            request.Headers.Add("User-Agent", "BlazorApp");
            //request.Headers.Authorization = _authHeader;

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) break;

            var pageItems = JsonSerializer.Deserialize<List<T>>(await response.Content.ReadAsStringAsync());
            if (pageItems == null || pageItems.Count == 0) break;

            collection.AddRange(pageItems);
            page++;
        }

        return collection;
    }
}
