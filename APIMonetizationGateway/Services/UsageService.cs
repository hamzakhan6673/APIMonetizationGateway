using APIMonetizationGateway.Data;
using APIMonetizationGateway.helpers;
using APIMonetizationGateway.Models;
using System.Diagnostics;

namespace APIMonetizationGateway.Services
{

    public interface IUsageService
    {
        Task LogAPICallAsync(int customerId, HttpContext context, Stopwatch timer);
        Task<List<UsageLog>> GetUsageLogsForMonthAsync(DateTime month);
    }

    public class UsageService : IUsageService
    {
        public Task<List<UsageLog>> GetUsageLogsForMonthAsync(DateTime month)
        {
            var startDate = GatewayHelper.GetFirstDayOfMonth();
            var endDate = GatewayHelper.GetLastDayOfMonth();
            return Task.FromResult(GlobalDataHelper._usageLogs.Where(log => log.Timestamp >= startDate && log.Timestamp < endDate).ToList());
        }

        public async Task LogAPICallAsync(int customerId, HttpContext context, Stopwatch timer)
        {
            var usage = new UsageLog
            {
                CustomerId = customerId,
                UserId = context.Request.Headers["userId"].FirstOrDefault() ?? $"{customerId}:UserId",
                EndPoint = context.Request.Path,
                MethodType = context.Request.Method,
                Timestamp = DateTime.UtcNow,
                ReturnStatus = context.Response.StatusCode,
                LatencyMSeconds = (int)timer.ElapsedMilliseconds
            };


            // We can either SaveChangesAsync direct or
            // batch save in background by saving initialy in list for performance;
            // Currently, I'm doing direct saving to a list
            GlobalDataHelper._usageLogs.Add(usage);
        }
    }
}
