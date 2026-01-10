namespace GPVBlazor.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> IsTokenValidAsync(string token);
        Task<GPVBlazor.Models.RateLimitInfo?> GetRateLimitAsync(string? token);
        Task<string?> GetAccessTokenFromCodeAsync(string code);
        string GetGitHubLoginUrl();
    }
}
