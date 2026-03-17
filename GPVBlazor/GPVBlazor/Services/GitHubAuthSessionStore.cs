using GPVBlazor.Models;
using GPVBlazor.Services.Interfaces;

using Microsoft.Extensions.Caching.Memory;

namespace GPVBlazor.Services
{
    public class GitHubAuthSessionStore : IGitHubAuthSessionStore
    {
        private readonly IMemoryCache _memoryCache;

        public GitHubAuthSessionStore(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Task<GitHubAuthSession?> GetAsync(string sessionId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _memoryCache.TryGetValue(GetCacheKey(sessionId), out GitHubAuthSession? session);
            return Task.FromResult(session);
        }

        public Task SetAsync(GitHubAuthSession session, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var expiration = session.RefreshTokenExpiresAt
                ?? session.AccessTokenExpiresAt
                ?? DateTimeOffset.UtcNow.AddDays(7);

            if (expiration <= DateTimeOffset.UtcNow)
            {
                expiration = DateTimeOffset.UtcNow.AddMinutes(30);
            }

            _memoryCache.Set(
                GetCacheKey(session.SessionId),
                session,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = expiration,
                    SlidingExpiration = TimeSpan.FromHours(12),
                }
            );

            return Task.CompletedTask;
        }

        public Task RemoveAsync(string sessionId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _memoryCache.Remove(GetCacheKey(sessionId));
            return Task.CompletedTask;
        }

        private static string GetCacheKey(string sessionId)
        {
            return $"github-auth-session:{sessionId}";
        }
    }
}
