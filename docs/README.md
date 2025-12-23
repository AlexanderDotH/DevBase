# DevBase Solution Documentation

Welcome to the comprehensive documentation for the DevBase solution. This solution contains a set of libraries designed to provide robust utilities, API clients, and helpers for .NET development.

## Documentation Index

### Core Libraries
- **[DevBase (Core)](DevBase.md)**  
  The foundation of the solution. Contains custom generic collections (`AList`), IO utilities, asynchronous task management, and basic helpers.

- **[DevBase.Extensions](DevBase.Extensions.md)**  
  Extensions for standard .NET types, featuring a robust `Stopwatch` formatter for benchmarking and logging.

- **[DevBase.Logging](DevBase.Logging.md)**  
  A lightweight, context-aware logging utility for debug output.

### Web & APIs
- **[DevBase.Net](DevBase.Net.md)**  
  A high-performance, fluent HTTP client library with support for SOCKS5 proxies, retry policies, and advanced response parsing.

- **[DevBase.Api](DevBase.Api.md)**  
  A collection of ready-to-use API clients for major services including:
  - **Music**: Deezer, Tidal, Apple Music, NetEase, Musixmatch.
  - **AI**: OpenAI, Replicate.
  - **Lyrics**: BeautifulLyrics, OpenLyricsClient.

### Data Formats & Cryptography
- **[DevBase.Format](DevBase.Format.md)**  
  Parsers for various file formats including LRC (Lyrics), SRT (Subtitles), and ENV files.

- **[DevBase.Cryptography](DevBase.Cryptography.md)**  
  Cryptographic implementations including Blowfish, MD5, and modern AES-GCM wrappers via BouncyCastle.

### UI & Graphics
- **[DevBase.Avalonia](DevBase.Avalonia.md)**  
  Utilities for the Avalonia UI framework, specifically focused on image color analysis (Dominant/Accent color extraction) and color space conversions.

### Testing & Tools
- **[DevBaseLive](DevBaseLive.md)**  
  A console application used for functional testing and performance benchmarking of the `DevBase.Net` library.
