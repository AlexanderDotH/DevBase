# DevBase.Extensions

**DevBase.Extensions** provides extension methods and utilities for .NET 9.0, focusing on enhanced Stopwatch functionality with detailed time breakdowns and formatted output.

## Features

- **Stopwatch Extensions** - Enhanced timing with detailed breakdowns
- **Time Utilities** - Time conversion and formatting helpers
- **Console Table Output** - Pretty-printed time tables in Markdown format

## Installation

```bash
dotnet add package DevBase.Extensions
```

## Stopwatch Extensions

Enhanced stopwatch functionality with detailed time breakdowns.

### Basic Usage

```csharp
using DevBase.Extensions.Stopwatch;
using System.Diagnostics;

var stopwatch = Stopwatch.StartNew();

// ... perform operations ...

stopwatch.Stop();

// Print detailed time table
stopwatch.PrintTimeTable();
```

### Output Example

```
| Data | Unit         |
|------|--------------|
|    2 | Hours        |
|   15 | Minutes      |
|   30 | Seconds      |
|  500 | Milliseconds |
| 1234 | Microseconds |
| 5678 | Nanoseconds  |
```

### Get Time Table as String

```csharp
var stopwatch = Stopwatch.StartNew();
// ... operations ...
stopwatch.Stop();

string timeTable = stopwatch.GetTimeTable();
Console.WriteLine(timeTable);

// Or save to file
File.WriteAllText("timing.md", timeTable);
```

## Time Utilities

Helper methods for time conversions and formatting.

### Get Individual Time Components

```csharp
using DevBase.Extensions.Utils;
using System.Diagnostics;

var stopwatch = Stopwatch.StartNew();
// ... operations ...
stopwatch.Stop();

// Get hours
(int hours, string unit) = TimeUtils.GetHours(stopwatch);
Console.WriteLine($"{hours} {unit}"); // "2 Hours"

// Get minutes
(int minutes, string unit) = TimeUtils.GetMinutes(stopwatch);
Console.WriteLine($"{minutes} {unit}"); // "15 Minutes"

// Get seconds
(int seconds, string unit) = TimeUtils.GetSeconds(stopwatch);
Console.WriteLine($"{seconds} {unit}"); // "30 Seconds"

// Get milliseconds
(int milliseconds, string unit) = TimeUtils.GetMilliseconds(stopwatch);
Console.WriteLine($"{milliseconds} {unit}"); // "500 Milliseconds"

// Get microseconds
(long microseconds, string unit) = TimeUtils.GetMicroseconds(stopwatch);
Console.WriteLine($"{microseconds} {unit}"); // "1234 Microseconds"

// Get nanoseconds
(long nanoseconds, string unit) = TimeUtils.GetNanoseconds(stopwatch);
Console.WriteLine($"{nanoseconds} {unit}"); // "5678 Nanoseconds"
```

## Usage Examples

### Benchmarking Operations

```csharp
using DevBase.Extensions.Stopwatch;
using System.Diagnostics;

public void BenchmarkOperation()
{
    var stopwatch = Stopwatch.StartNew();
    
    // Perform operation
    for (int i = 0; i < 1000000; i++)
    {
        _ = i * 2;
    }
    
    stopwatch.Stop();
    
    Console.WriteLine("Operation completed:");
    stopwatch.PrintTimeTable();
}
```

### Comparing Multiple Operations

```csharp
public void CompareOperations()
{
    // Operation 1
    var sw1 = Stopwatch.StartNew();
    PerformOperation1();
    sw1.Stop();
    
    // Operation 2
    var sw2 = Stopwatch.StartNew();
    PerformOperation2();
    sw2.Stop();
    
    Console.WriteLine("Operation 1:");
    Console.WriteLine(sw1.GetTimeTable());
    
    Console.WriteLine("\nOperation 2:");
    Console.WriteLine(sw2.GetTimeTable());
}
```

### Logging Performance Metrics

```csharp
using DevBase.Extensions.Stopwatch;
using DevBase.Logging.Logger;

public async Task<T> MeasureAsync<T>(Func<Task<T>> operation, string operationName)
{
    var stopwatch = Stopwatch.StartNew();
    
    try
    {
        var result = await operation();
        stopwatch.Stop();
        
        var logger = new Logger<string>("Performance");
        logger.Write($"{operationName} completed:\n{stopwatch.GetTimeTable()}", LogType.INFO);
        
        return result;
    }
    catch (Exception ex)
    {
        stopwatch.Stop();
        Console.WriteLine($"{operationName} failed after {stopwatch.Elapsed}");
        throw;
    }
}
```

### API Response Time Tracking

```csharp
using DevBase.Net.Core;
using DevBase.Extensions.Stopwatch;

public async Task<Response> TimedRequest(string url)
{
    var stopwatch = Stopwatch.StartNew();
    
    var response = await new Request(url).SendAsync();
    
    stopwatch.Stop();
    
    Console.WriteLine($"Request to {url}:");
    stopwatch.PrintTimeTable();
    
    return response;
}
```

### Database Query Performance

```csharp
public async Task<List<User>> GetUsersWithTiming()
{
    var stopwatch = Stopwatch.StartNew();
    
    var users = await database.Users.ToListAsync();
    
    stopwatch.Stop();
    
    Console.WriteLine($"Retrieved {users.Count} users:");
    stopwatch.PrintTimeTable();
    
    return users;
}
```

### File Processing Timing

