# DevBase

**DevBase** is a foundational .NET library providing core utilities, generic collections, I/O helpers, async task management, caching, and string manipulation tools. It serves as the base layer for the entire DevBase solution.

## Features

### 🔹 Generic Collections
- **`AList<T>`** - Enhanced array-backed list with optimized operations
- **`ATupleList<K,V>`** - Tuple-based key-value collection
- Memory-efficient operations with size-based comparisons

### 🔹 I/O Operations
- **`AFile`** - File reading with encoding detection
- **`AFileObject`** - File representation with metadata
- **`ADirectory`** - Directory operations and management
- Buffered stream reading with `Memory<byte>` support

### 🔹 Async Task Management
- **`Multitasking`** - Concurrent task scheduler with capacity limits
- **`TaskRegister`** - Task registration and tracking
- Automatic task lifecycle management

### 🔹 Caching
- **`DataCache<K,V>`** - Time-based expiration cache
- Automatic cleanup of expired entries
- Thread-safe operations

### 🔹 Typography & String Utilities
- **`AString`** - String wrapper with enhanced operations
- **`Base64EncodedAString`** - Base64 encoding/decoding
- **`StringUtils`** - Random string generation, separation, and formatting

### 🔹 Utilities
- **`MemoryUtils`** - Memory size calculations
- **`EncodingUtils`** - Encoding detection and conversion
- **`CollectionUtils`** - Collection manipulation helpers

## Installation

```bash
dotnet add package DevBase
```

## Usage Examples

### AList<T> - Enhanced Generic List

```csharp
using DevBase.Generics;

// Create and populate
AList<string> items = new AList<string>();
items.Add("apple");
items.AddRange("banana", "cherry");

// Access elements
string first = items[0];
string random = items.GetRandom();

// Search and filter
string found = items.Find(x => x.StartsWith("b"));
bool contains = items.Contains("apple");

// Slice into chunks
AList<AList<string>> chunks = items.Slice(2);

// Range operations
string[] range = items.GetRangeAsArray(0, 1);
items.RemoveRange(0, 1);

// Iteration
items.ForEach(item => Console.WriteLine(item));

// Sorting
items.Sort(Comparer<string>.Default);
```

### File Operations

```csharp
using DevBase.IO;

// Read file with encoding detection
Memory<byte> content = AFile.ReadFile("path/to/file.txt", out Encoding encoding);

// Read file to object
AFileObject fileObj = AFile.ReadFileToObject("path/to/file.txt");
Console.WriteLine($"Size: {fileObj.FileInfo.Length} bytes");
Console.WriteLine($"Encoding: {fileObj.Encoding}");

// Get all files in directory
AList<AFileObject> files = AFile.GetFiles("path/to/directory", readContent: true, filter: "*.txt");

// Check file accessibility
FileInfo file = new FileInfo("path/to/file.txt");
bool canRead = AFile.CanFileBeAccessed(file, FileAccess.Read);
```

### Multitasking - Concurrent Task Management

```csharp
using DevBase.Async.Task;

// Create task manager with capacity of 5 concurrent tasks
Multitasking taskManager = new Multitasking(capacity: 5, scheduleDelay: 100);

// Register tasks
for (int i = 0; i < 20; i++)
{
    int taskId = i;
    taskManager.Register(() => 
    {
        Console.WriteLine($"Task {taskId} executing");
        Thread.Sleep(1000);
    });
}

// Wait for all tasks to complete
await taskManager.WaitAll();

// Or cancel all tasks
await taskManager.KillAll();
```

### DataCache - Time-Based Caching

```csharp
using DevBase.Cache;

// Create cache with 5-second expiration
DataCache<string, User> userCache = new DataCache<string, User>(expirationMS: 5000);

// Write to cache
userCache.WriteToCache("user123", new User { Name = "John" });

// Read from cache
User user = userCache.DataFromCache("user123");

// Check if in cache
bool exists = userCache.IsInCache("user123");

// Multiple entries with same key
userCache.WriteToCache("tag", new User { Name = "Alice" });
userCache.WriteToCache("tag", new User { Name = "Bob" });
AList<User> users = userCache.DataFromCacheAsList("tag");
```

### String Utilities

```csharp
using DevBase.Utilities;
using DevBase.Typography;

// Generate random string
string random = StringUtils.RandomString(10);
string alphanumeric = StringUtils.RandomString(8, "0123456789ABCDEF");

// Separate and de-separate
string[] items = new[] { "apple", "banana", "cherry" };
string joined = StringUtils.Separate(items, ", "); // "apple, banana, cherry"
string[] split = StringUtils.DeSeparate(joined, ", ");

// AString operations
AString text = new AString("hello world\nline two");
AList<string> lines = text.AsList();
string capitalized = text.CapitalizeFirst(); // "Hello world\nline two"
```

### Base64 Encoding

```csharp
using DevBase.Typography.Encoded;

// Encode string to Base64
Base64EncodedAString encoded = new Base64EncodedAString("Hello World");
string base64 = encoded.ToString();

// Decode from Base64
Base64EncodedAString decoded = new Base64EncodedAString(base64, isEncoded: true);
string original = decoded.GetDecoded();
```

## Key Classes Reference

### Collections
| Class | Description |
|-------|-------------|
| `AList<T>` | Array-backed generic list with enhanced operations |
| `ATupleList<K,V>` | Tuple-based key-value collection |
| `GenericTypeConversion` | Type conversion utilities |

### I/O
| Class | Description |
|-------|-------------|
| `AFile` | Static file operations with encoding detection |
| `AFileObject` | File wrapper with content and metadata |
| `ADirectory` | Directory operations |
| `ADirectoryObject` | Directory wrapper with metadata |

### Async
| Class | Description |
|-------|-------------|
| `Multitasking` | Concurrent task scheduler with capacity control |
| `TaskRegister` | Task registration and tracking |
| `Multithreading` | Thread management utilities |

### Cache
| Class | Description |
|-------|-------------|
| `DataCache<K,V>` | Time-based expiration cache |
| `CacheElement<V>` | Cache entry with expiration timestamp |

### Typography
| Class | Description |
|-------|-------------|
| `AString` | String wrapper with enhanced operations |
| `Base64EncodedAString` | Base64 encoding/decoding |
| `EncodedAString` | Base class for encoded strings |

### Utilities
| Class | Description |
|-------|-------------|
| `StringUtils` | String generation and manipulation |
| `MemoryUtils` | Memory size calculations |
| `EncodingUtils` | Encoding detection and conversion |
| `CollectionUtils` | Collection manipulation helpers |

## Exceptions

| Exception | Description |
|-----------|-------------|
| `AListEntryException` | Thrown for invalid list operations (out of bounds, invalid range, entry not found) |
| `EncodingException` | Thrown for encoding-related errors |
| `ErrorStatementException` | Thrown for general error conditions |

## Performance Considerations

- **`AList<T>`** uses size-based comparison before equality checks for faster lookups
- **`SafeContains()`** and **`SafeRemove()`** skip size checks when needed
- File operations use `BufferedStream` and `Memory<byte>` for efficiency
- **`Multitasking`** limits concurrent tasks to prevent resource exhaustion

## Target Framework

- **.NET 9.0**

## Dependencies

- No external dependencies (pure .NET)

## License

MIT License - See LICENSE file for details

## Author

AlexanderDotH

## Repository

https://github.com/AlexanderDotH/DevBase
