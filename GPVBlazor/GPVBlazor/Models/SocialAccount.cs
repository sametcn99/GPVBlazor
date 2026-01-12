using System.Text.Json.Serialization;

namespace GPVBlazor.Models
{
    public class SocialAccount
    {
        [JsonPropertyName("provider")]
        public string? Provider { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }
}
