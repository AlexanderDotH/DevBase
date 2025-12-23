# DevBase.Extensions - AI Agent Guide

This guide helps AI agents effectively use the DevBase.Extensions library for enhanced Stopwatch functionality.

## Overview

DevBase.Extensions provides enhanced timing capabilities with detailed time breakdowns and formatted output.

**Target Framework:** .NET 9.0

## Core Components

### StopwatchExtension

```csharp
public static class StopwatchExtension
{
    public static void PrintTimeTable(this Stopwatch stopwatch);
    public static string GetTimeTable(this Stopwatch stopwatch);
}
```

### TimeUtils

```csharp
public static class TimeUtils
{
    public static (int Hours, string Unit) GetHours(Stopwatch stopwatch);
    public static (int Minutes, string Unit) GetMinutes(Stopwatch stopwatch);
    public static (int Seconds, string Unit) GetSeconds(Stopwatch stopwatch);
    public static (int Milliseconds, string Unit) GetMilliseconds(Stopwatch stopwatch);
    public static (long Microseconds, string Unit) GetMicroseconds(Stopwatch stopwatch);
    public static (long Nanoseconds, string Unit) GetNanoseconds(Stopwatch stopwatch);
}
```

## Usage Patterns for AI Agents

### Pattern 1: Basic Timing

```csharp
using DevBase.Extensions.Stopwatch;
using System.Diagnostics;

var stopwatch = Stopwatch.StartNew();

// Perform operation
PerformOperation();

stopwatch.Stop();

// Print detailed breakdown
stopwatch.PrintTimeTable();
```

### Pattern 2: Get Time Table as String

```csharp
var stopwatch = Stopwatch.StartNew();
await PerformAsyncOperation();
stopwatch.Stop();

string timeTable = stopwatch.GetTimeTable();
Console.WriteLine(timeTable);

// Save to file
File.WriteAllText("timing.md", timeTable);
```

### Pattern 3: Benchmarking

```csharp
public void BenchmarkOperation()
{
    var stopwatch = Stopwatch.StartNew();
    
    for (int i = 0; i < 1000000; i++)
    {
        _ = i * 2;
    }
    
    stopwatch.Stop();
    
    Console.WriteLine("Benchmark results:");
    stopwatch.PrintTimeTable();
}
```

### Pattern 4: API Request Timing

```csharp
using DevBase.Net.Core;
using DevBase.Extensions.Stopwatch;

var stopwatch = Stopwatch.StartNew();
var response = await new Request("https://api.example.com").SendAsync();
stopwatch.Stop();

Console.WriteLine($"Request completed with status {response.StatusCode}:");
stopwatch.PrintTimeTable();
```

### Pattern 5: Individual Time Components

```csharp
using DevBase.Extensions.Utils;

var stopwatch = Stopwatch.StartNew();
// ... operation ...
stopwatch.Stop();

var (hours, unit1) = TimeUtils.GetHours(stopwatch);
var (minutes, unit2) = TimeUtils.GetMinutes(stopwatch);
var (seconds, unit3) = TimeUtils.GetSeconds(stopwatch);
var (milliseconds, unit4) = TimeUtils.GetMilliseconds(stopwatch);

Console.WriteLine($"{hours} {unit1}, {minutes} {unit2}, {seconds} {unit3}, {milliseconds} {unit4}");
```

### Pattern 6: Performance Monitoring

```csharp
public async Task<T> MonitorPerformance<T>(Func<Task<T>> operation, string name)
{
    var stopwatch = Stopwatch.StartNew();
    
    try
    {
        var result = await operation();
        stopwatch.Stop();
        
        Console.WriteLine($"{name} completed:");
        stopwatch.PrintTimeTable();
        
        return result;
    }
    catch (Exception ex)
    {
        stopwatch.Stop();
        Console.WriteLine($"{name} failed after {stopwatch.Elapsed}");
        throw;
    }
}
```

## Important Concepts

### 1. Stopwatch Must Be Stopped

**Critical Rule:** Always stop stopwatch before calling `GetTimeTable()` or `PrintTimeTable()`.

```csharp
// ✅ Correct
var stopwatch = Stopwatch.StartNew();
PerformOperation();
stopwatch.Stop();
stopwatch.PrintTimeTable();

// ❌ Wrong - throws StopwatchException
var stopwatch = Stopwatch.StartNew();
PerformOperation();
stopwatch.PrintTimeTable(); // Exception!
```

### 2. Time Table Format

Output is Markdown-formatted table:

```
| Data | Unit         |
|------|--------------|
|    2 | Hours        |
|   15 | Minutes      |
|   30 | Seconds      |
|  500 | Milliseconds |
```

Only non-zero values are included.

### 3. Time Components

```csharp
// All return tuples: (value, unit)
var (hours, unit) = TimeUtils.GetHours(stopwatch);        // (2, "Hours")
var (minutes, unit) = TimeUtils.GetMinutes(stopwatch);    // (15, "Minutes")
var (seconds, unit) = TimeUtils.GetSeconds(stopwatch);    // (30, "Seconds")
var (ms, unit) = TimeUtils.GetMilliseconds(stopwatch);    // (500, "Milliseconds")
var (us, unit) = TimeUtils.GetMicroseconds(stopwatch);    // (1234, "Microseconds")
var (ns, unit) = TimeUtils.GetNanoseconds(stopwatch);     // (5678, "Nanoseconds")
```

## Common Mistakes to Avoid

### ❌ Mistake 1: Not Stopping Stopwatch

