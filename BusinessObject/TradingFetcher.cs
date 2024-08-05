namespace Birdsoft.SecuIntegrator24.BusinessObject;

using System;
using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EnvironmentManager;
using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EventLogManager;
using Birdsoft.SecuIntegrator24.DataAccessObject;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

/// <summary>
///     Fetches the trading data of the stock market from the Internet
/// </summary>
public class TradingFetcher
{
    private HttpClient _client = new HttpClient();

    /// <summary>
    ///     Fetches the trading data from the URL
    /// </summary>
    /// <param name="processDate"></param>
    public bool FetchListingTradingDataFromWeb(DateTime processDate)
    {
        bool isDownloaded = true;

        // Fetch the daily trading data from the URL :https://www.twse.com.tw/rwd/zh/afterTrading/MI_INDEX?date=20240719&type=ALLBUT0999&response=json&_=1721547765168
        string url = "https://www.twse.com.tw/rwd/zh/afterTrading/MI_INDEX?date=" + processDate.ToString("yyyyMMdd") + "&type=ALLBUT0999&response=json";

        // file path to save the json data
        string filePath = Path.Combine("Data", "Trading", processDate.Year.ToString(), "Listing_" + processDate.ToString("yyyyMMdd") + ".json");

        // Download the data from the URL and save it if the file does not exist
        if (!File.Exists(filePath))
        {
            try
            {
                HttpResponseMessage response = _client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    string json = response.Content.ReadAsStringAsync().Result;                  

                    // Create the directory if it does not exist
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? Path.Combine("Data", "Trading", processDate.Year.ToString()));

                    // Save json to file
                    File.WriteAllText(filePath, json);
                }
                else
                {
                    // Log the exception to the BusinessObject.EventLogManger
                    EventLogManager.WriteEventLog(new EventLog
                    {
                        Message = "Failed to fetch trading data from URL: " + url,
                        Type = EventType.Error
                    });

                    isDownloaded = false;
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

                isDownloaded = false;
            }
        }

