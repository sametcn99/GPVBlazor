namespace GPVBlazor.Models
{
    public class RateResource
    {
        public int Limit { get; set; }
        public int Remaining { get; set; }
        public long Reset { get; set; }
    }

    public class RateLimitResources
    {
        public RateResource Core { get; set; } = new RateResource();
        public RateResource Search { get; set; } = new RateResource();
        // add other resources if needed
    }

    public class RateLimitInfo
    {
        public RateLimitResources Resources { get; set; } = new RateLimitResources();
        public string? Rate { get; set; }
    }
}
