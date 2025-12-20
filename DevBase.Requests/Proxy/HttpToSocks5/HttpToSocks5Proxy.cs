using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;
using DevBase.Requests.Proxy.HttpToSocks5.Dns;
using DevBase.Requests.Proxy.HttpToSocks5.Enums;

namespace DevBase.Requests.Proxy.HttpToSocks5;

/// <summary>
/// Presents itself as an HTTP(s) proxy, but connects to a SOCKS5 proxy behind-the-scenes.
/// Based on MihaZupan's HttpToSocks5Proxy library (MIT License).
/// https://github.com/MihaZupan/HttpToSocks5Proxy
/// </summary>
public sealed class HttpToSocks5Proxy : IWebProxy, IDisposable
{
    private static readonly ArrayPool<byte> BufferPool = ArrayPool<byte>.Shared;
    
    public ICredentials? Credentials { get; set; }
    
    public Uri GetProxy(Uri destination) => _proxyUri;
    
    public bool IsBypassed(Uri host) => false;
    
    public int InternalServerPort { get; private set; }
    
    public IDnsResolver DnsResolver
    {
        get => _dnsResolver;
        set => _dnsResolver = value ?? throw new ArgumentNullException(nameof(value));
    }
    private IDnsResolver _dnsResolver;

    private readonly Uri _proxyUri;
    private readonly Socket _internalServerSocket;
    private readonly Socks5ProxyInfo[] _proxyList;
    
    /// <summary>
    /// Controls whether domain names are resolved locally or passed to the proxy server.
    /// False by default (SOCKS5h behavior).
    /// </summary>
    public bool ResolveHostnamesLocally { get; set; }

    private volatile bool _stopped;

    #region Constructors
    
    /// <summary>
    /// Create an HTTP(s) to SOCKS5 proxy using no authentication.
    /// </summary>
    public HttpToSocks5Proxy(string socks5Hostname, int socks5Port, int internalServerPort = 0)
        : this([new Socks5ProxyInfo(socks5Hostname, socks5Port)], internalServerPort) { }

    /// <summary>
    /// Create an HTTP(s) to SOCKS5 proxy using username and password authentication.
    /// </summary>
    public HttpToSocks5Proxy(string socks5Hostname, int socks5Port, string username, string password, int internalServerPort = 0)
        : this([new Socks5ProxyInfo(socks5Hostname, socks5Port, username, password)], internalServerPort) { }

    /// <summary>
    /// Create an HTTP(s) to SOCKS5 proxy using one or multiple chained proxies.
    /// </summary>
    public HttpToSocks5Proxy(Socks5ProxyInfo[] proxyList, int internalServerPort = 0)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(internalServerPort);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(internalServerPort, 65535);
        ArgumentNullException.ThrowIfNull(proxyList);
        
        if (proxyList.Length == 0)
            throw new ArgumentException("proxyList is empty", nameof(proxyList));
        
        if (proxyList.Any(p => p == null))
            throw new ArgumentNullException(nameof(proxyList), "Proxy in proxyList is null");

        _proxyList = proxyList;
        InternalServerPort = internalServerPort;
        _dnsResolver = new DefaultDnsResolver();

        _internalServerSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        _internalServerSocket.Bind(new IPEndPoint(IPAddress.Loopback, InternalServerPort));

        if (InternalServerPort == 0)
            InternalServerPort = ((IPEndPoint)_internalServerSocket.LocalEndPoint!).Port;

