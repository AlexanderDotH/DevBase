namespace DevBase.Net.Proxy.HttpToSocks5;

/// <summary>
/// Result of parsing HTTP headers from a socket.
/// </summary>
internal struct HeadersParseResult
{
    public bool Success;
    public string? Headers;
    public byte[]? OverRead;
}
