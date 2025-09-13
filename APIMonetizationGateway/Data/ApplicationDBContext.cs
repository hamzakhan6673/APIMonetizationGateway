using APIMonetizationGateway.Models;
using Microsoft.EntityFrameworkCore;

namespace APIMonetizationGateway.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Tier> Tiers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<UsageLog> UsageLogs { get; set; }
        public DbSet<MonthlyBillingSummary> MonthlyBillingSummaries { get; set; }
    }
}
