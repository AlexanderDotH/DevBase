namespace DevBase.Test.DevBaseRequests.Integration.Docker;

/// <summary>
/// Constants for Docker-based integration tests.
/// These values must match the docker-compose.yml configuration.
/// </summary>
public static class DockerTestConstants
{
    // Mock API Server
    public const string MockApiHost = "localhost";
    public const int MockApiPort = 5080;
    public const string MockApiBaseUrl = "http://localhost:5080";
    
    // HTTP Proxy with Authentication
    public const string HttpProxyHost = "localhost";
    public const int HttpProxyPort = 8888;
    public const string HttpProxyUsername = "testuser";
    public const string HttpProxyPassword = "testpass";
    public const string HttpProxyUrl = "http://localhost:8888";
    public const string HttpProxyUrlWithAuth = "http://testuser:testpass@localhost:8888";
    
    // HTTP Proxy without Authentication
    public const string HttpProxyNoAuthHost = "localhost";
    public const int HttpProxyNoAuthPort = 8889;
    public const string HttpProxyNoAuthUrl = "http://localhost:8889";
    
    // SOCKS5 Proxy with Authentication
    public const string Socks5ProxyHost = "localhost";
    public const int Socks5ProxyPort = 1080;
    public const string Socks5ProxyUsername = "testuser";
    public const string Socks5ProxyPassword = "testpass";
    public const string Socks5ProxyUrl = "socks5://localhost:1080";
    public const string Socks5ProxyUrlWithAuth = "socks5://testuser:testpass@localhost:1080";
    
    // SOCKS5 Proxy without Authentication
    public const string Socks5ProxyNoAuthHost = "localhost";
    public const int Socks5ProxyNoAuthPort = 1081;
    public const string Socks5ProxyNoAuthUrl = "socks5://localhost:1081";
    
    // Test timeouts
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);
    public static readonly TimeSpan LongTimeout = TimeSpan.FromMinutes(2);
    public static readonly TimeSpan ShortTimeout = TimeSpan.FromSeconds(5);
    
    // Docker compose file location (relative to test project)
    public const string DockerComposeDirectory = "DevBaseRequests/Integration/Docker";
    public const string DockerComposeFile = "docker-compose.yml";
}
