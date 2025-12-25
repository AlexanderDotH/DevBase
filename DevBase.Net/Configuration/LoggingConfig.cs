using DevBase.Net.Configuration.Enums;
using Serilog;

namespace DevBase.Net.Configuration;

/// <summary>
/// Configuration for request/response logging.
/// </summary>
public sealed class LoggingConfig
{
    /// <summary>
    /// Gets the logger instance to use.
    /// </summary>
    public ILogger? Logger { get; init; }
    
    /// <summary>
    /// Gets the log level. Defaults to Normal.
    /// </summary>
    public EnumRequestLogLevel LogLevel { get; init; } = EnumRequestLogLevel.Normal;
    
    /// <summary>
    /// Gets a value indicating whether request headers should be logged.
    /// </summary>
    public bool LogRequestHeaders { get; init; }
    
    /// <summary>
    /// Gets a value indicating whether response headers should be logged.
    /// </summary>
    public bool LogResponseHeaders { get; init; }
    
    /// <summary>
    /// Gets a value indicating whether the request body should be logged.
    /// </summary>
    public bool LogRequestBody { get; init; }
    
    /// <summary>
    /// Gets a value indicating whether the response body should be logged.
    /// </summary>
    public bool LogResponseBody { get; init; }
    
    /// <summary>
    /// Gets a value indicating whether timing information should be logged. Defaults to true.
    /// </summary>
    public bool LogTiming { get; init; } = true;
    
    /// <summary>
    /// Gets a value indicating whether proxy information should be logged. Defaults to true.
    /// </summary>
    public bool LogProxyInfo { get; init; } = true;

    /// <summary>
    /// Gets a configuration with no logging enabled.
    /// </summary>
    public static LoggingConfig None => new() { LogLevel = EnumRequestLogLevel.None };
    
    /// <summary>
    /// Gets a configuration with minimal logging enabled.
    /// </summary>
    public static LoggingConfig Minimal => new() { LogLevel = EnumRequestLogLevel.Minimal };
    
    /// <summary>
    /// Gets a configuration with verbose logging enabled (all details).
    /// </summary>
    public static LoggingConfig Verbose => new()
    {
        LogLevel = EnumRequestLogLevel.Verbose,
        LogRequestHeaders = true,
        LogResponseHeaders = true,
        LogRequestBody = true,
        LogResponseBody = true,
        LogTiming = true,
        LogProxyInfo = true
    };
}
