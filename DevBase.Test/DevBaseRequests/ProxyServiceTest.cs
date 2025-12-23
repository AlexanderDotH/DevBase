using DevBase.Net.Proxy;
using DevBase.Net.Proxy.Enums;
using NUnit.Framework;

namespace DevBase.Test.DevBaseRequests;

[TestFixture]
public class ProxyServiceTest
{
    #region ProxyInfo Tests

    [Test]
    public void ProxyInfo_Parse_HttpProxy()
    {
        var proxy = ProxyInfo.Parse("http://proxy.example.com:8080");
        
        Assert.That(proxy.Host, Is.EqualTo("proxy.example.com"));
        Assert.That(proxy.Port, Is.EqualTo(8080));
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Http));
    }

    [Test]
    public void ProxyInfo_Parse_HttpsProxy()
    {
        var proxy = ProxyInfo.Parse("https://secure.proxy.com:443");
        
        Assert.That(proxy.Host, Is.EqualTo("secure.proxy.com"));
        Assert.That(proxy.Port, Is.EqualTo(443));
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Https));
    }

    [Test]
    public void ProxyInfo_Parse_Socks5Proxy()
    {
        var proxy = ProxyInfo.Parse("socks5://socks.proxy.com:1080");
        
        Assert.That(proxy.Host, Is.EqualTo("socks.proxy.com"));
        Assert.That(proxy.Port, Is.EqualTo(1080));
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Socks5));
    }

    [Test]
    public void ProxyInfo_Parse_Socks5hProxy()
    {
        var proxy = ProxyInfo.Parse("socks5h://socks.proxy.com:1080");
        
        Assert.That(proxy.Host, Is.EqualTo("socks.proxy.com"));
        Assert.That(proxy.Port, Is.EqualTo(1080));
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Socks5h));
    }

    [Test]
    public void ProxyInfo_Parse_WithCredentials()
    {
        var proxy = ProxyInfo.Parse("http://user:pass@proxy.example.com:8080");
        
        Assert.That(proxy.Host, Is.EqualTo("proxy.example.com"));
        Assert.That(proxy.Port, Is.EqualTo(8080));
        Assert.That(proxy.Credentials, Is.Not.Null);
        Assert.That(proxy.Credentials?.UserName, Is.EqualTo("user"));
        Assert.That(proxy.Credentials?.Password, Is.EqualTo("pass"));
    }

    [Test]
    public void ProxyInfo_ToUri_ReturnsCorrectUri()
    {
        var proxy = ProxyInfo.Parse("http://proxy.example.com:8080");
        var uri = proxy.ToUri();
        
        Assert.That(uri.Host, Is.EqualTo("proxy.example.com"));
        Assert.That(uri.Port, Is.EqualTo(8080));
    }

    [Test]
    public void ProxyInfo_Key_ReturnsUniqueKey()
    {
        var proxy1 = ProxyInfo.Parse("http://proxy1.example.com:8080");
        var proxy2 = ProxyInfo.Parse("http://proxy2.example.com:8080");
        
        Assert.That(proxy1.Key, Is.Not.EqualTo(proxy2.Key));
    }

    #endregion

    #region TrackedProxyInfo Tests

    [Test]
    public void TrackedProxyInfo_Constructor_InitializesCorrectly()
    {
        var proxyInfo = ProxyInfo.Parse("http://proxy.example.com:8080");
        var tracked = new TrackedProxyInfo(proxyInfo);
        
        Assert.That(tracked.Proxy, Is.EqualTo(proxyInfo));
        Assert.That(tracked.FailureCount, Is.EqualTo(0));
        Assert.That(tracked.IsAvailable(), Is.True);
    }

    [Test]
    public void TrackedProxyInfo_ReportFailure_IncrementsFailureCount()
    {
        var proxyInfo = ProxyInfo.Parse("http://proxy.example.com:8080");
        var tracked = new TrackedProxyInfo(proxyInfo);
        
        tracked.ReportFailure();
        Assert.That(tracked.FailureCount, Is.EqualTo(1));
        
        tracked.ReportFailure();
        Assert.That(tracked.FailureCount, Is.EqualTo(2));
    }

    [Test]
    public void TrackedProxyInfo_ReportSuccess_ResetsFailureCount()
    {
        var proxyInfo = ProxyInfo.Parse("http://proxy.example.com:8080");
        var tracked = new TrackedProxyInfo(proxyInfo);
        
        tracked.ReportFailure();
        tracked.ReportFailure();
        tracked.ReportSuccess();
        
        Assert.That(tracked.FailureCount, Is.EqualTo(0));
    }

    [Test]
    public void TrackedProxyInfo_MaxFailures_BecomesUnavailable()
    {
        var proxyInfo = ProxyInfo.Parse("http://proxy.example.com:8080");
        var tracked = new TrackedProxyInfo(proxyInfo, maxFailures: 3);
        
        tracked.ReportFailure();
        tracked.ReportFailure();
        tracked.ReportFailure();
        
        Assert.That(tracked.IsAvailable(), Is.False);
    }

    [Test]
    public void TrackedProxyInfo_ToWebProxy_ReturnsWebProxy()
    {
        var proxyInfo = ProxyInfo.Parse("http://proxy.example.com:8080");
        var tracked = new TrackedProxyInfo(proxyInfo);
        
        var webProxy = tracked.ToWebProxy();
        
        Assert.That(webProxy, Is.Not.Null);
        if (webProxy is System.Net.WebProxy wp)
            Assert.That(wp.Address?.Host, Is.EqualTo("proxy.example.com"));
    }

    #endregion

    #region ProxyService Tests

    [Test]
    public void ProxyService_Constructor_WithProxies()
    {
        var proxies = new List<ProxyInfo>
        {
            ProxyInfo.Parse("http://proxy1.example.com:8080"),
            ProxyInfo.Parse("http://proxy2.example.com:8080")
        };
        
        using var service = new ProxyService(proxies);
        
        Assert.That(service.ValidProxyCount, Is.EqualTo(2));
    }

    [Test]
    public void ProxyService_GetNextProxy_ReturnsProxy()
    {
        var proxies = new List<ProxyInfo>
        {
            ProxyInfo.Parse("http://proxy.example.com:8080")
        };
        
        using var service = new ProxyService(proxies);
        
        var proxy = service.GetNextProxy();
        
        Assert.That(proxy, Is.Not.Null);
    }

    [Test]
    public void ProxyService_GetNextProxy_RoundRobin()
    {
        var proxies = new List<ProxyInfo>
        {
            ProxyInfo.Parse("http://proxy1.example.com:8080"),
            ProxyInfo.Parse("http://proxy2.example.com:8080")
        };
        
        using var service = new ProxyService(proxies);
        
        var first = service.GetNextProxy();
        var second = service.GetNextProxy();
        var third = service.GetNextProxy();
        
        Assert.That(first?.Proxy.Host, Is.Not.EqualTo(second?.Proxy.Host));
        Assert.That(first?.Proxy.Host, Is.EqualTo(third?.Proxy.Host));
    }

    [Test]
    public void ProxyService_GetRandomProxy_ReturnsProxy()
    {
        var proxies = new List<ProxyInfo>
        {
            ProxyInfo.Parse("http://proxy1.example.com:8080"),
            ProxyInfo.Parse("http://proxy2.example.com:8080")
        };
        
        using var service = new ProxyService(proxies);
        
        var proxy = service.GetRandomProxy();
        
        Assert.That(proxy, Is.Not.Null);
    }

    [Test]
    public void ProxyService_Empty_ReturnsNull()
    {
        using var service = new ProxyService(new List<ProxyInfo>());
        
        var proxy = service.GetNextProxy();
        
        Assert.That(proxy, Is.Null);
    }

    [Test]
    public void ProxyService_GetTimeoutStats_ReturnsStatistics()
    {
        var proxies = new List<ProxyInfo>
        {
            ProxyInfo.Parse("http://proxy1.example.com:8080"),
            ProxyInfo.Parse("http://proxy2.example.com:8080")
        };
        
        using var service = new ProxyService(proxies);
        
        var stats = service.GetTimeoutStats();
        
        Assert.That(stats.TotalProxies, Is.EqualTo(2));
        Assert.That(stats.ActiveProxies, Is.EqualTo(2));
    }

    [Test]
    public void ProxyService_Dispose_ClearsProxies()
    {
        var proxies = new List<ProxyInfo>
        {
            ProxyInfo.Parse("http://proxy.example.com:8080")
        };
        
        var service = new ProxyService(proxies);
        
        service.Dispose();
        
        Assert.That(service.ValidProxyCount, Is.EqualTo(0));
    }

    [Test]
    public void ProxyService_HasProxies_ReturnsTrueWhenPopulated()
    {
        var proxies = new List<ProxyInfo>
        {
            ProxyInfo.Parse("http://proxy.example.com:8080")
        };
        
        using var service = new ProxyService(proxies);
        
        Assert.That(service.HasProxies, Is.True);
    }

    [Test]
    public void ProxyService_HasProxies_ReturnsFalseWhenEmpty()
    {
        using var service = new ProxyService(new List<ProxyInfo>());
        
        Assert.That(service.HasProxies, Is.False);
    }

    #endregion
}
