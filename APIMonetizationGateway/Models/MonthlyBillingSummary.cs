namespace APIMonetizationGateway.Models
{
    public class MonthlyBillingSummary
    {
        public long Id { get; set; }
        public int CustomerId { get; set; }
        /// <summary>
        /// Format: "YYYY-MM"
        /// </summary>
        public string Month { get; set; } = string.Empty;
        public long TotalRequests { get; set; }
    }
}
