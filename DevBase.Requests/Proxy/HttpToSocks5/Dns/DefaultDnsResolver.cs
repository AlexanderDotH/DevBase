using System.Net;
using System.Net.Sockets;

namespace DevBase.Requests.Proxy.HttpToSocks5.Dns;

/// <summary>
/// Default DNS resolver using system DNS.
/// </summary>
public sealed class DefaultDnsResolver : IDnsResolver
{
    public IPAddress? TryResolve(string hostname)
    {
        if (IPAddress.TryParse(hostname, out var address))
            return address;

        try
        {
            var addresses = System.Net.Dns.GetHostAddresses(hostname);
            return addresses.FirstOrDefault(a => 
                a.AddressFamily == AddressFamily.InterNetwork || 
                a.AddressFamily == AddressFamily.InterNetworkV6);
        }
        catch (SocketException)
        {
            return null;
        }
    }
}
