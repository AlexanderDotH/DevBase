using Newtonsoft.Json.Linq;

namespace DevBase.Api.Apis.Musixmatch.Internal;

internal static class MusixMatchResponseParser
{
    public static MusixMatchApiResponse Parse(string content, bool strictErrorHandling)
    {
        JObject response = JObject.Parse(content);
        JToken? header = response.SelectToken("message.header") ?? response.SelectToken("$..header");
        JToken? body = response.SelectToken("message.body") ?? response.SelectToken("$..body");
        int statusCode = header?.SelectToken("status_code")?.Value<int?>()
            ?? response.SelectToken("$..status_code")?.Value<int?>()
            ?? (int)MusixMatchStatusCode.Undefined;

        if (statusCode != (int)MusixMatchStatusCode.Success && strictErrorHandling) {
            throw new MusixMatchRequestException(statusCode);
        }

        return new MusixMatchApiResponse
        {
            StatusCode = statusCode,
            TimeElapsed = header?.SelectToken("execute_time")?.Value<double?>() ?? 0,
            Header = header?.ToString() ?? "{}",
            Body = body?.ToString() ?? "{}"
        };
    }
}