        _proxyUri = new Uri($"http://127.0.0.1:{InternalServerPort}");
        _internalServerSocket.Listen(16);
        _ = AcceptConnectionsAsync();
    }
    
    #endregion

    private async Task AcceptConnectionsAsync()
    {
        while (!_stopped)
        {
            try
            {
                var clientSocket = await _internalServerSocket.AcceptAsync();
                _ = Task.Run(() => HandleRequestAsync(clientSocket));
            }
            catch when (_stopped)
            {
                break;
            }
            catch
            {
                // Continue accepting connections
            }
        }
    }

    private async Task HandleRequestAsync(Socket clientSocket)
    {
        Socket? socks5Socket = null;
        var success = true;

        try
        {
            var parseResult = await TryReadTargetAsync(clientSocket);
            
            if (parseResult.Success)
            {
                try
                {
                    socks5Socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                    var proxyAddress = _dnsResolver.TryResolve(_proxyList[0].Hostname);
                    
                    if (proxyAddress == null)
                    {
                        await SendErrorAsync(clientSocket, Socks5ConnectionResult.HostUnreachable, parseResult.HttpVersion);
                        success = false;
                    }
                    else
                    {
                        await socks5Socket.ConnectAsync(proxyAddress, _proxyList[0].Port);
                    }
                }
                catch (SocketException ex)
                {
                    await SendErrorAsync(clientSocket, ex.ToConnectionResult(), parseResult.HttpVersion);
                    success = false;
                }
                catch
                {
                    await SendErrorAsync(clientSocket, Socks5ConnectionResult.UnknownError, parseResult.HttpVersion);
                    success = false;
                }

                if (success && socks5Socket != null)
                {
                    // Chain through proxies
                    for (var i = 0; i < _proxyList.Length - 1 && success; i++)
                    {
                        var proxy = _proxyList[i];
                        var nextProxy = _proxyList[i + 1];
                        var result = await Socks5Protocol.TryCreateTunnelAsync(
                            socks5Socket, nextProxy.Hostname, nextProxy.Port, proxy,
                            ResolveHostnamesLocally ? _dnsResolver : null);
                        
                        if (result != Socks5ConnectionResult.OK)
                        {
                            await SendErrorAsync(clientSocket, result, parseResult.HttpVersion);
                            success = false;
                        }
                    }

                    if (success)
                    {
                        var lastProxy = _proxyList[^1];
                        var result = await Socks5Protocol.TryCreateTunnelAsync(
                            socks5Socket, parseResult.Hostname!, parseResult.Port, lastProxy,
                            ResolveHostnamesLocally ? _dnsResolver : null);
                        
                        if (result != Socks5ConnectionResult.OK)
                        {
                            await SendErrorAsync(clientSocket, result, parseResult.HttpVersion);
                            success = false;
                        }
                        else
                        {
                            if (!parseResult.IsConnect)
                            {
                                await SendStringAsync(socks5Socket, parseResult.Request!);
                                if (parseResult.OverRead != null)
                                {
                                    await socks5Socket.SendAsync(parseResult.OverRead, SocketFlags.None);
                                }
                            }
                            else
                            {
                                await SendStringAsync(clientSocket, 
                                    $"{parseResult.HttpVersion}200 Connection established\r\nProxy-Agent: DevBase-HttpToSocks5Proxy\r\n\r\n");
                            }
                        }
                    }
                }
            }
            else
            {
                success = false;
            }
        }
        catch
        {
            success = false;
            try { await SendErrorAsync(clientSocket, Socks5ConnectionResult.UnknownError); } catch { }
        }
        finally
        {
            if (success && socks5Socket != null)
            {
                SocketRelay.RelayBidirectionally(socks5Socket, clientSocket);
            }
            else
            {
                clientSocket.TryDispose();
                socks5Socket?.TryDispose();
            }
        }
    }

    private async Task<HttpRequestParseResult> TryReadTargetAsync(Socket clientSocket)
    {
        var result = new HttpRequestParseResult();
        
        var headersResult = await TryReadHeadersAsync(clientSocket);
        if (!headersResult.Success)
            return result;

        var headerString = headersResult.Headers!;
        var headerLines = headerString.Split('\n')
            .Select(i => i.TrimEnd('\r'))
            .Where(i => i.Length > 0)
            .ToList();
        
        var methodLine = headerLines[0].Split(' ');
        if (methodLine.Length != 3)
        {
            await SendErrorAsync(clientSocket, Socks5ConnectionResult.InvalidRequest);
            return result;
        }

        var method = methodLine[0];
        result.HttpVersion = methodLine[2].Trim() + " ";
        result.IsConnect = method.Equals("CONNECT", StringComparison.OrdinalIgnoreCase);
        string? hostHeader = null;

        if (result.IsConnect)
        {
            foreach (var headerLine in headerLines)
            {
                var colonIndex = headerLine.IndexOf(':');
                if (colonIndex == -1)
                {
                    await SendErrorAsync(clientSocket, Socks5ConnectionResult.InvalidRequest, result.HttpVersion);
                    return result;
                }
                
                var headerName = headerLine.AsSpan(0, colonIndex).Trim().ToString();
                if (headerName.Equals("Host", StringComparison.OrdinalIgnoreCase))
                {
                    hostHeader = headerLine[(colonIndex + 1)..].Trim();
                    break;
                }
            }
        }
        else
        {
            var hostUri = new Uri(methodLine[1]);
            var requestBuilder = new StringBuilder(512);

            requestBuilder.Append(methodLine[0]);
            requestBuilder.Append(' ');
            requestBuilder.Append(hostUri.PathAndQuery);
            requestBuilder.Append(hostUri.Fragment);
            requestBuilder.Append(' ');
            requestBuilder.Append(methodLine[2]);

            for (var i = 1; i < headerLines.Count; i++)
            {
                var colonIndex = headerLines[i].IndexOf(':');
                if (colonIndex == -1) continue;
                
                var headerName = headerLines[i].AsSpan(0, colonIndex).Trim().ToString();

                if (headerName.Equals("Host", StringComparison.OrdinalIgnoreCase))
                {
                    hostHeader = headerLines[i][(colonIndex + 1)..].Trim();
                    requestBuilder.Append("\r\n");
                    requestBuilder.Append(headerLines[i]);
                }
                else if (!HttpHelpers.IsHopByHopHeader(headerName))
                {
                    requestBuilder.Append("\r\n");
                    requestBuilder.Append(headerLines[i]);
                }
            }
            
            if (hostHeader == null)
            {
                requestBuilder.Append("\r\nHost: ");
                requestBuilder.Append(hostUri.Host);
            }
            
            requestBuilder.Append("\r\n\r\n");
            result.Request = requestBuilder.ToString();
        }

        // Parse hostname and port
        result.Port = result.IsConnect ? 443 : 80;

        if (string.IsNullOrEmpty(hostHeader))
        {
            var requestTarget = methodLine[1];
            result.Hostname = requestTarget;
            var colonIndex = requestTarget.LastIndexOf(':');
            
            if (colonIndex != -1)
            {
                if (int.TryParse(requestTarget.AsSpan(colonIndex + 1), out var port))
                {
                    result.Port = port;
                    result.Hostname = requestTarget[..colonIndex];
                }
            }
        }
        else
        {
            var colonIndex = hostHeader.LastIndexOf(':');
            
            if (colonIndex == -1)
            {
                result.Hostname = hostHeader;
                var requestTarget = methodLine[1];
                colonIndex = requestTarget.LastIndexOf(':');
                
                if (colonIndex != -1 && int.TryParse(requestTarget.AsSpan(colonIndex + 1), out var port))
                {
                    result.Port = port;
                }
            }
            else
            {
                result.Hostname = hostHeader[..colonIndex];
                if (int.TryParse(hostHeader.AsSpan(colonIndex + 1), out var port))
                {
                    result.Port = port;
                }
            }
        }

        result.OverRead = headersResult.OverRead;
        result.Success = true;
        return result;
    }

    private static async Task<HeadersParseResult> TryReadHeadersAsync(Socket clientSocket)
    {
        var result = new HeadersParseResult();
        var buffer = BufferPool.Rent(8192);
        
        try
        {
            var received = 0;
            var left = 8192;

            do
            {
                if (left == 0)
                {
                    await SendErrorAsync(clientSocket, Socks5ConnectionResult.InvalidRequest);
                    return result;
                }

                var offset = received;
                var read = await clientSocket.ReceiveAsync(buffer.AsMemory(received, left), SocketFlags.None);
                
                if (read == 0)
                    return result;

                received += read;
                left -= read;
            }
            while (!HttpHelpers.ContainsDoubleNewLine(buffer.AsSpan(0, received), Math.Max(0, received - 3 - 4), out var endOfHeader));

            HttpHelpers.ContainsDoubleNewLine(buffer.AsSpan(0, received), 0, out var actualEnd);
            result.Headers = Encoding.ASCII.GetString(buffer, 0, actualEnd);

            if (received != actualEnd)
            {
                var overReadCount = received - actualEnd;
                result.OverRead = new byte[overReadCount];
                Buffer.BlockCopy(buffer, actualEnd, result.OverRead, 0, overReadCount);
            }

            result.Success = true;
            return result;
        }
        finally
        {
            BufferPool.Return(buffer);
        }
    }

    private static async Task SendStringAsync(Socket socket, string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        await socket.SendAsync(bytes, SocketFlags.None);
    }

    private static async Task SendErrorAsync(Socket socket, Socks5ConnectionResult error, string httpVersion = "HTTP/1.1 ")
    {
        var response = error switch
        {
            Socks5ConnectionResult.AuthenticationError => $"{httpVersion}401 Unauthorized\r\n\r\n",
            Socks5ConnectionResult.HostUnreachable or 
            Socks5ConnectionResult.ConnectionRefused or 
            Socks5ConnectionResult.ConnectionReset => $"{httpVersion}502 {error}\r\n\r\n",
            _ => $"{httpVersion}500 Internal Server Error\r\nX-Proxy-Error-Type: {error}\r\n\r\n"
        };
        
        await SendStringAsync(socket, response);
    }

    public void StopInternalServer()
    {
        if (_stopped) return;
        _stopped = true;
        _internalServerSocket.Close();
    }

    public void Dispose()
    {
        StopInternalServer();
    }
}
