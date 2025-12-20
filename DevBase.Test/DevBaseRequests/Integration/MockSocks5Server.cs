using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DevBase.Test.DevBaseRequests.Integration;

/// <summary>
/// A mock SOCKS5 proxy server for integration testing.
/// Implements the SOCKS5 protocol (RFC 1928) with optional authentication.
/// </summary>
public sealed class MockSocks5Server : IDisposable
{
    private const byte Socks5Version = 0x05;
    private const byte NoAuthRequired = 0x00;
    private const byte UsernamePasswordAuth = 0x02;
    private const byte AuthSuccess = 0x00;
    private const byte AuthFailed = 0x01;
    private const byte ConnectCommand = 0x01;
    private const byte Ipv4AddressType = 0x01;
    private const byte DomainNameAddressType = 0x03;
    private const byte Ipv6AddressType = 0x04;

    private readonly TcpListener _listener;
    private readonly CancellationTokenSource _cts;
    private readonly Task _serverTask;
    private readonly string? _username;
    private readonly string? _password;

    public int Port { get; }
    public int ConnectionCount { get; private set; }
    public int AuthenticationAttempts { get; private set; }
    public int SuccessfulConnections { get; private set; }
    public int FailedConnections { get; private set; }
    public List<ProxyConnectionRecord> ConnectionRecords { get; } = new();
    public bool RequiresAuthentication => _username != null && _password != null;

    /// <summary>
    /// Creates a mock SOCKS5 server without authentication.
    /// </summary>
    public MockSocks5Server(int port = 0)
        : this(port, null, null) { }

    /// <summary>
    /// Creates a mock SOCKS5 server with username/password authentication.
    /// </summary>
    public MockSocks5Server(int port, string? username, string? password)
    {
        _username = username;
        _password = password;
        
        _listener = new TcpListener(IPAddress.Loopback, port);
        _listener.Start();
        Port = ((IPEndPoint)_listener.LocalEndpoint).Port;
        
        _cts = new CancellationTokenSource();
        _serverTask = Task.Run(AcceptConnectionsAsync);
    }

