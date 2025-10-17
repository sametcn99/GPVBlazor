using GPVBlazor.Models;

namespace GPVBlazor.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> FetchUserProfile(string username, string token);
        Task<List<Repository>> FetchUserRepositories(string username, string token, int count, int page = 1);
        Task<List<Repository>> FetchReadmes(string username, string token, List<Repository> repositories);
        Task<UserSearchResult> SearchUsers(string inputValue);
        Task<StarHistory> FetchStarHistory(string owner, string repo, string token);
    }
}
