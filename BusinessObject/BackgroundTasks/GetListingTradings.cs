namespace Birdsoft.SecuIntegrator24.BusinessObject;

using System.Net;
using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.BackgroundTask;
using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EnvironmentManager;
using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EventLogManager;
using System.Text.RegularExpressions;

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
            // Check if processDate is a holiday
            if (!HolidayManager.IsHoliday(processDate))
            {
                TradingFetcher tradingFetcher = new TradingFetcher();
                List<TradingData> tradingData = new List<TradingData>();

                bool isDownloaded = false;

                string filePath = Path.Combine("Data", "Trading", processDate.Year.ToString(), "Listing_" + processDate.ToString("yyyyMMdd") + ".json");
                if (!File.Exists(filePath))
                {
                    tradingFetcher.FetchListingTradingDataFromWeb(processDate);        // Fetch trading from the URL and save it to the json file if it is not downloaded yet
                    isDownloaded = true;
                }

                tradingData = tradingFetcher.FetchListingTradingDataFromJSON(processDate);     // Fetch trading from the json file

                tradingFetcher.SaveTradingToDatabase(tradingData, processDate);         // Save trading to the database

                if (isDownloaded)
                {
                    Thread.Sleep(2000); // Sleep for 2 seconds
                }
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return TaskStatus.Canceled;
            }
        }

        return TaskStatus.Completed;
    }
}
