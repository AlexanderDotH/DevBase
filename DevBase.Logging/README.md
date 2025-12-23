# DevBase.Logging

**DevBase.Logging** is a lightweight logging library for .NET 9.0 that provides simple, type-safe logging with minimal configuration.

## Features

- **Generic Type Logger** - Type-safe logging with generic type parameter
- **Multiple Log Levels** - INFO, WARNING, ERROR, DEBUG
- **Debug Output** - Writes to Debug console
- **Timestamp Support** - Automatic timestamp inclusion
- **Exception Logging** - Built-in exception handling

## Installation

```bash
dotnet add package DevBase.Logging
```

## Quick Start

```csharp
using DevBase.Logging.Logger;
using DevBase.Logging.Enums;

// Create logger with type
var logger = new Logger<MyClass>(this);

// Log messages
logger.Write("Application started", LogType.INFO);
logger.Write("Configuration loaded", LogType.DEBUG);
logger.Write("Connection timeout", LogType.WARNING);
logger.Write("Database error", LogType.ERROR);

// Log exceptions
try
{
    // ... operation ...
}
catch (Exception ex)
{
    logger.Write(ex);
}
```

## Log Types

```csharp
public enum LogType
{
    INFO,
    WARNING,
    ERROR,
    DEBUG
}
```

## Usage Examples

### Basic Logging

```csharp
using DevBase.Logging.Logger;
using DevBase.Logging.Enums;

public class MyService
{
    private readonly Logger<MyService> _logger;
    
    public MyService()
    {
        _logger = new Logger<MyService>(this);
    }
    
    public void DoWork()
    {
        _logger.Write("Starting work", LogType.INFO);
        
        try
        {
            // Perform work
            _logger.Write("Work in progress", LogType.DEBUG);
        }
        catch (Exception ex)
        {
            _logger.Write(ex);
        }
        
        _logger.Write("Work completed", LogType.INFO);
    }
}
```

### Logging in API Clients

```csharp
using DevBase.Api.Apis.Deezer;
using DevBase.Logging.Logger;
using DevBase.Logging.Enums;

public class MusicService
{
    private readonly Logger<MusicService> _logger;
    private readonly Deezer _deezer;
    
    public MusicService()
    {
        _logger = new Logger<MusicService>(this);
        _deezer = new Deezer();
    }
    
    public async Task<JsonDeezerSearch> SearchAsync(string query)
    {
        _logger.Write($"Searching for: {query}", LogType.INFO);
        
        try
        {
            var results = await _deezer.Search(query);
            _logger.Write($"Found {results.data.Length} results", LogType.DEBUG);
            return results;
        }
        catch (Exception ex)
        {
            _logger.Write(ex);
            return null;
        }
    }
}
```

### Logging HTTP Requests

```csharp
using DevBase.Net.Core;
using DevBase.Logging.Logger;
using DevBase.Logging.Enums;

public class ApiClient
{
    private readonly Logger<ApiClient> _logger;
    
    public ApiClient()
    {
        _logger = new Logger<ApiClient>(this);
    }
    
    public async Task<Response> GetAsync(string url)
    {
        _logger.Write($"GET {url}", LogType.INFO);
        
        try
        {
            var response = await new Request(url).SendAsync();
            
            if (response.IsSuccessStatusCode)
            {
                _logger.Write($"Success: {response.StatusCode}", LogType.DEBUG);
            }
            else
            {
                _logger.Write($"Failed: {response.StatusCode}", LogType.WARNING);
            }
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.Write(ex);
            throw;
        }
    }
}
```

### Application Lifecycle Logging

```csharp
public class Application
{
    private readonly Logger<Application> _logger;
    
    public Application()
    {
        _logger = new Logger<Application>(this);
    }
    
    public void Start()
    {
        _logger.Write("Application starting", LogType.INFO);
        
        try
        {
            InitializeComponents();
            _logger.Write("Components initialized", LogType.DEBUG);
            
            LoadConfiguration();
            _logger.Write("Configuration loaded", LogType.DEBUG);
            
            _logger.Write("Application started successfully", LogType.INFO);
        }
        catch (Exception ex)
        {
            _logger.Write(ex);
            _logger.Write("Application failed to start", LogType.ERROR);
            throw;
        }
    }
    
    public void Stop()
    {
        _logger.Write("Application stopping", LogType.INFO);
        
        try
        {
            CleanupResources();
            _logger.Write("Application stopped", LogType.INFO);
        }
        catch (Exception ex)
        {
            _logger.Write(ex);
        }
    }
}
```

