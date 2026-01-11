namespace GPVBlazor.Services.Interfaces
{
    public interface IAuthService
    {
        string? CurrentAccessToken { get; }
        GPVBlazor.Models.User? CurrentUser { get; }
        event Action OnAuthStateChanged;
        void Login(string token);
        void Logout();

        Task<GPVBlazor.Models.User?> FetchCurrentUserAsync(string token);
        Task<bool> IsTokenValidAsync(string token);
        Task<GPVBlazor.Models.RateLimitInfo?> GetRateLimitAsync(string? token);
        Task<string?> GetAccessTokenFromCodeAsync(string code);
        string GetGitHubLoginUrl();
    }
}
