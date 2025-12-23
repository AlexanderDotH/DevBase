using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DevBase.Net.Proxy.Socks;

public sealed class Socks5Client : IDisposable
{
    private const byte Socks5Version = 0x05;
    private const byte NoAuthRequired = 0x00;
    private const byte UsernamePasswordAuth = 0x02;
    private const byte ConnectCommand = 0x01;
    private const byte Ipv4AddressType = 0x01;
    private const byte DomainNameAddressType = 0x03;
    private const byte Ipv6AddressType = 0x04;

    private readonly ProxyInfo _proxyInfo;
    private readonly bool _resolveHostnameLocally;
    private Socket? _socket;

    public Socks5Client(ProxyInfo proxyInfo, bool resolveHostnameLocally = false)
    {
        _proxyInfo = proxyInfo ?? throw new ArgumentNullException(nameof(proxyInfo));
        _resolveHostnameLocally = resolveHostnameLocally || proxyInfo.Type != Enums.EnumProxyType.Socks5h;
    }

    public async Task<Stream> ConnectAsync(string host, int port, CancellationToken cancellationToken = default)
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        
        await _socket.ConnectAsync(_proxyInfo.Host, _proxyInfo.Port, cancellationToken);
        
        NetworkStream stream = new NetworkStream(_socket, ownsSocket: false);

        await NegotiateAuthenticationAsync(stream, cancellationToken);
        await SendConnectCommandAsync(stream, host, port, cancellationToken);

        return stream;
    }

    private async Task NegotiateAuthenticationAsync(NetworkStream stream, CancellationToken cancellationToken)
    {
        bool hasCredentials = _proxyInfo.Credentials != null;
        byte[] authMethods = hasCredentials 
            ? new byte[] { Socks5Version, 2, NoAuthRequired, UsernamePasswordAuth }
            : new byte[] { Socks5Version, 1, NoAuthRequired };

        await stream.WriteAsync(authMethods, cancellationToken);

        byte[] response = new byte[2];
        int bytesRead = await stream.ReadAsync(response, cancellationToken);
        
        if (bytesRead != 2)
            throw new InvalidOperationException("Invalid SOCKS5 authentication response");

        if (response[0] != Socks5Version)
            throw new InvalidOperationException($"Unexpected SOCKS version: {response[0]}");

        switch (response[1])
        {
            case NoAuthRequired:
                return;
                
            case UsernamePasswordAuth when hasCredentials:
                await AuthenticateWithUsernamePasswordAsync(stream, cancellationToken);
                break;
                
            case 0xFF:
                throw new InvalidOperationException("No acceptable authentication methods");
                
            default:
                throw new InvalidOperationException($"Unsupported authentication method: {response[1]}");
        }
    }

    private async Task AuthenticateWithUsernamePasswordAsync(NetworkStream stream, CancellationToken cancellationToken)
    {
        string username = _proxyInfo.Credentials!.UserName;
        string password = _proxyInfo.Credentials.Password;

        byte[] usernameBytes = Encoding.UTF8.GetBytes(username);
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

        byte[] authRequest = new byte[3 + usernameBytes.Length + passwordBytes.Length];
        authRequest[0] = 0x01; // Version
        authRequest[1] = (byte)usernameBytes.Length;
        Array.Copy(usernameBytes, 0, authRequest, 2, usernameBytes.Length);
        authRequest[2 + usernameBytes.Length] = (byte)passwordBytes.Length;
        Array.Copy(passwordBytes, 0, authRequest, 3 + usernameBytes.Length, passwordBytes.Length);

        await stream.WriteAsync(authRequest, cancellationToken);

        byte[] response = new byte[2];
        int bytesRead = await stream.ReadAsync(response, cancellationToken);
        
        if (bytesRead != 2 || response[1] != 0x00)
            throw new InvalidOperationException("SOCKS5 username/password authentication failed");
    }

    private async Task SendConnectCommandAsync(NetworkStream stream, string host, int port, CancellationToken cancellationToken)
    {
        byte[] request;

        if (_resolveHostnameLocally && IPAddress.TryParse(host, out _) == false)
        {
            IPAddress[] addresses = await Dns.GetHostAddressesAsync(host, cancellationToken);
            if (addresses.Length == 0)
                throw new InvalidOperationException($"Could not resolve hostname: {host}");

            IPAddress address = addresses.First(a => a.AddressFamily == AddressFamily.InterNetwork);
            byte[] addressBytes = address.GetAddressBytes();

            request = new byte[10];
            request[0] = Socks5Version;
            request[1] = ConnectCommand;
            request[2] = 0x00; // Reserved
            request[3] = Ipv4AddressType;
            Array.Copy(addressBytes, 0, request, 4, 4);
            request[8] = (byte)(port >> 8);
            request[9] = (byte)port;
        }
        else
        {
            byte[] hostBytes = Encoding.UTF8.GetBytes(host);
            request = new byte[7 + hostBytes.Length];
            request[0] = Socks5Version;
            request[1] = ConnectCommand;
            request[2] = 0x00; // Reserved
            request[3] = DomainNameAddressType;
            request[4] = (byte)hostBytes.Length;
            Array.Copy(hostBytes, 0, request, 5, hostBytes.Length);
            request[5 + hostBytes.Length] = (byte)(port >> 8);
            request[6 + hostBytes.Length] = (byte)port;
        }

        await stream.WriteAsync(request, cancellationToken);

        byte[] response = new byte[10];
        int bytesRead = await stream.ReadAsync(response.AsMemory(0, 4), cancellationToken);
        
        if (bytesRead < 4)
            throw new InvalidOperationException("Invalid SOCKS5 connect response");

        if (response[0] != Socks5Version)
            throw new InvalidOperationException($"Unexpected SOCKS version: {response[0]}");

        if (response[1] != 0x00)
        {
            string errorMessage = response[1] switch
            {
                0x01 => "General SOCKS server failure",
                0x02 => "Connection not allowed by ruleset",
                0x03 => "Network unreachable",
                0x04 => "Host unreachable",
                0x05 => "Connection refused",
                0x06 => "TTL expired",
                0x07 => "Command not supported",
                0x08 => "Address type not supported",
                _ => $"Unknown error: {response[1]}"
            };
            throw new InvalidOperationException($"SOCKS5 connect failed: {errorMessage}");
        }

        // Read the rest of the response based on address type
        byte addressType = response[3];
        int remainingBytes = addressType switch
        {
            Ipv4AddressType => 6,
            Ipv6AddressType => 18,
            DomainNameAddressType => throw new InvalidOperationException("Domain name address type in response not supported"),
            _ => throw new InvalidOperationException($"Unknown address type: {addressType}")
        };

        await stream.ReadAsync(response.AsMemory(4, remainingBytes - 4), cancellationToken);
    }

    public void Dispose()
    {
        _socket?.Dispose();
    }
}
