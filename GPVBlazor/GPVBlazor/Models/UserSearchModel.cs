using System.Text.Json.Serialization;

namespace GPVBlazor.Models
{
    public class UserSearchResult
    {
        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }

        [JsonPropertyName("incomplete_results")]
        public bool IncompleteResults { get; set; }

        [JsonPropertyName("items")]
        public List<UserSearchResultItem> Items { get; set; } = new();
    }

    public class UserSearchResultItem
    {
        [JsonPropertyName("login")]
        public string Login { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("node_id")]
        public string NodeId { get; set; }

        [JsonPropertyName("avatar_url")]
        public Uri AvatarUrl { get; set; }

        [JsonPropertyName("gravatar_id")]
        public string? GravatarId { get; set; }

        [JsonPropertyName("url")]
        public Uri Url { get; set; }

        [JsonPropertyName("html_url")]
        public Uri HtmlUrl { get; set; }

        [JsonPropertyName("followers_url")]
        public Uri FollowersUrl { get; set; }

        [JsonPropertyName("subscriptions_url")]
        public Uri SubscriptionsUrl { get; set; }

        [JsonPropertyName("organizations_url")]
        public Uri OrganizationsUrl { get; set; }

        [JsonPropertyName("repos_url")]
        public Uri ReposUrl { get; set; }

        [JsonPropertyName("received_events_url")]
        public Uri ReceivedEventsUrl { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }

        [JsonPropertyName("following_url")]
        public string FollowingUrl { get; set; }

        [JsonPropertyName("gists_url")]
        public string GistsUrl { get; set; }

        [JsonPropertyName("starred_url")]
        public string StarredUrl { get; set; }

        [JsonPropertyName("events_url")]
        public string EventsUrl { get; set; }

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

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("bio")]
        public string? Bio { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }

        [JsonPropertyName("site_admin")]
        public bool SiteAdmin { get; set; }

        [JsonPropertyName("hireable")]
        public bool? Hireable { get; set; }

        [JsonPropertyName("text_matches")]
        public List<SearchResultTextMatch>? TextMatches { get; set; }

        [JsonPropertyName("blog")]
        public string? Blog { get; set; }

        [JsonPropertyName("company")]
        public string? Company { get; set; }

        [JsonPropertyName("suspended_at")]
        public DateTime? SuspendedAt { get; set; }
    }

    public class SearchResultTextMatch
    {
        [JsonPropertyName("object_url")]
        public string ObjectUrl { get; set; }

        [JsonPropertyName("object_type")]
        public string? ObjectType { get; set; }

        [JsonPropertyName("property")]
        public string Property { get; set; }

        [JsonPropertyName("fragment")]
        public string Fragment { get; set; }

        [JsonPropertyName("matches")]
        public List<TextMatch> Matches { get; set; } = new();
    }

    public class TextMatch
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("indices")]
        public List<int> Indices { get; set; } = new();
    }
}
