using System.Collections.Frozen;
using DevBase.Net.Core;

namespace DevBase.Net.Cache;

/// <summary>
/// Represents a cached HTTP response.
/// </summary>
public sealed class CachedResponse
{
    public byte[] Content { get; init; } = [];
    public int StatusCode { get; init; }
    public FrozenDictionary<string, string[]> Headers { get; init; } = FrozenDictionary<string, string[]>.Empty;
    public string? ContentType { get; init; }
    public DateTime CachedAt { get; init; }

    public static async Task<CachedResponse> FromResponseAsync(Response response, CancellationToken cancellationToken = default)
    {
        byte[] content = await response.GetBytesAsync(cancellationToken);
        FrozenDictionary<string, string[]> headers = response.Headers.ToFrozenDictionary(
            h => h.Key,
            h => h.Value.ToArray(),
            StringComparer.OrdinalIgnoreCase);

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
