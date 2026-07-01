namespace DevBase.IntegrationTests.Infrastructure;

internal sealed record CapturedHttpRequest(
    string Method,
    string Target,
    string HttpVersion,
    IReadOnlyDictionary<string, string> Headers,
    string Body)
{
    public string Path => Target.Split('?', 2)[0];

    public string Query => Target.Contains('?') ? Target.Split('?', 2)[1] : string.Empty;
}
