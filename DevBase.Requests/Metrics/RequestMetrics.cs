namespace DevBase.Requests.Metrics;

public sealed class RequestMetrics
{
    public TimeSpan Duration { get; init; }
    public TimeSpan DnsLookupTime { get; init; }
    public TimeSpan ConnectionTime { get; init; }
    public TimeSpan TlsHandshakeTime { get; init; }
    public TimeSpan TimeToFirstByte { get; init; }
    public TimeSpan DownloadTime { get; init; }
    
    public long BytesSent { get; init; }
    public long BytesReceived { get; init; }
    public long HeadersSent { get; init; }
    public long HeadersReceived { get; init; }
    
    public bool ConnectionReused { get; init; }
    public string? RemoteAddress { get; init; }
    public int RemotePort { get; init; }
    public string? Protocol { get; init; }
    
    public bool UsedProxy { get; init; }
    public string? ProxyAddress { get; init; }
    
    public bool FromCache { get; init; }
    public int RetryCount { get; init; }
    
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }

    public override string ToString() =>
        $"Duration: {Duration.TotalMilliseconds:F0}ms, " +
        $"Bytes: {BytesSent}↑/{BytesReceived}↓, " +
        $"Protocol: {Protocol ?? "unknown"}, " +
        $"Retries: {RetryCount}";
}
