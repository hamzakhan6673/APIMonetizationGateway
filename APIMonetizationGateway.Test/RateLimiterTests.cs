using APIMonetizationGateway.Middleware;
using APIMonetizationGateway.Services;
using Microsoft.AspNetCore.Http;
using System.Net;
using Xunit;

namespace APIMonetizationGateway.Test
{
    public class RateLimiterTests : IClassFixture<ServiceFixture>
    {
        private readonly string requestPath = "/WeatherForecast";

        private readonly ITierService _tierService;
        private readonly IUsageService _usageService;

        public RateLimiterTests(ServiceFixture fixture)
        {
            _tierService = fixture.TierService;
            _usageService = fixture.UsageService;
        }

        [Fact]
        public async Task ExceedsSecondLimit_Returns429()
        {
            var middleware = new CheckRateLimitMW(next: (innerHttpContext) =>
            {
                innerHttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                return Task.CompletedTask;
            });

            var context = new DefaultHttpContext();
            context.Request.Path = requestPath;
            context.Request.Headers["customerId"] = "1";

            // Act & Assert - Make requests within the limit
            for (int i = 0; i < 2; i++) // Free tier rate limit is 2/sec
            {
                await middleware.InvokeAsync(context, _tierService, _usageService);
                Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
            }
        }

        [Fact]
        public async Task RateLimit_BlocksRequestsExceedingLimit()
        {
            // Arrange
            var middleware = new CheckRateLimitMW(next: (innerHttpContext) =>
            {
                innerHttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                return Task.CompletedTask;
            });
            var context = new DefaultHttpContext();
            context.Request.Path = requestPath;
            context.Request.Headers["customerId"] = "1";

            // Act - Make 2 requests, which are within the limit for the free tier
            await middleware.InvokeAsync(context, _tierService, _usageService);
            await middleware.InvokeAsync(context, _tierService, _usageService);

            // Assert - The 3rd request should be blocked
            await middleware.InvokeAsync(context, _tierService, _usageService);
            Assert.Equal((int)HttpStatusCode.TooManyRequests, context.Response.StatusCode);
        }

        [Fact]
        public async Task MonthlyQuota_AllowsRequestsWithinLimit()
        {
            // Arrange
            var middleware = new CheckRateLimitMW(next: (innerHttpContext) =>
            {
                innerHttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                return Task.CompletedTask;
            });
            var context = new DefaultHttpContext();
            context.Request.Path = requestPath;
            context.Request.Headers["customerId"] = "1";

            // Act & Assert - Make requests up to the free tier's monthly quota (100)
            for (int i = 0; i < 100; i++)
            {
                await middleware.InvokeAsync(context, _tierService, _usageService);
                Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
            }
        }

        [Fact]
        public async Task MonthlyQuota_BlocksRequestsExceedingLimit()
        {
            // Arrange
            var middleware = new CheckRateLimitMW(next: (innerHttpContext) =>
            {
                innerHttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                return Task.CompletedTask;
            });
            var context = new DefaultHttpContext();
            context.Request.Path = requestPath;
            context.Request.Headers["customerId"] = "1";

            // Act - Make requests up to the free tier's monthly quota (100)
            for (int i = 0; i < 100; i++)
            {
                await middleware.InvokeAsync(context, _tierService, _usageService);
            }

            // Assert - The 101st request should be blocked
            await middleware.InvokeAsync(context, _tierService, _usageService);
            Assert.Equal((int)HttpStatusCode.PaymentRequired, context.Response.StatusCode);
        }
    }
}
