namespace Birdsoft.SecuIntegrator24.SystemInfrastructureObject;

using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Threading;

using System.Runtime.CompilerServices;

/// <summary>
///     This class defines the event type.
/// </summary>
public enum EventType
{
    Info,
    Warning,
    Error
}

/// <summary>
///     This class defines the event log.
/// </summary>
public class EventLog
{
    /// <summary>
    ///     The time of the event.
    /// </summary>
    public DateTime Time { get; set; } = DateTime.Now;

    /// <summary>
    ///     The object class of the event.
    /// </summary>
    public string ObjectClass  { get; set; } = "";

    /// <summary>
    ///     The type of the event.
    /// </summary>
    public EventType Type { get; set; } = EventType.Info;
    
    /// <summary>
    ///     The message of the event.
    /// </summary>
    public string Message { get; set; } = "";

}

/// <summary>
///    This class is used to manage the event log.
/// </summary>
public static class EventLogManager
{
    /// <summary>
    ///    The lock object.
    /// </summary>
    private static readonly ReaderWriterLockSlim LockObj;

    public static int InfoCount { get; private set; } = 0;

    public static int WarningCount { get; private set; } = 0;

    public static int ErrorCount { get; private set; } = 0;

    /// <summary>
    ///     The static constructor.
    /// </summary>
    static EventLogManager()
    {
        LockObj = new ReaderWriterLockSlim();
    }

    /// <summary>
    ///     Write the event log to the file.
    /// </summary>
    /// <param name="eventLog"></param>
    public static void WriteEventLog(EventLog eventLog)
    {
        try
        {
            // Get the class name of the caller.
            StackTrace stackTrace = new StackTrace();
            StackFrame frame = stackTrace.GetFrame(1);      // 1 means the caller of this method.
            var method = frame.GetMethod();
            var declaringType = method.DeclaringType;

            string className = declaringType != null ? declaringType.Name : "UnknownClass";
            string methodName = method.Name;

            eventLog.ObjectClass = $"{className}.{methodName}";

            // Write the event log to the file.
            string json = JsonSerializer.Serialize(eventLog) + Environment.NewLine;

            LockObj.EnterWriteLock();
            try
            {
                // Create the Log folder if it does not exist.
                if (!Directory.Exists("Log"))
                {
                    Directory.CreateDirectory("Log");
                }

                // Write the event log to the file, the file name is SecuIntegrate24_YYYYMMDD.log.
                string logFileName = Path.Combine("Log", $"SecuIntegrate24_{DateTime.Now.ToString("yyyyMMdd")}.log");
                System.IO.File.AppendAllText(logFileName, json);

                DeleteOldLogFiles();
            }
            finally
            {
                LockObj.ExitWriteLock();
            }

            // Increase the count of the event type.
            IncreaseCount(eventLog.Type);
        }
        catch (Exception)
        {
            Console.WriteLine("Failed to write the event log.");
        }
    }

    /// <summary>
    ///    Delete the log files that are older than 60 days.
    /// </summary>
    private static void DeleteOldLogFiles()
    {
        // Delete the log files that are older than 60 days.
        string[] logFiles = Directory.GetFiles("Log", "SecuIntegrate24_*.log");
        DateTime sixtyDaysAgo = DateTime.Now.AddDays(-60);
        foreach (string logFile in logFiles)
        {
            // incorrect date format will be ignored
            if (DateTime.TryParseExact(Path.GetFileNameWithoutExtension(logFile).Substring(16), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime logDate))
            {
                if (logDate < sixtyDaysAgo)
                {
                    try
                    {
                        File.Delete(logFile);
                    }
                    catch (Exception)
                    {
                        // Ignore the exception.
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Reset the count of the event manager.
    /// </summary>
    public static void ResetCount()
    {
        LockObj.EnterWriteLock();
        try
        {
            InfoCount = 0;
            WarningCount = 0;
            ErrorCount = 0;
        }
        finally
        {
            LockObj.ExitWriteLock();
        }  
    }

    /// <summary>
    ///     Increase the count of the info event.
    /// </summary>
    /// <param name="eventType"></param>
    private static void IncreaseCount(EventType eventType)
    {
        LockObj.EnterWriteLock();
        try
        {
            switch (eventType)
            {
                case EventType.Info:
                    InfoCount++;
                    break;
                case EventType.Warning:
                    WarningCount++;
                    break;
                case EventType.Error:
                    ErrorCount++;
                    break;
            }
        }
        finally
        {
            LockObj.ExitWriteLock();
        }
    }
}