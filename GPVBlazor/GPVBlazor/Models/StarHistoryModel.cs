using System.Text.Json.Serialization;

namespace GPVBlazor.Models
{
    public class StarHistoryPoint
    {
        [JsonPropertyName("starred_at")]
        public DateTime StarredAt { get; set; }
    }

    public class StarHistory
    {
        public List<StarHistoryPoint> Points { get; set; } = new();
        public Dictionary<string, int> MonthlyData { get; set; } = new();
    }
}