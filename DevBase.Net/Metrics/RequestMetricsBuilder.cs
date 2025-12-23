namespace DevBase.Net.Metrics;

/// <summary>
/// Builder for constructing RequestMetrics instances.
/// </summary>
public sealed class RequestMetricsBuilder
{
    private readonly DateTime _startTime = DateTime.UtcNow;
    private DateTime _dnsStart, _dnsEnd;
    private DateTime _connectStart, _connectEnd;
    private DateTime _tlsStart, _tlsEnd;
    private DateTime _firstByteTime;
    private DateTime _downloadEnd;
    private long _bytesSent, _bytesReceived;
    private long _headersSent, _headersReceived;
    private bool _connectionReused;
    private string? _remoteAddress;
    private int _remotePort;
    private string? _protocol;
    private bool _usedProxy;
    private string? _proxyAddress;
    private bool _fromCache;
    private int _retryCount;

    public void MarkDnsStart() => _dnsStart = DateTime.UtcNow;
    public void MarkDnsEnd() => _dnsEnd = DateTime.UtcNow;
    public void MarkConnectStart() => _connectStart = DateTime.UtcNow;
    public void MarkConnectEnd() => _connectEnd = DateTime.UtcNow;
    public void MarkTlsStart() => _tlsStart = DateTime.UtcNow;
    public void MarkTlsEnd() => _tlsEnd = DateTime.UtcNow;
    public void MarkFirstByte() => _firstByteTime = DateTime.UtcNow;
    public void MarkDownloadEnd() => _downloadEnd = DateTime.UtcNow;
    
    public void SetBytesSent(long bytes) => _bytesSent = bytes;
    public void SetBytesReceived(long bytes) => _bytesReceived = bytes;
    public void SetHeadersSent(long bytes) => _headersSent = bytes;
    public void SetHeadersReceived(long bytes) => _headersReceived = bytes;
    public void SetConnectionReused(bool reused) => _connectionReused = reused;
    public void SetRemoteEndpoint(string? address, int port) { _remoteAddress = address; _remotePort = port; }
    public void SetProtocol(string? protocol) => _protocol = protocol;
    public void SetProxy(bool used, string? address = null) { _usedProxy = used; _proxyAddress = address; }
    public void SetFromCache(bool fromCache) => _fromCache = fromCache;
    public void IncrementRetryCount() => _retryCount++;

    public RequestMetrics Build()
    {
        DateTime endTime = DateTime.UtcNow;
        
        return new RequestMetrics
        {
            StartTime = _startTime,
            EndTime = endTime,
            Duration = endTime - _startTime,
            DnsLookupTime = _dnsEnd > _dnsStart ? _dnsEnd - _dnsStart : TimeSpan.Zero,
            ConnectionTime = _connectEnd > _connectStart ? _connectEnd - _connectStart : TimeSpan.Zero,
            TlsHandshakeTime = _tlsEnd > _tlsStart ? _tlsEnd - _tlsStart : TimeSpan.Zero,
            TimeToFirstByte = _firstByteTime > _startTime ? _firstByteTime - _startTime : TimeSpan.Zero,
            DownloadTime = _downloadEnd > _firstByteTime ? _downloadEnd - _firstByteTime : TimeSpan.Zero,
            BytesSent = _bytesSent,
            BytesReceived = _bytesReceived,
            HeadersSent = _headersSent,
            HeadersReceived = _headersReceived,
            ConnectionReused = _connectionReused,
            RemoteAddress = _remoteAddress,
            RemotePort = _remotePort,
            Protocol = _protocol,
            UsedProxy = _usedProxy,
            ProxyAddress = _proxyAddress,
            FromCache = _fromCache,
            RetryCount = _retryCount
        };
    }
}
