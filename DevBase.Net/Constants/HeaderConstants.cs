namespace DevBase.Net.Constants;

public static class HeaderConstants
{
    public static readonly ReadOnlyMemory<char> Accept = "Accept".AsMemory();
    public static readonly ReadOnlyMemory<char> UserAgent = "User-Agent".AsMemory();
    public static readonly ReadOnlyMemory<char> ContentLength = "Content-Length".AsMemory();
    public static readonly ReadOnlyMemory<char> ContentType = "Content-Type".AsMemory();
    public static readonly ReadOnlyMemory<char> AcceptEncoding = "Accept-Encoding".AsMemory();
    public static readonly ReadOnlyMemory<char> Authorization = "Authorization".AsMemory();
    public static readonly ReadOnlyMemory<char> Referer = "Referer".AsMemory();
    public static readonly ReadOnlyMemory<char> Cookie = "Cookie".AsMemory();
    public static readonly ReadOnlyMemory<char> SetCookie = "Set-Cookie".AsMemory();
}
