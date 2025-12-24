using DevBase.Net.Proxy;

namespace DevBase.Net.Batch.Strategies;

public sealed class RandomStrategy : IProxyRotationStrategy
{
    private static readonly Random Random = new();

    public TrackedProxyInfo? SelectProxy(List<TrackedProxyInfo> proxies, ref int currentIndex)
    {
        List<TrackedProxyInfo> available = proxies.Where(p => p.IsAvailable()).ToList();
        if (available.Count == 0)
            return null;

        int index = Random.Next(available.Count);
        TrackedProxyInfo selected = available[index];
        currentIndex = proxies.IndexOf(selected);
        return selected;
    }
}
