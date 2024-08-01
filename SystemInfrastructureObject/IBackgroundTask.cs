namespace Birdsoft.SecuIntegrator24.SystemInfrastructureObject.BackgroundTask
{
    public interface IBackgroundTask : INameable
    {
        /// <summary>
        ///     Starts the task
        /// </summary>
        public TaskStatus Start(CancellationToken cancellationToken);
    }

    /// <summary>
    ///     Interface for objects that have a name.
    /// </summary>
    public interface INameable
    {
        public string Name { get; }
    }
}