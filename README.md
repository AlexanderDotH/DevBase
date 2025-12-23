# DevBase

DevBase is a versatile and multi-purpose library designed primarily for use with the OpenLyricsClient. The library features a collection of API endpoint implementations for popular services like Deezer, OpenAI, OpenLyricsClient, Replicate, and Tidal. In addition, DevBase offers various utilities, such as color calculation for images, cryptographic implementations, and parsers for various file formats.

## Table of Contents

- [DevBase.Api](#devbaseapi)
- [DevBase.Avalonia](#devbaseavalonia)
- [DevBase.Cryptography and DevBase.Cryptography.BouncyCastle](#devbasecryptography-and-devbasecryptographybouncycastle)
- [DevBase.Format](#devbaseformat)
- [DevBase.Logging](#devbaselogging)

## DevBase.Api

DevBase.Api contains API endpoint implementations for Deezer, OpenAI, OpenLyricsClient, Replicate, and Tidal. The library provides partial implementations that are tailored to meet the needs of the current project.

## DevBase.Avalonia

DevBase.Avalonia features a color calculator for images. The calculator helps determine dominant and accent colors within an image, making it easier to create visually appealing and harmonious designs.

## DevBase.Cryptography and DevBase.Cryptography.BouncyCastle

The DevBase.Cryptography and DevBase.Cryptography.BouncyCastle libraries offer a collection of cryptographic implementations for various use cases. They provide a wide range of cryptographic algorithms and tools, including encryption, decryption, hashing, and digital signatures.

## DevBase.Format

DevBase.Format contains parsers for the following file formats:

- **.env**: Environment files used to store configuration variables for applications, typically key-value pairs.
- **.lrc**: Lyric files used to store song lyrics and timestamps, allowing for synchronization between lyrics and audio playback.
- **.srt**: SubRip Text files used to store subtitles and their associated timestamps for video playback.

## DevBase.Logging

DevBase.Logging is a lightweight logger that makes it easy to log and debug your application. It offers a simple interface for logging messages, errors, and other information at various levels of granularity.

## DevBase.Requests

DevBase.Requests is a high-performance HTTP client library featuring:

- **Builder pattern** for fluent request construction
- **Memory<>/Span<>** optimizations for performance
- **SOCKS5 proxy support** with HTTP tunnel capability
- **Connection pooling** and HTTP/3 support
- **Retry policies** with exponential backoff
- **Request/Response interceptors**

### HttpToSocks5Proxy

The `HttpToSocks5Proxy` component allows using SOCKS5 proxies with any HTTP client that supports `IWebProxy`. It creates a local HTTP proxy server that tunnels traffic through SOCKS5.

```csharp
using DevBase.Requests.Proxy.HttpToSocks5;

// Simple usage
var proxy = new HttpToSocks5Proxy("socks-server.com", 1080);

// With authentication
var proxy = new HttpToSocks5Proxy("socks-server.com", 1080, "username", "password");

// Chain multiple proxies
var proxy = new HttpToSocks5Proxy(new[] {
    new Socks5ProxyInfo("first-proxy.com", 1080),
    new Socks5ProxyInfo("second-proxy.com", 1090)
});

// Use with HttpClient
var handler = new HttpClientHandler { Proxy = proxy };
var client = new HttpClient(handler);
```

## Credits

### HttpToSocks5Proxy

The `HttpToSocks5Proxy` implementation is based on [MihaZupan's HttpToSocks5Proxy](https://github.com/MihaZupan/HttpToSocks5Proxy) library.

**Original Author:** [Miha Zupan](https://github.com/MihaZupan)  
**License:** MIT License  
**Copyright:** © 2018 Miha Zupan

The original library has been integrated and optimized for performance with:
- Async/await patterns throughout
- ArrayPool<byte> for buffer management
- Span<T> for string parsing
- Modern C# features and nullable reference types

## 🌟 Join the DevBase Community and Contribute!

We're excited to see DevBase grow and improve with the help of the community! If you'd like to contribute, we welcome your ideas, bug fixes, and enhancements. Join us in making DevBase even better, and let's build something amazing together!

<!-- GitAds-Verify: VBE4GZBOLA67HUH4HIX2KZCK1C5NKGAL -->
