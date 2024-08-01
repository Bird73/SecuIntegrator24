namespace Birdsoft.SecuIntegrator24.SystemInfrastructureObject.BackgroundTask;

using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EventLogManager;

/// <summary>
///     Defines the schedule for the tasks
/// </summary>
public class TaskSchedule
{
    /// <summary>
    ///     The task to be scheduled
    /// </summary>
    public required IBackgroundTask Task { get; set; }

    /// <summary>
    ///     The schedule for the task
    /// </summary>
    public required Schedule Schedule { get; set; }

    /// <summary>
    ///     The precondition for the task
    /// </summary>
    public Precondition? Precondition { get; set; }

    /// <summary>
    ///     The status of the task
    /// </summary>
    public TaskStatus Status { get; set; } = TaskStatus.WaitingToStart;
}