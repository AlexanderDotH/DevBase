using System.Net;

namespace DevBase.Requests.Proxy.HttpToSocks5.Dns;

/// <summary>
/// Interface for custom DNS resolution.
/// </summary>
public interface IDnsResolver
{
    /// <summary>
    /// Attempts to resolve a hostname to an IP address.
    /// </summary>
    /// <param name="hostname">The hostname to resolve.</param>
    /// <returns>The resolved IP address, or null if resolution failed.</returns>
    IPAddress? TryResolve(string hostname);
}
