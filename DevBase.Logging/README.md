# DevBase.Logging

A simple, lightweight logging library for .NET applications.

## Features

- **Simple API** - Easy-to-use logging interface
- **Multiple Log Levels** - Debug, Info, Warning, Error, Fatal
- **Lightweight** - Minimal dependencies
- **Flexible Output** - Console and custom outputs

## Installation

```xml
<PackageReference Include="DevBase.Logging" Version="x.x.x" />
```

Or via NuGet CLI:

```bash
dotnet add package DevBase.Logging
```

## Usage Examples

### Basic Logging

```csharp
using DevBase.Logging.Logger;

Logger logger = new Logger();

// Different log levels
logger.Debug("Debug message");
logger.Info("Information message");
logger.Warning("Warning message");
logger.Error("Error message");
logger.Fatal("Fatal error message");
```

### With Context

```csharp
logger.Info("User logged in", new { UserId = 123, Username = "john" });
logger.Error("Operation failed", exception);
```

### Configure Log Level

```csharp
using DevBase.Logging.Enums;

Logger logger = new Logger(EnumLogLevel.Warning);
// Only Warning, Error, and Fatal will be logged
```

## Log Levels

| Level | Description |
|-------|-------------|
| `Debug` | Detailed debugging information |
| `Info` | General informational messages |
| `Warning` | Warning conditions |
| `Error` | Error conditions |
| `Fatal` | Critical failures |

## API Reference

### Logger Class

| Method | Description |
|--------|-------------|
| `Debug(string)` | Log debug message |
| `Info(string)` | Log info message |
| `Warning(string)` | Log warning message |
| `Error(string)` | Log error message |
| `Fatal(string)` | Log fatal message |

## License

MIT License - see LICENSE file for details.