### Performance Monitoring

```csharp
using DevBase.Extensions.Stopwatch;
using DevBase.Logging.Logger;
using DevBase.Logging.Enums;

public class PerformanceMonitor
{
    private readonly Logger<PerformanceMonitor> _logger;
    
    public PerformanceMonitor()
    {
        _logger = new Logger<PerformanceMonitor>(this);
    }
    
    public void MonitorOperation(Action operation, string name)
    {
        _logger.Write($"Starting: {name}", LogType.DEBUG);
        
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            operation();
            stopwatch.Stop();
            
            _logger.Write($"Completed: {name} in {stopwatch.Elapsed}", LogType.INFO);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.Write($"Failed: {name} after {stopwatch.Elapsed}", LogType.ERROR);
            _logger.Write(ex);
            throw;
        }
    }
}
```

### Database Operations

```csharp
public class DatabaseService
{
    private readonly Logger<DatabaseService> _logger;
    
    public DatabaseService()
    {
        _logger = new Logger<DatabaseService>(this);
    }
    
    public async Task<User> GetUserAsync(int id)
    {
        _logger.Write($"Fetching user {id}", LogType.DEBUG);
        
        try
        {
            var user = await database.Users.FindAsync(id);
            
            if (user == null)
            {
                _logger.Write($"User {id} not found", LogType.WARNING);
            }
            else
            {
                _logger.Write($"User {id} retrieved", LogType.DEBUG);
            }
            
            return user;
        }
        catch (Exception ex)
        {
            _logger.Write(ex);
            throw;
        }
    }
}
```

## Log Output Format

```
HH:mm:ss.fffffff : ClassName : LogType : Message
```

Example:
```
14:23:45.1234567 : MyService : INFO : Application started
14:23:45.2345678 : MyService : DEBUG : Configuration loaded
14:23:46.3456789 : MyService : ERROR : Connection failed
```

## Best Practices

1. **Create logger per class** - One logger instance per class
2. **Use appropriate log levels** - INFO for important events, DEBUG for details
3. **Log exceptions** - Always log exceptions with context
4. **Avoid sensitive data** - Don't log passwords, tokens, or PII
5. **Be concise** - Keep log messages short and informative

## Common Patterns

### Pattern 1: Try-Catch Logging

```csharp
try
{
    PerformOperation();
}
catch (Exception ex)
{
    _logger.Write(ex);
    throw; // Re-throw if needed
}
```

### Pattern 2: Conditional Logging

```csharp
if (debugMode)
{
    _logger.Write("Debug information", LogType.DEBUG);
}
```

### Pattern 3: Operation Tracking

```csharp
_logger.Write("Starting operation", LogType.INFO);
// ... operation ...
_logger.Write("Operation completed", LogType.INFO);
```

## Integration with DevBase Libraries

### With DevBase.Net

```csharp
var logger = new Logger<HttpClient>(this);

logger.Write("Sending request", LogType.DEBUG);
var response = await new Request(url).SendAsync();
logger.Write($"Response: {response.StatusCode}", LogType.INFO);
```

### With DevBase.Api

```csharp
var logger = new Logger<MusicClient>(this);

logger.Write("Searching Deezer", LogType.INFO);
var results = await deezer.Search(query);
logger.Write($"Found {results.data.Length} results", LogType.DEBUG);
```

### With DevBase.Format

```csharp
var logger = new Logger<LyricsParser>(this);

logger.Write("Parsing lyrics", LogType.DEBUG);
var lyrics = parser.ParseFromDisk("song.lrc");
logger.Write($"Parsed {lyrics.Length} lines", LogType.INFO);
```

## Limitations

- **Debug output only** - Logs to Debug console, not file or other sinks
- **No configuration** - Fixed format and output
- **No filtering** - All log levels are output
- **No async logging** - Synchronous writes only

## When to Use

**Good for:**
- Simple applications
- Debug logging during development
- Quick prototyping
- Internal tools

**Not recommended for:**
- Production applications requiring file logging
- Applications needing log aggregation
- Systems requiring structured logging
- High-performance scenarios

## Alternatives

For production applications, consider:
- **Serilog** - Structured logging with sinks
- **NLog** - Flexible logging framework
- **Microsoft.Extensions.Logging** - Built-in .NET logging

## Target Framework

- **.NET 9.0**

## Dependencies

- **System.Diagnostics** - Debug output

## License

MIT License - See LICENSE file for details

## Author

AlexanderDotH

## Repository

https://github.com/AlexanderDotH/DevBase
