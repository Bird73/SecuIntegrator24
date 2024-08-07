namespace Birdsoft.SecuIntegrator24.DataAccessObject;

using Microsoft.EntityFrameworkCore;


public class FinanceDbContext : DbContext
{
    public DbSet<MonthlyRevenue> MonthlyRevenues { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=SecuIntegrator24.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the primary key for the MonthlyRevenue
        modelBuilder.Entity<MonthlyRevenue>()
            .HasKey(m => new { m.Code, m.Year, m.Month });

        base.OnModelCreating(modelBuilder);
    }
}