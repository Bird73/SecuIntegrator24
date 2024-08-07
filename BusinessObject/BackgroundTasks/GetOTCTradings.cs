namespace Birdsoft.SecuIntegrator24.BusinessObject;

using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.BackgroundTask;
using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EnvironmentManager;
using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EventLogManager;

public class GetOTCTradings : IBackgroundTask
{
    public string Name => "GetOTCTradings";

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

                if (!HolidayManager.IsHoliday(processDate))
                {
                    TradingFetcher tradingFetcher = new TradingFetcher();
                    List<TradingData> tradingData = new List<TradingData>();

                    bool NeedDownload = false;
                    bool isFileExists = true;

                    // Check if file not exists then download
                    string filePath = Path.Combine("Data", "Trading", processDate.Year.ToString(), "OTC_" + processDate.ToString("yyyyMMdd") + ".json");
                    if (!File.Exists(filePath))
                    {
                        NeedDownload = tradingFetcher.FetchOTCTradingDataFromWeb(processDate);
                        isFileExists = NeedDownload;
                    }

                    // If file exists then fetch data from file
                    if (isFileExists)
                    {
                        tradingData = tradingFetcher.FetchOTCTradingDataFromJSON(processDate);

                        // Save data to database
                        tradingFetcher.SaveTradingToDatabase(tradingData, processDate, DataAccessObject.MarketType.OTC);

                        // if download is required then wait for connection interval
                        if (NeedDownload)
                        {
                            Thread.Sleep(EnvironmentManager.EnvironmentConfig.ConnectionInterval * 1000);
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