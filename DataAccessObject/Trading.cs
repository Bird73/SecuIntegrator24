namespace Birdsoft.SecuIntegrator24.DataAccessObject;

using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EventLogManager;

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
///     Represents a trading
/// </summary>
public class Trading
{
    /// <summary>
    ///     The code of the stock (Primary Key)
    /// </summary>
    [Key, Column(Order = 0)]
    [Required]
    public string Code { get; set; } = "";

    /// <summary>
    ///     The date of the trading
    /// </summary>
    [Key, Column(Order = 1)]
    [Required]
    public DateTime TradingDate { get; set; } = DateTime.Today;

    /// <summary>
    ///     The market type of the stock
    /// </summary>
    public MarketType MarketType { get; set; } = MarketType.Listing;

    /// <summary>
    ///     The trading volume of the stock
    /// </summary>
    public decimal TradeVolume { get; set; } = 0;

    /// <summary>
    ///     The number of transactions of the stock
    /// </summary>
    public ulong Transaction { get; set; } = 0;

    /// <summary>
    ///     The trading value of the stock
    /// </summary>
    public decimal TradeValue { get; set; } = 0;

    /// <summary>
    ///     The opening price of the stock
    /// </summary>
    public decimal? OpeningPrice { get; set; }

    /// <summary>
    ///     The highest price of the stock
    /// </summary>
    public decimal? HighestPrice { get; set; }

    /// <summary>
    ///     The lowest price of the stock
    /// </summary>
    public decimal? LowestPrice { get; set; }

    /// <summary>
    ///     The closing price of the stock
    /// </summary>
    public decimal? ClosingPrice { get; set; }

    /// <summary>
    ///     The change indicator of the stock
    /// </summary>
    public char? ChangeIndicator { get; set; }

    /// <summary>
    ///     The change percentage of the stock
    /// </summary>
    public decimal Change { get; set; }

    /// <summary>
    ///     The change value of the stock
    /// </summary>
    public decimal ChangeValue { get; set; } = 0;

    /// <summary>
    ///     The final bid price of the stock
    /// </summary>
    public decimal? FinalBidPrice { get; set; }

    /// <summary>
    ///     The final bid volume of the stock
    /// </summary>
    public ulong? FinalBidVolume { get; set; }

    /// <summary>
    ///     The final ask price of the stock
    /// </summary>
    public decimal? FinalAskPrice { get; set; }

    /// <summary>
    ///     The final ask volume of the stock
    /// </summary>
    public ulong? FinalAskVolume { get; set; }

    /// <summary>
    ///     The P/E ratio of the stock
    /// </summary>
    public decimal PERatio { get; set; } = 0;
}

/// <summary>
///     Represents the type of the market
/// </summary>
public enum MarketType
{
    /// <summary>
    ///     The stock is listed in the stock exchange
    /// </summary>
    Listing,

    /// <summary>
    ///     The stock is traded over-the-counter
    /// </summary>
    OTC,

    /// <summary>
    ///     The stock is traded in the emerging market
    /// </summary>
    Emerging
}

/// <summary>
///     Represents the context for the tradings
/// </summary>
public class TradingContext : DbContext
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

public class TradingDAO
{
    /// <summary>
    ///     Gets the list of tradings
    /// </summary>
    /// <returns></returns>
    public List<Trading> GetTradings()
    {
        try
        {
            using (var db = new TradingContext())
            {
                return db.Tradings.ToList();
            }
        }
        catch (Exception ex)
        {
            EventLogManager.WriteEventLog(new EventLog
            {
                Type = EventType.Error,
                Message = $"TradingDAO.GetTradings Error: {ex.Message}"
            });

            return new List<Trading>();
        }
    }

    public void AddTradingIfNotExists(Trading trading)
    {
        try
        {
            using (var db = new TradingContext())
            {
                if (db.Tradings.Find(trading.Code, trading.TradingDate) == null)
                {
                    db.Tradings.Add(trading);
                    db.SaveChanges();
                }
            }
        }
        catch (Exception ex)
        {
            EventLogManager.WriteEventLog(new EventLog
            {
                Type = EventType.Error,
                Message = $"TradingDAO.AddTradingIfNotExists Stock Code-{trading.Code} Error: {ex.Message}"
            });
        }
    }

    public bool CheckTradingDataExists(DateTime date)
    {
        try
        {
            using (var db = new TradingContext())
            {
                return db.Tradings.Any(trading => trading.TradingDate == date);
            }
        }
        catch (Exception ex)
        {
            EventLogManager.WriteEventLog(new EventLog
            {
                Type = EventType.Error,
                Message = $"TradingDAO.CheckTradingDataExists Error: {ex.Message}"
            });

            return false;
        }
    }
}