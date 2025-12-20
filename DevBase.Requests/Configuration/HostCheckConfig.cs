namespace DevBase.Requests.Configuration;

public enum HostCheckMethod
{
    Ping,
    TcpConnect
}

public sealed class HostCheckConfig
{
    public bool Enabled { get; init; }
    public HostCheckMethod Method { get; init; } = HostCheckMethod.TcpConnect;
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(5);
    public int Port { get; init; } = 443;

    public static HostCheckConfig Default => new()
    {
        Enabled = true,
        Method = HostCheckMethod.TcpConnect,
        Timeout = TimeSpan.FromSeconds(5)
    };
}
