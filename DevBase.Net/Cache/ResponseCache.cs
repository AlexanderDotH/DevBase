using System.Security.Cryptography;
using System.Text;
using DevBase.Net.Core;
using ZiggyCreatures.Caching.Fusion;

namespace DevBase.Net.Cache;

/// <summary>
/// Provides caching mechanisms for HTTP responses.
/// </summary>
public sealed class ResponseCache : IDisposable
{
    private readonly IFusionCache _cache;
    private bool _disposed;

    /// <summary>
    /// Gets or sets a value indicating whether caching is enabled.
    /// </summary>
    public bool Enabled { get; set; }
    
    /// <summary>
    /// Gets or sets the default expiration duration for cached items.
    /// </summary>
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseCache"/> class.
    /// </summary>
    /// <param name="cache">Optional underlying cache implementation. Uses FusionCache by default.</param>
    public ResponseCache(IFusionCache? cache = null)
    {
        _cache = cache ?? new FusionCache(new FusionCacheOptions
        {
            DefaultEntryOptions = new FusionCacheEntryOptions
            {
                Duration = DefaultExpiration
            }
        });
    }

    /// <summary>
    /// Retrieves a cached response for the specified request asynchronously.
    /// </summary>
    /// <param name="request">The request to lookup.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cached response, or null if not found or caching is disabled.</returns>
    public async Task<CachedResponse?> GetAsync(Core.Request request, CancellationToken cancellationToken = default)
    {
        if (!Enabled)
            return null;

        string key = GenerateCacheKey(request);
        return await _cache.GetOrDefaultAsync<CachedResponse>(key, token: cancellationToken);
    }

    /// <summary>
    /// Caches an HTTP response for the specified request asynchronously.
    /// </summary>
    /// <param name="request">The request associated with the response.</param>
    /// <param name="response">The response to cache.</param>
    /// <param name="expiration">Optional expiration duration. Uses DefaultExpiration if null.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task SetAsync(Core.Request request, Response response, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        if (!Enabled)
            return;

        string key = GenerateCacheKey(request);
        CachedResponse cached = await CachedResponse.FromResponseAsync(response, cancellationToken);
        
        await _cache.SetAsync(key, cached, expiration ?? DefaultExpiration, token: cancellationToken);
    }

    /// <summary>
    /// Removes a cached response for the specified request asynchronously.
    /// </summary>
    /// <param name="request">The request key to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the operation completed.</returns>
    public async Task<bool> RemoveAsync(Core.Request request, CancellationToken cancellationToken = default)
    {
        string key = GenerateCacheKey(request);
        await _cache.RemoveAsync(key, token: cancellationToken);
        return true;
    }

    /// <summary>
    /// Clears all entries from the cache asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        await _cache.ExpireAsync("*", token: cancellationToken);
    }

    private static string GenerateCacheKey(Core.Request request)
    {
        StringBuilder keyBuilder = new StringBuilder();
        keyBuilder.Append(request.Method);
        keyBuilder.Append('|');
        keyBuilder.Append(request.GetUri()?.ToString() ?? string.Empty);

        byte[] keyBytes = Encoding.UTF8.GetBytes(keyBuilder.ToString());
        byte[] hashBytes = SHA256.HashData(keyBytes);
        return Convert.ToHexString(hashBytes);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        
        if (_cache is IDisposable disposable)
            disposable.Dispose();
    }
}
