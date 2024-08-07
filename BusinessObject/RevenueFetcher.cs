
namespace Birdsoft.SecuIntegrator24.BusinessObject;

using Birdsoft.SecuIntegrator24.DataAccessObject;
using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EventLogManager;

/// <summary>
///     The class to fetch the revenue data from the web
/// </summary>
public class RevenueFetcher
{
    private HttpClient _client = new HttpClient();

    /// <summary>
    ///     Fetch the listing revenue data from the web
    /// </summary>
    /// <param name="processDate"></param>
    /// <returns></returns>
    public bool FetchListingRevenueDataFromWeb(DateTime processDate)
    {
        // year = year - 1911
        string url = $"https://mops.twse.com.tw/nas/t21/sii/t21sc03_{processDate.Year - 1911}_{processDate.Month}.csv";
        HttpResponseMessage response = _client.GetAsync(url).Result;
        if (response.IsSuccessStatusCode)
        {
            string csv = response.Content.ReadAsStringAsync().Result;
            File.WriteAllText(Path.Combine("Data", "MonthlyRevenue", "Listing_" + processDate.ToString("yyyyMM") + ".csv"), csv);
            return true;
        }
        else
        {
            EventLogManager.WriteEventLog(new EventLog
            {
                Message = $"Failed to fetch listing revenue data from {url}",
                Type = EventType.Error
            });

            return false;
        }
    }

    /// <summary>
    ///     Fetch the listing revenue data from the CSV file
    /// </summary>
    /// <param name="processDate"></param>
    /// <returns></returns>
    public List<RevenueData> FetchListingRevenueDataFromCSV(DateTime processDate)
    {
        List<RevenueData> revenueData = new List<RevenueData>();
        string filePath = Path.Combine("Data", "MonthlyRevenue", "Listing_" + processDate.ToString("yyyyMM") + ".csv");
        if (File.Exists(filePath))
        {
            // Skip the first line which is the header
            string[] lines = File.ReadAllLines(filePath);
            for (int i = 1; i < lines.Length; i++)
            {
                try
                {
                    // Split the line by comma
                    string[] columns = lines[i].Split(',');
                    RevenueData data = new RevenueData();

                    data.Code = columns[2].Replace("\"", "");
                    data.Year = processDate.Year;
                    data.Month = processDate.Month;
                    data.MarketType = DataAccessObject.MarketType.Listing;
                    data.CurrentMonthRevenue = decimal.Parse(columns[5].Replace("\"", "").Replace(",", "") == "" ? "0" : columns[5].Replace("\"", "").Replace(",", ""));
                    data.PreviousMonthRevenue = decimal.Parse(columns[6].Replace("\"", "").Replace(",", "") == "" ? "0" : columns[6].Replace("\"", "").Replace(",", ""));
                    data.LastYearMonthRevenue = decimal.Parse(columns[7].Replace("\"", "").Replace(",", "") == "" ? "0" : columns[7].Replace("\"", "").Replace(",", ""));
                    data.MonthOverMonthChangePercentage = decimal.Parse(columns[8].Replace("\"", "").Replace(",", "") == "" ? "0" : columns[8].Replace("\"", "").Replace(",", ""));
                    data.YearOverYearChangePercentage = decimal.Parse(columns[9].Replace("\"", "").Replace(",", "") == "" ? "0" : columns[9].Replace("\"", "").Replace(",", ""));
                    data.CurrentMonthCumulativeRevenue = decimal.Parse(columns[10].Replace("\"", "").Replace(",", "") == "" ? "0" : columns[10].Replace("\"", "").Replace(",", ""));
                    data.LastYearCumulativeRevenue = decimal.Parse(columns[11].Replace("\"", "").Replace(",", "") == "" ? "0" : columns[11].Replace("\"", "").Replace(",", ""));
                    data.CumulativeChangePercentage = decimal.Parse(columns[12].Replace("\"", "").Replace(",", "") == "" ? "0" : columns[12].Replace("\"", "").Replace(",", ""));
                    data.Comments = columns[13].Replace("\"", "");

                    revenueData.Add(data);
                }
                catch (Exception ex)
                {
                    EventLogManager.WriteEventLog(new EventLog
                    {
                        Message = ex.Message,
                        Type = EventType.Error
                    });
                }
            }
        }
        return revenueData;
    }

    /// <summary>
    ///     Fetch the OTC revenue data from the web
    /// </summary>
    /// <param name="processDate"></param>
    /// <returns></returns>
    public bool FetchOTCRevenueDataFromWeb(DateTime processDate)
    {
        // year = year - 1911
        string url = $"https://mops.twse.com.tw/nas/t21/otc/t21sc03_{processDate.Year - 1911}_{processDate.Month}.csv";
        HttpResponseMessage response = _client.GetAsync(url).Result;
        if (response.IsSuccessStatusCode)
        {
            string csv = response.Content.ReadAsStringAsync().Result;
            File.WriteAllText(Path.Combine("Data", "MonthlyRevenue", "OTC_" + processDate.ToString("yyyyMM") + ".csv"), csv);
            return true;
        }
        else
        {
            EventLogManager.WriteEventLog(new EventLog
            {
                Message = $"Failed to fetch OTC revenue data from {url}",
                Type = EventType.Error
            });

            return false;
        }
    }

