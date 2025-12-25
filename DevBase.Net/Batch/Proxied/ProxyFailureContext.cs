using DevBase.Net.Core;
using DevBase.Net.Proxy;

namespace DevBase.Net.Batch.Proxied;

/// <summary>
/// Provides context about a proxy failure event.
/// </summary>
public sealed record ProxyFailureContext(
    TrackedProxyInfo Proxy,
    System.Exception Exception,
    Request FailedRequest,
    bool ProxyTimedOut,
    int CurrentFailureCount,
    int RemainingAvailableProxies
);
