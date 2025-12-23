namespace DevBase.Net.Proxy.HttpToSocks5;

/// <summary>
/// Result of parsing an HTTP request for proxy tunneling.
/// </summary>
internal struct HttpRequestParseResult
{
    public bool Success;
    public string? Hostname;
    public int Port;
    public string? HttpVersion;
    public bool IsConnect;
    public string? Request;
    public byte[]? OverRead;
}
