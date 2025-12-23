namespace DevBase.Net.Constants;

public static class AuthConstants
{
    public static readonly ReadOnlyMemory<char> Basic = "Basic".AsMemory();
    public static readonly ReadOnlyMemory<char> Bearer = "Bearer".AsMemory();
}
