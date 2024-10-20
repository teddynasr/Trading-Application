using Microsoft.EntityFrameworkCore;
using PortfolioMicroService.Models;

namespace PortfolioMicroService.Data
{
    public class PortfolioDBContext : DbContext
    {
        public PortfolioDBContext(DbContextOptions<PortfolioDBContext> options) : base(options)
        {
        }

        public DbSet<UserPortfolio> UserPortfolios { get; set; }
        public DbSet<Position> Positions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder){
            modelBuilder.Entity<Position>()
            .HasOne<UserPortfolio>()
            .WithMany()
            .HasForeignKey(p => p.PortfolioID)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
