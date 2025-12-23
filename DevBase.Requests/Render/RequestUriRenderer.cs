using System.Text;

namespace DevBase.Requests.Render;

public static class RequestUriRenderer
{
    public static ReadOnlyMemory<char> RenderUri(Uri requestUri) => requestUri.OriginalString.AsMemory();
}