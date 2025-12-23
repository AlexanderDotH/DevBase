namespace DevBase.Net.Proxy;

public sealed class ProxyTimeoutStats
{
    public int TotalProxies { get; init; }
    public int ActiveProxies { get; init; }
    public int TimedOutProxies { get; init; }
    public int TotalTimeoutEvents { get; init; }
    
    public double ActivePercentage => TotalProxies > 0 
        ? (double)ActiveProxies / TotalProxies * 100 
        : 0;

    public override string ToString() =>
        $"Proxies: {ActiveProxies}/{TotalProxies} active ({ActivePercentage:F1}%), {TimedOutProxies} timed out, {TotalTimeoutEvents} total timeout events";
}
