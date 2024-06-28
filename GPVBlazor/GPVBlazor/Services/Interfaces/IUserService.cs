using GPVBlazor.Models;

namespace GPVBlazor.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> FetchUserProfile(string username);
        Task<UserSearchResult> SearchUsers(string inputValue);
        Task<List<Repository>> FetchUserRepositories(string username, int page = 1);
    }
}
