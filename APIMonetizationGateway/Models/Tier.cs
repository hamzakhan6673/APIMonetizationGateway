namespace APIMonetizationGateway.Models
{
    public class Tier
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public long MonthlyQuota { get; set; }
        public int RateLimitPerSecond { get; set; }
        public decimal Price { get; set; }
    }
}
