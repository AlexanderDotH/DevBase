namespace DevBase.Net.Constants;

public static class PlatformConstants
{
    public static readonly ReadOnlyMemory<char> Windows = "Windows".AsMemory();
    public static readonly ReadOnlyMemory<char> Linux = "Linux".AsMemory();
    public static readonly ReadOnlyMemory<char> MacOS = "macOS".AsMemory();
}
