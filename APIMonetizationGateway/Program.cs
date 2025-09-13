using APIMonetizationGateway.jobs;
using APIMonetizationGateway.Middleware;
using APIMonetizationGateway.Services;

namespace APIMonetizationGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddScoped<ITierService, TierService>();
            builder.Services.AddScoped<IUsageService, UsageService>();

            builder.Services.AddHostedService<MonthlyUsageSummarizerService>();


            var app = builder.Build();

            app.UseMiddleware<CheckRateLimitMW>();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
