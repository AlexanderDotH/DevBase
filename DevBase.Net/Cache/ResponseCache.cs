using System.Security.Cryptography;
using System.Text;
using DevBase.Net.Core;
using ZiggyCreatures.Caching.Fusion;

namespace DevBase.Net.Cache;

public sealed class ResponseCache : IDisposable
{
    private readonly IFusionCache _cache;
    private bool _disposed;

    public bool Enabled { get; set; }
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(5);

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

    public async Task<CachedResponse?> GetAsync(Core.Request request, CancellationToken cancellationToken = default)
    {
        if (!Enabled)
            return null;

        string key = GenerateCacheKey(request);
        return await _cache.GetOrDefaultAsync<CachedResponse>(key, token: cancellationToken);
    }

    public async Task SetAsync(Core.Request request, Response response, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        if (!Enabled)
            return;

        string key = GenerateCacheKey(request);
        CachedResponse cached = await CachedResponse.FromResponseAsync(response, cancellationToken);
        
        await _cache.SetAsync(key, cached, expiration ?? DefaultExpiration, token: cancellationToken);
    }

    public async Task<bool> RemoveAsync(Core.Request request, CancellationToken cancellationToken = default)
    {
        string key = GenerateCacheKey(request);
        await _cache.RemoveAsync(key, token: cancellationToken);
        return true;
    }

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

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        
        if (_cache is IDisposable disposable)
            disposable.Dispose();
    }
}