    private async Task AcceptConnectionsAsync()
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            try
            {
                var client = await _listener.AcceptTcpClientAsync(_cts.Token);
                ConnectionCount++;
                _ = Task.Run(() => HandleClientAsync(client));
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (ObjectDisposedException)
            {
                break;
            }
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        var record = new ProxyConnectionRecord
        {
            ConnectedAt = DateTime.UtcNow,
            ClientEndpoint = client.Client.RemoteEndPoint?.ToString() ?? "unknown"
        };

        try
        {
            using var stream = client.GetStream();
            stream.ReadTimeout = 30000;
            stream.WriteTimeout = 30000;

            // Step 1: Handle SOCKS5 greeting
            var greeting = new byte[256];
            var read = await stream.ReadAsync(greeting.AsMemory(0, 2), _cts.Token);
            
            if (read < 2 || greeting[0] != Socks5Version)
            {
                record.FailureReason = "Invalid SOCKS5 greeting";
                FailedConnections++;
                return;
            }

            var numMethods = greeting[1];
            read = await stream.ReadAsync(greeting.AsMemory(0, numMethods), _cts.Token);
            
            if (read < numMethods)
            {
                record.FailureReason = "Incomplete auth methods";
                FailedConnections++;
                return;
            }

            var methods = greeting.AsSpan(0, numMethods);
            
            // Step 2: Authentication
            if (RequiresAuthentication)
            {
                AuthenticationAttempts++;
                
                if (!methods.Contains(UsernamePasswordAuth))
                {
                    await stream.WriteAsync(new byte[] { Socks5Version, 0xFF }, _cts.Token);
                    record.FailureReason = "No acceptable auth method";
                    FailedConnections++;
                    return;
                }

                // Request username/password authentication
                await stream.WriteAsync(new byte[] { Socks5Version, UsernamePasswordAuth }, _cts.Token);

                // Read authentication request
                var authData = new byte[513];
                read = await stream.ReadAsync(authData.AsMemory(0, 2), _cts.Token);
                
                if (read < 2 || authData[0] != 0x01)
                {
                    record.FailureReason = "Invalid auth version";
                    FailedConnections++;
                    return;
                }

                var usernameLen = authData[1];
                read = await stream.ReadAsync(authData.AsMemory(0, usernameLen), _cts.Token);
                var receivedUsername = Encoding.UTF8.GetString(authData, 0, usernameLen);

                read = await stream.ReadAsync(authData.AsMemory(0, 1), _cts.Token);
                var passwordLen = authData[0];
                read = await stream.ReadAsync(authData.AsMemory(0, passwordLen), _cts.Token);
                var receivedPassword = Encoding.UTF8.GetString(authData, 0, passwordLen);

                record.AuthenticatedUsername = receivedUsername;

                if (receivedUsername != _username || receivedPassword != _password)
                {
                    await stream.WriteAsync(new byte[] { 0x01, AuthFailed }, _cts.Token);
                    record.FailureReason = "Authentication failed";
                    record.AuthenticationSuccessful = false;
                    FailedConnections++;
                    return;
                }

                await stream.WriteAsync(new byte[] { 0x01, AuthSuccess }, _cts.Token);
                record.AuthenticationSuccessful = true;
            }
            else
            {
                if (!methods.Contains(NoAuthRequired))
                {
                    await stream.WriteAsync(new byte[] { Socks5Version, 0xFF }, _cts.Token);
                    record.FailureReason = "No auth not supported by client";
                    FailedConnections++;
                    return;
                }

                await stream.WriteAsync(new byte[] { Socks5Version, NoAuthRequired }, _cts.Token);
            }

            // Step 3: Handle connection request
            var request = new byte[256];
            read = await stream.ReadAsync(request.AsMemory(0, 4), _cts.Token);
            
            if (read < 4)
            {
                record.FailureReason = "Incomplete connection request";
                FailedConnections++;
                return;
            }

            if (request[0] != Socks5Version)
            {
                record.FailureReason = "Invalid SOCKS version in request";
                FailedConnections++;
                return;
            }

            var command = request[1];
            if (command != ConnectCommand)
            {
                await SendReply(stream, 0x07); // Command not supported
                record.FailureReason = $"Unsupported command: {command}";
                FailedConnections++;
                return;
            }

            var addressType = request[3];
            string targetHost;
            int targetPort;

            switch (addressType)
            {
                case Ipv4AddressType:
                    read = await stream.ReadAsync(request.AsMemory(0, 6), _cts.Token);
                    if (read < 6)
                    {
                        record.FailureReason = "Incomplete IPv4 address";
                        FailedConnections++;
                        return;
                    }
                    targetHost = new IPAddress(request.AsSpan(0, 4)).ToString();
                    targetPort = (request[4] << 8) | request[5];
                    break;

                case DomainNameAddressType:
                    read = await stream.ReadAsync(request.AsMemory(0, 1), _cts.Token);
                    var domainLen = request[0];
                    read = await stream.ReadAsync(request.AsMemory(0, domainLen + 2), _cts.Token);
                    targetHost = Encoding.ASCII.GetString(request, 0, domainLen);
                    targetPort = (request[domainLen] << 8) | request[domainLen + 1];
                    break;

                case Ipv6AddressType:
                    read = await stream.ReadAsync(request.AsMemory(0, 18), _cts.Token);
                    if (read < 18)
                    {
                        record.FailureReason = "Incomplete IPv6 address";
                        FailedConnections++;
                        return;
                    }
                    targetHost = new IPAddress(request.AsSpan(0, 16)).ToString();
                    targetPort = (request[16] << 8) | request[17];
                    break;

                default:
                    await SendReply(stream, 0x08); // Address type not supported
                    record.FailureReason = $"Unsupported address type: {addressType}";
                    FailedConnections++;
                    return;
            }

            record.TargetHost = targetHost;
            record.TargetPort = targetPort;

            // Step 4: Connect to the target
            TcpClient? targetClient = null;
            try
            {
                targetClient = new TcpClient();
                await targetClient.ConnectAsync(targetHost, targetPort, _cts.Token);
                
                var localEp = (IPEndPoint)targetClient.Client.LocalEndPoint!;
                await SendReply(stream, 0x00, localEp.Address, localEp.Port);
                
                record.ConnectionSuccessful = true;
                SuccessfulConnections++;

                // Step 5: Relay data between client and target
                await RelayDataAsync(stream, targetClient.GetStream());
            }
            catch (SocketException ex)
            {
                byte errorCode = ex.SocketErrorCode switch
                {
                    SocketError.NetworkUnreachable => 0x03,
                    SocketError.HostUnreachable => 0x04,
                    SocketError.ConnectionRefused => 0x05,
                    _ => 0x01
                };
                await SendReply(stream, errorCode);
                record.FailureReason = $"Connection failed: {ex.Message}";
                FailedConnections++;
            }
            finally
            {
                targetClient?.Dispose();
            }
        }
        catch (System.Exception ex)
        {
            record.FailureReason = $"Exception: {ex.Message}";
            FailedConnections++;
        }
        finally
        {
            record.DisconnectedAt = DateTime.UtcNow;
            ConnectionRecords.Add(record);
            client.Dispose();
        }
    }

