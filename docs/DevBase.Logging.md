# DevBase.Logging

DevBase.Logging is a lightweight logging utility designed for simple debug output to the Visual Studio Output window (or any listener attached to `System.Diagnostics.Debug`).

## Table of Contents
- [Logger<T>](#loggert)
- [LogType](#logtype)

## Logger<T>
The generic `Logger<T>` class allows you to instantiate a logger bound to a specific context (usually the class where it is used).

**Features:**
- Writes to `System.Diagnostics.Debug`.
- formats output with Timestamp, Class Name, Log Type, and Message.

**Constructor:**
- `Logger(T type)`: Initializes the logger. passing `this` is common practice.

**Methods:**
- `Write(string message, LogType debugType)`: Logs a formatted message.
- `Write(Exception exception)`: Logs an exception message with `LogType.ERROR`.

**Example:**
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
        _logger.Write("Starting work...", LogType.INFO);

        try 
        {
            // ... work
        }
        catch (Exception ex)
        {
            _logger.Write(ex);
        }
    }
}
```

**Output Format:**
```text
14:23:45.1234567 : MyService : INFO : Starting work...
```

## LogType
Enum defining the severity levels of logs.

- `INFO`
- `DEBUG`
- `ERROR`
- `FATAL`
