using System.Text.Json.Serialization;

namespace GPVBlazor.Models
{
    public class Activity
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("actor")]
        public ActivityActor Actor { get; set; } = new();

        [JsonPropertyName("repo")]
        public ActivityRepo Repo { get; set; } = new();

        [JsonPropertyName("payload")]
        public ActivityPayload Payload { get; set; } = new();

        [JsonPropertyName("public")]
        public bool Public { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
    }

    public class ActivityActor
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("login")]
        public string Login { get; set; } = string.Empty;

        [JsonPropertyName("display_login")]
        public string DisplayLogin { get; set; } = string.Empty;

        [JsonPropertyName("gravatar_id")]
        public string GravatarId { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; } = string.Empty;
    }

    public class ActivityRepo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        public DateTime? CreatedAt { get; set; }
    }

    public class ActivityPayload
    {
        [JsonPropertyName("action")]
        public string? Action { get; set; }

        [JsonPropertyName("ref")]
        public string? Ref { get; set; }

        [JsonPropertyName("ref_type")]
        public string? RefType { get; set; }

        [JsonPropertyName("master_branch")]
        public string? MasterBranch { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("pusher_type")]
        public string? PusherType { get; set; }

        [JsonPropertyName("commits")]
        public List<ActivityCommit>? Commits { get; set; }

        [JsonPropertyName("size")]
        public int? Size { get; set; }

        [JsonPropertyName("distinct_size")]
        public int? DistinctSize { get; set; }

        [JsonPropertyName("head")]
        public string? Head { get; set; }

        [JsonPropertyName("before")]
        public string? Before { get; set; }

        [JsonPropertyName("issue")]
        public ActivityIssue? Issue { get; set; }

        [JsonPropertyName("comment")]
        public ActivityComment? Comment { get; set; }

        [JsonPropertyName("pull_request")]
        public ActivityPullRequest? PullRequest { get; set; }

        [JsonPropertyName("forkee")]
        public ActivityForkee? Forkee { get; set; }

        [JsonPropertyName("release")]
        public ActivityRelease? Release { get; set; }

        [JsonPropertyName("member")]
        public ActivityMember? Member { get; set; }

        [JsonPropertyName("pages")]
        public List<ActivityPage>? Pages { get; set; }
    }

    public class ActivityCommit
    {
        [JsonPropertyName("sha")]
        public string Sha { get; set; } = string.Empty;

        [JsonPropertyName("author")]
        public ActivityAuthor Author { get; set; } = new();

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("distinct")]
        public bool Distinct { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }

    public class ActivityAuthor
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class ActivityIssue
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = string.Empty;

        [JsonPropertyName("state")]
        public string State { get; set; } = string.Empty;

        [JsonPropertyName("body")]
        public string? Body { get; set; }

        [JsonPropertyName("labels")]
        public List<ActivityLabel>? Labels { get; set; }
    }

    public class ActivityLabel
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("color")]
        public string Color { get; set; } = string.Empty;
    }

    public class ActivityComment
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; } = string.Empty;

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = string.Empty;

        [JsonPropertyName("commit_id")]
        public string? CommitId { get; set; }
    }

    public class ActivityPullRequest
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = string.Empty;

        [JsonPropertyName("state")]
        public string State { get; set; } = string.Empty;

        [JsonPropertyName("merged")]
        public bool? Merged { get; set; }

        [JsonPropertyName("body")]
        public string? Body { get; set; }

        [JsonPropertyName("additions")]
        public int? Additions { get; set; }

        [JsonPropertyName("deletions")]
        public int? Deletions { get; set; }

        [JsonPropertyName("changed_files")]
        public int? ChangedFiles { get; set; }
    }

    public class ActivityForkee
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("full_name")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    public class ActivityRelease
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("tag_name")]
        public string TagName { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = string.Empty;

        [JsonPropertyName("body")]
        public string? Body { get; set; }

        [JsonPropertyName("prerelease")]
        public bool Prerelease { get; set; }

        [JsonPropertyName("draft")]
        public bool Draft { get; set; }
    }

    public class ActivityMember
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("login")]
        public string Login { get; set; } = string.Empty;

        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; } = string.Empty;

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = string.Empty;
    }

    public class ActivityPage
    {
        [JsonPropertyName("page_name")]
        public string PageName { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("action")]
        public string Action { get; set; } = string.Empty;

        [JsonPropertyName("sha")]
        public string Sha { get; set; } = string.Empty;

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = string.Empty;
    }
}
