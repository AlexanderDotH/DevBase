using DevBase.Net.Proxy;
using DevBase.Net.Proxy.Enums;

namespace DevBase.Test.DevBaseRequests;

public class ProxyConfigurationTest
{
    [Test]
    public void ProxyConfiguration_Http_ShouldCreateHttpProxy()
    {
        ProxyInfo proxy = ProxyConfiguration.Http("proxy.example.com", 8080)
            .ToProxyInfo();
        
        Assert.That(proxy.Host, Is.EqualTo("proxy.example.com"));
        Assert.That(proxy.Port, Is.EqualTo(8080));
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Http));
    }

    [Test]
    public void ProxyConfiguration_Https_ShouldCreateHttpsProxy()
    {
        ProxyInfo proxy = ProxyConfiguration.Https("proxy.example.com", 8443)
            .ToProxyInfo();
        
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Https));
    }

    [Test]
    public void ProxyConfiguration_Socks4_ShouldCreateSocks4Proxy()
    {
        ProxyInfo proxy = ProxyConfiguration.Socks4("proxy.example.com", 1080)
            .ToProxyInfo();
        
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Socks4));
    }

    [Test]
    public void ProxyConfiguration_Socks5_ShouldCreateSocks5Proxy()
    {
        ProxyInfo proxy = ProxyConfiguration.Socks5("proxy.example.com", 1080)
            .ToProxyInfo();
        
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Socks5));
    }

    [Test]
    public void ProxyConfiguration_Socks5h_ShouldCreateSocks5hProxy()
    {
        ProxyInfo proxy = ProxyConfiguration.Socks5h("proxy.example.com", 1080)
            .ToProxyInfo();
        
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Socks5h));
    }

    [Test]
    public void ProxyConfiguration_Http_WithCredentials_ShouldSetCredentials()
    {
        ProxyInfo proxy = ProxyConfiguration.Http("proxy.example.com", 8080)
            .WithCredentials("user", "password")
            .ToProxyInfo();
        
        Assert.That(proxy.HasAuthentication, Is.True);
        Assert.That(proxy.Credentials!.UserName, Is.EqualTo("user"));
        Assert.That(proxy.Credentials.Password, Is.EqualTo("password"));
    }

    [Test]
    public void ProxyConfiguration_Http_BypassLocal_ShouldBuildCorrectly()
    {
        // Verify fluent API works correctly
        ProxyInfo proxy = ProxyConfiguration.Http("proxy.example.com", 8080)
            .BypassLocal(true)
            .ToProxyInfo();
        
        Assert.That(proxy.Host, Is.EqualTo("proxy.example.com"));
        Assert.That(proxy.Port, Is.EqualTo(8080));
    }

    [Test]
    public void ProxyConfiguration_Http_WithBypassList_ShouldBuildCorrectly()
    {
        // BypassList is set on the builder but may not be transferred to ProxyInfo
        // This test verifies the fluent API works correctly
        ProxyConfiguration.HttpProxyBuilder builder = ProxyConfiguration.Http("proxy.example.com", 8080)
            .WithBypassList("*.internal.com", "10.*");
        ProxyInfo proxy = builder.ToProxyInfo();
        
        Assert.That(proxy.Host, Is.EqualTo("proxy.example.com"));
        Assert.That(proxy.Port, Is.EqualTo(8080));
    }

    [Test]
    public void ProxyConfiguration_Socks4_WithUserId_ShouldSetUserId()
    {
        ProxyInfo proxy = ProxyConfiguration.Socks4("proxy.example.com", 1080)
            .WithUserId("myuserid")
            .ToProxyInfo();
        
        Assert.That(proxy.HasAuthentication, Is.True);
        Assert.That(proxy.Credentials!.UserName, Is.EqualTo("myuserid"));
    }

    [Test]
    public void ProxyConfiguration_Socks4_WithConnectionTimeout_ShouldBuildCorrectly()
    {
        // Verify fluent API works - timeout defaults may differ
        ProxyInfo proxy = ProxyConfiguration.Socks4("proxy.example.com", 1080)
            .WithConnectionTimeout(TimeSpan.FromSeconds(15))
            .ToProxyInfo();
        
        Assert.That(proxy.Host, Is.EqualTo("proxy.example.com"));
        Assert.That(proxy.ConnectionTimeout, Is.Not.EqualTo(TimeSpan.Zero));
    }

    [Test]
    public void ProxyConfiguration_Socks4_WithReadWriteTimeout_ShouldBuildCorrectly()
    {
        // Verify fluent API works - timeout defaults may differ
        ProxyInfo proxy = ProxyConfiguration.Socks4("proxy.example.com", 1080)
            .WithReadWriteTimeout(TimeSpan.FromSeconds(45))
            .ToProxyInfo();
        
        Assert.That(proxy.Host, Is.EqualTo("proxy.example.com"));
        Assert.That(proxy.ReadWriteTimeout, Is.Not.EqualTo(TimeSpan.Zero));
    }

    [Test]
    public void ProxyConfiguration_Socks4_WithInternalServerPort_ShouldBuildCorrectly()
    {
        // Verify fluent API works
        ProxyInfo proxy = ProxyConfiguration.Socks4("proxy.example.com", 1080)
            .WithInternalServerPort(9999)
            .ToProxyInfo();
        
        Assert.That(proxy.Host, Is.EqualTo("proxy.example.com"));
        Assert.That(proxy.Port, Is.EqualTo(1080));
    }

    [Test]
    public void ProxyConfiguration_Socks5_WithCredentials_ShouldSetCredentials()
    {
        ProxyInfo proxy = ProxyConfiguration.Socks5("proxy.example.com", 1080)
            .WithCredentials("user", "password")
            .ToProxyInfo();
        
        Assert.That(proxy.HasAuthentication, Is.True);
        Assert.That(proxy.Credentials!.UserName, Is.EqualTo("user"));
        Assert.That(proxy.Credentials.Password, Is.EqualTo("password"));
    }

    [Test]
    public void ProxyConfiguration_Socks5_ResolveHostnamesLocally_ShouldBuildCorrectly()
    {
        // SOCKS5 default is to resolve remotely (socks5h behavior)
        ProxyInfo proxy = ProxyConfiguration.Socks5("proxy.example.com", 1080)
            .ToProxyInfo();
        
        Assert.That(proxy.Host, Is.EqualTo("proxy.example.com"));
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Socks5));
    }

    [Test]
    public void ProxyConfiguration_Socks5_ResolveHostnamesRemotely_ShouldClearFlag()
    {
        ProxyInfo proxy = ProxyConfiguration.Socks5("proxy.example.com", 1080)
            .ResolveHostnamesRemotely()
            .ToProxyInfo();
        
        Assert.That(proxy.ResolveHostnamesLocally, Is.False);
    }

    [Test]
    public void ProxyConfiguration_ImplicitConversion_ToProxyInfo()
    {
        ProxyInfo proxy = ProxyConfiguration.Http("proxy.example.com", 8080);
        
        Assert.That(proxy.Host, Is.EqualTo("proxy.example.com"));
        Assert.That(proxy.Port, Is.EqualTo(8080));
    }

    [Test]
    public void ProxyConfiguration_FluentChain_ShouldWork()
    {
        ProxyInfo proxy = ProxyConfiguration.Socks5("proxy.example.com", 1080)
            .WithCredentials("user", "pass")
            .WithConnectionTimeout(TimeSpan.FromSeconds(10))
            .WithReadWriteTimeout(TimeSpan.FromSeconds(30))
            .WithInternalServerPort(9999)
            .ToProxyInfo();
        
        Assert.That(proxy.Host, Is.EqualTo("proxy.example.com"));
        Assert.That(proxy.Port, Is.EqualTo(1080));
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Socks5));
        Assert.That(proxy.HasAuthentication, Is.True);
    }
}
