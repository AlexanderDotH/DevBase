namespace DevBase.Net.Render;

public static class RequestUriRenderer
{
    public static ReadOnlyMemory<char> RenderUri(Uri requestUri) => requestUri.OriginalString.AsMemory();
}