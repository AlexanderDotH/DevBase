using System.Text;

namespace DevBase.Requests.Proxy.HttpToSocks5;

/// <summary>
/// SOCKS5 proxy server information for use with HttpToSocks5Proxy.
/// </summary>
public sealed class Socks5ProxyInfo
{
    /// <summary>
    /// Proxy server hostname or IP address.
    /// </summary>
    public string Hostname { get; }
    
    /// <summary>
    /// Proxy server port.
    /// </summary>
    public int Port { get; }
    
    /// <summary>
    /// Indicates whether credentials were provided for authentication.
    /// </summary>
    public bool Authenticate { get; }
    
    /// <summary>
    /// Pre-built authentication message for performance.
    /// </summary>
    internal ReadOnlyMemory<byte> AuthenticationMessage { get; }

    public Socks5ProxyInfo(string hostname, int port)
    {
        ArgumentException.ThrowIfNullOrEmpty(hostname);
        ArgumentOutOfRangeException.ThrowIfNegative(port);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(port, 65535);

        Hostname = hostname;
        Port = port;
        Authenticate = false;
        AuthenticationMessage = ReadOnlyMemory<byte>.Empty;
    }

    public Socks5ProxyInfo(string hostname, int port, string username, string password)
        : this(hostname, port)
    {
        ArgumentException.ThrowIfNullOrEmpty(username);
        ArgumentException.ThrowIfNullOrEmpty(password);

        Authenticate = true;
        AuthenticationMessage = BuildAuthenticationMessage(username, password);
    }

    private static byte[] BuildAuthenticationMessage(string username, string password)
    {
        var usernameBytes = Encoding.UTF8.GetBytes(username);
        if (usernameBytes.Length > 255)
            throw new ArgumentOutOfRangeException(nameof(username), "Username is too long (max 255 bytes)");

        var passwordBytes = Encoding.UTF8.GetBytes(password);
        if (passwordBytes.Length > 255)
            throw new ArgumentOutOfRangeException(nameof(password), "Password is too long (max 255 bytes)");

        var authMessage = new byte[3 + usernameBytes.Length + passwordBytes.Length];
        authMessage[0] = 0x01; // Subnegotiation version
        authMessage[1] = (byte)usernameBytes.Length;
        Buffer.BlockCopy(usernameBytes, 0, authMessage, 2, usernameBytes.Length);
        authMessage[2 + usernameBytes.Length] = (byte)passwordBytes.Length;
        Buffer.BlockCopy(passwordBytes, 0, authMessage, 3 + usernameBytes.Length, passwordBytes.Length);
        
        return authMessage;
    }
}
