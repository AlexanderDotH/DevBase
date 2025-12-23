using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DevBase.Requests.Proxy.Socks;

public sealed class Socks4Client : IDisposable
{
    private const byte Socks4Version = 0x04;
    private const byte ConnectCommand = 0x01;
    private const byte RequestGranted = 0x5A;

    private readonly ProxyInfo _proxyInfo;
    private Socket? _socket;

    public Socks4Client(ProxyInfo proxyInfo)
    {
        _proxyInfo = proxyInfo ?? throw new ArgumentNullException(nameof(proxyInfo));
    }

    public async Task<Stream> ConnectAsync(string host, int port, CancellationToken cancellationToken = default)
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        
        await _socket.ConnectAsync(_proxyInfo.Host, _proxyInfo.Port, cancellationToken);
        
        NetworkStream stream = new NetworkStream(_socket, ownsSocket: false);

        await SendConnectCommandAsync(stream, host, port, cancellationToken);

        return stream;
    }

    private async Task SendConnectCommandAsync(NetworkStream stream, string host, int port, CancellationToken cancellationToken)
    {
        IPAddress[] addresses = await Dns.GetHostAddressesAsync(host, cancellationToken);
        if (addresses.Length == 0)
            throw new InvalidOperationException($"Could not resolve hostname: {host}");

        IPAddress address = addresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork)
            ?? throw new InvalidOperationException("SOCKS4 only supports IPv4 addresses");
        
        byte[] addressBytes = address.GetAddressBytes();
        string userId = _proxyInfo.Credentials?.UserName ?? string.Empty;
        byte[] userIdBytes = Encoding.ASCII.GetBytes(userId);

        byte[] request = new byte[9 + userIdBytes.Length];
        request[0] = Socks4Version;
        request[1] = ConnectCommand;
        request[2] = (byte)(port >> 8);
        request[3] = (byte)port;
        Array.Copy(addressBytes, 0, request, 4, 4);
        Array.Copy(userIdBytes, 0, request, 8, userIdBytes.Length);
        request[8 + userIdBytes.Length] = 0x00; // Null terminator

        await stream.WriteAsync(request, cancellationToken);

        byte[] response = new byte[8];
        int bytesRead = await stream.ReadAsync(response, cancellationToken);
        
        if (bytesRead < 8)
            throw new InvalidOperationException("Invalid SOCKS4 response");

        if (response[1] != RequestGranted)
        {
            string errorMessage = response[1] switch
            {
                0x5B => "Request rejected or failed",
                0x5C => "Request failed because client is not running identd",
                0x5D => "Request failed because client's identd could not confirm the user ID",
                _ => $"Unknown error: {response[1]}"
            };
            throw new InvalidOperationException($"SOCKS4 connect failed: {errorMessage}");
        }
    }

    public void Dispose()
    {
        _socket?.Dispose();
    }
}
