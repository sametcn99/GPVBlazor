using GPVBlazor.Models;

namespace GPVBlazor.Services.Interfaces
{
    public interface IGitHubAuthSessionStore
    {
        Task<GitHubAuthSession?> GetAsync(string sessionId, CancellationToken cancellationToken = default);
        Task SetAsync(GitHubAuthSession session, CancellationToken cancellationToken = default);
        Task RemoveAsync(string sessionId, CancellationToken cancellationToken = default);
    }
}
