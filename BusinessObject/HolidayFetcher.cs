namespace Birdsoft.SecuIntegrator24.BusinessObject;

using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EventLogManager;
using System.Text.RegularExpressions;

/// <summary>
///     Fetches the holidays from the Internet
/// </summary>
public class HolidayFetcher()
{
    private HttpClient _client = new HttpClient();

    /// <summary>
    ///     Fetches the holidays from the URL
    /// </summary>
    /// <param name="year"></param>
    /// <param name="client"></param>
    public void FetchHolidaysFromURL(int year)
    {
        // Load the holidays from the URL
        // url = "https://www.tpex.org.tw/storage/zh-tw/web/bulletin/trading_date/trading_date_" + processYear - 1911 +".htm"
        string url = $"https://www.tpex.org.tw/storage/zh-tw/web/bulletin/trading_date/trading_date_{year - 1911}.htm";
        HttpResponseMessage response = _client.GetAsync(url).Result;
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
    private void ParseHolidaysFromHTML(int processYear, string html)
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

                        HolidayManager.Holidays.Add(holiday);
                    }
                    catch (Exception ex)
                    {
                        // Log the exception to the BusinessObject.EventLogManger
                        EventLogManager.WriteEventLog(new EventLog
                        {
                            Message = ex.Message,
                            Type = EventType.Error
                        });

                        EventLogManager.WriteEventLog(new EventLog
                        {
                            Message = d,
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
    private DateTime ParseDate(int year, string dateString)
    {
        // Combine the year and date string from "1月1日" to "2024/01/01"
        string[] dateParts = dateString.Split(new char[] { '月', '日' }, StringSplitOptions.RemoveEmptyEntries);
        return new DateTime(year, int.Parse(dateParts[0]), int.Parse(dateParts[1]));
    }
}