    private static async Task SendReply(NetworkStream stream, byte status, IPAddress? bindAddress = null, int bindPort = 0)
    {
        bindAddress ??= IPAddress.Any;
        var addressBytes = bindAddress.GetAddressBytes();
        var isIpv6 = bindAddress.AddressFamily == AddressFamily.InterNetworkV6;

        var reply = new byte[isIpv6 ? 22 : 10];
        reply[0] = Socks5Version;
        reply[1] = status;
        reply[2] = 0x00; // Reserved
        reply[3] = isIpv6 ? Ipv6AddressType : Ipv4AddressType;
        Array.Copy(addressBytes, 0, reply, 4, addressBytes.Length);
        reply[^2] = (byte)(bindPort >> 8);
        reply[^1] = (byte)(bindPort & 0xFF);

        await stream.WriteAsync(reply);
    }

    private async Task RelayDataAsync(NetworkStream clientStream, NetworkStream targetStream)
    {
        var clientToTarget = RelayOneWayAsync(clientStream, targetStream, "client->target");
        var targetToClient = RelayOneWayAsync(targetStream, clientStream, "target->client");

        await Task.WhenAny(clientToTarget, targetToClient);
    }

    private async Task RelayOneWayAsync(NetworkStream source, NetworkStream destination, string direction)
    {
        var buffer = new byte[8192];
        try
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                var read = await source.ReadAsync(buffer, _cts.Token);
                if (read == 0) break;
                await destination.WriteAsync(buffer.AsMemory(0, read), _cts.Token);
            }
        }
        catch
        {
            // Connection closed
        }
    }

    public void ResetCounters()
    {
        ConnectionCount = 0;
        AuthenticationAttempts = 0;
        SuccessfulConnections = 0;
        FailedConnections = 0;
        ConnectionRecords.Clear();
    }

    public void Dispose()
    {
        _cts.Cancel();
        _listener.Stop();
        
        try { _serverTask.Wait(TimeSpan.FromSeconds(2)); }
        catch { /* ignore */ }
        
        _cts.Dispose();
    }
}

public class ProxyConnectionRecord
{
    public DateTime ConnectedAt { get; init; }
    public DateTime DisconnectedAt { get; set; }
    public string ClientEndpoint { get; init; } = "";
    public string? TargetHost { get; set; }
    public int TargetPort { get; set; }
    public bool ConnectionSuccessful { get; set; }
    public bool? AuthenticationSuccessful { get; set; }
    public string? AuthenticatedUsername { get; set; }
    public string? FailureReason { get; set; }
    public TimeSpan Duration => DisconnectedAt - ConnectedAt;
}
