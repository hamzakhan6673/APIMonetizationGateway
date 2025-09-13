namespace APIMonetizationGateway.Models
{
    public class UsageLog
    {
        public long Id { get; set; }
        public int CustomerId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string EndPoint { get; set; } = string.Empty;
        public string MethodType { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public int ReturnStatus { get; set; }
        public int LatencyMSeconds { get; set; }
    }
}
