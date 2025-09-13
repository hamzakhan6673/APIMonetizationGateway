using APIMonetizationGateway.Services;
using System.Diagnostics;
using System.Net;
using static APIMonetizationGateway.helpers.GatewayHelper;

namespace APIMonetizationGateway.Middleware
{
    public class CheckRateLimitMW
    {
        private readonly RequestDelegate _next;

        public CheckRateLimitMW(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITierService _tierService, IUsageService _usageService)
        {
            if (context.Request.Path.HasValue && context.Request.Path.Value.IndexOf("swagger") > -1)
                await _next(context);
            else
            {
                int customerId = 0;
                if (!int.TryParse(context.Request.Headers["customerId"].FirstOrDefault(), out customerId))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Missing or invalid Customer Id");
                    return;
                }

                var tier = await _tierService.GetByCustomerIdAsync(customerId);
                if (tier is null)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Tier not found");
                    return;
                }


                // Per second requests rate limiting
                (bool isAllowed, int totalRequests) = CheckPerSecondRateLimit(customerId, tier.RateLimitPerSecond);
                if (!isAllowed)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    context.Response.Headers["Retry-After"] = GetRetryAfterValue(RequestType.PerSecond);
                    await context.Response.WriteAsync("Per second rate limit exceeded. Try again after 1 second.");
                    return;
                }

                // Per month quota checking
                var (isMonthlyAllowed, totalMonthlyRequests) = CheckPerMonthQuota(customerId, tier.MonthlyQuota);
                if (!isMonthlyAllowed)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    context.Response.Headers["Retry-After"] = GetRetryAfterValue(RequestType.PerMonth);
                    await context.Response.WriteAsync("Monthly quota reached.");
                    return;
                }


                // Proceed further
                var _timer = Stopwatch.StartNew();

                try
                {
                    await _next(context);
                }
                finally
                {
                    // Once api is successful, log the successful API call
                    _timer.Stop();

                    var status = context.Response.StatusCode;
                    if (status >= 200 && status < 300)
                    {
                        await _usageService.LogAPICallAsync(customerId, context, _timer);
                    }

                    Console.WriteLine($"API call logged for customer {customerId}. Current usage: {totalMonthlyRequests}.");
                }
            }
        }
    }
}
