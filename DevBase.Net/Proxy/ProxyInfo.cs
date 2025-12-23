using System.Net;
using DevBase.Net.Proxy.Enums;

namespace DevBase.Net.Proxy;

public sealed class ProxyInfo
{
    public string Host { get; }
    public int Port { get; }
    public EnumProxyType Type { get; }
    public NetworkCredential? Credentials { get; }
    
    public string Key => $"{this.Type}://{this.Host}:{this.Port}";

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

        return new ProxyInfo(host, port, type, credentials);
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
            EnumProxyType.Socks5 => "socks5",
            EnumProxyType.Socks5h => "socks5h",
            _ => "http"
        };
        
        return new Uri($"{scheme}://{this.Host}:{this.Port}");
    }

    public override string ToString() => this.Key;
    
    public override int GetHashCode() => HashCode.Combine(this.Host, this.Port, this.Type);
    
    public override bool Equals(object? obj) => 
        obj is ProxyInfo other && this.Host == other.Host && this.Port == other.Port && this.Type == other.Type;
}
