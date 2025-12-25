# DevBase.Logging Project Documentation

This document contains all class, method, and field signatures with their corresponding comments for the DevBase.Logging project.

## Table of Contents

- [Enums](#enums)
  - [LogType](#logtype)
- [Logger](#logger)
  - [Logger&lt;T&gt;](#loggert)

## Enums

### LogType

```csharp
/// <summary>
/// Represents the severity level of a log message.
/// </summary>
public enum LogType
{
    /// <summary>
    /// Informational message, typically used for general application flow.
    /// </summary>
    INFO, 
    
    /// <summary>
    /// Debugging message, used for detailed information during development.
    /// </summary>
    DEBUG, 
    
    /// <summary>
    /// Error message, indicating a failure in a specific operation.
    /// </summary>
    ERROR, 
    
    /// <summary>
    /// Fatal error message, indicating a critical failure that may cause the application to crash.
    /// </summary>
    FATAL
}
```

## Logger

### Logger&lt;T&gt;

```csharp
/// <summary>
/// A generic logger class that provides logging functionality scoped to a specific type context.
/// </summary>
/// <typeparam name="T">The type of the context object associated with this logger.</typeparam>
public class Logger<T>
{
    /// <summary>
    /// The context object used to identify the source of the log messages.
    /// </summary>
    private T _type
         
    /// <summary>
    /// Initializes a new instance of the <see cref="Logger{T}"/> class.
    /// </summary>
    /// <param name="type">The context object associated with this logger instance.</param>
    public Logger(T type)
    
    /// <summary>
    /// Logs an exception with <see cref="LogType.ERROR"/> severity.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    public void Write(Exception exception)
    
    /// <summary>
    /// Logs a message with the specified severity level.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="debugType">The severity level of the log message.</param>
    public void Write(string message, LogType debugType)
    
    /// <summary>
    /// Formats and writes the log message to the debug listeners.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="debugType">The severity level of the log message.</param>
    private void Print(string message, LogType debugType)
}
```
