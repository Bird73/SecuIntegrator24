namespace Birdsoft.SecuIntegrator24.BusinessObject;

using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.BackgroundTask;
using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EnvironmentManager;
using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EventLogManager;

public class GetListingTradings : IBackgroundTask
{
    public string Name => "GetListingTradings";
    
    /// <summary>
    ///     Fetch trading data from the URL
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public TaskStatus Start(CancellationToken cancellationToken)
    {
        DateTime initialDate = new DateTime(EnvironmentManager.EnvironmentConfig.InitialYear, 1, 1);

        for (DateTime processDate = DateTime.Today; processDate >= initialDate; processDate = processDate.AddDays(-1))
        {
            try
            {
                // Check if proccess time is before 4pm today to avoid fetching today's data
                if  (processDate == DateTime.Today && DateTime.Now.Hour < 16)
                {
                    continue;
                }

                // Check if processDate is a holiday
                if (!HolidayManager.IsHoliday(processDate))
                {
                    TradingFetcher tradingFetcher = new TradingFetcher();
                    List<TradingData> tradingData = new List<TradingData>();

                    bool NeedDownload = false;
                    bool isFileExists = true;

                    // Check if file not exists then download
                    string filePath = Path.Combine("Data", "Trading", processDate.Year.ToString(), "Listing_" + processDate.ToString("yyyyMMdd") + ".json");
                    if (!File.Exists(filePath))
                    {
                        NeedDownload = tradingFetcher.FetchListingTradingDataFromWeb(processDate);        // Fetch trading from the URL and save it to the json file if it is not downloaded yet
                        isFileExists = NeedDownload;
                    }

                    // If file exists then fetch data from file
                    if (isFileExists)
                    {
                        tradingData = tradingFetcher.FetchListingTradingDataFromJSON(processDate);     // Fetch trading from the json file

                        // Save trading to the database
                        tradingFetcher.SaveTradingToDatabase(tradingData, processDate, DataAccessObject.MarketType.Listing);        

                        if (NeedDownload)
                        {
                            Thread.Sleep(EnvironmentManager.EnvironmentConfig.ConnectionInterval * 1000);        // Wait for the connection interval
                        }
                    }
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return TaskStatus.Cancelled;
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

        return TaskStatus.Completed;
    }
}