        return isDownloaded;
    }

    public List<TradingData> FetchListingTradingDataFromJSON(DateTime processDate)
    {
        // file path to save the json data
        string filePath = Path.Combine("Data", "Trading", processDate.Year.ToString(), "Listing_" + processDate.ToString("yyyyMMdd") + ".json");

        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);

                // Check if the json contains "沒有符合條件的資料", which means no data is available
                if (json.Contains("沒有符合條件的資料"))
                {
                    return new List<TradingData>();
                }
                
                return ParseListingJsonData(json);
            }
            catch (Exception ex)
            {
                // Log the exception to the BusinessObject.EventLogManger
                EventLogManager.WriteEventLog(new EventLog
                {
                    Message = $"Failed to fetch listing trading data from file: {filePath} - error: {ex.Message}",
                    Type = EventType.Error
                });
            }
        }

        return new List<TradingData>();
    }

    private List<TradingData> ParseListingJsonData(string json)
    {
        List<TradingData> returnData = new List<TradingData>();
        try
        {    
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement root = doc.RootElement;
                JsonElement tables = root.GetProperty("tables");

                foreach (JsonElement table in tables.EnumerateArray())
                {
                    string title = table.GetProperty("title").GetString() ?? "";
                    if (title.Contains("每日收盤行情"))
                    {
                        // Parse the fields
                        List<string> fields = new List<string>();
                        foreach (JsonElement field in table.GetProperty("fields").EnumerateArray())
                        {
                            fields.Add(field.GetString() ?? "");
                        }

                        // Parse the data
                        foreach (JsonElement row in table.GetProperty("data").EnumerateArray())
                        {
                            // 透過 fields 的順序來取得資料 放入 returnData
                            TradingData tradingData = new TradingData();
                            int index = 0;

                            foreach (JsonElement field in row.EnumerateArray())
                            {
                                string fieldValue = field.GetString() ?? "";

                                switch (fields[index])
                                {
                                    case "證券代號":
                                        tradingData.Code = fieldValue;
                                        break;
                                    case "證券名稱":
                                        tradingData.Name = fieldValue;
                                        break;
                                    case "成交股數":
                                        tradingData.TradeVolume = fieldValue;
                                        break;
                                    case "成交筆數":
                                        tradingData.Transaction = fieldValue;
                                        break;
                                    case "成交金額":
                                        tradingData.TradeValue = fieldValue;
                                        break;
                                    case "開盤價":
                                        tradingData.OpeningPrice = fieldValue;
                                        break;
                                    case "最高價":
                                        tradingData.HighestPrice = fieldValue;
                                        break;
                                    case "最低價":
                                        tradingData.LowestPrice = fieldValue;
                                        break;
                                    case "收盤價":
                                        tradingData.ClosingPrice = fieldValue;
                                        break;
                                    case "漲跌(+/-)":
                                        tradingData.ChangeIndicator = fieldValue;
                                        break;
                                    case "漲跌價差":
                                        tradingData.Change = fieldValue;
                                        break;
                                    case "最後揭示買價":
                                        tradingData.FinalBidPrice = fieldValue;
                                        break;
                                    case "最後揭示買量":
                                        tradingData.FinalBidVolume = fieldValue;
                                        break;
                                    case "最後揭示賣價":
                                        tradingData.FinalAskPrice = fieldValue;
                                        break;
                                    case "最後揭示賣量":
                                        tradingData.FinalAskVolume = fieldValue;
                                        break;
                                    case "本益比":
                                        tradingData.PERatio = fieldValue;
                                        break;
                                }

                                index++;
                            }

                            returnData.Add(tradingData);
                        }

                        break;
                    }
                }
            } 
        }
        catch (Exception ex)
        {
            // Log the exception to the BusinessObject.EventLogManger
            EventLogManager.WriteEventLog(new EventLog
            {
                Message = "Failed to parse listing JSON data: " + ex.Message,
                Type = EventType.Error
            });
        }

        return returnData;
    }

    /// <summary>
    ///     Fetches the trading data of the OTC market from the Internet
    /// </summary>
    /// <param name="processDate"></param>
    public bool FetchOTCTradingDataFromWeb(DateTime processDate)
    {
        bool isDownloaded = true;

        // Fetch the daily trading data from the URL : https://www.tpex.org.tw/web/stock/aftertrading/daily_close_quotes/stk_quote_result.php?l=zh-tw&d=113/08/01
        string url = "https://www.tpex.org.tw/web/stock/aftertrading/daily_close_quotes/stk_quote_result.php?l=zh-tw&d=" + (processDate.Year - 1911) + "/" + processDate.ToString("MM/dd");

        // file path to save the json data
        string filePath = Path.Combine("Data", "Trading", processDate.Year.ToString(), "OTC_" + processDate.ToString("yyyyMMdd") + ".json");

        // Download the data from the URL and save it if the file does not exist
        if (!File.Exists(filePath))
        {
            try
            {
                HttpResponseMessage response = _client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    string json = response.Content.ReadAsStringAsync().Result;

                    // Create the directory if it does not exist
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? Path.Combine("Data", "Trading", processDate.Year.ToString()));

                    // convert unicode to string ex: \u4e0a\u6ac3 -> 上橳
                    json = Regex.Unescape(json);

                    // Save json to file
                    File.WriteAllText(filePath, json);
                }
                else
                {
                    // Log the exception to the BusinessObject.EventLogManger
                    EventLogManager.WriteEventLog(new EventLog
                    {
                        Message = "Failed to fetch OTC trading data from URL: " + url,
                        Type = EventType.Error
                    });

                    isDownloaded = false;
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

                isDownloaded = false;
            }
        }

        return isDownloaded;
    }

    public List<TradingData> FetchOTCTradingDataFromJSON(DateTime processDate)
    {
        // file path to save the json data
        string filePath = Path.Combine("Data", "Trading", processDate.Year.ToString(), "OTC_" + processDate.ToString("yyyyMMdd") + ".json");

        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);

                // Check if the json contains ""iTotalRecords": 0", which means no data is available
                if (json.Contains("\"iTotalRecords\": 0"))
                {
                    return new List<TradingData>();
                }
                
                return ParseOTCJsonData(json);
            }
            catch (Exception ex)
            {
                // Log the exception to the BusinessObject.EventLogManger
                EventLogManager.WriteEventLog(new EventLog
                {
                    Message = $"Failed to fetch OTC trading data from file: {filePath} - error: {ex.Message}",
                    Type = EventType.Error
                });
            }
        }

        return new List<TradingData>();
    }

    private List<TradingData> ParseOTCJsonData(string json)
    {
        List<TradingData> returnData = new List<TradingData>();
        try
        {    
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement root = doc.RootElement;
                JsonElement tables = root.GetProperty("aaData");

                foreach (JsonElement row in tables.EnumerateArray())
                {
                    // 透過 fields 的順序來取得資料 放入 returnData
                    TradingData tradingData = new TradingData();
                    int index = 0;

                    foreach (JsonElement field in row.EnumerateArray())
                    {
                        // field.GetString() will return null if the field is empty
                        // trim() if the field is not empty
                        string fieldValue = field.GetString()?.Trim() ?? "";
                        fieldValue = fieldValue == "---" ? "--" : fieldValue;   // Replace "---" with "--"

                        switch (index)
                        {
                            case 0:
                                tradingData.Code = fieldValue;
                                break;
                            case 1:
                                tradingData.Name = fieldValue;
                                break;
                            case 2:
                                tradingData.ClosingPrice = fieldValue;
                                break;
                            case 3:
                                tradingData.Change = fieldValue;
                                break;
                            case 4:
                                tradingData.OpeningPrice = fieldValue;
                                break;
                            case 5:
                                tradingData.HighestPrice = fieldValue;
                                break;
                            case 6:
                                tradingData.LowestPrice = fieldValue;
                                break;
                            case 7:
                                tradingData.AveragePrice = fieldValue;
                                break;
                            case 8:
                                tradingData.TradeVolume = fieldValue;
                                break;
                            case 9:
                                tradingData.TradeValue = fieldValue;
                                break;
                            case 10:
                                tradingData.Transaction = fieldValue;
                                break;
                            case 11:
                                tradingData.FinalBidPrice = fieldValue;
                                break;
                            case 12:
                                tradingData.FinalBidVolume = fieldValue;
                                break;
                            case 13:
                                tradingData.FinalAskPrice = fieldValue;
                                break;
                            case 14:
                                tradingData.FinalAskVolume = fieldValue;
                                break;
                        }

                        index++;
                    }

                    returnData.Add(tradingData);
                }
            } 
        }
        catch (Exception ex)
        {
            // Log the exception to the BusinessObject.EventLogManger
            EventLogManager.WriteEventLog(new EventLog
            {
                Message = "Failed to parse OTC JSON data: " + ex.Message,
                Type = EventType.Error
            });
        }

        return returnData;
    }

    /// <summary>
    ///     Save the trading data to the database
    /// </summary>
    /// <param name="tradingData"></param>
    public void SaveTradingToDatabase(List<TradingData> tradingData, DateTime tradingDate, MarketType marketType)
    {
        // Save the trading data to the database
        foreach (TradingData data in tradingData)
        {
            // Check if the stock code is valid
            // The stock code should be a 4-digit number and first character should not be 0
            if (!string.IsNullOrEmpty(data.Code) && Regex.IsMatch(data.Code, @"^[1-9]\d{3}$"))
            {
                try
                {
                    TradingDAO tradingDAO = new TradingDAO();
                    Trading trading = new Trading();

                    trading.Code = data.Code;
                    trading.TradingDate = tradingDate;
                    trading.MarketType = marketType;
                    trading.TradeVolume = Convert.ToDecimal(data.TradeVolume.Replace(",", ""));
                    trading.Transaction = Convert.ToUInt64(data.Transaction.Replace(",", ""));
                    trading.TradeValue = Convert.ToDecimal(data.TradeValue.Replace(",", ""));
                    trading.OpeningPrice = Convert.ToDecimal(data.OpeningPrice == "--" || data.OpeningPrice == "---" ? null : data.OpeningPrice.Replace(",", ""));
                    trading.HighestPrice = Convert.ToDecimal(data.HighestPrice == "--" || data.HighestPrice == "---" ? null : data.HighestPrice.Replace(",", ""));
                    trading.LowestPrice = Convert.ToDecimal(data.LowestPrice == "--" || data.LowestPrice == "---" ? null : data.LowestPrice.Replace(",", ""));
                    trading.ClosingPrice = Convert.ToDecimal(data.ClosingPrice == "--" || data.ClosingPrice == "---" ? null : data.ClosingPrice.Replace(",", ""));
                    trading.AveragePrice = Convert.ToDecimal(data.AveragePrice.Replace(",", "") == "" ? null : data.AveragePrice.Replace(",", ""));

                    // Convert the change value to decimal
                    decimal changeValue;
                    if (decimal.TryParse(data.Change, out changeValue))
                    {
                        trading.Change = changeValue;
                    }
                    else
                    {
                        trading.Change = 0;
                    }

                    // trading.Change = Convert.ToDecimal(data.Change == "--" || data.Change == "---" ? null : data.Change.Replace(",", ""));
                    if ( trading.MarketType == MarketType.Listing)
                    {
                        // Get the first character of the change indicator
                        string changeIndicator = Regex.Match(data.ChangeIndicator, @"(?<=<p.*>).*?(?=<\/p>)").Value;
                        trading.ChangeIndicator = changeIndicator.Length > 0 ? changeIndicator[0] : ' ';
                    }
                    else
                    {
                        trading.ChangeIndicator = trading.Change > 0 ? '+' : trading.Change < 0 ? '-' : ' ';
                    }
                    
                    trading.FinalBidPrice = Convert.ToDecimal(data.FinalBidPrice == "--" || data.FinalBidPrice == "---" ? null : data.FinalBidPrice.Replace(",", ""));
                    trading.FinalBidVolume = Convert.ToUInt64(data.FinalBidVolume.Replace(",", "") == "" ? null : data.FinalBidVolume.Replace(",", ""));
                    trading.FinalAskPrice = Convert.ToDecimal(data.FinalAskPrice == "--" || data.FinalAskPrice == "---" ? null : data.FinalAskPrice.Replace(",", "")) ;
                    trading.FinalAskVolume = Convert.ToUInt64(data.FinalAskVolume.Replace(",", "") == "" ? null : data.FinalAskVolume.Replace(",", ""));
                    trading.PERatio = Convert.ToDecimal(data.PERatio.Replace(",", "") == "" ? null : data.PERatio.Replace(",", ""));

                    tradingDAO.AddTradingIfNotExists(trading);
                }
                catch (Exception ex)
                {
                    // Log the exception to the BusinessObject.EventLogManger
                    EventLogManager.WriteEventLog(new EventLog
                    {
                        Message = "Failed to save trading data to the database: " + ex.Message,
                        Type = EventType.Error
                    });
                }
            }
        }
    }
}

