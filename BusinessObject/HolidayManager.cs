namespace Birdsoft.SecuIntegrator24.BusinessObject;

using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EventLogManager;
using Birdsoft.SecuIntegrator24.DataAccessObject;

using System.Text.RegularExpressions;

/// <summary>
/// Represents a holiday
/// </summary>
public struct Holiday
{
    /// <summary>
    /// The name of the holiday
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The date of the holiday
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// The description of the holiday
    /// </summary>
    public string Description { get; set; }
};

/// <summary>   
///     Manages the holidays
/// </summary>
public static class HolidayManager
{
    /// <summary>
    ///     Gets the list of holidays
    /// </summary>
    public static List<Holiday> Holidays { get; private set; }

    static HolidayManager()
    {
        Holidays = new List<Holiday>();
    }

    /// <summary>
    ///    Initializes the holiday manager
    /// </summary>
    public static void Initialize()
    {
        try
        {
            // Load the holidays from the database
            LoadHolidaysFromDatabase();

            // If there are no holidays, load them from the URL
            if (Holidays.Count == 0)
            {
                using (HttpClient client = new HttpClient())
                {
                    // Fetch the holidays from the URL
                    FetchHolidaysFromURL(DateTime.Now.Year, client);

                    // Save the holidays to the database
                    SaveHolidaysToDatabase(DateTime.Now.Year);
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception to the BusinessObject.EventLogManger
            EventLogManager.WriteEventLog(new EventLog
            {
                Message = ex.Message,
                Type = EventType.Error
            });
        }
    }

    /// <summary>
    ///     Gets the holidays for the specified year
    /// </summary>
    private static void LoadHolidaysFromDatabase()
    {
        try
        {
            // Load the holidays from the database
            HolidayDAO holidayDAO = new HolidayDAO();
            var holidays = holidayDAO.GetHolidays();

            // Convert the holidays from DataAccessObject.Holiday to BusinessObject.Holiday
            Holidays = holidays.Select(h => new Holiday
            {
                Name = h.Name,
                Date = h.Date,
                Description = h.Description as string ?? string.Empty
            }).ToList();
        }
        catch (Exception ex)
        {
            // Log the exception to the BusinessObject.EventLogManger
            EventLogManager.WriteEventLog(new EventLog
            {
                Message = ex.Message,
                Type = EventType.Error
            });
        }
    }

    /// <summary>
    ///     Saves the holidays to the database
    /// </summary>
    /// <param name="year"></param>
    private static void SaveHolidaysToDatabase(int year)
    {
        try
        {
            // Convert the holidays from BusinessObject.Holiday to DataAccessObject.Holiday
            // Add the holidays for specified year to the database if they do not exist
            HolidayDAO holidayDAO = new HolidayDAO();
            foreach (var holiday in Holidays.Where(h => h.Date.Year == year))
            {
                holidayDAO.AddHolidayIfNotExists(new DataAccessObject.Holiday
                {
                    Name = holiday.Name,
                    Date = holiday.Date,
                    Description = holiday.Description
                });
            }
        }
        catch (Exception ex)
        {
            // Log the exception to the BusinessObject.EventLogManger
            EventLogManager.WriteEventLog(new EventLog
            {
                Message = ex.Message,
                Type = EventType.Error
            });
        }
    }

    /// <summary>
    ///     Fetches the holidays from the URL
    /// </summary>
    /// <param name="year"></param>
    /// <param name="client"></param>
    private static void FetchHolidaysFromURL(int year, HttpClient client)
    {
        // Load the holidays from the URL
        // url = "https://www.tpex.org.tw/storage/zh-tw/web/bulletin/trading_date/trading_date_" + processYear - 1911 +".htm"
        string url = $"https://www.tpex.org.tw/storage/zh-tw/web/bulletin/trading_date/trading_date_{year - 1911}.htm";
        HttpResponseMessage response = client.GetAsync(url).Result;
        if (response.IsSuccessStatusCode)
        {
            string html = response.Content.ReadAsStringAsync().Result;
            ParseHolidaysFromHTML(year, html);     
        }
        else
        {
            // Log the exception to the BusinessObject.EventLogManger
            EventLogManager.WriteEventLog(new EventLog
            {
                Message = $"Failed to fetch holidays from {url}",
                Type = EventType.Error
            });
        }
    }

    /// <summary>
    ///     Parses the holidays from the HTML
    /// </summary>
    /// <param name="processYear"></param>
    /// <param name="html"></param>
    private static void ParseHolidaysFromHTML(int processYear, string html)
    {
        // Use regular expressions to match the holiday information in the HTML
        string pattern = @"<tr>\s*<td[^>]*left"">([\s\S]*?)</td>\s*?<td[^>]*?>([\s\S]*?)</td>\s*?<td[^>]*?>([\s\S]*?)</td>\s*<td[^>]*?>([\s\S]*?)<[\s\S]*?/tr>";
                    
        // Remove unnecessary characters from the HTML
        html = html.Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace("<strong>", "").Replace("</strong>", "").Replace("&nbsp;","");

        MatchCollection matches = Regex.Matches(html, pattern);

        // Iterate through the matches and extract the holiday information
        foreach (Match match in matches)
        {
            // 如果 match.Groups[0].Value 出現 "最後交易日" 或 match.Groups[4].Value 出現 "開始交易", 則不是假日
            if (!match.Groups[1].Value.Contains("最後交易日") && !match.Groups[4].Value.Contains("開始交易"))
            {
                Holiday holiday = new Holiday();
                        
                // Split the date string by "<br />"
                string[] date = match.Groups[2].Value.Split("<br />");
                foreach (string d in date)
                {
                    try
                    {
                        holiday.Name = match.Groups[1].Value.Trim();
                        holiday.Date = ParseDate(processYear, d);       // Combine the year and date string from "1月1日" to "2024/01/01"        
                        holiday.Description = match.Groups[4].Value.Trim();

                        Holidays.Add(holiday);
                    }
                    catch (Exception ex)
                    {
                        // Log the exception to the BusinessObject.EventLogManger
                        EventLogManager.WriteEventLog(new EventLog
                        {
                            Message = ex.Message,
                            Type = EventType.Error
                        });
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Parses the date string from "1月1日" to "2024/01/01"
    /// </summary>
    /// <param name="year"></param>
    /// <param name="dateString"></param>
    /// <returns></returns>
    private static DateTime ParseDate(int year, string dateString)
    {
        // Combine the year and date string from "1月1日" to "2024/01/01"
        string[] dateParts = dateString.Split(new char[] { '月', '日' }, StringSplitOptions.RemoveEmptyEntries);
        return new DateTime(year, int.Parse(dateParts[0]), int.Parse(dateParts[1]));
    }
}