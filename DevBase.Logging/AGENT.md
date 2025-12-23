# DevBase.Logging - AI Agent Guide

This guide helps AI agents effectively use the DevBase.Logging library for simple logging operations.

## Overview

DevBase.Logging is a lightweight logging library that writes to the Debug console. It's designed for simplicity and ease of use.

**Target Framework:** .NET 9.0

## Core Components

### Logger<T>

```csharp
public class Logger<T>
{
    public Logger(T type);
    public void Write(string message, LogType logType);
    public void Write(Exception exception);
}
```

### LogType Enum

```csharp
public enum LogType
{
    INFO,
    WARNING,
    ERROR,
    DEBUG
}
```

## Usage Patterns for AI Agents

### Pattern 1: Basic Logging

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
        // ... work ...
        _logger.Write("Work completed", LogType.INFO);
    }
}
```

### Pattern 2: Exception Logging

```csharp
try
{
    PerformOperation();
}
catch (Exception ex)
{
    _logger.Write(ex); // Logs as ERROR
}
```

### Pattern 3: Different Log Levels

```csharp
_logger.Write("Application started", LogType.INFO);
_logger.Write("Debug information", LogType.DEBUG);
_logger.Write("Potential issue", LogType.WARNING);
_logger.Write("Critical error", LogType.ERROR);
```

### Pattern 4: API Request Logging

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
            _logger.Write($"Response: {response.StatusCode}", LogType.DEBUG);
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

### Pattern 5: Performance Logging

```csharp
using DevBase.Extensions.Stopwatch;

public void MonitorOperation()
{
    _logger.Write("Starting operation", LogType.INFO);
    
    var stopwatch = Stopwatch.StartNew();
    
    try
    {
        PerformOperation();
        stopwatch.Stop();
        
        _logger.Write($"Completed in {stopwatch.Elapsed}", LogType.INFO);
    }
    catch (Exception ex)
    {
        stopwatch.Stop();
        _logger.Write($"Failed after {stopwatch.Elapsed}", LogType.ERROR);
        _logger.Write(ex);
        throw;
    }
}
```

## Important Concepts

### 1. Logger Instantiation

**Create one logger per class:**

```csharp
// ✅ Correct - one logger per class
public class MyService
{
    private readonly Logger<MyService> _logger;
    
    public MyService()
    {
        _logger = new Logger<MyService>(this);
    }
}

// ❌ Wrong - creating logger each time
public void DoWork()
{
    var logger = new Logger<MyService>(this); // Don't do this
}
```

### 2. Log Output Format

```
HH:mm:ss.fffffff : ClassName : LogType : Message
```

Example:
```
14:23:45.1234567 : MyService : INFO : Application started
```

### 3. Exception Logging

```csharp
// Automatically logs as ERROR with exception message
_logger.Write(exception);

// Equivalent to:
_logger.Write(exception.Message, LogType.ERROR);
```

### 4. Log Levels

| Level | Use Case |
|-------|----------|
| INFO | Important events, milestones |
| DEBUG | Detailed diagnostic information |
| WARNING | Potential issues, non-critical errors |
| ERROR | Errors, exceptions |

## Common Mistakes to Avoid

### ❌ Mistake 1: Creating Logger in Method

```csharp
// Wrong - creates new logger each call
public void DoWork()
{
    var logger = new Logger<MyService>(this);
    logger.Write("Working", LogType.INFO);
}

// Correct - reuse logger instance
private readonly Logger<MyService> _logger;

public MyService()
{
    _logger = new Logger<MyService>(this);
}

public void DoWork()
{
    _logger.Write("Working", LogType.INFO);
}
```

### ❌ Mistake 2: Not Logging Exceptions

```csharp
// Wrong - swallowing exception
try
{
    PerformOperation();
}
catch (Exception ex)
{
    // No logging!
}

