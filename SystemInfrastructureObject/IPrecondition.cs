namespace Birdsoft.SecuIntegrator24.SystemInfrastructureObject.BackgroundTask
{
    /// <summary>
    ///     Interface for tasks that have preconditions
    /// </summary>
    public interface IPrecondition
    {
        /// <summary>
        ///     The preconditions for the task
        /// </summary>
        public Precondition? Precondition { get; set; }
    }

    /// <summary>
    ///     The condition for the tasks
    /// </summary>
    public enum TaskCondition
    {
        /// <summary>
        ///     All tasks must be completed
        /// </summary>
        AllCompleted,
        /// <summary>
        ///     Any task must be completed
        /// </summary>
        AnyCompleted
    }

    /// <summary>
    ///     Represents a precondition for a task
    /// </summary>
    public class Precondition
    {
        public required List<string> TaskNames { get; set; }
        public TaskCondition Condition { get; set; }
    }
}