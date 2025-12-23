using System.Net.Sockets;
using DevBase.Net.Proxy.HttpToSocks5.Enums;

namespace DevBase.Net.Proxy.HttpToSocks5;

/// <summary>
/// HTTP protocol helpers and socket extensions.
/// </summary>
internal static class HttpHelpers
{
    private static readonly HashSet<string> HopByHopHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        "Connection", "Keep-Alive", "Proxy-Authenticate", "Proxy-Authorization",
        "TE", "Trailer", "Transfer-Encoding", "Upgrade"
    };

    public static bool IsHopByHopHeader(string header) => HopByHopHeaders.Contains(header);

    public static bool ContainsDoubleNewLine(ReadOnlySpan<byte> buffer, int offset, out int endOfHeader)
    {
        const byte R = (byte)'\r';
        const byte N = (byte)'\n';

        bool foundOne = false;
        endOfHeader = offset;
        
        for (; endOfHeader < buffer.Length; endOfHeader++)
        {
            if (buffer[endOfHeader] == N)
            {
                if (foundOne)
                {
                    endOfHeader++;
                    return true;
                }
                foundOne = true;
            }
            else if (buffer[endOfHeader] != R)
            {
                foundOne = false;
            }
        }

        return false;
    }

    public static Socks5ConnectionResult ToConnectionResult(this SocketException exception)
    {
        return exception.SocketErrorCode switch
        {
            SocketError.ConnectionRefused => Socks5ConnectionResult.ConnectionRefused,
            SocketError.HostUnreachable => Socks5ConnectionResult.HostUnreachable,
            SocketError.NetworkUnreachable => Socks5ConnectionResult.NetworkUnreachable,
            SocketError.TimedOut => Socks5ConnectionResult.TTLExpired,
            SocketError.ConnectionReset => Socks5ConnectionResult.ConnectionReset,
            _ => Socks5ConnectionResult.ConnectionError
        };
    }

    public static void TryDispose(this Socket? socket)
    {
        if (socket is null) return;

        if (socket.Connected)
        {
            try { socket.Shutdown(SocketShutdown.Both); } catch { }
        }
        
        try { socket.Close(); } catch { }
    }

    public static void TryDispose(this SocketAsyncEventArgs? saea)
    {
        if (saea is null) return;

        try
        {
            saea.UserToken = null;
            saea.AcceptSocket = null;
            saea.Dispose();
        }
        catch { }
    }
}