// Correct
try
{
    PerformOperation();
}
catch (Exception ex)
{
    _logger.Write(ex);
    throw; // Or handle appropriately
}
```

### ❌ Mistake 3: Wrong Log Level

```csharp
// Wrong - using ERROR for non-errors
_logger.Write("User logged in", LogType.ERROR);

// Correct
_logger.Write("User logged in", LogType.INFO);
```

### ❌ Mistake 4: Logging Sensitive Data

```csharp
// Wrong - logging password
_logger.Write($"User: {username}, Password: {password}", LogType.DEBUG);

// Correct
_logger.Write($"User logged in: {username}", LogType.INFO);
```

## Integration Examples

### With DevBase.Api

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
        _logger.Write($"Searching: {query}", LogType.INFO);
        
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

### With DevBase.Format

```csharp
using DevBase.Format;
using DevBase.Format.Formats.LrcFormat;
using DevBase.Logging.Logger;
using DevBase.Logging.Enums;

public class LyricsService
{
    private readonly Logger<LyricsService> _logger;
    
    public LyricsService()
    {
        _logger = new Logger<LyricsService>(this);
    }
    
    public AList<TimeStampedLyric> ParseLyrics(string lrcContent)
    {
        _logger.Write("Parsing LRC lyrics", LogType.DEBUG);
        
        try
        {
            var parser = new FileParser<LrcFormat, AList<TimeStampedLyric>>();
            var lyrics = parser.ParseFromString(lrcContent);
            
            _logger.Write($"Parsed {lyrics.Length} lines", LogType.INFO);
            return lyrics;
        }
        catch (Exception ex)
        {
            _logger.Write(ex);
            return null;
        }
    }
}
```

### With DevBase.Net

```csharp
using DevBase.Net.Core;
using DevBase.Logging.Logger;
using DevBase.Logging.Enums;

public class HttpService
{
    private readonly Logger<HttpService> _logger;
    
    public HttpService()
    {
        _logger = new Logger<HttpService>(this);
    }
    
    public async Task<string> FetchDataAsync(string url)
    {
        _logger.Write($"Fetching: {url}", LogType.INFO);
        
        try
        {
            var response = await new Request(url).SendAsync();
            
            if (response.IsSuccessStatusCode)
            {
                _logger.Write("Request successful", LogType.DEBUG);
                return await response.GetStringAsync();
            }
            else
            {
                _logger.Write($"Request failed: {response.StatusCode}", LogType.WARNING);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.Write(ex);
            return null;
        }
    }
}
```

## Best Practices

1. **One logger per class** - Create in constructor, store as field
2. **Use appropriate levels** - INFO for events, DEBUG for details, ERROR for exceptions
3. **Log exceptions** - Always log exceptions with `Write(exception)`
4. **Be concise** - Keep messages short and informative
5. **Avoid sensitive data** - Never log passwords, tokens, or PII
6. **Log at boundaries** - Log at method entry/exit for important operations
7. **Include context** - Add relevant information (IDs, names, counts)

## Limitations

- **Debug output only** - No file or other sinks
- **No configuration** - Fixed format
- **No filtering** - All levels output
- **Synchronous** - No async logging

## When to Use

**Good for:**
- Development and debugging
- Simple applications
- Internal tools
- Quick prototyping

**Not for:**
- Production applications
- High-volume logging
- Structured logging needs
- Log aggregation systems

## Quick Reference

| Task | Code |
|------|------|
| Create logger | `new Logger<MyClass>(this)` |
| Log info | `_logger.Write(message, LogType.INFO)` |
| Log debug | `_logger.Write(message, LogType.DEBUG)` |
| Log warning | `_logger.Write(message, LogType.WARNING)` |
| Log error | `_logger.Write(message, LogType.ERROR)` |
| Log exception | `_logger.Write(exception)` |

## Testing Considerations

- Logger writes to Debug output
- Use Debug listeners in tests to capture output
- Consider mocking logger for unit tests
- Test exception logging paths

## Version

Current version: **1.0.0**  
Target framework: **.NET 9.0**

## Dependencies

- System.Diagnostics
