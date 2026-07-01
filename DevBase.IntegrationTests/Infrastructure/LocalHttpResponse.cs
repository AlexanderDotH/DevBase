using System.Net;

namespace DevBase.IntegrationTests.Infrastructure;

internal sealed record LocalHttpResponse(
    HttpStatusCode StatusCode,
    string ContentType,
    string Body)
{
    public static LocalHttpResponse Json(string body)
    {
        return new LocalHttpResponse(HttpStatusCode.OK, "application/json; charset=utf-8", body);
    }
}
