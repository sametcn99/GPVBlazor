namespace GPVBlazor.Services
{
    public static class GitHubAuthenticationDefaults
    {
        public const string AuthenticationScheme = "GitHubSession";
        public const string SessionIdClaimType = "gpv:session-id";
        public const string AuthSourceClaimType = "gpv:auth-source";
    }
}
