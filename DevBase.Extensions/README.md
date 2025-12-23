# DevBase.Extensions

A collection of useful extension methods and utilities for .NET development.

## Features

- **Stopwatch Extensions** - Enhanced stopwatch functionality
- **Console Tables** - Formatted console output
- **Utility Methods** - Common helper functions

## Installation

```xml
<PackageReference Include="DevBase.Extensions" Version="x.x.x" />
```

Or via NuGet CLI:

```bash
dotnet add package DevBase.Extensions
```

## Usage Examples

### Stopwatch Extensions

```csharp
using DevBase.Extensions.Stopwatch;

Stopwatch sw = Stopwatch.StartNew();
// ... some work ...
sw.Stop();

// Get formatted elapsed time
string elapsed = sw.GetFormattedElapsed();
Console.WriteLine($"Elapsed: {elapsed}");
```

### Console Table Output

```csharp
using DevBase.Extensions.Utils;

// Create formatted table output
var data = new[]
{
    new { Name = "John", Age = 30 },
    new { Name = "Jane", Age = 25 }
};

ConsoleTableUtils.PrintTable(data);
```

## Dependencies

- ConsoleTables

## License

MIT License - see LICENSE file for details.
