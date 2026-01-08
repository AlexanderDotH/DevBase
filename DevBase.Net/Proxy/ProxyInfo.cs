using System.Collections.Concurrent;
using System.Net;
using DevBase.Net.Proxy.Enums;
using DevBase.Net.Proxy.HttpToSocks5;

namespace DevBase.Net.Proxy;

public sealed class ProxyInfo
{
    private static readonly ConcurrentDictionary<string, IWebProxy> ProxyCache = new();
    
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public EnumProxyType Type { get; set; } = EnumProxyType.Http;
    public NetworkCredential? Credentials { get; set; }
    
    public string? Username
    {
        get => Credentials?.UserName;
        set
        {
            if (value != null)
            {
                Credentials = new NetworkCredential(value, Credentials?.Password ?? string.Empty);
            }
        }
    }
    
    public string? Password
    {
        get => Credentials?.Password;
        set
        {
            if (Credentials != null)
            {
                Credentials = new NetworkCredential(Credentials.UserName, value ?? string.Empty);
            }
            else if (value != null)
            {
                Credentials = new NetworkCredential(string.Empty, value);
            }
        }
    }
    
    public bool BypassLocalAddresses { get; init; }
    public string[]? BypassList { get; init; }
    public bool ResolveHostnamesLocally { get; init; }
    public TimeSpan ConnectionTimeout { get; init; } = TimeSpan.FromSeconds(30);
    public TimeSpan ReadWriteTimeout { get; init; } = TimeSpan.FromSeconds(60);
    public int InternalServerPort { get; init; }
    
    public string Key => $"{this.Type.ToString().ToLowerInvariant()}://{this.Host}:{this.Port}";
    public bool HasAuthentication => Credentials != null && !string.IsNullOrEmpty(Credentials.UserName);

    public ProxyInfo()
    {
    }

    public ProxyInfo(string host, int port, EnumProxyType type = EnumProxyType.Http, NetworkCredential? credentials = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(host);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(port);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(port, 65535);
        
        this.Host = host;
        this.Port = port;
        this.Type = type;
        this.Credentials = credentials;
    }

    public ProxyInfo(string host, int port, string username, string password, EnumProxyType type = EnumProxyType.Http)
        : this(host, port, type, new NetworkCredential(username, password))
    {
    }

    public static ProxyInfo FromConfiguration(ProxyConfiguration config)
    {
        return new ProxyInfo(config.Host, config.Port, config.Type, config.Credentials)
        {
            BypassLocalAddresses = config.BypassLocalAddresses,
            BypassList = config.BypassList,
            ResolveHostnamesLocally = config.ResolveHostnamesLocally,
            ConnectionTimeout = config.ConnectionTimeout,
            ReadWriteTimeout = config.ReadWriteTimeout,
            InternalServerPort = config.InternalServerPort
        };
    }

