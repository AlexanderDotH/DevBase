namespace DevBase.Net.Constants;

public static class EncodingConstants
{
    public static readonly ReadOnlyMemory<char> Gzip = "gzip".AsMemory();
    public static readonly ReadOnlyMemory<char> Deflate = "deflate".AsMemory();
    public static readonly ReadOnlyMemory<char> Br = "br".AsMemory();
    public static readonly ReadOnlyMemory<char> Identity = "identity".AsMemory();
}
