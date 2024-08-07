using System.Net;

namespace Birdsoft.SecuIntegrator24.SystemInfrastructureObject.BackgroundTask
{
    /// <summary>
    ///     Represents the status of a task
    /// </summary>
    public enum TaskStatus
    {
        /// <summary>
        ///     The task is waiting to start
        /// </summary>
        WaitingToStart,
        /// <summary>
        ///     The task is running
        /// </summary>
        Running,
        /// <summary>
        ///     The task has completed
        /// </summary>
        Completed,
        /// <summary>
        ///     The task has been canceled
        /// </summary>
        Cancelled,
        /// <summary>
        ///     The task has faulted
        /// </summary>
        Faulted
    }

    public interface ITaskStatus
    {
        public TaskStatus Status { get; set;}
    }
}