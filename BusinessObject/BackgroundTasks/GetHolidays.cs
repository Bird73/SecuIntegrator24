namespace Birdsoft.SecuIntegrator24.BusinessObject;

using System.Net;
using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.BackgroundTask;
using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EnvironmentManager;
using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EventLogManager;

using System.Text.RegularExpressions;

public class GetHolidays : IBackgroundTask
{
    public string Name => "GetHolidays";
    
    /// <summary>
    ///     Fetch holidays from the URL, add natural disaster holidays, and save holidays to the database
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public TaskStatus Start(CancellationToken cancellationToken)
    {
        for (int processYear = DateTime.Now.Year - 1; processYear >= EnvironmentManager.EnvironmentConfig.InitialYear; processYear--)
        {
            HolidayFetcher holidayFetcher = new HolidayFetcher();

            holidayFetcher.FetchHolidaysFromURL(processYear);           // Fetch holidays from the URL

            HolidayManager.AddNaturalDisasterHoliday(processYear);      // Add natural disaster holidays

            HolidayManager.SaveHolidaysToDatabase(processYear);         // Save holidays to the database

            if (cancellationToken.IsCancellationRequested)
            {
                return TaskStatus.Cancelled;
            }

            Thread.Sleep(2000);
        }

        return TaskStatus.Completed;
    }
}