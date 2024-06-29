using GPVBlazor.Models;

namespace GPVBlazor.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> FetchUserProfile(string username, string token);
        Task<List<Repository>> FetchUserRepositories(string username, string token, int page = 1);
        Task<UserSearchResult> SearchUsers(string inputValue);
    }
}
