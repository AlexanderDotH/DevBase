using DevBase.Net.Configuration.Enums;

namespace DevBase.Net.Configuration;

public sealed class HostCheckConfig
{
    public bool Enabled { get; init; }
    public EnumHostCheckMethod Method { get; init; } = EnumHostCheckMethod.TcpConnect;
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(5);
    public int Port { get; init; } = 443;

    public static HostCheckConfig Default => new()
    {
        Enabled = true,
        Method = EnumHostCheckMethod.TcpConnect,
        Timeout = TimeSpan.FromSeconds(5)
    };
}
