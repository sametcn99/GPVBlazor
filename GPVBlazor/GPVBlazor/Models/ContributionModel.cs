using System.Text.Json.Serialization;

namespace GPVBlazor.Models
{
    public class ContributionResponse
    {
        [JsonPropertyName("total")]
        public Dictionary<string, int>? Total { get; set; }

        [JsonPropertyName("contributions")]
        public List<ContributionDay>? Contributions { get; set; }
    }

    public class ContributionDay
    {
        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("level")]
        public int Level { get; set; }
    }
}
