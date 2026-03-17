namespace GPVBlazor.Models
{
    public class GitHubAuthSession
    {
        public string SessionId { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public DateTimeOffset? AccessTokenExpiresAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTimeOffset? RefreshTokenExpiresAt { get; set; }
        public string AuthSource { get; set; } = "github";
        public string? UserLogin { get; set; }
        public string? UserAvatarUrl { get; set; }
    }
}
