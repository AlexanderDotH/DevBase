using DevBase.Net.Configuration.Enums;

namespace DevBase.Net.Configuration;

/// <summary>
/// Configuration for host availability checks.
/// </summary>
public sealed class HostCheckConfig
{
    /// <summary>
    /// Gets the method used for checking host availability. Defaults to TCP Connect.
    /// </summary>
    public EnumHostCheckMethod Method { get; init; } = EnumHostCheckMethod.TcpConnect;
    
    /// <summary>
    /// Gets the timeout for the host check. Defaults to 5 seconds.
    /// </summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(5);
    
    /// <summary>
    /// Gets the port to check (for TCP Connect). Defaults to 443 (HTTPS).
    /// </summary>
    public int Port { get; init; } = 443;

    /// <summary>
    /// Gets the default configuration.
    /// </summary>
    public static HostCheckConfig Default => new()
    {
        Method = EnumHostCheckMethod.TcpConnect,
        Timeout = TimeSpan.FromSeconds(5)
    };
}
