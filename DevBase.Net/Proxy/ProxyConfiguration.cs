using System.Net;
using DevBase.Net.Proxy.Enums;

namespace DevBase.Net.Proxy;

public sealed class ProxyConfiguration
{
    public string Host { get; private set; } = string.Empty;
    public int Port { get; private set; }
    public EnumProxyType Type { get; private set; } = EnumProxyType.Http;
    public NetworkCredential? Credentials { get; private set; }
    
    public bool BypassLocalAddresses { get; private set; }
    public string[]? BypassList { get; private set; }
    
    public bool ResolveHostnamesLocally { get; private set; }
    public TimeSpan ConnectionTimeout { get; private set; } = TimeSpan.FromSeconds(30);
    public TimeSpan ReadWriteTimeout { get; private set; } = TimeSpan.FromSeconds(60);
    
    public int InternalServerPort { get; private set; }
    
    private ProxyConfiguration() { }

    public static HttpProxyBuilder Http(string host, int port) => new(host, port, EnumProxyType.Http);
    public static HttpProxyBuilder Https(string host, int port) => new(host, port, EnumProxyType.Https);
    public static Socks4ProxyBuilder Socks4(string host, int port) => new(host, port);
    public static Socks5ProxyBuilder Socks5(string host, int port) => new(host, port, EnumProxyType.Socks5);
    public static Socks5ProxyBuilder Socks5h(string host, int port) => new(host, port, EnumProxyType.Socks5h);

    public ProxyInfo ToProxyInfo()
    {
        return new ProxyInfo(Host, Port, Type, Credentials);
    }

    public sealed class HttpProxyBuilder
    {
        private readonly ProxyConfiguration _config = new();

        internal HttpProxyBuilder(string host, int port, EnumProxyType type)
        {
            _config.Host = host;
            _config.Port = port;
            _config.Type = type;
        }

        public HttpProxyBuilder WithCredentials(string username, string password)
        {
            _config.Credentials = new NetworkCredential(username, password);
            return this;
        }

        public HttpProxyBuilder WithCredentials(NetworkCredential credentials)
        {
            _config.Credentials = credentials;
            return this;
        }

        public HttpProxyBuilder BypassLocal(bool bypass = true)
        {
            _config.BypassLocalAddresses = bypass;
            return this;
        }

        public HttpProxyBuilder WithBypassList(params string[] addresses)
        {
            _config.BypassList = addresses;
            return this;
        }

        public ProxyConfiguration Build() => _config;
        
        public ProxyInfo ToProxyInfo() => Build().ToProxyInfo();
        
        public static implicit operator ProxyInfo(HttpProxyBuilder builder) => builder.ToProxyInfo();
        public static implicit operator ProxyConfiguration(HttpProxyBuilder builder) => builder.Build();
    }

    public sealed class Socks4ProxyBuilder
    {
        private readonly ProxyConfiguration _config = new();

        internal Socks4ProxyBuilder(string host, int port)
        {
            _config.Host = host;
            _config.Port = port;
            _config.Type = EnumProxyType.Socks4;
        }

        public Socks4ProxyBuilder WithUserId(string userId)
        {
            _config.Credentials = new NetworkCredential(userId, string.Empty);
            return this;
        }

        public Socks4ProxyBuilder WithConnectionTimeout(TimeSpan timeout)
        {
            _config.ConnectionTimeout = timeout;
            return this;
        }

        public Socks4ProxyBuilder WithReadWriteTimeout(TimeSpan timeout)
        {
            _config.ReadWriteTimeout = timeout;
            return this;
        }

        public Socks4ProxyBuilder WithInternalServerPort(int port)
        {
            _config.InternalServerPort = port;
            return this;
        }

        public ProxyConfiguration Build() => _config;
        
        public ProxyInfo ToProxyInfo() => Build().ToProxyInfo();
        
        public static implicit operator ProxyInfo(Socks4ProxyBuilder builder) => builder.ToProxyInfo();
        public static implicit operator ProxyConfiguration(Socks4ProxyBuilder builder) => builder.Build();
    }

    public sealed class Socks5ProxyBuilder
    {
        private readonly ProxyConfiguration _config = new();

        internal Socks5ProxyBuilder(string host, int port, EnumProxyType type)
        {
            _config.Host = host;
            _config.Port = port;
            _config.Type = type;
            _config.ResolveHostnamesLocally = type != EnumProxyType.Socks5h;
        }

        public Socks5ProxyBuilder WithCredentials(string username, string password)
        {
            _config.Credentials = new NetworkCredential(username, password);
            return this;
        }

        public Socks5ProxyBuilder WithCredentials(NetworkCredential credentials)
        {
            _config.Credentials = credentials;
            return this;
        }

        public Socks5ProxyBuilder ResolveHostnamesLocally(bool resolveLocally = true)
        {
            _config.ResolveHostnamesLocally = resolveLocally;
            return this;
        }

        public Socks5ProxyBuilder ResolveHostnamesRemotely()
        {
            _config.ResolveHostnamesLocally = false;
            return this;
        }

        public Socks5ProxyBuilder WithConnectionTimeout(TimeSpan timeout)
        {
            _config.ConnectionTimeout = timeout;
            return this;
        }

        public Socks5ProxyBuilder WithReadWriteTimeout(TimeSpan timeout)
        {
            _config.ReadWriteTimeout = timeout;
            return this;
        }

        public Socks5ProxyBuilder WithInternalServerPort(int port)
        {
            _config.InternalServerPort = port;
            return this;
        }

        public ProxyConfiguration Build() => _config;
        
        public ProxyInfo ToProxyInfo() => Build().ToProxyInfo();
        
        public static implicit operator ProxyInfo(Socks5ProxyBuilder builder) => builder.ToProxyInfo();
        public static implicit operator ProxyConfiguration(Socks5ProxyBuilder builder) => builder.Build();
    }
}
