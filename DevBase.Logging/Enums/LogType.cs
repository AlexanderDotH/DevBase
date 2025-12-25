namespace DevBase.Logging.Enums;

/// <summary>
/// Represents the severity level of a log message.
/// </summary>
public enum LogType
{
    /// <summary>
    /// Informational message, typically used for general application flow.
    /// </summary>
    INFO, 
    
    /// <summary>
    /// Debugging message, used for detailed information during development.
    /// </summary>
    DEBUG, 
    
    /// <summary>
    /// Error message, indicating a failure in a specific operation.
    /// </summary>
    ERROR, 
    
    /// <summary>
    /// Fatal error message, indicating a critical failure that may cause the application to crash.
    /// </summary>
    FATAL
}