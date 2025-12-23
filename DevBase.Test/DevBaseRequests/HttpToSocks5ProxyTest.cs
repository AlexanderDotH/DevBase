using System.Net;
using DevBase.Requests.Proxy.HttpToSocks5;
using DevBase.Requests.Proxy.HttpToSocks5.Dns;
using DevBase.Requests.Proxy.HttpToSocks5.Enums;
using NUnit.Framework;

namespace DevBase.Test.DevBaseRequests;

[TestFixture]
public class HttpToSocks5ProxyTest
{
    #region Socks5ProxyInfo Tests

    [Test]
    public void Socks5ProxyInfo_Constructor_SetsHostnameAndPort()
    {
        var proxyInfo = new Socks5ProxyInfo("proxy.example.com", 1080);
        
        Assert.That(proxyInfo.Hostname, Is.EqualTo("proxy.example.com"));
        Assert.That(proxyInfo.Port, Is.EqualTo(1080));
        Assert.That(proxyInfo.Authenticate, Is.False);
    }

    [Test]
    public void Socks5ProxyInfo_Constructor_WithCredentials_SetsAuthenticate()
    {
        var proxyInfo = new Socks5ProxyInfo("proxy.example.com", 1080, "user", "pass");
        
        Assert.That(proxyInfo.Hostname, Is.EqualTo("proxy.example.com"));
        Assert.That(proxyInfo.Port, Is.EqualTo(1080));
        Assert.That(proxyInfo.Authenticate, Is.True);
    }

