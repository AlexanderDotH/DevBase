using DevBase.Net.Proxy;

namespace DevBase.Net.Batch.Strategies;

/// <summary>
/// Defines a strategy for selecting a proxy from a pool.
/// </summary>
public interface IProxyRotationStrategy
{
    /// <summary>
    /// Selects a proxy from the provided list based on the strategy logic.
    /// </summary>
    /// <param name="proxies">The list of available proxies.</param>
    /// <param name="currentIndex">Reference to the current index, which may be updated by the strategy.</param>
    /// <returns>The selected proxy, or null if no proxy could be selected.</returns>
    TrackedProxyInfo? SelectProxy(List<TrackedProxyInfo> proxies, ref int currentIndex);
}
