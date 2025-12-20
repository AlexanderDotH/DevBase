namespace DevBase.Requests.Cache;

/// <summary>
/// Represents a cached HTTP response.
/// </summary>
public sealed class CachedResponse
{
    public byte[] Content { get; init; } = [];
    public int StatusCode { get; init; }
    public Dictionary<string, string[]> Headers { get; init; } = [];
    public string? ContentType { get; init; }
    public DateTime CachedAt { get; init; }

    public static async Task<CachedResponse> FromResponseAsync(Response response, CancellationToken cancellationToken = default)
    {
        var content = await response.GetBytesAsync(cancellationToken);
        var headers = new Dictionary<string, string[]>();

        foreach (var header in response.Headers)
        {
            headers[header.Key] = header.Value.ToArray();
        }

        return new CachedResponse
        {
            Content = content,
            StatusCode = (int)response.StatusCode,
            Headers = headers,
            ContentType = response.ContentType,
            CachedAt = DateTime.UtcNow
        };
    }
}
