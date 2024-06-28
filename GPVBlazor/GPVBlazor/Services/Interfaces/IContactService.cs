namespace GPVBlazor.Services.Interfaces
{
    public interface IContactService
    {
        Task<List<T>> FetchModalData<T>(string username, string endpoint, int page = 1);
    }
}
