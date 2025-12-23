using System.Net;
using System.Net.Http.Headers;

namespace DevBase.Requests.Core;

internal class BaseResponse
{
    public byte[] Buffer { get; private set; }
    public string Content { get; private set; }

    // TODO: Implement rendering
    public object Rendered { get; private set; }
    
    public CookieCollection Cookies { get; private set; }
    public HttpResponseHeaders Headers { get; private set; }
    
    public HttpStatusCode Code { get; private set; }

    public BaseResponse(HttpResponseMessage response)
    {
        // Response reading etc
    }
}