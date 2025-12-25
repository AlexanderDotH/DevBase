# DevBase.Extensions Project Documentation

This document contains all class, method, and field signatures with their corresponding comments for the DevBase.Extensions project.

## Table of Contents

- [Exceptions](#exceptions)
  - [StopwatchException](#stopwatchexception)
- [Stopwatch](#stopwatch)
  - [StopwatchExtension](#stopwatchextension)
- [Utils](#utils)
  - [TimeUtils](#timeutils)

## Exceptions

### StopwatchException

```csharp
/// <summary>
/// Exception thrown when a stopwatch operation is invalid, such as accessing results while it is still running.
/// </summary>
public class StopwatchException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StopwatchException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public StopwatchException(string message)
}
```

## Stopwatch

### StopwatchExtension

```csharp
/// <summary>
/// Provides extension methods for <see cref="System.Diagnostics.Stopwatch"/> to display elapsed time in a formatted table.
/// </summary>
public static class StopwatchExtension
{
    /// <summary>
    /// Prints a markdown formatted table of the elapsed time to the console.
    /// </summary>
    /// <param name="stopwatch">The stopwatch instance.</param>
    public static void PrintTimeTable(this System.Diagnostics.Stopwatch stopwatch)
    
    /// <summary>
    /// Generates a markdown formatted table string of the elapsed time, broken down by time units.
    /// </summary>
    /// <param name="stopwatch">The stopwatch instance.</param>
    /// <returns>A string containing the markdown table of elapsed time.</returns>
    /// <exception cref="StopwatchException">Thrown if the stopwatch is still running.</exception>
    public static string GetTimeTable(this System.Diagnostics.Stopwatch stopwatch)
}
```

## Utils

### TimeUtils

```csharp
/// <summary>
/// Internal utility class for calculating time units from a stopwatch.
/// </summary>
internal class TimeUtils
{
    /// <summary>
    /// Gets the hours component from the stopwatch elapsed time.
    /// </summary>
    /// <param name="stopwatch">The stopwatch instance.</param>
    /// <returns>A tuple containing the value and the unit string (Hour/Hours).</returns>
    public static (int Hours, string Unit) GetHours(System.Diagnostics.Stopwatch stopwatch)
    
    /// <summary>
    /// Gets the minutes component from the stopwatch elapsed time.
    /// </summary>
    /// <param name="stopwatch">The stopwatch instance.</param>
    /// <returns>A tuple containing the value and the unit string (Minute/Minutes).</returns>
    public static (int Minutes, string Unit) GetMinutes(System.Diagnostics.Stopwatch stopwatch)
    
    /// <summary>
    /// Gets the seconds component from the stopwatch elapsed time.
    /// </summary>
    /// <param name="stopwatch">The stopwatch instance.</param>
    /// <returns>A tuple containing the value and the unit string (Second/Seconds).</returns>
    public static (int Seconds, string Unit) GetSeconds(System.Diagnostics.Stopwatch stopwatch)
    
    /// <summary>
    /// Gets the milliseconds component from the stopwatch elapsed time.
    /// </summary>
    /// <param name="stopwatch">The stopwatch instance.</param>
    /// <returns>A tuple containing the value and the unit string (Millisecond/Milliseconds).</returns>
    public static (int Milliseconds, string Unit) GetMilliseconds(System.Diagnostics.Stopwatch stopwatch)
    
    /// <summary>
    /// Calculates the microseconds component from the stopwatch elapsed ticks.
    /// </summary>
    /// <param name="stopwatch">The stopwatch instance.</param>
    /// <returns>A tuple containing the value and the unit string (Microsecond/Microseconds).</returns>
    public static (long Microseconds, string Unit) GetMicroseconds(System.Diagnostics.Stopwatch stopwatch)
    
    /// <summary>
    /// Calculates the nanoseconds component from the stopwatch elapsed ticks.
    /// </summary>
    /// <param name="stopwatch">The stopwatch instance.</param>
    /// <returns>A tuple containing the value and the unit string (Nanosecond/Nanoseconds).</returns>
    public static (long Nanoseconds, string Unit) GetNanoseconds(System.Diagnostics.Stopwatch stopwatch)
}
```