/// <summary>
///     Storing transaction data obtained by web crawlers.
/// </summary>
public class TradingData()
{
    // 證券代號
    public string Code { get; set; } = "";

    // 證券名稱
    public string Name { get; set; } = "";

    // 成交股數
    public string TradeVolume { get; set; } = "";

    // 成交筆數
    public string Transaction { get; set; } = "";

    // 成交金額
    public string TradeValue { get; set; } = "";

    // 開盤價
    public string OpeningPrice { get; set; } = "";

    // 最高價
    public string HighestPrice { get; set; } = "";

    // 最低價
    public string LowestPrice { get; set; } = "";

    // 收盤價
    public string ClosingPrice { get; set; } = "";

    // 平均價
    public string AveragePrice { get; set; } = "";

    // 漲跌(+/-)
    public string ChangeIndicator { get; set; } = "";

    // 漲跌價差
    public string Change { get; set; } = "";

    // 最後揭示買價
    public string FinalBidPrice { get; set; } = "";

    // 最後揭示買量
    public string FinalBidVolume { get; set; } = "";

    // 最後揭示賣價
    public string FinalAskPrice { get; set; } = "";

    // 最後揭示賣量
    public string FinalAskVolume { get; set; } = "";

    // 本益比
    public string PERatio { get; set; } = "";
}
