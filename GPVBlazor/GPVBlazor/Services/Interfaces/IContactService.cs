namespace GPVBlazor.Services.Interfaces
{
    public interface IContactService
    {
        Task<List<T>> FetchModalData<T>(string username, string endpoint, string token, int page = 1);
    }
}
