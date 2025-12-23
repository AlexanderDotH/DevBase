namespace DevBase.Requests.Proxy.HttpToSocks5.Enums;

/// <summary>
/// SOCKS5 address types as defined in RFC 1928.
/// </summary>
internal enum Socks5AddressType : byte
{
    IPv4 = 0x01,
    DomainName = 0x03,
    IPv6 = 0x04
}
