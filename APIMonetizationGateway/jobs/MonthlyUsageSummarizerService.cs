using APIMonetizationGateway.Data;
using APIMonetizationGateway.helpers;
using APIMonetizationGateway.Models;
using APIMonetizationGateway.Services;
using Microsoft.EntityFrameworkCore;

namespace APIMonetizationGateway.jobs
{
    public class MonthlyUsageSummarizerService : BackgroundService
    {
        private readonly IServiceProvider _sProvider;
        private readonly ILogger<MonthlyUsageSummarizerService> _logger;

        public MonthlyUsageSummarizerService(IServiceProvider sProvider,
            ILogger<MonthlyUsageSummarizerService> logger)
        {
            _sProvider = sProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var midnight = DateTime.UtcNow.Date.AddDays(1);
                var delay = midnight - now;

                if (delay.TotalMilliseconds > 0)
                {
                    _logger.LogInformation("Runs at midnight. Next run at: {Midnight}", midnight);
                    await Task.Delay(delay, stoppingToken);
                }

                try
                {
                    await AggregateMonthlyUsage(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in monthly billing summarizer");
                }
            }
        }

        private async Task AggregateMonthlyUsage(CancellationToken ct)
        {
            using var scope = _sProvider.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            var _tierService = scope.ServiceProvider.GetRequiredService<ITierService>();


            DateTime firstOfMonth = GatewayHelper.GetFirstDayOfMonth(),
                prevMonth = GatewayHelper.GetPreviousMonth();

            var monthKey = prevMonth.ToString("yyyy-MM");

            var summaries = GlobalDataHelper._usageLogs
                .Where(u => u.Timestamp >= prevMonth && u.Timestamp < firstOfMonth)
                .GroupBy(u => u.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    Total = g.LongCount()
                })
                .ToList();

            foreach (var s in summaries)
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == s.CustomerId, ct);
                if (customer == null) continue;

                var tier = await _tierService.GetByCustomerIdAsync(customer.Id);
                if (tier == null) continue;
                decimal price = tier.Price;

                var bill = new MonthlyBillingSummary
                {
                    CustomerId = s.CustomerId,
                    Month = monthKey,
                    TotalRequests = s.Total
                };
                GlobalDataHelper._monthlyBillingSummary.Add(bill);
            }
        }
    }
}
