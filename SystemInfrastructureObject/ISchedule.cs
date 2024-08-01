namespace Birdsoft.SecuIntegrator24.SystemInfrastructureObject.BackgroundTask
{

    public interface ISchedule
    {
        /// <summary>
        ///     The schedule for the task.
        /// </summary>
        public Schedule Schedule { get; }
    }

    /// <summary>
    ///     Represents a schedule for a task.
    /// </summary>
    public class Schedule 
    {
        /// <summary>
        ///     The days of the week that the task should run on.
        /// </summary>
        public List<DayOfWeek> WeeklyDays { get; set; } = new List<DayOfWeek>();

        /// <summary>
        ///     The days of the month that the task should run on.
        /// </summary>
        public List<int> MonthlyDays { get; set; } = new List<int>();

        /// <summary>
        ///      The dates of the year that the task should run on.
        /// </summary>
        public List<DateTime> YearlyDates { get; set; }  = new List<DateTime>();

        /// <summary>
        ///     The time of day that the task should run.
        /// </summary>
        public DateTime ExecutionTime { get; set;} = DateTime.Today.AddDays(1);
        /// <summary>
        ///     Whether the task should run on startup.
        /// </summary>
        public bool RunOnStartup { get; set; } = true;
    }
}