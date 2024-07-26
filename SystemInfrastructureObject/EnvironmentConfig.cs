namespace Birdsoft.SecuIntegrator24.SystemInfrastructureObject;

/// <summary>
///   This class is used to store the configuration of the environment.
/// </summary>
public class EnvironmentConfig
{
    /// <summary>
    ///     The initial year of the data.
    /// </summary>
    /// <remarks>
    ///    The default value is the current year.
    ///  </remarks>
    public int InitialYear { get; set; } = DateTime.Now.Year;

    /// <summary>
    ///    Set the connection interval in seconds to avoid being blocked by the server due to frequent connections.
    /// </summary>
    /// <remarks>
    ///    The default value is 1 second.
    /// </remarks>
    public int ConnectionInterval { get; set; } = 1; 
    /// <summary>
    ///     Set the tasks to run automatically when the program starts.
    /// </summary>
    /// <remarks>
    ///   The default value is enabled.
    /// </remarks>
    public bool isAutoRunEnabled { get; set; } = true;
}