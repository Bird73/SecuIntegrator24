namespace Birdsoft.SecuIntegrator24.BusinessObject;

using System.Text.Json;

public static class EnvironmentManager
{
    /// <summary>
    ///     This class is used to store the configuration of the environment.
    /// </summary>
    public static EnvironmentConfig EnvironmentConfig = new EnvironmentConfig();

    /// <summary>
    ///     Load the configuration from the file.
    /// </summary>
    /// <param name="errorMessages"></param>
    public static void LoadConfig()
    {
        try
        {
            // if config file is not exist, create a new one.
            if (!File.Exists("SecuIntegrate24.json"))
            {
                // Create a new configuration file.
                SaveConfig();
            }
            else
            {
                // Load the configuration from the file.
                string json = System.IO.File.ReadAllText("SecuIntegrate24.json");
                var env = JsonSerializer.Deserialize<EnvironmentConfig>(json);

                EnvironmentConfig.InitialYear = env.InitialYear;
                EnvironmentConfig.ConnectionInterval = env.ConnectionInterval;
                EnvironmentConfig.isAutoRunEnabled = env.isAutoRunEnabled;
            }
        }
        catch (Exception ex)
        {
            EventLogManager.WriteEventLog(new EventLog
            {
                Type = EventType.Error,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    ///     Save the configuration to the file.
    /// </summary>
    /// <param name="errorMessages"></param>
    public static void SaveConfig()
    {
        try
        {
            // Save the configuration to the file.
            string json = JsonSerializer.Serialize(EnvironmentConfig);
            System.IO.File.WriteAllText("SecuIntegrate24.json", json);
        }
        catch (Exception ex)
        {
            EventLogManager.WriteEventLog(new EventLog
            {
                Type = EventType.Error,
                Message = ex.Message
            });
        }
    }
}
