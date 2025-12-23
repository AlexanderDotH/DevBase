namespace DevBase.Net.Constants;

public static class ProtocolConstants
{
    public static readonly ReadOnlyMemory<char> Http = "http".AsMemory();
    public static readonly ReadOnlyMemory<char> Https = "https".AsMemory();
}
