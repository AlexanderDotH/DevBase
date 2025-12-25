using DevBase.Net.Proxy;

namespace DevBase.Net.Batch.Strategies;

/// <summary>
/// Proxy rotation strategy that attempts to stick to the current proxy if it is available.
/// </summary>
public sealed class StickyStrategy : IProxyRotationStrategy
{
    /// <inheritdoc />
    public TrackedProxyInfo? SelectProxy(List<TrackedProxyInfo> proxies, ref int currentIndex)
    {
        if (proxies.Count == 0)
            return null;

        if (currentIndex >= 0 && currentIndex < proxies.Count)
        {
            TrackedProxyInfo current = proxies[currentIndex];
            if (current.IsAvailable())
                return current;
        }

        for (int i = 0; i < proxies.Count; i++)
        {
            if (proxies[i].IsAvailable())
            {
                currentIndex = i;
                return proxies[i];
            }
        }

        return null;
    }
}