    public List<RevenueData> FetchOTCRevenueDataFromCSV(DateTime processDate)
    {
        List<RevenueData> revenueData = new List<RevenueData>();
        string filePath = Path.Combine("Data", "MonthlyRevenue", "OTC_" + processDate.ToString("yyyyMM") + ".csv");
        if (File.Exists(filePath))
        {
            // Skip the first line which is the header
            string[] lines = File.ReadAllLines(filePath);
            for (int i = 1; i < lines.Length; i++)
            {
                try
                {
                    // Split the line by comma
                    string[] columns = lines[i].Split(',');
                    RevenueData data = new RevenueData();

                    data.Code = columns[2].Replace("\"", "");
                    data.Year = processDate.Year;
                    data.Month = processDate.Month;
                    data.MarketType = DataAccessObject.MarketType.OTC;
                    data.CurrentMonthRevenue = decimal.Parse(columns[5].Replace("\"", "").Replace(",", "") == "" ? "0" : columns[5].Replace("\"", "").Replace(",", ""));
                    data.PreviousMonthRevenue = decimal.Parse(columns[6].Replace("\"", "").Replace(",", "") == "" ? "0" : columns[6].Replace("\"", "").Replace(",", ""));
                    data.LastYearMonthRevenue = decimal.Parse(columns[7].Replace("\"", "").Replace(",", "") == "" ? "0" : columns[7].Replace("\"", "").Replace(",", ""));
                    data.MonthOverMonthChangePercentage = decimal.Parse(columns[8].Replace("\"", "").Replace(",", "") == "" ? "0" : columns[8].Replace("\"", "").Replace(",", ""));
                    data.YearOverYearChangePercentage = decimal.Parse(columns[9].Replace("\"", "").Replace(",", "") == "" ? "0" : columns[9].Replace("\"", "").Replace(",", ""));
                    data.CurrentMonthCumulativeRevenue = decimal.Parse(columns[10].Replace("\"", "").Replace(",", "") == "" ? "0" : columns[10].Replace("\"", "").Replace(",", ""));
                    data.LastYearCumulativeRevenue = decimal.Parse(columns[11].Replace("\"", "").Replace(",", "") == "" ? "0" : columns[11].Replace("\"", "").Replace(",", ""));
                    data.CumulativeChangePercentage = decimal.Parse(columns[12].Replace("\"", "").Replace(",", "") == "" ? "0" : columns[12].Replace("\"", "").Replace(",", ""));
                    data.Comments = columns[13].Replace("\"", "");

                    revenueData.Add(data);
                }
                catch (Exception ex)
                {
                    EventLogManager.WriteEventLog(new EventLog
                    {
                        Message = ex.Message,
                        Type = EventType.Error
                    });
                }
            }
        }
        return revenueData;
    }

    /// <summary>
    ///     Save the revenue data to the database
    /// </summary>
    /// <param name="revenueData"></param>
    /// <param name="processDate"></param>
    /// <param name="marketType"></param>
    public void SaveRevenueToDatabase(List<RevenueData> revenueData, DateTime processDate, DataAccessObject.MarketType marketType)
    {
        try
        {
            MonthlyRevenueDAO monthlyRevenueDAO = new MonthlyRevenueDAO();

            foreach (RevenueData data in revenueData)
            {
                MonthlyRevenue revenue = new MonthlyRevenue
                {
                    Code = data.Code,
                    Year = data.Year,
                    Month = data.Month,
                    MarketType = data.MarketType,
                    CurrentMonthRevenue = data.CurrentMonthRevenue,
                    PreviousMonthRevenue = data.PreviousMonthRevenue,
                    LastYearMonthRevenue = data.LastYearMonthRevenue,
                    MonthOverMonthChangePercentage = data.MonthOverMonthChangePercentage,
                    YearOverYearChangePercentage = data.YearOverYearChangePercentage,
                    CurrentMonthCumulativeRevenue = data.CurrentMonthCumulativeRevenue,
                    LastYearCumulativeRevenue = data.LastYearCumulativeRevenue,
                    CumulativeChangePercentage = data.CumulativeChangePercentage,
                    Comments = data.Comments
                };

                monthlyRevenueDAO.AddMonthlyRevenueIfNotExist(revenue);
            }
        }
        catch (Exception ex)
        {
            EventLogManager.WriteEventLog(new EventLog
            {
                Message = ex.Message,
                Type = EventType.Error
            });
        }
    }
}

public class RevenueData
{
    /// <summary>
    ///     The code of the company
    /// </summary>
    public string Code { get; set; } = "";

    /// <summary>
    ///     The year of the revenue
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    ///     The month of the revenue
    /// </summary>
    public int Month { get; set; }
    
    /// <summary>
    ///     The market type of the company
    /// </summary>
    public DataAccessObject.MarketType MarketType { get; set; }

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
    ///    備註
    ///    Comments
    /// </summary>
    public string? Comments { get; set; }
}