namespace GPVBlazor.Models
{
    public class ChartModel
    {
        public string type { get; set; } = string.Empty;

        public string label { get; set; } = string.Empty;

        public string chartId { get; set; } = string.Empty;

        public string[] labels { get; set; } = Array.Empty<string>();

        public ChartDataset[] datasets { get; set; } = Array.Empty<ChartDataset>();

        public class ChartDataset
        {
            public string label { get; set; } = string.Empty;

            public int[] data { get; set; } = Array.Empty<int>();

            public string[] backgroundColor { get; set; } = Array.Empty<string>();
        }
    }
}
