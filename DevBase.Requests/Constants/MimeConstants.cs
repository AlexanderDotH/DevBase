namespace DevBase.Requests.Constants;

public static class MimeConstants
{
    public static readonly ReadOnlyMemory<char> Wildcard = "*/*".AsMemory();
    public static readonly ReadOnlyMemory<char> Json = "json".AsMemory();
    public static readonly ReadOnlyMemory<char> Xml = "xml".AsMemory();
    public static readonly ReadOnlyMemory<char> Html = "html".AsMemory();
}
