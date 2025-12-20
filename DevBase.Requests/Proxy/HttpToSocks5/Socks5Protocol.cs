using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;
using DevBase.Requests.Proxy.HttpToSocks5.Dns;
using DevBase.Requests.Proxy.HttpToSocks5.Enums;

namespace DevBase.Requests.Proxy.HttpToSocks5;

/// <summary>
/// SOCKS5 protocol implementation with async support and performance optimizations.
/// </summary>
internal static class Socks5Protocol
{
    private const byte SocksVersion = 0x05;
    private const byte SubnegotiationVersion = 0x01;
    private const byte NoAuthentication = 0x00;
    private const byte UsernamePasswordAuth = 0x02;
    private const byte ConnectCommand = 0x01;

    private static readonly ArrayPool<byte> BufferPool = ArrayPool<byte>.Shared;

    public static async Task<Socks5ConnectionResult> TryCreateTunnelAsync(
        Socket socket,
        string destAddress,
        int destPort,
        Socks5ProxyInfo proxy,
        IDnsResolver? dnsResolver = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Send HELLO
            var helloMessage = BuildHelloMessage(proxy.Authenticate);
            await socket.SendAsync(helloMessage, SocketFlags.None, cancellationToken);

            // Receive HELLO response
            var buffer = BufferPool.Rent(256);
            try
            {
                var received = await socket.ReceiveAsync(buffer.AsMemory(0, 2), SocketFlags.None, cancellationToken);
                
                if (received != 2)
                    return Socks5ConnectionResult.InvalidProxyResponse;
                
                if (buffer[0] != SocksVersion)
                    return Socks5ConnectionResult.InvalidProxyResponse;

                if (buffer[1] == UsernamePasswordAuth)
                {
                    if (!proxy.Authenticate)
                        return Socks5ConnectionResult.InvalidProxyResponse;

                    // Authenticate with username/password
                    await socket.SendAsync(proxy.AuthenticationMessage, SocketFlags.None, cancellationToken);
                    
                    received = await socket.ReceiveAsync(buffer.AsMemory(0, 2), SocketFlags.None, cancellationToken);
                    
                    if (received != 2)
                        return Socks5ConnectionResult.InvalidProxyResponse;
                    
                    if (buffer[0] != SubnegotiationVersion)
                        return Socks5ConnectionResult.InvalidProxyResponse;
                    
                    if (buffer[1] != 0)
                        return Socks5ConnectionResult.AuthenticationError;
                }
                else if (buffer[1] != NoAuthentication)
                {
                    return Socks5ConnectionResult.AuthenticationError;
                }

                // Resolve hostname locally if requested
                var resolvedAddress = destAddress;
                if (dnsResolver != null)
                {
                    var addressType = GetAddressType(destAddress);
                    if (addressType == Socks5AddressType.DomainName)
                    {
                        var ipAddress = dnsResolver.TryResolve(destAddress);
                        if (ipAddress == null)
                            return Socks5ConnectionResult.HostUnreachable;
                        
                        resolvedAddress = ipAddress.ToString();
                    }
                }

                // Send CONNECT request
                var requestMessage = BuildRequestMessage(resolvedAddress, destPort);
                await socket.SendAsync(requestMessage, SocketFlags.None, cancellationToken);

                // Receive response
                received = await socket.ReceiveAsync(buffer.AsMemory(0, 256), SocketFlags.None, cancellationToken);
                
                if (received < 8)
                    return Socks5ConnectionResult.InvalidProxyResponse;
                
                if (buffer[0] != SocksVersion)
                    return Socks5ConnectionResult.InvalidProxyResponse;
                
                if (buffer[1] > 8)
                    return Socks5ConnectionResult.InvalidProxyResponse;
                
                if (buffer[1] != 0)
                    return (Socks5ConnectionResult)buffer[1];
                
                if (buffer[2] != 0)
                    return Socks5ConnectionResult.InvalidProxyResponse;

                var boundAddressType = (Socks5AddressType)buffer[3];
                var expectedLength = boundAddressType switch
                {
                    Socks5AddressType.IPv4 => 10,
                    Socks5AddressType.IPv6 => 22,
                    Socks5AddressType.DomainName => 7 + buffer[4],
                    _ => -1
                };

                if (expectedLength == -1 || received != expectedLength)
                    return Socks5ConnectionResult.InvalidProxyResponse;

                return Socks5ConnectionResult.OK;
            }
            finally
            {
                BufferPool.Return(buffer);
            }
        }
        catch (SocketException ex)
        {
            return ex.ToConnectionResult();
        }
        catch
        {
            return Socks5ConnectionResult.UnknownError;
        }
    }

    private static byte[] BuildHelloMessage(bool doUsernamePasswordAuth)
    {
        if (doUsernamePasswordAuth)
        {
            return [SocksVersion, 2, NoAuthentication, UsernamePasswordAuth];
        }
        return [SocksVersion, 1, NoAuthentication];
    }

    private static byte[] BuildRequestMessage(string address, int port)
    {
        var addressType = GetAddressType(address);
        
        byte[] addressBytes;
        int addressLength;

        switch (addressType)
        {
            case Socks5AddressType.IPv4:
            case Socks5AddressType.IPv6:
                addressBytes = IPAddress.Parse(address).GetAddressBytes();
                addressLength = addressBytes.Length;
                break;

            case Socks5AddressType.DomainName:
                var domainBytes = Encoding.UTF8.GetBytes(address);
                addressLength = 1 + domainBytes.Length;
                addressBytes = new byte[addressLength];
                addressBytes[0] = (byte)domainBytes.Length;
                Buffer.BlockCopy(domainBytes, 0, addressBytes, 1, domainBytes.Length);
                break;

            default:
                throw new ArgumentException("Unknown address type");
        }

        var request = new byte[6 + addressLength];
        request[0] = SocksVersion;
        request[1] = ConnectCommand;
        request[2] = 0x00; // Reserved
        request[3] = (byte)addressType;
        Buffer.BlockCopy(addressBytes, 0, request, 4, addressLength);
        request[^2] = (byte)(port >> 8);
        request[^1] = (byte)(port & 0xFF);
        
        return request;
    }

    private static Socks5AddressType GetAddressType(string hostname)
    {
        if (IPAddress.TryParse(hostname, out var ip))
        {
            return ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                ? Socks5AddressType.IPv4
                : Socks5AddressType.IPv6;
        }
        return Socks5AddressType.DomainName;
    }
}
