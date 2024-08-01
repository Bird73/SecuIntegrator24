namespace Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EnvironmentManager;

using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EventLogManager;
using System.Text.Json;

public static class EnvironmentManager
{
    /// <summary>
    ///     This class is used to store the configuration of the environment.
    /// </summary>
    public static EnvironmentConfig EnvironmentConfig = new EnvironmentConfig();

    /// <summary>
    ///     The path of the configuration file.
    /// </summary>
    public const string ConfigPath = "Config";

    /// <summary>
    ///     The path of the data file.
    /// </summary>
    public const string DataPath = "DownloadedData";

    /// <summary>
    ///     Load the configuration from the file.
    /// </summary>
    /// <param name="errorMessages"></param>
    public static void LoadConfig()
    {
        string filePath = Path.Combine(ConfigPath, "SecuIntegrate24.json");

        try
        {
            // if config file is not exist, create a new one.
            if (!File.Exists(filePath))
            {
                // Create a new configuration file.
                SaveConfig();
            }
            else
            {
                // Load the configuration from the file.
                string json = System.IO.File.ReadAllText(filePath);
                var env = JsonSerializer.Deserialize<EnvironmentConfig>(json);

                if (env != null)
                {
                    // Load the configuration from the file.
                    EnvironmentConfig.InitialYear = env.InitialYear;
                    EnvironmentConfig.ConnectionInterval = env.ConnectionInterval;
                    EnvironmentConfig.isAutoRunEnabled = env.isAutoRunEnabled;
                }
            }
        }
        catch (Exception ex)
        {
            EventLogManager.WriteEventLog(new EventLog
            {
                Type = EventType.Error,
                Message = $"Error while accessing the configuration file : {filePath}. {ex.Message}"
            });
        }
    }

    /// <summary>
    ///     Save the configuration to the file.
    /// </summary>
    /// <param name="errorMessages"></param>
    public static void SaveConfig()
    {
        string filePath = Path.Combine(ConfigPath, "SecuIntegrate24.json");
        try
        {
            // Save the configuration to the file.
            string json = JsonSerializer.Serialize(EnvironmentConfig);
            System.IO.File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            EventLogManager.WriteEventLog(new EventLog
            {
                Type = EventType.Error,
                Message = $"Error while saving the configuration file : {filePath}. {ex.Message}"
            });
        }
    }
}
