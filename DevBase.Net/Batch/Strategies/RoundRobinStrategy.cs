using DevBase.Net.Proxy;

namespace DevBase.Net.Batch.Strategies;

/// <summary>
/// Proxy rotation strategy that selects proxies in a round-robin fashion.
/// </summary>
public sealed class RoundRobinStrategy : IProxyRotationStrategy
{
    /// <inheritdoc />
    public TrackedProxyInfo? SelectProxy(List<TrackedProxyInfo> proxies, ref int currentIndex)
    {
        if (proxies.Count == 0)
            return null;

        int attempts = 0;
        while (attempts < proxies.Count)
        {
            currentIndex = (currentIndex + 1) % proxies.Count;
            TrackedProxyInfo proxy = proxies[currentIndex];

            if (proxy.IsAvailable())
                return proxy;

            attempts++;
        }

        return null;
    }
}
