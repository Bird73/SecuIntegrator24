
namespace Birdsoft.SecuIntegrator24.SystemInfrastructureObject.BackgroundTask;

using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EventLogManager;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
///     Manages background tasks
/// </summary>
public static class TaskScheduleManager
{
    /// <summary>
    ///     List of tasks
    /// </summary>
    private static List<TaskSchedule> _taskSchedules = new List<TaskSchedule>();

    /// <summary>
    ///     Dictionary of task and their respective threads
    /// </summary>
    private static Dictionary<TaskSchedule, Thread> _taskThreads = new Dictionary<TaskSchedule, Thread>();

    /// <summary>
    ///     Cancellation token source
    /// </summary>
    private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    /// <summary>
    ///     Lock object
    /// </summary>
    private static object _lock = new object();

    /// <summary>
    ///     Register a background task
    /// </summary>
    /// <param name="task"></param>
    public static void RegisterTask(IBackgroundTask task, Schedule schedule, Precondition? precondition)
    {
        _taskSchedules.Add(new TaskSchedule
        {
            Task = task,
            Schedule = schedule,
            Precondition = precondition,
            Status = TaskStatus.WaitingToStart
        });
    }

    /// <summary>
    ///     Start all tasks on the schedule
    /// </summary>
    public static void StartAllTasks()
    {
        foreach (var taskSchedule in _taskSchedules)
        {
            try
            {
                
                DateTime nextRunTime;

                // Check if task should run on startup
                if (taskSchedule.Schedule.RunOnStartup)
                {
                    nextRunTime = DateTime.Now.AddSeconds(3);   // Start task 3 seconds after startup
                }
                else
                {
                    // Calculate next run time based on schedule
                    nextRunTime = CalculateNextRunTime(taskSchedule.Schedule);
                }

                // Start task execution loop
                var thread = new Thread(() => TaskExecutionLoop(taskSchedule, nextRunTime));
                
                _taskThreads[taskSchedule] = thread;        // Add task and thread to dictionary
                thread.Start();                             // Start the thread
            }
            catch (Exception ex)
            {
                // Write an event log
                EventLogManager.WriteEventLog(new EventLog
                {
                    Type = EventType.Error,
                    Message = $"Error starting task {taskSchedule.Task.Name}. {ex.Message}"
                });
            }
                                        
        }
    }

