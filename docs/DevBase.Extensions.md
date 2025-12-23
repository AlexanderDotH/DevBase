# DevBase.Extensions

DevBase.Extensions provides extension methods to enhance standard .NET types. Currently, it focuses on extensions for `System.Diagnostics.Stopwatch` to provide detailed and formatted execution time reporting.

## Table of Contents
- [Stopwatch Extensions](#stopwatch-extensions)

## Stopwatch Extensions

### StopwatchExtension

Provides extension methods for `System.Diagnostics.Stopwatch` to generate or print a markdown-formatted table of elapsed time, broken down into hours, minutes, seconds, milliseconds, microseconds, and nanoseconds.

**Dependencies:**
- `ConsoleTables`

**Key Methods:**
- `PrintTimeTable()`: Prints the formatted table directly to the Console.
- `GetTimeTable()`: Returns the formatted table as a string.

**Note:** The stopwatch must be stopped before calling these methods, otherwise a `StopwatchException` is thrown.

**Example:**
```csharp
using System.Diagnostics;
using DevBase.Extensions.Stopwatch;

var stopwatch = new Stopwatch();
stopwatch.Start();

// Perform some heavy work
Thread.Sleep(1234);

stopwatch.Stop();

// Print table to console
stopwatch.PrintTimeTable();

// Or get the string for logging
string table = stopwatch.GetTimeTable();
Console.WriteLine(table);
```

**Output Format:**
The output is a markdown table showing only the non-zero time units.

```text
 --------------------------
 | Data | Unit            |
 --------------------------
 |    1 | Second          |
 |  234 | Milliseconds    |
 --------------------------
```
