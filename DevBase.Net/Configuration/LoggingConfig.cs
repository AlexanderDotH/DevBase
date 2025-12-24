using DevBase.Net.Configuration.Enums;
using Serilog;

namespace DevBase.Net.Configuration;

public sealed class LoggingConfig
{
    public ILogger? Logger { get; init; }
    public EnumRequestLogLevel LogLevel { get; init; } = EnumRequestLogLevel.Normal;
    public bool LogRequestHeaders { get; init; }
    public bool LogResponseHeaders { get; init; }
    public bool LogRequestBody { get; init; }
    public bool LogResponseBody { get; init; }
    public bool LogTiming { get; init; } = true;
    public bool LogProxyInfo { get; init; } = true;

    public static LoggingConfig None => new() { LogLevel = EnumRequestLogLevel.None };
    
    public static LoggingConfig Minimal => new() { LogLevel = EnumRequestLogLevel.Minimal };
    
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
