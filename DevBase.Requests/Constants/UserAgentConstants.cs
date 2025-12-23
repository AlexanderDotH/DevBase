namespace DevBase.Requests.Constants;

public static class UserAgentConstants
{
    public static readonly ReadOnlyMemory<char> AppleWebKit = "AppleWebKit".AsMemory();
    public static readonly ReadOnlyMemory<char> KhtmlLikeGecko = "(KHTML, like Gecko)".AsMemory();
    public static readonly ReadOnlyMemory<char> Chrome = "Chrome".AsMemory();
    public static readonly ReadOnlyMemory<char> Safari = "Safari".AsMemory();
    public static readonly ReadOnlyMemory<char> Firefox = "Firefox".AsMemory();
    public static readonly ReadOnlyMemory<char> Gecko = "Gecko".AsMemory();
    public static readonly ReadOnlyMemory<char> GeckoTrail = "20100101".AsMemory();
    public static readonly ReadOnlyMemory<char> RvPrefix = "; rv:".AsMemory();
    public static readonly ReadOnlyMemory<char> Edge = "Edg".AsMemory();
    public static readonly ReadOnlyMemory<char> Opera = "OPR".AsMemory();
}
