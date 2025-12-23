namespace DevBase.Requests.Proxy.HttpToSocks5.Enums;

/// <summary>
/// SOCKS5 connection result codes.
/// Values 0-8 match the SOCKS5 protocol specification.
/// </summary>
public enum Socks5ConnectionResult
{
    OK = 0,
    GeneralSocksServerFailure = 1,
    ConnectionNotAllowedByRuleset = 2,
    NetworkUnreachable = 3,
    HostUnreachable = 4,
    ConnectionRefused = 5,
    TTLExpired = 6,
    CommandNotSupported = 7,
    AddressTypeNotSupported = 8,

    // Library-specific error codes
    InvalidRequest = int.MinValue,
    UnknownError,
    AuthenticationError,
    ConnectionReset,
    ConnectionError,
    InvalidProxyResponse
}
