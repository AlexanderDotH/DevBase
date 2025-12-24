using DevBase.Net.Proxy;
using DevBase.Net.Proxy.Enums;

namespace DevBase.Test.DevBaseRequests;

public class ProxyInfoTest
{
    [Test]
    public void ProxyInfo_Constructor_ShouldSetProperties()
    {
        ProxyInfo proxy = new ProxyInfo("proxy.example.com", 8080, EnumProxyType.Http);
        
        Assert.That(proxy.Host, Is.EqualTo("proxy.example.com"));
        Assert.That(proxy.Port, Is.EqualTo(8080));
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Http));
        Assert.That(proxy.HasAuthentication, Is.False);
    }

    [Test]
    public void ProxyInfo_ConstructorWithCredentials_ShouldSetAuthentication()
    {
        ProxyInfo proxy = new ProxyInfo("proxy.example.com", 8080, "user", "pass");
        
        Assert.That(proxy.HasAuthentication, Is.True);
        Assert.That(proxy.Credentials, Is.Not.Null);
        Assert.That(proxy.Credentials!.UserName, Is.EqualTo("user"));
        Assert.That(proxy.Credentials.Password, Is.EqualTo("pass"));
    }

    [Test]
    public void ProxyInfo_Parse_HttpProxy_ShouldParse()
    {
        ProxyInfo proxy = ProxyInfo.Parse("http://proxy.example.com:8080");
        
        Assert.That(proxy.Host, Is.EqualTo("proxy.example.com"));
        Assert.That(proxy.Port, Is.EqualTo(8080));
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Http));
    }

    [Test]
    public void ProxyInfo_Parse_HttpsProxy_ShouldParse()
    {
        ProxyInfo proxy = ProxyInfo.Parse("https://proxy.example.com:8443");
        
        Assert.That(proxy.Host, Is.EqualTo("proxy.example.com"));
        Assert.That(proxy.Port, Is.EqualTo(8443));
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Https));
    }

    [Test]
    public void ProxyInfo_Parse_Socks4Proxy_ShouldParse()
    {
        ProxyInfo proxy = ProxyInfo.Parse("socks4://proxy.example.com:1080");
        
        Assert.That(proxy.Host, Is.EqualTo("proxy.example.com"));
        Assert.That(proxy.Port, Is.EqualTo(1080));
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Socks4));
        Assert.That(proxy.ResolveHostnamesLocally, Is.True);
    }

    [Test]
    public void ProxyInfo_Parse_Socks5Proxy_ShouldParseWithLocalDns()
    {
        ProxyInfo proxy = ProxyInfo.Parse("socks5://proxy.example.com:1080");
        
        Assert.That(proxy.Host, Is.EqualTo("proxy.example.com"));
        Assert.That(proxy.Port, Is.EqualTo(1080));
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Socks5));
        Assert.That(proxy.ResolveHostnamesLocally, Is.True);
    }

    [Test]
    public void ProxyInfo_Parse_Socks5hProxy_ShouldParseWithRemoteDns()
    {
        ProxyInfo proxy = ProxyInfo.Parse("socks5h://proxy.example.com:1080");
        
        Assert.That(proxy.Host, Is.EqualTo("proxy.example.com"));
        Assert.That(proxy.Port, Is.EqualTo(1080));
        Assert.That(proxy.Type, Is.EqualTo(EnumProxyType.Socks5h));
        Assert.That(proxy.ResolveHostnamesLocally, Is.False);
    }

    [Test]
    public void ProxyInfo_Parse_WithCredentials_ShouldParse()
    {
        ProxyInfo proxy = ProxyInfo.Parse("socks5://user:pass@proxy.example.com:1080");
        
        Assert.That(proxy.Host, Is.EqualTo("proxy.example.com"));
        Assert.That(proxy.Port, Is.EqualTo(1080));
        Assert.That(proxy.HasAuthentication, Is.True);
        Assert.That(proxy.Credentials!.UserName, Is.EqualTo("user"));
        Assert.That(proxy.Credentials.Password, Is.EqualTo("pass"));
    }

    [Test]
    public void ProxyInfo_TryParse_ValidProxy_ShouldReturnTrue()
    {
        bool result = ProxyInfo.TryParse("http://proxy.example.com:8080", out ProxyInfo? proxy);
        
        Assert.That(result, Is.True);
        Assert.That(proxy, Is.Not.Null);
        Assert.That(proxy!.Host, Is.EqualTo("proxy.example.com"));
    }

    [Test]
    public void ProxyInfo_TryParse_InvalidProxy_ShouldReturnFalse()
    {
        bool result = ProxyInfo.TryParse("invalid-proxy", out ProxyInfo? proxy);
        
        Assert.That(result, Is.False);
        Assert.That(proxy, Is.Null);
    }

    [Test]
    public void ProxyInfo_ToUri_Http_ShouldReturnCorrectUri()
    {
        ProxyInfo proxy = new ProxyInfo("proxy.example.com", 8080, EnumProxyType.Http);
        
        Uri uri = proxy.ToUri();
        
        Assert.That(uri.Scheme, Is.EqualTo("http"));
        Assert.That(uri.Host, Is.EqualTo("proxy.example.com"));
        Assert.That(uri.Port, Is.EqualTo(8080));
    }

    [Test]
    public void ProxyInfo_ToUri_Socks5_ShouldReflectDnsSetting()
    {
        ProxyInfo proxy = ProxyInfo.Parse("socks5://proxy.example.com:1080");
        
        Uri uri = proxy.ToUri();
        
        Assert.That(uri.Scheme, Is.EqualTo("socks5"));
    }

    [Test]
    public void ProxyInfo_ToUri_Socks5h_ShouldReturnSocks5h()
    {
        ProxyInfo proxy = ProxyInfo.Parse("socks5h://proxy.example.com:1080");
        
        Uri uri = proxy.ToUri();
        
        Assert.That(uri.Scheme, Is.EqualTo("socks5h"));
    }

    [Test]
    public void ProxyInfo_Key_ShouldBeUnique()
    {
        ProxyInfo proxy1 = new ProxyInfo("proxy1.example.com", 8080, EnumProxyType.Http);
        ProxyInfo proxy2 = new ProxyInfo("proxy2.example.com", 8080, EnumProxyType.Http);
        ProxyInfo proxy3 = new ProxyInfo("proxy1.example.com", 8080, EnumProxyType.Http);
        
        Assert.That(proxy1.Key, Is.Not.EqualTo(proxy2.Key));
        Assert.That(proxy1.Key, Is.EqualTo(proxy3.Key));
    }

    [Test]
    public void ProxyInfo_Equals_SameProxy_ShouldBeEqual()
    {
        ProxyInfo proxy1 = new ProxyInfo("proxy.example.com", 8080, EnumProxyType.Http);
        ProxyInfo proxy2 = new ProxyInfo("proxy.example.com", 8080, EnumProxyType.Http);
        
        Assert.That(proxy1.Equals(proxy2), Is.True);
        Assert.That(proxy1.GetHashCode(), Is.EqualTo(proxy2.GetHashCode()));
    }

    [Test]
    public void ProxyInfo_Constructor_InvalidPort_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ProxyInfo("proxy.example.com", 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ProxyInfo("proxy.example.com", -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ProxyInfo("proxy.example.com", 70000));
    }

    [Test]
    public void ProxyInfo_Constructor_NullHost_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => new ProxyInfo(null!, 8080));
        Assert.Throws<ArgumentException>(() => new ProxyInfo("", 8080));
        Assert.Throws<ArgumentException>(() => new ProxyInfo("   ", 8080));
    }

    [Test]
    public void ProxyInfo_ToString_ShouldReturnKey()
    {
        ProxyInfo proxy = new ProxyInfo("proxy.example.com", 8080, EnumProxyType.Http);
        
        Assert.That(proxy.ToString(), Is.EqualTo(proxy.Key));
    }
}
