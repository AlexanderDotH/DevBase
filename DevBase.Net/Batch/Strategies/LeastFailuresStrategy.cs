using DevBase.Net.Proxy;

namespace DevBase.Net.Batch.Strategies;

public sealed class LeastFailuresStrategy : IProxyRotationStrategy
{
    public TrackedProxyInfo? SelectProxy(List<TrackedProxyInfo> proxies, ref int currentIndex)
    {
        List<TrackedProxyInfo> available = proxies.Where(p => p.IsAvailable()).ToList();
        if (available.Count == 0)
            return null;

        TrackedProxyInfo selected = available.OrderBy(p => p.FailureCount).ThenBy(p => p.TotalTimeouts).First();
        currentIndex = proxies.IndexOf(selected);
        return selected;
    }
}
