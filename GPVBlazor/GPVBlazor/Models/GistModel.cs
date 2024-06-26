namespace GPVBlazor.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    namespace GPVBlazor.Models
    {
        public class GistFile
        {
            [JsonPropertyName("filename")]
            public string Filename { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("language")]
            public string Language { get; set; }

            [JsonPropertyName("raw_url")]
            public string RawUrl { get; set; }

            [JsonPropertyName("size")]
            public int Size { get; set; }
        }


        public class Owner : User
        {
        }

        public class Gist
        {
            [JsonPropertyName("url")]
            public string Url { get; set; }

            [JsonPropertyName("forks_url")]
            public string ForksUrl { get; set; }

            [JsonPropertyName("commits_url")]
            public string CommitsUrl { get; set; }

            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("node_id")]
            public string NodeId { get; set; }

            [JsonPropertyName("git_pull_url")]
            public string GitPullUrl { get; set; }

            [JsonPropertyName("git_push_url")]
            public string GitPushUrl { get; set; }

            [JsonPropertyName("html_url")]
            public string HtmlUrl { get; set; }

            [JsonPropertyName("files")]
            public Dictionary<string, GistFile> Files { get; set; }

            [JsonPropertyName("public")]
            public bool Public { get; set; }

            [JsonPropertyName("created_at")]
            public DateTime CreatedAt { get; set; }

            [JsonPropertyName("updated_at")]
            public DateTime UpdatedAt { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("comments")]
            public int Comments { get; set; }

            [JsonPropertyName("user")]
            public User User { get; set; }

            [JsonPropertyName("comments_url")]
            public string CommentsUrl { get; set; }

            [JsonPropertyName("owner")]
            public Owner Owner { get; set; }

            [JsonPropertyName("truncated")]
            public bool Truncated { get; set; }

            [JsonPropertyName("forks")]
            public List<object> Forks { get; set; }

            [JsonPropertyName("history")]
            public List<object> History { get; set; }
        }
    }

}
