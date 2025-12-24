using DevBase.Net.Proxy;

namespace DevBase.Net.Batch.Strategies;

public sealed class RoundRobinStrategy : IProxyRotationStrategy
{
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
