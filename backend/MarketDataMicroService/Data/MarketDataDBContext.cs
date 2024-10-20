using Microsoft.EntityFrameworkCore;
using MarketDataMicroService.Models;

namespace MarketDataMicroService.Data
{
    public class MarketDataDBContext : DbContext
    {
        public MarketDataDBContext(DbContextOptions<MarketDataDBContext> options)
            : base(options)
        {
        }

        public DbSet<CurrencyPair> CurrencyPairs { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {}
    }
}