```csharp
using DevBase.IO;
using DevBase.Extensions.Stopwatch;

public void ProcessFilesWithTiming(string directory)
{
    var stopwatch = Stopwatch.StartNew();
    
    var files = AFile.GetFiles(directory, readContent: true);
    
    foreach (var file in files)
    {
        ProcessFile(file);
    }
    
    stopwatch.Stop();
    
    Console.WriteLine($"Processed {files.Length} files:");
    stopwatch.PrintTimeTable();
}
```

## Advanced Usage

### Custom Time Formatting

```csharp
public string FormatElapsedTime(Stopwatch stopwatch)
{
    var (hours, _) = TimeUtils.GetHours(stopwatch);
    var (minutes, _) = TimeUtils.GetMinutes(stopwatch);
    var (seconds, _) = TimeUtils.GetSeconds(stopwatch);
    var (milliseconds, _) = TimeUtils.GetMilliseconds(stopwatch);
    
    if (hours > 0)
        return $"{hours}h {minutes}m {seconds}s";
    else if (minutes > 0)
        return $"{minutes}m {seconds}s";
    else if (seconds > 0)
        return $"{seconds}s {milliseconds}ms";
    else
        return $"{milliseconds}ms";
}
```

### Performance Threshold Alerts

```csharp
public void CheckPerformance(Stopwatch stopwatch, TimeSpan threshold)
{
    stopwatch.Stop();
    
    if (stopwatch.Elapsed > threshold)
    {
        Console.WriteLine("⚠️ Performance threshold exceeded!");
        stopwatch.PrintTimeTable();
    }
}
```

### Aggregate Timing Statistics

```csharp
public class TimingStats
{
    private List<TimeSpan> timings = new();
    
    public void AddTiming(Stopwatch stopwatch)
    {
        timings.Add(stopwatch.Elapsed);
    }
    
    public void PrintStatistics()
    {
        var avg = TimeSpan.FromTicks((long)timings.Average(t => t.Ticks));
        var min = timings.Min();
        var max = timings.Max();
        
        Console.WriteLine($"Average: {avg}");
        Console.WriteLine($"Min: {min}");
        Console.WriteLine($"Max: {max}");
    }
}
```

## Exception Handling

### StopwatchException

Thrown when trying to get time table from a running stopwatch:

```csharp
using DevBase.Extensions.Exceptions;

try
{
    var stopwatch = Stopwatch.StartNew();
    // Don't stop it
    stopwatch.PrintTimeTable(); // Throws StopwatchException
}
catch (StopwatchException ex)
{
    Console.WriteLine("Stopwatch must be stopped first");
    stopwatch.Stop();
    stopwatch.PrintTimeTable();
}
```

## Integration with Other DevBase Libraries

### With DevBase.Net

```csharp
using DevBase.Net.Core;
using DevBase.Extensions.Stopwatch;

var stopwatch = Stopwatch.StartNew();
var response = await new Request("https://api.example.com").SendAsync();
stopwatch.Stop();

Console.WriteLine("API Request:");
stopwatch.PrintTimeTable();
Console.WriteLine($"Response: {response.StatusCode}");
```

### With DevBase.Api

```csharp
using DevBase.Api.Apis.Deezer;
using DevBase.Extensions.Stopwatch;

var stopwatch = Stopwatch.StartNew();
var deezer = new Deezer();
var results = await deezer.Search("Rick Astley");
stopwatch.Stop();

Console.WriteLine("Deezer Search:");
stopwatch.PrintTimeTable();
```

### With DevBase.Format

```csharp
using DevBase.Format;
using DevBase.Format.Formats.LrcFormat;
using DevBase.Extensions.Stopwatch;

var stopwatch = Stopwatch.StartNew();
var parser = new FileParser<LrcFormat, AList<TimeStampedLyric>>();
var lyrics = parser.ParseFromDisk("song.lrc");
stopwatch.Stop();

Console.WriteLine("LRC Parsing:");
stopwatch.PrintTimeTable();
```

## Performance Tips

1. **Stop before printing** - Always stop stopwatch before calling `GetTimeTable()` or `PrintTimeTable()`
2. **Reuse stopwatch instances** - Use `Restart()` instead of creating new instances
3. **Minimal overhead** - Extension methods have negligible performance impact
4. **High precision** - Uses `Stopwatch.Frequency` for accurate measurements

## Common Patterns

### Pattern 1: Try-Finally Timing

```csharp
var stopwatch = Stopwatch.StartNew();
try
{
    PerformOperation();
}
finally
{
    stopwatch.Stop();
    stopwatch.PrintTimeTable();
}
```

### Pattern 2: Async Operation Timing

```csharp
var stopwatch = Stopwatch.StartNew();
await PerformAsyncOperation();
stopwatch.Stop();
stopwatch.PrintTimeTable();
```

### Pattern 3: Conditional Timing

```csharp
Stopwatch stopwatch = null;
if (enableTiming)
{
    stopwatch = Stopwatch.StartNew();
}

PerformOperation();

if (stopwatch != null)
{
    stopwatch.Stop();
    stopwatch.PrintTimeTable();
}
```

## Target Framework

- **.NET 9.0**

## Dependencies

- **ConsoleTables** - For formatted table output
- **System.Diagnostics** - Stopwatch functionality

## License

MIT License - See LICENSE file for details

## Author

AlexanderDotH

## Repository

https://github.com/AlexanderDotH/DevBase
