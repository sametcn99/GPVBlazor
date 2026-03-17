using System.Security.Claims;

using Microsoft.AspNetCore.Http;

namespace GPVBlazor.Services.Interfaces
{
    public interface IAuthService
    {
        string? CurrentAccessToken { get; }
        string? CurrentAuthSource { get; }
        GPVBlazor.Models.User? CurrentUser { get; }
        event Action? OnAuthStateChanged;

        Task<bool> InitializeFromSessionAsync(CancellationToken cancellationToken = default);
        Task<string?> GetActiveAccessTokenAsync(CancellationToken cancellationToken = default);
        Task<string?> GetActiveAccessTokenAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default);
        Task<GPVBlazor.Models.User?> GetCurrentUserAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default);
        Task<GPVBlazor.Models.User?> FetchCurrentUserAsync(string token, CancellationToken cancellationToken = default);
        Task<bool> IsTokenValidAsync(string token, CancellationToken cancellationToken = default);
        Task<GPVBlazor.Models.RateLimitInfo?> GetRateLimitAsync(string? token = null, ClaimsPrincipal? principal = null, CancellationToken cancellationToken = default);
        Task<GPVBlazor.Models.AuthTokenResponse?> GetAccessTokenFromCodeAsync(string code, string redirectUri, CancellationToken cancellationToken = default);
        Task<GPVBlazor.Models.AuthTokenResponse?> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<bool> SignInWithGitHubCodeAsync(HttpContext httpContext, string code, string? state, CancellationToken cancellationToken = default);
        Task<bool> SignInWithPersonalAccessTokenAsync(HttpContext httpContext, string token, string authSource = "token", CancellationToken cancellationToken = default);
        Task SignOutAsync(HttpContext httpContext, ClaimsPrincipal principal, CancellationToken cancellationToken = default);
        string GetGitHubLoginUrl(HttpContext httpContext);
    }
}
