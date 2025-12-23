# DevBase

A comprehensive .NET development base library providing essential utilities, data structures, and helper classes for everyday development needs.

## Features

- **Generic Collections** - `AList<T>`, `ATupleList<T1,T2>` with enhanced functionality
- **Async Utilities** - Task management, multithreading helpers, and suspension tokens
- **Caching** - Simple in-memory caching with `DataCache<T>`
- **IO Operations** - File and directory abstractions (`AFile`, `ADirectory`)
- **String Handling** - `AString` with encoding support and Base64 utilities
- **Web Utilities** - HTTP request helpers with cookie management
- **Type Utilities** - Generic type conversion and encoding helpers

## Installation

```xml
<PackageReference Include="DevBase" Version="x.x.x" />
```

Or via NuGet CLI:

```bash
dotnet add package DevBase
```

## Usage Examples

### Generic Collections

```csharp
using DevBase.Generics;

// Enhanced list with additional methods
AList<string> list = new AList<string>();
list.Add("item1");
list.Add("item2");

// Tuple list for key-value pairs
ATupleList<string, int> tupleList = new ATupleList<string, int>();
tupleList.Add("key", 42);
```

### Async Task Management

```csharp
using DevBase.Async.Task;

// Task registration and management
TaskRegister register = new TaskRegister();
register.RegisterTask(async () => await DoWorkAsync());

// Multitasking with suspension support
Multitasking tasks = new Multitasking();
tasks.AddTask(myTask);
await tasks.RunAllAsync();
```

### Caching

```csharp
using DevBase.Cache;

DataCache<MyData> cache = new DataCache<MyData>();
cache.Set("key", myData);
MyData? cached = cache.Get("key");
```

### File Operations

```csharp
using DevBase.IO;

// File abstraction
AFile file = new AFile("path/to/file.txt");
string content = file.ReadAllText();

// Directory operations
ADirectory dir = new ADirectory("path/to/dir");
var files = dir.GetFiles();
```

### String Utilities

```csharp
using DevBase.Typography;

AString str = new AString("Hello World");
string result = str.ToLowerCase();

// Base64 encoding
using DevBase.Typography.Encoded;
Base64EncodedAString encoded = new Base64EncodedAString("data");
string base64 = encoded.Encode();
```

### Web Requests

```csharp
using DevBase.Web;

Request request = new Request("https://api.example.com/data");
ResponseData response = await request.GetAsync();
string content = response.Content;
```

## API Reference

### Collections

| Class | Description |
|-------|-------------|
| `AList<T>` | Enhanced generic list |
| `ATupleList<T1,T2>` | List of tuples |
| `GenericTypeConversion` | Type conversion utilities |

### Async

| Class | Description |
|-------|-------------|
| `TaskRegister` | Task registration and management |
| `Multitasking` | Parallel task execution |
| `TaskSuspensionToken` | Task suspension control |
| `AThread` | Thread abstraction |

### IO

| Class | Description |
|-------|-------------|
| `AFile` | File operations wrapper |
| `ADirectory` | Directory operations wrapper |
| `AFileObject` | File metadata |
| `ADirectoryObject` | Directory metadata |

### Utilities

| Class | Description |
|-------|-------------|
| `StringUtils` | String manipulation helpers |
| `EncodingUtils` | Encoding utilities |
| `MemoryUtils` | Memory management helpers |
| `CollectionUtils` | Collection utilities |

## License

MIT License - see LICENSE file for details.