    public static ProxyInfo Parse(string proxyString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(proxyString);
        
        EnumProxyType type = EnumProxyType.Http;
        ReadOnlySpan<char> remaining = proxyString.AsSpan();
        
        if (remaining.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
        {
            type = EnumProxyType.Http;
            remaining = remaining[7..];
        }
        else if (remaining.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            type = EnumProxyType.Https;
            remaining = remaining[8..];
        }
        else if (remaining.StartsWith("socks4://", StringComparison.OrdinalIgnoreCase))
        {
            type = EnumProxyType.Socks4;
            remaining = remaining[9..];
        }
        else if (remaining.StartsWith("socks5h://", StringComparison.OrdinalIgnoreCase))
        {
            type = EnumProxyType.Socks5h;
            remaining = remaining[10..];
        }
        else if (remaining.StartsWith("socks5://", StringComparison.OrdinalIgnoreCase))
        {
            type = EnumProxyType.Socks5;
            remaining = remaining[9..];
        }
        else if (remaining.StartsWith("ssh://", StringComparison.OrdinalIgnoreCase))
        {
            type = EnumProxyType.Ssh;
            remaining = remaining[6..];
        }

        NetworkCredential? credentials = null;
        int atIndex = remaining.IndexOf('@');
        
        if (atIndex > 0)
        {
            ReadOnlySpan<char> authPart = remaining[..atIndex];
            int colonIndex = authPart.IndexOf(':');
            
            if (colonIndex > 0)
            {
                string username = authPart[..colonIndex].ToString();
                string password = authPart[(colonIndex + 1)..].ToString();
                credentials = new NetworkCredential(username, password);
            }
            
            remaining = remaining[(atIndex + 1)..];
        }

        int portSeparatorIndex = remaining.LastIndexOf(':');
        if (portSeparatorIndex <= 0)
            throw new FormatException("Invalid proxy format. Expected: [protocol://][user:pass@]host:port");

        string host = remaining[..portSeparatorIndex].ToString();
        ReadOnlySpan<char> portStr = remaining[(portSeparatorIndex + 1)..];
        
        if (!int.TryParse(portStr, out int port))
            throw new FormatException($"Invalid port number: {portStr.ToString()}");

        return new ProxyInfo(host, port, type, credentials)
        {
            ResolveHostnamesLocally = type switch
            {
                EnumProxyType.Socks5 => true,   // socks5:// = local DNS
                EnumProxyType.Socks5h => false, // socks5h:// = remote DNS
                EnumProxyType.Socks4 => true,   // SOCKS4 always local
                _ => false
            }
        };
    }

    public static bool TryParse(string proxyString, out ProxyInfo? proxyInfo)
    {
        try
        {
            proxyInfo = Parse(proxyString);
            return true;
        }
        catch
        {
            proxyInfo = null;
            return false;
        }
    }

    public Uri ToUri()
    {
        string scheme = this.Type switch
        {
            EnumProxyType.Http => "http",
            EnumProxyType.Https => "https",
            EnumProxyType.Socks4 => "socks4",
            EnumProxyType.Socks5 => ResolveHostnamesLocally ? "socks5" : "socks5h",
            EnumProxyType.Socks5h => "socks5h",
            EnumProxyType.Ssh => "ssh",
            _ => "http"
        };
        
        return new Uri($"{scheme}://{this.Host}:{this.Port}");
    }

    public IWebProxy ToWebProxy()
    {
        return ProxyCache.GetOrAdd(Key, _ => CreateWebProxy());
    }

    private IWebProxy CreateWebProxy()
    {
        switch (Type)
        {
            case EnumProxyType.Http:
            case EnumProxyType.Https:
                return CreateHttpProxy();

            case EnumProxyType.Socks4:
                return CreateSocks4Proxy();

            case EnumProxyType.Socks5:
            case EnumProxyType.Socks5h:
                return CreateSocks5Proxy();

            case EnumProxyType.Ssh:
                return CreateSshProxy();

            default:
                throw new NotSupportedException($"Proxy type {Type} is not supported");
        }
    }

    private WebProxy CreateHttpProxy()
    {
        var httpAddress = $"http://{Host}:{Port}";
        var httpProxy = new WebProxy(httpAddress)
        {
            BypassProxyOnLocal = BypassLocalAddresses
        };

        if (BypassList != null && BypassList.Length > 0)
            httpProxy.BypassList = BypassList;

        if (Credentials != null)
            httpProxy.Credentials = Credentials;

        return httpProxy;
    }

    private HttpToSocks5Proxy CreateSocks4Proxy()
    {
        try
        {
            HttpToSocks5Proxy proxy;
            if (Credentials != null && !string.IsNullOrEmpty(Credentials.UserName))
            {
                proxy = new HttpToSocks5Proxy(Host, Port, Credentials.UserName, string.Empty, InternalServerPort);
            }
            else
            {
                proxy = new HttpToSocks5Proxy(Host, Port, InternalServerPort);
            }

            proxy.ResolveHostnamesLocally = true;
            return proxy;
        }
        catch (System.Exception e)
        {
            throw new NotSupportedException($"SOCKS4 proxy creation failed: {e.Message}", e);
        }
    }

    private HttpToSocks5Proxy CreateSocks5Proxy()
    {
        try
        {
            HttpToSocks5Proxy proxy;
            if (Credentials != null)
            {
                proxy = new HttpToSocks5Proxy(
                    Host,
                    Port,
                    Credentials.UserName,
                    Credentials.Password,
                    InternalServerPort);
            }
            else
            {
                proxy = new HttpToSocks5Proxy(Host, Port, InternalServerPort);
            }

            proxy.ResolveHostnamesLocally = ResolveHostnamesLocally;

            return proxy;
        }
        catch (System.Exception e)
        {
            throw new NotSupportedException($"SOCKS5 proxy creation failed: {e.Message}", e);
        }
    }

    private HttpToSocks5Proxy CreateSshProxy()
    {
        // SSH tunnels typically use SOCKS5 protocol for dynamic port forwarding
        // The SSH connection must be established externally (e.g., ssh -D local_port user@host)
        // This creates a SOCKS5 proxy pointing to the local SSH tunnel endpoint
        try
        {
            HttpToSocks5Proxy proxy;
            if (Credentials != null)
            {
                // For SSH, credentials are used for the SSH connection itself
                // The local SOCKS proxy created by SSH doesn't require auth
                proxy = new HttpToSocks5Proxy(Host, Port, InternalServerPort);
            }
            else
            {
                proxy = new HttpToSocks5Proxy(Host, Port, InternalServerPort);
            }

            proxy.ResolveHostnamesLocally = false; // SSH tunnels resolve remotely
            return proxy;
        }
        catch (System.Exception e)
        {
            throw new NotSupportedException($"SSH proxy creation failed: {e.Message}", e);
        }
    }

    public static void ClearProxyCache()
    {
        ProxyCache.Clear();
    }

    public override string ToString() => 
        HasAuthentication 
            ? $"{this.Type.ToString().ToLowerInvariant()}://{this.Host}:{this.Port}:{this.Username}:{this.Password}"
            : this.Key;
    
    public override int GetHashCode() => HashCode.Combine(this.Host, this.Port, this.Type);
    
    public override bool Equals(object? obj) => 
        obj is ProxyInfo other && this.Host == other.Host && this.Port == other.Port && this.Type == other.Type;
}
