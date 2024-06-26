using System.Text.Json.Serialization;

namespace GPVBlazor.Models
{
    public class Repository
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("node_id")]
        public string? NodeId { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("full_name")]
        public string? FullName { get; set; }

        [JsonPropertyName("owner")]
        public Owner? Owner { get; set; }

        [JsonPropertyName("private")]
        public bool Private { get; set; }

        [JsonPropertyName("html_url")]
        public string? HtmlUrl { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("fork")]
        public bool Fork { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("archive_url")]
        public string? ArchiveUrl { get; set; }

        [JsonPropertyName("assignees_url")]
        public string? AssigneesUrl { get; set; }

        [JsonPropertyName("blobs_url")]
        public string? BlobsUrl { get; set; }

        [JsonPropertyName("branches_url")]
        public string? BranchesUrl { get; set; }

        [JsonPropertyName("collaborators_url")]
        public string? CollaboratorsUrl { get; set; }

        [JsonPropertyName("comments_url")]
        public string? CommentsUrl { get; set; }

        [JsonPropertyName("commits_url")]
        public string? CommitsUrl { get; set; }

        [JsonPropertyName("compare_url")]
        public string? CompareUrl { get; set; }

        [JsonPropertyName("contents_url")]
        public string? ContentsUrl { get; set; }

        [JsonPropertyName("contributors_url")]
        public string? ContributorsUrl { get; set; }

        [JsonPropertyName("deployments_url")]
        public string? DeploymentsUrl { get; set; }

        [JsonPropertyName("downloads_url")]
        public string? DownloadsUrl { get; set; }

        [JsonPropertyName("events_url")]
        public string? EventsUrl { get; set; }

        [JsonPropertyName("forks_url")]
        public string? ForksUrl { get; set; }

        [JsonPropertyName("git_commits_url")]
        public string? GitCommitsUrl { get; set; }

        [JsonPropertyName("git_refs_url")]
        public string? GitRefsUrl { get; set; }

        [JsonPropertyName("git_tags_url")]
        public string? GitTagsUrl { get; set; }

        [JsonPropertyName("git_url")]
        public string? GitUrl { get; set; }

        [JsonPropertyName("issue_comment_url")]
        public string? IssueCommentUrl { get; set; }

        [JsonPropertyName("issue_events_url")]
        public string? IssueEventsUrl { get; set; }

        [JsonPropertyName("issues_url")]
        public string? IssuesUrl { get; set; }

        [JsonPropertyName("keys_url")]
        public string? KeysUrl { get; set; }

        [JsonPropertyName("labels_url")]
        public string? LabelsUrl { get; set; }

        [JsonPropertyName("languages_url")]
        public string? LanguagesUrl { get; set; }

        [JsonPropertyName("merges_url")]
        public string? MergesUrl { get; set; }

        [JsonPropertyName("milestones_url")]
        public string? MilestonesUrl { get; set; }

        [JsonPropertyName("notifications_url")]
        public string? NotificationsUrl { get; set; }

        [JsonPropertyName("pulls_url")]
        public string? PullsUrl { get; set; }

        [JsonPropertyName("releases_url")]
        public string? ReleasesUrl { get; set; }

        [JsonPropertyName("ssh_url")]
        public string? SshUrl { get; set; }

        [JsonPropertyName("stargazers_url")]
        public string? StargazersUrl { get; set; }

        [JsonPropertyName("statuses_url")]
        public string? StatusesUrl { get; set; }

        [JsonPropertyName("subscribers_url")]
        public string? SubscribersUrl { get; set; }

        [JsonPropertyName("subscription_url")]
        public string? SubscriptionUrl { get; set; }

        [JsonPropertyName("tags_url")]
        public string? TagsUrl { get; set; }

        [JsonPropertyName("teams_url")]
        public string? TeamsUrl { get; set; }

        [JsonPropertyName("trees_url")]
        public string? TreesUrl { get; set; }

        [JsonPropertyName("clone_url")]
        public string? CloneUrl { get; set; }

        [JsonPropertyName("mirror_url")]
        public string? MirrorUrl { get; set; }

        [JsonPropertyName("hooks_url")]
        public string? HooksUrl { get; set; }

        [JsonPropertyName("sbn_url")]
        public string? SbnUrl { get; set; }

        [JsonPropertyName("homepage")]
        public string? Homepage { get; set; }

        [JsonPropertyName("language")]
        public string? Language { get; set; }

        [JsonPropertyName("forks_count")]
        public int ForksCount { get; set; }

        [JsonPropertyName("stargazers_count")]
        public int StargazersCount { get; set; }

        [JsonPropertyName("watchers_count")]
        public int WatchersCount { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("default_branch")]
        public string? DefaultBranch { get; set; }

        [JsonPropertyName("open_issues_count")]
        public int OpenIssuesCount { get; set; }

        [JsonPropertyName("is_template")]
        public bool IsTemplate { get; set; }

        [JsonPropertyName("topics")]
        public List<string>? Topics { get; set; }

        [JsonPropertyName("has_issues")]
        public bool HasIssues { get; set; }

        [JsonPropertyName("has_projects")]
        public bool HasProjects { get; set; }

        [JsonPropertyName("has_wiki")]
        public bool HasWiki { get; set; }

        [JsonPropertyName("has_pages")]
        public bool HasPages { get; set; }

        [JsonPropertyName("has_downloads")]
        public bool HasDownloads { get; set; }

        [JsonPropertyName("has_discussions")]
        public bool HasDiscussions { get; set; }

        [JsonPropertyName("archived")]
        public bool Archived { get; set; }

        [JsonPropertyName("disabled")]
        public bool Disabled { get; set; }

        [JsonPropertyName("visibility")]
        public string? Visibility { get; set; }

        [JsonPropertyName("pushed_at")]
        public DateTime? PushedAt { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("permissions")]
        public Permissions? Permissions { get; set; }

        [JsonPropertyName("role_name")]
        public string? RoleName { get; set; }

        [JsonPropertyName("temp_clone_token")]
        public string? TempCloneToken { get; set; }

        [JsonPropertyName("delete_branch_on_merge")]
        public bool? DeleteBranchOnMerge { get; set; }

        [JsonPropertyName("subscribers_count")]
        public int SubscribersCount { get; set; }

        [JsonPropertyName("network_count")]
        public int NetworkCount { get; set; }

        [JsonPropertyName("code_of_conduct")]
        public CodeOfConduct? CodeOfConduct { get; set; }

        [JsonPropertyName("license")]
        public License? License { get; set; }

        [JsonPropertyName("forks")]
        public int Forks { get; set; }

        [JsonPropertyName("open_issues")]
        public int OpenIssues { get; set; }

        [JsonPropertyName("watchers")]
        public int Watchers { get; set; }

        [JsonPropertyName("allow_forking")]
        public bool AllowForking { get; set; }

        [JsonPropertyName("web_commit_signoff_required")]
        public bool WebCommitSignoffRequired { get; set; }

        [JsonPropertyName("security_and_analysis")]
        public SecurityAndAnalysis? SecurityAndAnalysis { get; set; }
    }

    public class Owner : User { }

    public class Permissions
    {
        [JsonPropertyName("admin")]
        public bool Admin { get; set; }

        [JsonPropertyName("maintain")]
        public bool Maintain { get; set; }

        [JsonPropertyName("push")]
        public bool Push { get; set; }

        [JsonPropertyName("triage")]
        public bool Triage { get; set; }

        [JsonPropertyName("pull")]
        public bool Pull { get; set; }
    }

    public class CodeOfConduct
    {
        [JsonPropertyName("key")]
        public string? Key { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("body")]
        public string? Body { get; set; }

        [JsonPropertyName("html_url")]
        public string? HtmlUrl { get; set; }
    }

    public class License
    {
        [JsonPropertyName("key")]
        public string? Key { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("spdx_id")]
        public string? SpdxId { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("node_id")]
        public string? NodeId { get; set; }
    }

    public class SecurityAndAnalysis
    {
        [JsonPropertyName("advanced_security")]
        public SecurityStatus? AdvancedSecurity { get; set; }

        [JsonPropertyName("dependabot_security_updates")]
        public SecurityStatus? DependabotSecurityUpdates { get; set; }

        [JsonPropertyName("secret_scanning")]
        public SecurityStatus? SecretScanning { get; set; }

        [JsonPropertyName("secret_scanning_push_protection")]
        public SecurityStatus? SecretScanningPushProtection { get; set; }
    }

    public class SecurityStatus
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }
}
