namespace Birdsoft.SecuIntegrator24.BusinessObject;

using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.BackgroundTask;
using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EventLogManager;
using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EnvironmentManager;
using System.Text.Json;

public class TaskScheduler()
{
    private class ScheduleConfig
    {
        public string Name { get; set; } = "";
        public Schedule Schedule { get; set; } = new Schedule();
        public Precondition? Precondition { get; set; }
    }

    /// <summary>
    ///     List of task schedules
    /// </summary>
    private List<ScheduleConfig> _scheduleConfig { get; set; } = new List<ScheduleConfig>();

    /// <summary>
    ///     Load the schedule from the json file
    /// </summary>
    public void LoadScheduleFomrConfig()
    {
        // Load the schedule from the json file
        string filePath = Path.Combine(EnvironmentManager.ConfigPath, "Schedule.json");

        // Load schedule from json file
        if (File.Exists(filePath))
        {
            try
            {
                // Load the schedule from the file
                string json = System.IO.File.ReadAllText(filePath);
                _scheduleConfig = JsonSerializer.Deserialize<List<ScheduleConfig>>(json) ?? new List<ScheduleConfig>();
            }
            catch (Exception ex)
            {
                // Write an error log
                EventLogManager.WriteEventLog(new EventLog
                {
                    Type = EventType.Error,
                    Message = $"Error loading schedule from file \"{filePath}\". {ex.Message}"
                });
            }
        }
        else
        {
            // Write an error log
            EventLogManager.WriteEventLog(new EventLog
            {
                Type = EventType.Error,
                Message = $"file \"{filePath}\" not found."
            });
        }
    }

    /// <summary>
    ///     Register and schedule a task
    /// </summary>
    public void RegisterAndScheduleTasks()
    {
        try 
        {
            GetHolidays getHolidays = new GetHolidays();

            // Get the schedule from the config then register the task
            var schedule = _scheduleConfig.FirstOrDefault(x => x.Name == getHolidays.Name);

            if (schedule != null)
            {
                TaskScheduleManager.RegisterTask(getHolidays, schedule.Schedule, schedule.Precondition);
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
