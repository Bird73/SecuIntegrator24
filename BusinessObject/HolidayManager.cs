namespace Birdsoft.SecuIntegrator24.BusinessObject;

using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EventLogManager;
using Birdsoft.SecuIntegrator24.DataAccessObject;

using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

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
    public static List<Holiday> Holidays { get; private set; } = new List<Holiday>();

    /// <summary>
    ///    Initializes the holiday manager
    /// </summary>
    public static void Initialize()
    {
        try
        {
            // Load the holidays from the database
            LoadHolidaysFromDatabase();         // Load the holidays from the database

            // If there are no holidays, load them from the URL
            if (Holidays.Count == 0)
            {
                HolidayFetcher holidayFetcher = new HolidayFetcher();
                holidayFetcher.FetchHolidaysFromURL(DateTime.Now.Year);     // Fetch the holidays from the URL
            }

            AddNaturalDisasterHoliday(DateTime.Now.Year);       // Add natural disaster holidays
            SaveHolidaysToDatabase(DateTime.Now.Year);          // Save the holidays to the database
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

    public static void AddNaturalDisasterHoliday(int year)
    {
        try
        {
            switch (year)
            {
                case 2024:
                    Holidays.AddRange(new[]
                    {
                        new Holiday
                        {
                            Name = "凱米颱風",
                            Date = new DateTime(2024, 7, 24),
                            Description = "颱風休市一天"
                        },
                        new Holiday
                        {
                            Name = "凱米颱風",
                            Date = new DateTime(2024, 7, 25),
                            Description = "颱風休市一天"
                        }
                    });
                    break;
                case 2023:
                    Holidays.Add(new Holiday
                    {
                            Name = "卡努颱風",
                            Date = new DateTime(2023, 8, 3),
                            Description = "颱風休市一天" 
                    });
                    break;
                case 2019:
                    Holidays.AddRange(new[]
                    {
                        new Holiday
                        {
                            Name = "利奇馬颱風",
                            Date = new DateTime(2019, 8, 9),
                            Description = "颱風休市一天"
                        },
                        new Holiday
                        {
                            Name = "米塔颱風",
                            Date = new DateTime(2019, 9, 30),
                            Description = "颱風休市一天"
                        }
                    });
                    break;
                case 2016:
                    Holidays.AddRange(new[]
                    {
                        new Holiday
                        {
                            Name = "尼伯特颱風",
                            Date = new DateTime(2016, 7, 8),
                            Description = "颱風休市一天"
                        },
                        new Holiday
                        {
                            Name = "梅姬颱風",
                            Date = new DateTime(2016, 9, 27),
                            Description = "颱風休市一天"
                        },
                        new Holiday
                        {
                            Name = "梅姬颱風",
                            Date = new DateTime(2016, 9, 28),
                            Description = "颱風休市一天"
                        }
                    });
                    break;
            }

            // Save the holidays to the database
            SaveHolidaysToDatabase(DateTime.Now.Year);
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
    public static void SaveHolidaysToDatabase(int year)
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
}