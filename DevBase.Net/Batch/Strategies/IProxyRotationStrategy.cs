using DevBase.Net.Proxy;

namespace DevBase.Net.Batch.Strategies;

public interface IProxyRotationStrategy
{
    TrackedProxyInfo? SelectProxy(List<TrackedProxyInfo> proxies, ref int currentIndex);
}
