namespace Birdsoft.SecuIntegrator24.BusinessObject;

using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.BackgroundTask;
using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EnvironmentManager;
using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EventLogManager;

/// <summary>
///     Background task to fetch monthly revenues from the web and save to database
/// </summary>
public class GetMonthlyRevenues : IBackgroundTask
{
    public string Name => "GetMonthlyRevenues";

    /// <summary>
    ///     Start the task of fetching monthly revenues from the web and save to database
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public TaskStatus Start(CancellationToken cancellationToken)
    {
        // Create the directory if it does not exist
        string fileBasePath = Path.Combine("Data", "MonthlyRevenue");
        Directory.CreateDirectory(fileBasePath);

        DateTime initialDate = new DateTime(EnvironmentManager.EnvironmentConfig.InitialYear, 1, 1);

        // Process the data from the initial date to the previous month
        DateTime previousMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
        DateTime processDate = previousMonth;
        for ( ; processDate >= initialDate; processDate = processDate.AddMonths(-1))
        {
            try
            {
                RevenueFetcher revenueFetcher = new RevenueFetcher();

                // Check if file not exists then download
                string filePath = Path.Combine(fileBasePath, "Listing_" + processDate.ToString("yyyyMM") + ".csv");
                if (!File.Exists(filePath) || processDate == previousMonth)
                {
                    if (revenueFetcher.FetchListingRevenueDataFromWeb(processDate))
                    {
                        // Read data from file
                        List<RevenueData> revenueData = revenueFetcher.FetchListingRevenueDataFromCSV(processDate);

                        // Save data to database
                        revenueFetcher.SaveRevenueToDatabase(revenueData, processDate, DataAccessObject.MarketType.Listing);
                    } 
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return TaskStatus.Cancelled;
                }

                // interval between two requests
                Thread.Sleep(EnvironmentManager.EnvironmentConfig.ConnectionInterval * 1000);

                // Check if file not exists then download
                filePath = Path.Combine(fileBasePath, "OTC_" + processDate.ToString("yyyyMM") + ".csv");
                
                if (!File.Exists(filePath) || processDate == previousMonth)
                {
                    if (revenueFetcher.FetchOTCRevenueDataFromWeb(processDate))
                    {
                        // Read data from file
                        List<RevenueData> revenueData = revenueFetcher.FetchOTCRevenueDataFromCSV(processDate);

                        // Save data to database
                        revenueFetcher.SaveRevenueToDatabase(revenueData, processDate, DataAccessObject.MarketType.OTC);
                    }
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return TaskStatus.Cancelled;
                }

                // interval between two requests
                Thread.Sleep(EnvironmentManager.EnvironmentConfig.ConnectionInterval * 1000);
        
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