using APIMonetizationGateway.Data;

namespace APIMonetizationGateway.helpers
{
    public static class GatewayHelper
    {
        public static (bool, int) CheckPerSecondRateLimit(int customerId, int limit)
        {
            var now = DateTime.UtcNow;
            var key = $"{customerId}:per-second-rate-limit";
            var (requests, lastReset) = GlobalDataHelper._rateLimitStore.GetOrAdd(key, (0, now));

            // Reset counter if a new second has started
            if ((now - lastReset).TotalSeconds >= 1)
            {
                requests = 0;
                lastReset = now;
            }

            // Return false if limit has reached
            if (requests >= limit) return (false, requests);

            GlobalDataHelper._rateLimitStore[key] = (requests + 1, lastReset);
            return (true, requests + 1);
        }

        public static (bool, int) CheckPerMonthQuota(int customerId, long limit)
        {
            var key = $"{customerId}:month:{DateTime.UtcNow:yyyy-MM}";
            var requests = GlobalDataHelper._monthlyQuotaStore.GetOrAdd(key, 0);

            if (requests >= limit && limit > 0) return (false, requests);

            GlobalDataHelper._monthlyQuotaStore[key] = requests + 1;
            return (true, requests + 1);
        }

        public static string GetRetryAfterValue(RequestType type)
        {
            var now = DateTime.UtcNow;
            string value = type switch
            {
                RequestType.PerSecond => "1",
                RequestType.PerMonth => ((DateTime.DaysInMonth(now.Year, now.Month) - now.Day) * 24 * 60 * 60).ToString(),
                _ => "3600"
            };
            return value;
        }

        public static DateTime GetFirstDayOfMonth() => new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        public static DateTime GetLastDayOfMonth() => GetFirstDayOfMonth().AddMonths(1);
        public static DateTime GetPreviousMonth() => GetFirstDayOfMonth().AddMonths(-1);

        public enum RequestType
        {
            PerSecond,
            PerMonth
        }
    }
}
