using System.Text.Json.Serialization;

namespace GPVBlazor.Models
{
    public class Organization
    {
        [JsonPropertyName("login")]
        public string? Login { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("avatar_url")]
        public string? AvatarUrl { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
