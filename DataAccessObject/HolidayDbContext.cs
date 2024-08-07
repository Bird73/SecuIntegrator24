namespace Birdsoft.SecuIntegrator24.DataAccessObject;

using Microsoft.EntityFrameworkCore;

/// <summary>
///     Represents the context for the holidays
/// </summary>
public class HolidayDbContext : DbContext
{
    public DbSet<Holiday> Holidays { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=SecuIntegrator24.db");
    }
}