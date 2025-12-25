# DevBaseLive Project Documentation

This document contains all class, method, and field signatures with their corresponding comments for the DevBaseLive project.

## Table of Contents

- [Application Entry Point](#application-entry-point)
  - [Program](#program)
  - [Person](#person)
- [Objects](#objects)
  - [Track](#track)
- [Tracks](#tracks)
  - [TrackMiner](#trackminer)

## Application Entry Point

### Program

```csharp
/// <summary>
/// Entry point class for the DevBaseLive application.
/// </summary>
class Program
{
    /// <summary>
    /// The main entry point of the application.
    /// Demonstrates usage of DevBase networking, logging, and other utilities.
    /// Creates 20 HTTP requests with various configurations including:
    /// - Host checking
    /// - SOCKS5 proxy authentication
    /// - Basic authentication
    /// - Retry policies
    /// - Serilog logging
    /// - Multiple file uploads
    /// - Scraping bypass with Firefox browser profile
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    public static async Task Main(string[] args)
}
```

### Person

```csharp
/// <summary>
/// Represents a person record.
/// </summary>
/// <param name="name">The name of the person.</param>
/// <param name="age">The age of the person.</param>
record Person(string name, int age);
```

## Objects

### Track

```csharp
/// <summary>
/// Represents a music track with basic metadata.
/// </summary>
public class Track
{
    /// <summary>
    /// Gets or sets the title of the track.
    /// </summary>
    public string Title { get; set; }
    
    /// <summary>
    /// Gets or sets the album name.
    /// </summary>
    public string Album { get; set; }
    
    /// <summary>
    /// Gets or sets the duration of the track in seconds (or milliseconds, depending on source).
    /// </summary>
    public int Duration { get; set; }
    
    /// <summary>
    /// Gets or sets the list of artists associated with the track.
    /// </summary>
    public string[] Artists { get; set; }
}
```

## Tracks

### TrackMiner

```csharp
/// <summary>
/// Mines tracks from Tidal using random word generation for search queries.
/// </summary>
public class TrackMiner
{
    private string[] _searchParams;
    private Tidal _tidal;

    /// <summary>
    /// Initializes a new instance of the <see cref="TrackMiner"/> class.
    /// </summary>
    /// <param name="searchParams">The number of random words to generate for search parameters.</param>
    public TrackMiner(int searchParams)
    
    /// <summary>
    /// Finds tracks by searching Tidal with the generated random words.
    /// For each search word, retrieves up to 1000 results and converts them to Track objects.
    /// </summary>
    /// <returns>A list of found tracks.</returns>
    public async Task<AList<Track>> FindTracks()
    
    /// <summary>
    /// Converts a list of JsonTidalArtist objects to an array of artist names.
    /// </summary>
    /// <param name="artists">The list of Tidal artist objects.</param>
    /// <returns>An array of artist names.</returns>
    private string[] ConvertArtists(List<JsonTidalArtist> artists)
}
```

## Project Overview

The DevBaseLive project is a demonstration application that showcases various features of the DevBase framework:

### Key Features Demonstrated

1. **HTTP Client Capabilities** (Program.cs):
   - Proxy support with SOCKS5 authentication
   - Basic authentication
   - Retry policies with configurable maximum retries
   - Host checking configuration
   - Scraping bypass with browser profile emulation
   - Multiple file uploads
   - Custom headers
   - Logging integration with Serilog

2. **API Integration** (TrackMiner.cs):
   - Tidal API integration for music search
   - Random word generation for search queries
   - Data transformation from API responses to domain objects

3. **Data Structures**:
   - Use of AList generic collection from DevBase.Generics
   - Record types for immutable data structures
   - Custom object models for music tracks

### Dependencies
- DevBase.Net for HTTP client functionality
- DevBase.Api for Tidal API integration
- DevBase.IO for file operations
- DevBase.Generics for AList collection
- Serilog for structured logging
- Newtonsoft.Json for JSON manipulation
- CrypticWizard.RandomWordGenerator for random word generation

### Usage Examples

The Program.cs demonstrates a complete HTTP request configuration:

```csharp
Request request = new Request()
    .AsGet()
    .WithHostCheck(new HostCheckConfig())
    .WithProxy(new ProxyInfo("host", port, "username", "password", EnumProxyType.Socks5h))
    .UseBasicAuthentication("user", "pass")
    .WithRetryPolicy(new RetryPolicy() { MaxRetries = 2 })
    .WithLogging(new LoggingConfig() { Logger = logger })
    .WithMultipleFiles(
        ("file1", fileData1),
        ("file2", fileData2)
    )
    .WithScrapingBypass(new ScrapingBypassConfig()
    {
        BrowserProfile = EnumBrowserProfile.Firefox
    })
    .WithUrl("https://example.com");
```

The TrackMiner class shows how to integrate with music APIs and process results:

```csharp
TrackMiner miner = new TrackMiner(10); // Generate 10 random search words
AList<Track> tracks = await miner.FindTracks();
```
