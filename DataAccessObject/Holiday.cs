namespace Birdsoft.SecuIntegrator24.DataAccessObject;

using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EventLogManager;

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

/// <summary>
///     Represents a holiday
/// </summary>
public class Holiday
{
    /// <summary>
    ///     The date of the holiday (Primary Key)
    /// </summary>
    [Key]
    public DateTime Date { get; set; }

    /// <summary>
    ///     The name of the holiday
    /// </summary>
    [Required]
    public string Name { get; set; } = "";

    /// <summary>
    ///     The description of the holiday
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
///     Data Access Object for the holidays
/// </summary>
public class HolidayDAO
{
    /// <summary>
    ///     Gets the list of holidays
    /// </summary>
    /// <returns></returns>
    public List<Holiday> GetHolidays()
    {
        try
        {
            using (var db = new HolidayDbContext())
            {
                return db.Holidays.ToList();
            }
        }
        catch (Exception ex)
        {
            EventLogManager.WriteEventLog(new EventLog
            {
                Message = ex.Message,
                Type = EventType.Error
            });

            return new List<Holiday>();
        }
    }

    /// <summary>
    ///     Gets the list of holidays by year
    /// </summary>
    /// <param name="year"></param>
    /// <returns></returns>
    public List<Holiday> GetHolidaysByYear(int year)
    {
        try
        {
            using (var db = new HolidayDbContext())
            {
                var query = $"SELECT * FROM Holidays WHERE strftime('%Y', Date) = '{year}'";
                return db.Holidays.FromSqlRaw(query).ToList();
            }
        }
        catch (Exception ex)
        {
            EventLogManager.WriteEventLog(new EventLog
            {
                Message = ex.Message,
                Type = EventType.Error
            });

            return new List<Holiday>();
        }
    }

    /// <summary>
    ///     Adds a holiday if it does not exist
    /// </summary>
    /// <param name="holiday"></param>
    public void AddHolidayIfNotExists(Holiday holiday)
    {
        try
        {
            // add if not exists
            using (var db = new HolidayDbContext())
            {
                var existingHoliday = db.Holidays.FirstOrDefault(h => h.Date == holiday.Date);
                if (existingHoliday == null)
                {
                    db.Holidays.Add(holiday);
                    db.SaveChanges();
                }
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
