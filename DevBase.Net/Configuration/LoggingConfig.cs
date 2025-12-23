using Serilog;

namespace DevBase.Net.Configuration;

public enum RequestLogLevel
{
    None,
    Minimal,
    Normal,
    Verbose
}

public sealed class LoggingConfig
{
    public ILogger? Logger { get; init; }
    public RequestLogLevel LogLevel { get; init; } = RequestLogLevel.Normal;
    public bool LogRequestHeaders { get; init; }
    public bool LogResponseHeaders { get; init; }
    public bool LogRequestBody { get; init; }
    public bool LogResponseBody { get; init; }
    public bool LogTiming { get; init; } = true;
    public bool LogProxyInfo { get; init; } = true;

    public static LoggingConfig None => new() { LogLevel = RequestLogLevel.None };
    
    public static LoggingConfig Minimal => new() { LogLevel = RequestLogLevel.Minimal };
    
    public static LoggingConfig Verbose => new()
    {
        LogLevel = RequestLogLevel.Verbose,
        LogRequestHeaders = true,
        LogResponseHeaders = true,
        LogRequestBody = true,
        LogResponseBody = true,
        LogTiming = true,
        LogProxyInfo = true
    };
}