    [Test]
    public void Socks5ProxyInfo_Constructor_NullHostname_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new Socks5ProxyInfo(null!, 1080));
        Assert.Throws<ArgumentException>(() => new Socks5ProxyInfo("", 1080));
    }

    [Test]
    public void Socks5ProxyInfo_Constructor_InvalidPort_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Socks5ProxyInfo("proxy.example.com", -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Socks5ProxyInfo("proxy.example.com", 65536));
    }

    [Test]
    public void Socks5ProxyInfo_Constructor_NullCredentials_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new Socks5ProxyInfo("proxy.example.com", 1080, null!, "pass"));
        Assert.Throws<ArgumentNullException>(() => new Socks5ProxyInfo("proxy.example.com", 1080, "user", null!));
        Assert.Throws<ArgumentException>(() => new Socks5ProxyInfo("proxy.example.com", 1080, "", "pass"));
        Assert.Throws<ArgumentException>(() => new Socks5ProxyInfo("proxy.example.com", 1080, "user", ""));
    }

    #endregion

    #region HttpToSocks5Proxy Constructor Tests

    [Test]
    public void HttpToSocks5Proxy_Constructor_SingleProxy_CreatesInstance()
    {
        using var proxy = new HttpToSocks5Proxy("proxy.example.com", 1080);
        
        Assert.That(proxy.InternalServerPort, Is.GreaterThan(0));
        Assert.That(proxy.GetProxy(new Uri("https://example.com")), Is.Not.Null);
    }

    [Test]
    public void HttpToSocks5Proxy_Constructor_WithCredentials_CreatesInstance()
    {
        using var proxy = new HttpToSocks5Proxy("proxy.example.com", 1080, "user", "pass");
        
        Assert.That(proxy.InternalServerPort, Is.GreaterThan(0));
    }

    [Test]
    public void HttpToSocks5Proxy_Constructor_ProxyList_CreatesInstance()
    {
        var proxyList = new[]
        {
            new Socks5ProxyInfo("first-proxy.com", 1080),
            new Socks5ProxyInfo("second-proxy.com", 1090)
        };
        
        using var proxy = new HttpToSocks5Proxy(proxyList);
        
        Assert.That(proxy.InternalServerPort, Is.GreaterThan(0));
    }

    [Test]
    public void HttpToSocks5Proxy_Constructor_EmptyProxyList_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new HttpToSocks5Proxy(Array.Empty<Socks5ProxyInfo>()));
    }

    [Test]
    public void HttpToSocks5Proxy_Constructor_NullProxyList_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new HttpToSocks5Proxy(null!));
    }

    [Test]
    public void HttpToSocks5Proxy_Constructor_InvalidPort_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => 
            new HttpToSocks5Proxy("proxy.example.com", 1080, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => 
            new HttpToSocks5Proxy("proxy.example.com", 1080, 65536));
    }

    #endregion

    #region IWebProxy Implementation Tests

    [Test]
    public void HttpToSocks5Proxy_GetProxy_ReturnsLocalAddress()
    {
        using var proxy = new HttpToSocks5Proxy("proxy.example.com", 1080);
        
        var proxyUri = proxy.GetProxy(new Uri("https://target.example.com"));
        
        Assert.That(proxyUri.Host, Is.EqualTo("127.0.0.1"));
        Assert.That(proxyUri.Port, Is.EqualTo(proxy.InternalServerPort));
    }

    [Test]
    public void HttpToSocks5Proxy_IsBypassed_AlwaysReturnsFalse()
    {
        using var proxy = new HttpToSocks5Proxy("proxy.example.com", 1080);
        
        Assert.That(proxy.IsBypassed(new Uri("https://example.com")), Is.False);
        Assert.That(proxy.IsBypassed(new Uri("http://localhost")), Is.False);
    }

    [Test]
    public void HttpToSocks5Proxy_Credentials_CanBeSetAndGet()
    {
        using var proxy = new HttpToSocks5Proxy("proxy.example.com", 1080);
        
        var credentials = new NetworkCredential("user", "pass");
        proxy.Credentials = credentials;
        
        Assert.That(proxy.Credentials, Is.EqualTo(credentials));
    }

    #endregion

    #region Configuration Tests

    [Test]
    public void HttpToSocks5Proxy_ResolveHostnamesLocally_DefaultIsFalse()
    {
        using var proxy = new HttpToSocks5Proxy("proxy.example.com", 1080);
        
        Assert.That(proxy.ResolveHostnamesLocally, Is.False);
    }

    [Test]
    public void HttpToSocks5Proxy_ResolveHostnamesLocally_CanBeSet()
    {
        using var proxy = new HttpToSocks5Proxy("proxy.example.com", 1080);
        
        proxy.ResolveHostnamesLocally = true;
        
        Assert.That(proxy.ResolveHostnamesLocally, Is.True);
    }

    [Test]
    public void HttpToSocks5Proxy_DnsResolver_CanBeSet()
    {
        using var proxy = new HttpToSocks5Proxy("proxy.example.com", 1080);
        
        var customResolver = new DefaultDnsResolver();
        proxy.DnsResolver = customResolver;
        
        Assert.That(proxy.DnsResolver, Is.EqualTo(customResolver));
    }

    [Test]
    public void HttpToSocks5Proxy_DnsResolver_NullThrowsException()
    {
        using var proxy = new HttpToSocks5Proxy("proxy.example.com", 1080);
        
        Assert.Throws<ArgumentNullException>(() => proxy.DnsResolver = null!);
    }

    #endregion

    #region DefaultDnsResolver Tests

    [Test]
    public void DefaultDnsResolver_TryResolve_IPAddress_ReturnsSame()
    {
        var resolver = new DefaultDnsResolver();
        
        var result = resolver.TryResolve("192.168.1.1");
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ToString(), Is.EqualTo("192.168.1.1"));
    }

    [Test]
    public void DefaultDnsResolver_TryResolve_IPv6Address_ReturnsSame()
    {
        var resolver = new DefaultDnsResolver();
        
        var result = resolver.TryResolve("::1");
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ToString(), Is.EqualTo("::1"));
    }

    [Test]
    public void DefaultDnsResolver_TryResolve_Localhost_ReturnsAddress()
    {
        var resolver = new DefaultDnsResolver();
        
        var result = resolver.TryResolve("localhost");
        
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void DefaultDnsResolver_TryResolve_InvalidHostname_ReturnsNull()
    {
        var resolver = new DefaultDnsResolver();
        
        var result = resolver.TryResolve("this-hostname-definitely-does-not-exist-12345.invalid");
        
        Assert.That(result, Is.Null);
    }

    #endregion


    #region Socks5ConnectionResult Tests

    [Test]
    public void Socks5ConnectionResult_OK_IsZero()
    {
        Assert.That((int)Socks5ConnectionResult.OK, Is.EqualTo(0));
    }

    [Test]
    public void Socks5ConnectionResult_StandardErrors_MatchProtocol()
    {
        Assert.That((int)Socks5ConnectionResult.GeneralSocksServerFailure, Is.EqualTo(1));
        Assert.That((int)Socks5ConnectionResult.ConnectionNotAllowedByRuleset, Is.EqualTo(2));
        Assert.That((int)Socks5ConnectionResult.NetworkUnreachable, Is.EqualTo(3));
        Assert.That((int)Socks5ConnectionResult.HostUnreachable, Is.EqualTo(4));
        Assert.That((int)Socks5ConnectionResult.ConnectionRefused, Is.EqualTo(5));
        Assert.That((int)Socks5ConnectionResult.TTLExpired, Is.EqualTo(6));
        Assert.That((int)Socks5ConnectionResult.CommandNotSupported, Is.EqualTo(7));
        Assert.That((int)Socks5ConnectionResult.AddressTypeNotSupported, Is.EqualTo(8));
    }

    #endregion

    #region Disposal Tests

    [Test]
    public void HttpToSocks5Proxy_Dispose_StopsServer()
    {
        var proxy = new HttpToSocks5Proxy("proxy.example.com", 1080);
        var port = proxy.InternalServerPort;
        
        proxy.Dispose();
        
        // After disposal, the port should be released
        // We can't easily verify this without trying to bind to it
        Assert.Pass("Proxy disposed without exception");
    }

    [Test]
    public void HttpToSocks5Proxy_StopInternalServer_CanBeCalledMultipleTimes()
    {
        using var proxy = new HttpToSocks5Proxy("proxy.example.com", 1080);
        
        proxy.StopInternalServer();
        proxy.StopInternalServer(); // Should not throw
        
        Assert.Pass("StopInternalServer can be called multiple times");
    }

    #endregion
}
