namespace Birdsoft.SecuIntegrator24.DataAccessObject;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class MonthlyRevenue
{
    /// <summary>
    ///     The code of the company
    /// </summary>
    [Key, Column(Order = 0)]
    public string Code { get; set; } = "";

    /// <summary>
    ///     The year of the revenue
    /// </summary>
    [Key, Column(Order = 1)]
    public int Year { get; set; }

    /// <summary>
    ///     The month of the revenue
    /// </summary>
    [Key, Column(Order = 2)]
    public int Month { get; set; }
    
    /// <summary>
    ///     The market type of the company
    /// </summary>
    public MarketType MarketType { get; set; }

    /// <summary>
    ///     當月營收
    ///     The revenue of the company
    /// </summary>
    public decimal CurrentMonthRevenue { get; set; }

    /// <summary>
    ///     上月營收
    ///     The revenue of the previous month
    /// </summary>
    public decimal PreviousMonthRevenue { get; set; }

    /// <summary>
    ///     去年當月營收
    ///     The revenue of the same month last year
    /// </summary>
    public decimal LastYearMonthRevenue { get; set; }

    /// <summary>
    ///     上月比較增減(%)
    ///     The percentage of the month-over-month change
    /// </summary>
    public decimal MonthOverMonthChangePercentage { get; set; }

    /// <summary>
    ///     去年同月增減(%)
    ///     The percentage of the year-over-year change
    /// </summary>
    public decimal YearOverYearChangePercentage { get; set; }

    /// <summary>
    ///    當月累計營收
    ///    The cumulative revenue of the company
    /// </summary>
    public decimal CurrentMonthCumulativeRevenue { get; set; }

    /// <summary>
    ///     去年累計營收
    ///     The cumulative revenue of the same period last year
    /// </summary>
    public decimal LastYearCumulativeRevenue { get; set; }

    /// <summary>
    ///     累計增減(%)
    ///     The percentage of the cumulative change
    /// </summary>
    public decimal CumulativeChangePercentage { get; set; }

    /// <summary>
    ///     備註
    ///     The comments of the monthly revenue
    /// </summary>
    public string? Comments { get; set; }
}

/// <summary>
///     The data access object of the monthly revenue
/// </summary>
public class  MonthlyRevenueDAO
{
    /// <summary>
    ///     Add a monthly revenue if it does not exist
    /// </summary>
    /// <param name="monthlyRevenue"></param>
    public void AddMonthlyRevenueIfNotExist(MonthlyRevenue monthlyRevenue)
    {
        using (var context = new FinanceDbContext())
        {
            var existingMonthlyRevenue = context.MonthlyRevenues
                .Where(m => m.Code == monthlyRevenue.Code)
                .Where(m => m.Year == monthlyRevenue.Year)
                .Where(m => m.Month == monthlyRevenue.Month)
                .FirstOrDefault();

            if (existingMonthlyRevenue == null)
            {
                context.MonthlyRevenues.Add(monthlyRevenue);
                context.SaveChanges();
            }
        }

    }
}