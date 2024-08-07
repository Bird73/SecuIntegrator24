namespace Birdsoft.SecuIntegrator24.DataAccessObject;

using Microsoft.EntityFrameworkCore;

/// <summary>
///     Represents the context for the tradings
/// </summary>
public class TradingDbContext : DbContext
{
    public DbSet<Trading> Tradings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=SecuIntegrator24.db");
    }

    /// <summary>
    ///     Configures the model for the tradings
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the primary key for the trading
        modelBuilder.Entity<Trading>()
            .HasKey(t => new { t.Code, t.TradingDate });

        base.OnModelCreating(modelBuilder);
    }

}