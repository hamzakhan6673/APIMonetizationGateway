using APIMonetizationGateway.Models;
using System.Collections.Concurrent;

namespace APIMonetizationGateway.Data
{
    public static class GlobalDataHelper
    {
        public static readonly ConcurrentDictionary<string, (int Requests, DateTime LastReset)> _rateLimitStore = new();
        public static readonly ConcurrentDictionary<string, int> _monthlyQuotaStore = new();

        public static readonly ConcurrentBag<UsageLog> _usageLogs = new();
        public static readonly ConcurrentBag<MonthlyBillingSummary> _monthlyBillingSummary = new();


        public static readonly ConcurrentBag<Customer> _customers = new() {
            new Customer(){Id = 1, Name="Customer Free", TierId = 1},
            new Customer(){Id = 2, Name="Customer Pro", TierId = 2}
        };

        public static readonly ConcurrentBag<Tier> _tiers = new() {
            new Tier(){Id = 1, Name="Free", MonthlyQuota=100, Price=0,RateLimitPerSecond=2},
            new Tier(){Id = 2, Name="Pro", MonthlyQuota=100000, Price=50,RateLimitPerSecond=10}
        };
    }
}
