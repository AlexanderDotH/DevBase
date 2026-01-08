using System.Net;
using DevBase.Net.Core;
using DevBase.Net.Proxy;
using DevBase.Net.Proxy.Enums;
using NUnit.Framework;

namespace DevBase.Test.DevBaseRequests;

/// <summary>
/// Tests for proxy functionality including:
/// - ProxyInfo string parsing for all proxy types
/// - Proxy configuration on Request
/// - ProxyInfo creation and properties
/// </summary>
[TestFixture]
public class ProxyTest
{
    #region ProxyInfo String Parsing Tests

    [Test]
    public void Parse_HttpProxy_ParsesCorrectly()
    {
        var proxy = ProxyInfo.Parse("http://proxy.example.com:8080");
        
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Http));
        Assert.That(proxy.Host, Is.EqualTo("proxy.example.com"));
        Assert.That(proxy.Port, Is.EqualTo(8080));
        Assert.That(proxy.HasAuthentication, Is.False);
    }

    [Test]
    public void Parse_HttpsProxy_ParsesCorrectly()
    {
        var proxy = ProxyInfo.Parse("https://secure-proxy.example.com:443");
        
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Https));
        Assert.That(proxy.Host, Is.EqualTo("secure-proxy.example.com"));
        Assert.That(proxy.Port, Is.EqualTo(443));
    }

    [Test]
    public void Parse_Socks4Proxy_ParsesCorrectly()
    {
        var proxy = ProxyInfo.Parse("socks4://socks.example.com:1080");
        
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Socks4));
        Assert.That(proxy.Host, Is.EqualTo("socks.example.com"));
        Assert.That(proxy.Port, Is.EqualTo(1080));
    }

    [Test]
    public void Parse_Socks5Proxy_ParsesCorrectly()
    {
        var proxy = ProxyInfo.Parse("socks5://socks5.example.com:1080");
        
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Socks5));
        Assert.That(proxy.Host, Is.EqualTo("socks5.example.com"));
        Assert.That(proxy.Port, Is.EqualTo(1080));
        Assert.That(proxy.ResolveHostnamesLocally, Is.True);
    }

    [Test]
    public void Parse_Socks5hProxy_ParsesCorrectly()
    {
        var proxy = ProxyInfo.Parse("socks5h://socks5h.example.com:1080");
        
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Socks5h));
        Assert.That(proxy.Host, Is.EqualTo("socks5h.example.com"));
        Assert.That(proxy.Port, Is.EqualTo(1080));
        Assert.That(proxy.ResolveHostnamesLocally, Is.False);
    }

    [Test]
    public void Parse_SshProxy_ParsesCorrectly()
    {
        var proxy = ProxyInfo.Parse("ssh://ssh.example.com:22");
        
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Ssh));
        Assert.That(proxy.Host, Is.EqualTo("ssh.example.com"));
        Assert.That(proxy.Port, Is.EqualTo(22));
    }

    [Test]
    public void Parse_WithCredentials_ExtractsUsernameAndPassword()
    {
        var proxy = ProxyInfo.Parse("socks5://paid1_563X7:rtVVhrth4545++A@dc.oxylabs.io:8005");
        
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Socks5));
        Assert.That(proxy.Host, Is.EqualTo("dc.oxylabs.io"));
        Assert.That(proxy.Port, Is.EqualTo(8005));
        Assert.That(proxy.HasAuthentication, Is.True);
        Assert.That(proxy.Credentials!.UserName, Is.EqualTo("paid1_563X7"));
        Assert.That(proxy.Credentials!.Password, Is.EqualTo("rtVVhrth4545++A"));
    }

    [Test]
    public void Parse_Socks4WithCredentials_ExtractsUsername()
    {
        var proxy = ProxyInfo.Parse("socks4://username:password@socks4.example.com:1080");
        
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Socks4));
        Assert.That(proxy.HasAuthentication, Is.True);
        Assert.That(proxy.Credentials!.UserName, Is.EqualTo("username"));
    }

    [Test]
    public void Parse_HttpWithCredentials_ExtractsCredentials()
    {
        var proxy = ProxyInfo.Parse("http://user:pass123@proxy.example.com:8080");
        
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Http));
        Assert.That(proxy.HasAuthentication, Is.True);
        Assert.That(proxy.Credentials!.UserName, Is.EqualTo("user"));
        Assert.That(proxy.Credentials!.Password, Is.EqualTo("pass123"));
    }

    [Test]
    public void Parse_SshWithCredentials_ExtractsCredentials()
    {
        var proxy = ProxyInfo.Parse("ssh://admin:secretpass@ssh.example.com:22");
        
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Ssh));
        Assert.That(proxy.HasAuthentication, Is.True);
        Assert.That(proxy.Credentials!.UserName, Is.EqualTo("admin"));
        Assert.That(proxy.Credentials!.Password, Is.EqualTo("secretpass"));
    }

    [Test]
    public void Parse_WithoutProtocol_DefaultsToHttp()
    {
        var proxy = ProxyInfo.Parse("proxy.example.com:8080");
        
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Http));
        Assert.That(proxy.Host, Is.EqualTo("proxy.example.com"));
        Assert.That(proxy.Port, Is.EqualTo(8080));
    }

    [Test]
    public void Parse_CaseInsensitiveProtocol_ParsesCorrectly()
    {
        var proxy1 = ProxyInfo.Parse("SOCKS5://host:1080");
        var proxy2 = ProxyInfo.Parse("Socks5://host:1080");
        var proxy3 = ProxyInfo.Parse("HTTP://host:8080");
        
        Assert.That(proxy1.Type, Is.EqualTo(EnumProxyType.Socks5));
        Assert.That(proxy2.Type, Is.EqualTo(EnumProxyType.Socks5));
        Assert.That(proxy3.Type, Is.EqualTo(EnumProxyType.Http));
    }

    [Test]
    public void Parse_InvalidFormat_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => ProxyInfo.Parse("invalid-proxy-string"));
        Assert.Throws<FormatException>(() => ProxyInfo.Parse("http://host-without-port"));
    }

    [Test]
    public void Parse_InvalidPort_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => ProxyInfo.Parse("http://host:notanumber"));
    }

    [Test]
    public void Parse_EmptyString_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => ProxyInfo.Parse(""));
        Assert.Throws<ArgumentException>(() => ProxyInfo.Parse("   "));
    }

    [Test]
    public void TryParse_ValidProxy_ReturnsTrue()
    {
        bool result = ProxyInfo.TryParse("socks5://host:1080", out var proxy);
        
        Assert.That(result, Is.True);
        Assert.That(proxy, Is.Not.Null);
        Assert.That(proxy!.Type, Is.EqualTo(EnumProxyType.Socks5));
    }

    [Test]
    public void TryParse_InvalidProxy_ReturnsFalse()
    {
        bool result = ProxyInfo.TryParse("invalid", out var proxy);
        
        Assert.That(result, Is.False);
        Assert.That(proxy, Is.Null);
    }

    #endregion

    #region ProxyInfo Constructor Tests

    [Test]
    public void Constructor_WithHostAndPort_CreatesHttpProxy()
    {
        var proxy = new ProxyInfo("proxy.example.com", 8080);
        
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Http));
        Assert.That(proxy.Host, Is.EqualTo("proxy.example.com"));
        Assert.That(proxy.Port, Is.EqualTo(8080));
    }

    [Test]
    public void Constructor_WithCredentials_StoresCredentials()
    {
        var proxy = new ProxyInfo("host", 8080, "user", "pass", EnumProxyType.Socks5);
        
        Assert.That(proxy.HasAuthentication, Is.True);
        Assert.That(proxy.Credentials!.UserName, Is.EqualTo("user"));
        Assert.That(proxy.Credentials!.Password, Is.EqualTo("pass"));
    }

    [Test]
    public void Constructor_InvalidHost_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ProxyInfo("", 8080));
        Assert.Throws<ArgumentException>(() => new ProxyInfo("   ", 8080));
    }

    [Test]
    public void Constructor_InvalidPort_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ProxyInfo("host", 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ProxyInfo("host", -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ProxyInfo("host", 65536));
    }

    #endregion

    #region ProxyInfo ToUri Tests

    [Test]
    public void ToUri_HttpProxy_ReturnsHttpUri()
    {
        var proxy = new ProxyInfo("host", 8080, EnumProxyType.Http);
        var uri = proxy.ToUri();
        
        Assert.That(uri.Scheme, Is.EqualTo("http"));
        Assert.That(uri.Host, Is.EqualTo("host"));
        Assert.That(uri.Port, Is.EqualTo(8080));
    }

    [Test]
    public void ToUri_Socks5Proxy_ReturnsSocks5Uri()
    {
        // When created via constructor, ResolveHostnamesLocally defaults to false
        // so scheme is socks5h (remote DNS). Use Parse for local DNS behavior.
        var proxy = ProxyInfo.Parse("socks5://host:1080");
        var uri = proxy.ToUri();
        
        Assert.That(uri.Scheme, Is.EqualTo("socks5"));
    }

    [Test]
    public void ToUri_SshProxy_ReturnsSshUri()
    {
        var proxy = new ProxyInfo("host", 22, EnumProxyType.Ssh);
        var uri = proxy.ToUri();
        
        Assert.That(uri.Scheme, Is.EqualTo("ssh"));
    }

    #endregion

    #region ProxyInfo ToWebProxy Tests

    [Test]
    public void ToWebProxy_HttpProxy_ReturnsWebProxy()
    {
        ProxyInfo.ClearProxyCache();
        var proxy = new ProxyInfo("proxy.example.com", 8080, EnumProxyType.Http);
        var webProxy = proxy.ToWebProxy();
        
        Assert.That(webProxy, Is.Not.Null);
        Assert.That(webProxy, Is.InstanceOf<WebProxy>());
    }

    [Test]
    public void ToWebProxy_HttpProxyWithCredentials_SetsCredentials()
    {
        ProxyInfo.ClearProxyCache();
        var proxy = new ProxyInfo("proxy.example.com", 8080, "user", "pass", EnumProxyType.Http);
        var webProxy = proxy.ToWebProxy() as WebProxy;
        
        Assert.That(webProxy, Is.Not.Null);
        Assert.That(webProxy!.Credentials, Is.Not.Null);
    }

    [Test]
    public void ToWebProxy_Socks5Proxy_ReturnsHttpToSocks5Proxy()
    {
        ProxyInfo.ClearProxyCache();
        var proxy = new ProxyInfo("socks.example.com", 1080, EnumProxyType.Socks5);
        var webProxy = proxy.ToWebProxy();
        
        Assert.That(webProxy, Is.Not.Null);
    }

    [Test]
    public void ToWebProxy_Socks4Proxy_ReturnsProxy()
    {
        ProxyInfo.ClearProxyCache();
        var proxy = new ProxyInfo("socks4.example.com", 1080, EnumProxyType.Socks4);
        var webProxy = proxy.ToWebProxy();
        
        Assert.That(webProxy, Is.Not.Null);
    }

    [Test]
    public void ToWebProxy_SshProxy_ReturnsProxy()
    {
        ProxyInfo.ClearProxyCache();
        var proxy = new ProxyInfo("ssh.example.com", 22, EnumProxyType.Ssh);
        var webProxy = proxy.ToWebProxy();
        
        Assert.That(webProxy, Is.Not.Null);
    }

    [Test]
    public void ToWebProxy_CachesProxy_ReturnsSameInstance()
    {
        ProxyInfo.ClearProxyCache();
        var proxy = new ProxyInfo("cached.example.com", 8080, EnumProxyType.Http);
        
        var webProxy1 = proxy.ToWebProxy();
        var webProxy2 = proxy.ToWebProxy();
        
        Assert.That(webProxy1, Is.SameAs(webProxy2));
    }

    #endregion

    #region Request WithProxy Tests

    [Test]
    public void Request_WithProxyInfo_SetsProxy()
    {
        var proxy = new ProxyInfo("proxy.example.com", 8080, EnumProxyType.Socks5);
        var request = new Request("https://example.com")
            .WithProxy(proxy);
        
        Assert.That(request, Is.Not.Null);
    }

    [Test]
    public void Request_WithProxyString_ParsesAndSetsProxy()
    {
        var request = new Request("https://example.com")
            .WithProxy("socks5://user:pass@proxy.example.com:1080");
        
        Assert.That(request, Is.Not.Null);
    }

    [Test]
    public void Request_WithHttpProxyString_Works()
    {
        var request = new Request("https://example.com")
            .WithProxy("http://proxy.example.com:8080");
        
        Assert.That(request, Is.Not.Null);
    }

    [Test]
    public void Request_WithSocks4ProxyString_Works()
    {
        var request = new Request("https://example.com")
            .WithProxy("socks4://user@proxy.example.com:1080");
        
        Assert.That(request, Is.Not.Null);
    }

    [Test]
    public void Request_WithSocks5hProxyString_Works()
    {
        var request = new Request("https://example.com")
            .WithProxy("socks5h://user:pass@proxy.example.com:1080");
        
        Assert.That(request, Is.Not.Null);
    }

    [Test]
    public void Request_WithSshProxyString_Works()
    {
        var request = new Request("https://example.com")
            .WithProxy("ssh://admin:pass@ssh.example.com:22");
        
        Assert.That(request, Is.Not.Null);
    }

    [Test]
    public void Request_WithInvalidProxyString_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => 
            new Request("https://example.com")
                .WithProxy("invalid-proxy"));
    }

    #endregion

    #region ProxyInfo Equality Tests

    [Test]
    public void Equals_SameProperties_ReturnsTrue()
    {
        var proxy1 = new ProxyInfo("host", 8080, EnumProxyType.Http);
        var proxy2 = new ProxyInfo("host", 8080, EnumProxyType.Http);
        
        Assert.That(proxy1.Equals(proxy2), Is.True);
    }

    [Test]
    public void Equals_DifferentHost_ReturnsFalse()
    {
        var proxy1 = new ProxyInfo("host1", 8080, EnumProxyType.Http);
        var proxy2 = new ProxyInfo("host2", 8080, EnumProxyType.Http);
        
        Assert.That(proxy1.Equals(proxy2), Is.False);
    }

    [Test]
    public void Equals_DifferentPort_ReturnsFalse()
    {
        var proxy1 = new ProxyInfo("host", 8080, EnumProxyType.Http);
        var proxy2 = new ProxyInfo("host", 8081, EnumProxyType.Http);
        
        Assert.That(proxy1.Equals(proxy2), Is.False);
    }

    [Test]
    public void Equals_DifferentType_ReturnsFalse()
    {
        var proxy1 = new ProxyInfo("host", 8080, EnumProxyType.Http);
        var proxy2 = new ProxyInfo("host", 8080, EnumProxyType.Socks5);
        
        Assert.That(proxy1.Equals(proxy2), Is.False);
    }

    [Test]
    public void GetHashCode_SameProperties_ReturnsSameHash()
    {
        var proxy1 = new ProxyInfo("host", 8080, EnumProxyType.Http);
        var proxy2 = new ProxyInfo("host", 8080, EnumProxyType.Http);
        
        Assert.That(proxy1.GetHashCode(), Is.EqualTo(proxy2.GetHashCode()));
    }

    [Test]
    public void ToString_ReturnsKey()
    {
        var proxy = new ProxyInfo("host", 8080, EnumProxyType.Socks5);
        
        Assert.That(proxy.ToString(), Is.EqualTo("socks5://host:8080"));
    }

    #endregion

    #region DNS Resolution Mode Tests

    [Test]
    public void Parse_Socks5_SetsLocalDnsResolution()
    {
        var proxy = ProxyInfo.Parse("socks5://host:1080");
        Assert.That(proxy.ResolveHostnamesLocally, Is.True);
    }

    [Test]
    public void Parse_Socks5h_SetsRemoteDnsResolution()
    {
        var proxy = ProxyInfo.Parse("socks5h://host:1080");
        Assert.That(proxy.ResolveHostnamesLocally, Is.False);
    }

    [Test]
    public void Parse_Socks4_SetsLocalDnsResolution()
    {
        var proxy = ProxyInfo.Parse("socks4://host:1080");
        Assert.That(proxy.ResolveHostnamesLocally, Is.True);
    }

    #endregion
}
