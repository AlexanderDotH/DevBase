using System.Collections.Frozen;
using DevBase.Net.Core;

namespace DevBase.Net.Cache;

/// <summary>
/// Represents a cached HTTP response.
/// </summary>
public sealed class CachedResponse
{
    /// <summary>
    /// Gets the raw content bytes of the response.
    /// </summary>
    public byte[] Content { get; init; } = [];
    
    /// <summary>
    /// Gets the HTTP status code of the response.
    /// </summary>
    public int StatusCode { get; init; }
    
    /// <summary>
    /// Gets the headers of the response.
    /// </summary>
    public FrozenDictionary<string, string[]> Headers { get; init; } = FrozenDictionary<string, string[]>.Empty;
    
    /// <summary>
    /// Gets the content type of the response.
    /// </summary>
    public string? ContentType { get; init; }
    
    /// <summary>
    /// Gets the timestamp when the response was cached.
    /// </summary>
    public DateTime CachedAt { get; init; }

    /// <summary>
    /// Creates a cached response from a live HTTP response asynchronously.
    /// </summary>
    /// <param name="response">The HTTP response to cache.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A new <see cref="CachedResponse"/> instance.</returns>
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