    /// <summary>
    ///     Execute the task based on schedule and preconditions
    /// </summary>
    private static void TaskExecutionLoop(TaskSchedule taskSchedule, DateTime nextRunTime)
    {
        try
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                if (DateTime.Now >= nextRunTime && taskSchedule.Status == TaskStatus.WaitingToStart)
                {
                    if (taskSchedule.Precondition == null)
                    {
                        taskSchedule.Task.Start(_cancellationTokenSource.Token);
                        taskSchedule.Status = TaskStatus.Completed;
                        nextRunTime = CalculateNextRunTime(taskSchedule.Schedule); // Recalculate next run time
                        continue;
                    }
                    else
                    {
                        if (CheckPreconditions(taskSchedule.Precondition))
                        {
                            taskSchedule.Task.Start(_cancellationTokenSource.Token);
                            taskSchedule.Status = TaskStatus.Completed;
                            nextRunTime = CalculateNextRunTime(taskSchedule.Schedule); // Recalculate next run time
                            continue;
                        }
                    }
                }

                Thread.Sleep(2000);     // check every 2 seconds
            }
        }
        catch (Exception ex)
        {
            // Write an event log
            EventLogManager.WriteEventLog(new EventLog
            {
                Type = EventType.Error,
                Message = $"Error executing task {taskSchedule.Task.Name}. {ex.Message}"
            });
        }
    }

    /// <summary>
    ///     Calculate next run time based on schedule
    /// </summary>
    /// <param name="schedule"></param>
    /// <returns></returns>
    private static DateTime CalculateNextRunTime(Schedule schedule)
    {
        try
        {
            // Calculate next run time based on schedule
            DateTime now = DateTime.Now;
            DateTime nextRunTime = DateTime.MaxValue;

            foreach (var yearlyDate in schedule.YearlyDates)
            {
                // Calculate next run time for each yearly date
                DateTime tempNextRunTime = new DateTime(now.Year, yearlyDate.Month, yearlyDate.Day, schedule.ExecutionTime.Hour, schedule.ExecutionTime.Minute, schedule.ExecutionTime.Second);
                if (tempNextRunTime < now)
                {
                    tempNextRunTime = tempNextRunTime.AddYears(1);
                }

                if (tempNextRunTime < nextRunTime)
                {
                    nextRunTime = tempNextRunTime;
                }
            }

            foreach (var monthlyDay in schedule.MonthlyDays)
            {
                DateTime tempNextRunTime = new DateTime(now.Year, now.Month, monthlyDay, schedule.ExecutionTime.Hour, schedule.ExecutionTime.Minute, schedule.ExecutionTime.Second);
                if (tempNextRunTime < now)
                {
                    tempNextRunTime = tempNextRunTime.AddMonths(1);
                }

                if (tempNextRunTime < nextRunTime)
                {
                    nextRunTime = tempNextRunTime;
                }
            }

            foreach (var weeklyDay in schedule.WeeklyDays)
            {
                DateTime tempNextRunTime = now.Date;
                while (tempNextRunTime.DayOfWeek != weeklyDay)
                {
                    tempNextRunTime = tempNextRunTime.AddDays(1);
                }
                tempNextRunTime = tempNextRunTime.AddHours(schedule.ExecutionTime.Hour).AddMinutes(schedule.ExecutionTime.Minute).AddSeconds(schedule.ExecutionTime.Second);
                if (tempNextRunTime > now && tempNextRunTime < nextRunTime)
                {
                    nextRunTime = tempNextRunTime;
                }
            }

            return nextRunTime;
        }
        catch (Exception ex)
        {
            // Write an event log
            EventLogManager.WriteEventLog(new EventLog
            {
                Type = EventType.Error,
                Message = $"Error calculating next run time. {ex.Message}"
            });

            return DateTime.MaxValue;
        }
    }

    /// <summary>
    ///     Check if preconditions are met
    /// </summary>
    /// <param name="precondition"></param>
    /// <returns></returns>
    private static bool CheckPreconditions(Precondition precondition)
    {
        try
        {
            var tasksToCheck = _taskSchedules.Where(t => precondition.TaskNames.Contains(t.Task.Name)).ToList();
            bool conditionMet = precondition.Condition switch
            {
                TaskCondition.AllCompleted => tasksToCheck.All(t => t.Status == TaskStatus.Completed),
                TaskCondition.AnyCompleted => tasksToCheck.Any(t => t.Status == TaskStatus.Completed),
                _ => false
            };

            return conditionMet;
        }
        catch (Exception ex)
        {
            // Write an event log
            EventLogManager.WriteEventLog(new EventLog
            {
                Type = EventType.Error,
                Message = $"Error checking preconditions. {ex.Message}"
            });

            return false;
        }
    }

    /// <summary>
    ///     Reset all tasks to waiting state
    /// </summary>
    private static void ResetTasks()
    {
        lock (_lock)
        {
            foreach (var taskSchedule in _taskSchedules)
            {
                if (taskSchedule.Status == TaskStatus.Completed)
                {
                    taskSchedule.Status = TaskStatus.WaitingToStart;
                }
            }
        }
    }

    /// <summary>
    ///     Start the daily reset task
    /// </summary>
    public static void StartDailyReset()
    {
        Task.Run(() =>
        {
            while (true)
            {
                try
                {
                    ResetTasks();

                    // wait until all tasks are completed then reset them
                    while (_taskSchedules.Any(t => t.Status == TaskStatus.Running) || _taskSchedules.Any(t => t.Status == TaskStatus.Completed))
                    {
                        foreach (var task in _taskSchedules.Where(t => t.Status == TaskStatus.Completed))
                        {
                            task.Status = TaskStatus.WaitingToStart;
                        }

                        Thread.Sleep(1000); // sleep for 1 second
                    }
                    
                    // wait until next day
                    var now = DateTime.Now;
                    var nextReset = new DateTime(now.Year, now.Month, now.Day).AddDays(1);
                    var delay = nextReset - now;
                    Task.Delay(delay, _cancellationTokenSource.Token).Wait();
                }
                catch (Exception ex)
                {
                    // Write an event log
                    EventLogManager.WriteEventLog(new EventLog
                    {
                        Type = EventType.Error,
                        Message = $"Error in daily reset: {ex.Message}"
                    });
                }
            }
        }, _cancellationTokenSource.Token);
    }

    /// <summary>
    ///     Stop all tasks
    /// </summary>
    public static void StopAllTasks()
    {
        lock (_lock)
        {
            _cancellationTokenSource.Cancel();

            foreach (var taskSchedule in _taskSchedules)
            {
                if (_taskThreads.TryGetValue(taskSchedule, out var thread))
                {
                    try
                    {
                        if (!thread.Join(TimeSpan.FromSeconds(5)))
                        {
                            // If the thread does not stop within 5 seconds, log a warning
                            EventLogManager.WriteEventLog(new EventLog
                            {
                                Type = EventType.Warning,
                                Message = $"Task {taskSchedule.Task.Name} did not stop within the timeout period."
                            });
                        }
                        else
                        {
                            taskSchedule.Status = TaskStatus.Canceled;
                        }
                    }
                    catch (Exception ex)
                    {
                        EventLogManager.WriteEventLog(new EventLog
                        {
                            Type = EventType.Error,
                            Message = $"Failed to stop task {taskSchedule.Task.Name}: {ex.Message}"
                        });
                    }
                }
            }

            _cancellationTokenSource = new CancellationTokenSource(); // Reset the cancellation token source
        }
    }
}