```csharp
// Wrong
var stopwatch = Stopwatch.StartNew();
PerformOperation();
stopwatch.PrintTimeTable(); // Throws StopwatchException!

// Correct
var stopwatch = Stopwatch.StartNew();
PerformOperation();
stopwatch.Stop();
stopwatch.PrintTimeTable();
```

### ❌ Mistake 2: Forgetting to Start Stopwatch

```csharp
// Wrong
var stopwatch = new Stopwatch(); // Not started!
PerformOperation();
stopwatch.Stop();
stopwatch.PrintTimeTable(); // Shows 0 for everything

// Correct
var stopwatch = Stopwatch.StartNew();
PerformOperation();
stopwatch.Stop();
stopwatch.PrintTimeTable();
```

### ❌ Mistake 3: Multiple Stops

```csharp
// Wrong - stopping multiple times
var stopwatch = Stopwatch.StartNew();
PerformOperation1();
stopwatch.Stop();
PerformOperation2();
stopwatch.Stop(); // Doesn't restart timing!

// Correct - use Restart()
var stopwatch = Stopwatch.StartNew();
PerformOperation1();
stopwatch.Stop();
stopwatch.PrintTimeTable();

stopwatch.Restart(); // Restart for next operation
PerformOperation2();
stopwatch.Stop();
stopwatch.PrintTimeTable();
```

### ❌ Mistake 4: Not Handling Exceptions

```csharp
// Wrong - stopwatch never stopped if exception occurs
var stopwatch = Stopwatch.StartNew();
PerformOperation(); // Might throw
stopwatch.Stop();

// Correct - use try-finally
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

## Advanced Usage

### Comparing Multiple Operations

```csharp
public void CompareOperations()
{
    var sw1 = Stopwatch.StartNew();
    Operation1();
    sw1.Stop();
    
    var sw2 = Stopwatch.StartNew();
    Operation2();
    sw2.Stop();
    
    Console.WriteLine("Operation 1:");
    Console.WriteLine(sw1.GetTimeTable());
    
    Console.WriteLine("\nOperation 2:");
    Console.WriteLine(sw2.GetTimeTable());
    
    // Compare
    if (sw1.Elapsed < sw2.Elapsed)
        Console.WriteLine("Operation 1 was faster");
    else
        Console.WriteLine("Operation 2 was faster");
}
```

### Aggregate Statistics

```csharp
public class PerformanceTracker
{
    private List<TimeSpan> timings = new();
    
    public void Track(Action operation)
    {
        var stopwatch = Stopwatch.StartNew();
        operation();
        stopwatch.Stop();
        
        timings.Add(stopwatch.Elapsed);
        stopwatch.PrintTimeTable();
    }
    
    public void PrintStats()
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

### Conditional Timing

```csharp
public void ConditionalTiming(bool enableTiming)
{
    Stopwatch stopwatch = enableTiming ? Stopwatch.StartNew() : null;
    
    PerformOperation();
    
    if (stopwatch != null)
    {
        stopwatch.Stop();
        stopwatch.PrintTimeTable();
    }
}
```

## Integration Examples

### With DevBase.Net

```csharp
using DevBase.Net.Core;
using DevBase.Extensions.Stopwatch;

var stopwatch = Stopwatch.StartNew();
var response = await new Request("https://api.example.com").SendAsync();
stopwatch.Stop();

Console.WriteLine($"Request: {response.StatusCode}");
stopwatch.PrintTimeTable();
```

### With DevBase.Api

```csharp
using DevBase.Api.Apis.Deezer;
using DevBase.Extensions.Stopwatch;

var stopwatch = Stopwatch.StartNew();
var deezer = new Deezer();
var results = await deezer.Search("artist");
stopwatch.Stop();

Console.WriteLine($"Found {results.data.Length} results:");
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

Console.WriteLine($"Parsed {lyrics.Length} lines:");
stopwatch.PrintTimeTable();
```

## Exception Handling

### StopwatchException

```csharp
using DevBase.Extensions.Exceptions;

try
{
    var stopwatch = Stopwatch.StartNew();
    stopwatch.PrintTimeTable(); // Throws!
}
catch (StopwatchException ex)
{
    Console.WriteLine("Must stop stopwatch first");
}
```

## Performance Tips

1. **Minimal overhead** - Extension methods add negligible overhead
2. **High precision** - Uses Stopwatch.Frequency for accuracy
3. **Reuse instances** - Use Restart() instead of creating new stopwatches
4. **Stop before printing** - Always stop before calling GetTimeTable()

## Quick Reference

| Task | Code |
|------|------|
| Start timing | `Stopwatch.StartNew()` |
| Stop timing | `stopwatch.Stop()` |
| Print table | `stopwatch.PrintTimeTable()` |
| Get table string | `stopwatch.GetTimeTable()` |
| Get hours | `TimeUtils.GetHours(stopwatch)` |
| Get minutes | `TimeUtils.GetMinutes(stopwatch)` |
| Get seconds | `TimeUtils.GetSeconds(stopwatch)` |
| Get milliseconds | `TimeUtils.GetMilliseconds(stopwatch)` |
| Get microseconds | `TimeUtils.GetMicroseconds(stopwatch)` |
| Get nanoseconds | `TimeUtils.GetNanoseconds(stopwatch)` |
| Restart | `stopwatch.Restart()` |

## Testing Considerations

- Test with very short operations (< 1ms)
- Test with long operations (> 1 hour)
- Test exception handling during timing
- Test with async operations
- Verify time table formatting

## Version

Current version: **1.0.0**  
Target framework: **.NET 9.0**

## Dependencies

- ConsoleTables (for formatted output)
- System.Diagnostics
