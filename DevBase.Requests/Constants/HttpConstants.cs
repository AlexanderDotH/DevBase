namespace DevBase.Requests.Constants;

public static class HttpConstants
{
    public static readonly ReadOnlyMemory<char> Http401Unauthorized = "401 Unauthorized\r\n\r\n".AsMemory();
    public static readonly ReadOnlyMemory<char> Http502Prefix = "502 ".AsMemory();
    public static readonly ReadOnlyMemory<char> Http500InternalError = "500 Internal Server Error\r\nX-Proxy-Error-Type: ".AsMemory();
    public static readonly ReadOnlyMemory<char> HttpCrlfCrlf = "\r\n\r\n".AsMemory();
    public static readonly ReadOnlyMemory<char> HttpConnectionEstablished = "200 Connection established\r\nProxy-Agent: DevBase-HttpToSocks5Proxy\r\n\r\n".AsMemory();
    public static readonly ReadOnlyMemory<char> HttpSchemePrefix = "http://127.0.0.1:".AsMemory();
    public static readonly ReadOnlyMemory<char> ProtocolHttpPrefix = "HTTP/".AsMemory();
    public static readonly ReadOnlyMemory<char> DirectProxyKey = "direct".AsMemory();
}
