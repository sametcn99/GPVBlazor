using System.Text.Json.Serialization;

namespace GPVBlazor.Models
{
    public class User
    {
        [JsonPropertyName("login")]
        public string? Login { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("node_id")]
        public string? NodeId { get; set; }

        [JsonPropertyName("avatar_url")]
        public string? AvatarUrl { get; set; }

        [JsonPropertyName("gravatar_id")]
        public string? GravatarId { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("html_url")]
        public string? HtmlUrl { get; set; }

        [JsonPropertyName("followers_url")]
        public string? FollowersUrl { get; set; }

        [JsonPropertyName("following_url")]
        public string? FollowingUrl { get; set; }

        [JsonPropertyName("gists_url")]
        public string? GistsUrl { get; set; }

        [JsonPropertyName("starred_url")]
        public string? StarredUrl { get; set; }

        [JsonPropertyName("subscriptions_url")]
        public string? SubscriptionsUrl { get; set; }

        [JsonPropertyName("organizations_url")]
        public string? OrganizationsUrl { get; set; }

        [JsonPropertyName("repos_url")]
        public string? ReposUrl { get; set; }

        [JsonPropertyName("events_url")]
        public string? EventsUrl { get; set; }

        [JsonPropertyName("received_events_url")]
        public string? ReceivedEventsUrl { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("site_admin")]
        public bool SiteAdmin { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("company")]
        public string? Company { get; set; }

        [JsonPropertyName("blog")]
        public string? Blog { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("notification_email")]
        public string? NotificationEmail { get; set; }

        [JsonPropertyName("hireable")]
        public bool? Hireable { get; set; }

        [JsonPropertyName("bio")]
        public string? Bio { get; set; }

        [JsonPropertyName("twitter_username")]
        public string? TwitterUsername { get; set; }

        [JsonPropertyName("public_repos")]
        public int PublicRepos { get; set; }

        [JsonPropertyName("public_gists")]
        public int PublicGists { get; set; }

        [JsonPropertyName("followers")]
        public int Followers { get; set; }

        [JsonPropertyName("following")]
        public int Following { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("private_gists")]
        public int PrivateGists { get; set; }

        [JsonPropertyName("total_private_repos")]
        public int TotalPrivateRepos { get; set; }

        [JsonPropertyName("owned_private_repos")]
        public int OwnedPrivateRepos { get; set; }

        [JsonPropertyName("disk_usage")]
        public int DiskUsage { get; set; }

        [JsonPropertyName("collaborators")]
        public int Collaborators { get; set; }

        [JsonPropertyName("two_factor_authentication")]
        public bool TwoFactorAuthentication { get; set; }

        [JsonPropertyName("suspended_at")]
        public DateTime? SuspendedAt { get; set; }

        [JsonPropertyName("business_plus")]
        public bool BusinessPlus { get; set; }

        [JsonPropertyName("ldap_dn")]
        public string? LdapDn { get; set; }
    }
}
