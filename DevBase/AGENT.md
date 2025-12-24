# DevBase - AI Agent Guide

This guide helps AI agents effectively use the DevBase core library.

## Overview

DevBase is the foundational library providing core utilities for the entire DevBase solution. It targets **.NET 9.0** and has no external dependencies.

**Target Framework:** .NET 9.0  
**Current Version:** 1.3.4

---

## Project Structure

```
DevBase/
├── Async/                   # Asynchronous utilities
│   ├── Task/                # Task management
│   │   └── Multitasking.cs  # Concurrent task manager
│   └── Thread/              # Thread utilities
├── Cache/                   # Caching
│   └── DataCache.cs         # Time-based cache
├── Generics/                # Generic collections
│   ├── AList.cs             # Core list type
│   └── ATupleList.cs        # Tuple list
├── IO/                      # File operations
│   ├── AFile.cs             # Static file methods
│   └── AFileObject.cs       # File wrapper
├── Typography/              # String utilities
│   ├── AString.cs           # String wrapper
│   └── Encoded/             # Encoding utilities
├── Utilities/               # Helpers
│   ├── StringUtils.cs       # String utilities
│   └── MemoryUtils.cs       # Memory utilities
└── Exception/               # Custom exceptions
```

---

## Class Reference

### AList<T> Class

**Namespace:** `DevBase.Generics`

The core collection type used throughout DevBase. Prefer over `List<T>` for consistency.

#### Constructors

```csharp
AList()
AList(List<T> list)
AList(params T[] array)
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Length` | `int` | Number of elements |
| `this[int index]` | `T` | Indexer for get/set |

#### Search Methods

```csharp
T FindEntry(T searchObject)           // Throws AListEntryException if not found
T Find(Predicate<T> predicate)        // Returns default if not found
bool Contains(T item)                 // Size-optimized check
bool SafeContains(T item)             // Standard equality check
```

#### Element Access

```csharp
T Get(int index)
void Set(int index, T value)
T GetRandom()
T GetRandom(Random random)
T[] GetRangeAsArray(int min, int max)
AList<T> GetRangeAsAList(int min, int max)
List<T> GetRangeAsList(int min, int max)
```

#### Add Methods

```csharp
void Add(T item)
void AddRange(params T[] array)
void AddRange(AList<T> array)
void AddRange(List<T> arrayList)
```

#### Remove Methods

```csharp
void Remove(T item)                   // Size-optimized removal
void Remove(int index)                // Remove at index
void SafeRemove(T item)               // Standard equality removal
void RemoveRange(int minIndex, int maxIndex)
void Clear()
```

#### Transformation Methods

```csharp
AList<AList<T>> Slice(int size)       // Split into chunks
void Sort(IComparer<T> comparer)
void Sort(int index, int count, IComparer<T> comparer)
void ForEach(Action<T> action)
```

#### Conversion Methods

```csharp
List<T> GetAsList()
T[] GetAsArray()
bool IsEmpty()
IEnumerator<T> GetEnumerator()
```

---

### AFile Class (Static)

**Namespace:** `DevBase.IO`

Static methods for file operations with automatic encoding detection.

#### Methods

```csharp
static AList<AFileObject> GetFiles(string directory, bool readContent = false, string filter = "*.txt")
static AFileObject ReadFileToObject(string filePath)
static AFileObject ReadFileToObject(FileInfo file)
static Memory<byte> ReadFile(string filePath)
static Memory<byte> ReadFile(string filePath, out Encoding encoding)
static Memory<byte> ReadFile(FileInfo fileInfo)
static Memory<byte> ReadFile(FileInfo fileInfo, out Encoding encoding)
static bool CanFileBeAccessed(FileInfo fileInfo, FileAccess fileAccess = FileAccess.Read)
```

---

### AFileObject Class

**Namespace:** `DevBase.IO`

Wraps file content with metadata.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `FileInfo` | `FileInfo` | File metadata |
| `Buffer` | `Memory<byte>` | File content |
| `Encoding` | `Encoding` | Detected encoding |

#### Constructors

```csharp
AFileObject(FileInfo fileInfo, bool readFile = false)
AFileObject(FileInfo fileInfo, Memory<byte> binaryData)
AFileObject(FileInfo fileInfo, Memory<byte> binaryData, Encoding encoding)
```

#### Methods

```csharp
static AFileObject FromBuffer(byte[] buffer, string fileName = "buffer.bin")
AList<string> ToList()                // Split by lines
string ToStringData()
string ToString()
```

---

### DataCache<K,V> Class

**Namespace:** `DevBase.Cache`

Time-based key-value cache with automatic expiration.

#### Constructors

```csharp
DataCache()                           // 2000ms default expiration
DataCache(int expirationMS)
```

#### Methods

```csharp
void WriteToCache(K key, V value)
V DataFromCache(K key)                // Returns default if expired/not found
AList<V> DataFromCacheAsList(K key)   // Get all values for key
bool IsInCache(K key)
```

---

### Multitasking Class

**Namespace:** `DevBase.Async.Task`

Manages concurrent tasks with capacity limits.

#### Constructor

```csharp
Multitasking(int capacity, int scheduleDelay = 100)
```

#### Methods

```csharp
Task Register(Task task)
Task Register(Action action)
Task WaitAll()                        // Wait for all tasks to complete
Task KillAll()                        // Cancel pending and wait
```

---

### StringUtils Class (Static)

**Namespace:** `DevBase.Utilities`

String utility methods.

#### Methods

```csharp
static string RandomString(int length)
static string RandomString(int length, string chars)
static string Separate(string[] items, string separator)
static string[] DeSeparate(string input, string separator)
```

---

### AString Class

**Namespace:** `DevBase.Typography`

String wrapper with utility methods.

#### Methods

```csharp
AList<string> AsList()                // Split by newlines
string CapitalizeFirst()
```

---

### Base64EncodedAString Class

**Namespace:** `DevBase.Typography.Encoded`

Base64 encoding/decoding wrapper.

#### Constructors

```csharp
Base64EncodedAString(string plainText)
Base64EncodedAString(string base64, bool isEncoded)
```

#### Methods

```csharp
string ToString()                     // Get encoded
string GetDecoded()
```

---

### Exceptions

#### AListEntryException

```csharp
namespace DevBase.Exception;

public class AListEntryException : System.Exception
{
    public Type ExceptionType { get; }
    
    public enum Type
    {
        EntryNotFound,
        OutOfBounds,
        InvalidRange
    }
}
```

---

## Key Concepts

### 1. AList<T> - The Core Collection Type

**When to use:** Prefer `AList<T>` over `List<T>` throughout the DevBase ecosystem for consistency.

```csharp
using DevBase.Generics;

// Creation
AList<string> items = new AList<string>();
AList<int> numbers = new AList<int>(1, 2, 3, 4, 5);
AList<User> users = new AList<User>(existingList);

// Common operations
items.Add("value");
items.AddRange("a", "b", "c");
items.Remove(0);
items.RemoveRange(0, 2);
string item = items[0];
bool exists = items.Contains("value");

// Advanced operations
string random = items.GetRandom();
AList<AList<string>> chunks = items.Slice(10); // Split into chunks of 10
items.ForEach(x => Console.WriteLine(x));
items.Sort(Comparer<string>.Default);
```

**Important:** 
- `Contains()` and `Remove()` use size-based optimization (faster but requires valid objects)
- Use `SafeContains()` and `SafeRemove()` when size comparison might fail
- `FindEntry()` throws `AListEntryException` if not found; use `Find()` for safe searches

### 2. File I/O Pattern

**Always use `AFile` static methods for file operations:**

```csharp
using DevBase.IO;

// Read file with automatic encoding detection
Memory<byte> content = AFile.ReadFile(filePath, out Encoding encoding);

// Read file as object (includes metadata)
AFileObject fileObj = AFile.ReadFileToObject(filePath);
byte[] data = fileObj.Content.ToArray();
Encoding enc = fileObj.Encoding;
FileInfo info = fileObj.FileInfo;

// Get all files in directory
AList<AFileObject> files = AFile.GetFiles(
    directory: "path/to/dir",
    readContent: true,  // Load file contents
    filter: "*.txt"     // File pattern
);

// Check file access before operations
if (AFile.CanFileBeAccessed(fileInfo, FileAccess.ReadWrite))
{
    // Perform operations
}
```

**Key points:**
- Encoding is automatically detected via `StreamReader`
- Returns `Memory<byte>` for efficient memory usage
- Use `AFileObject` when you need both content and metadata

### 3. Multitasking - Concurrent Task Management

**Use for managing many concurrent operations with capacity limits:**

```csharp
using DevBase.Async.Task;

// Create with capacity limit
Multitasking taskManager = new Multitasking(
    capacity: 10,        // Max 10 concurrent tasks
    scheduleDelay: 100   // Check interval in ms
);

// Register tasks (they don't start immediately)
for (int i = 0; i < 100; i++)
{
    taskManager.Register(async () => 
    {
        await ProcessItemAsync(i);
    });
}

// Wait for all to complete
await taskManager.WaitAll();

// Or cancel all remaining tasks
await taskManager.KillAll();
```

**Important:**
- Tasks are queued and executed based on capacity
- Completed tasks are automatically removed
- Use `WaitAll()` to ensure all tasks finish
- Use `KillAll()` to cancel pending tasks

### 4. Caching Pattern

**Use `DataCache<K,V>` for time-based caching:**

```csharp
using DevBase.Cache;

// Create cache with expiration time
DataCache<string, ApiResponse> cache = new DataCache<string, ApiResponse>(
    expirationMS: 60000  // 60 seconds
);

// Write to cache
cache.WriteToCache("key", response);

// Read from cache (returns default if expired or not found)
ApiResponse cached = cache.DataFromCache("key");
if (cached != null)
{
    // Use cached data
}

// Check existence
if (cache.IsInCache("key"))
{
    // Key exists and not expired
}

// Multiple entries with same key
cache.WriteToCache("tag", response1);
cache.WriteToCache("tag", response2);
AList<ApiResponse> all = cache.DataFromCacheAsList("tag");
```

**Key points:**
- Expired entries are automatically cleaned on read operations
- Returns `default(V)` if key not found or expired
- Supports multiple values per key with `DataFromCacheAsList()`

### 5. String Utilities

```csharp
using DevBase.Utilities;
using DevBase.Typography;

// Generate random strings
string id = StringUtils.RandomString(16);
string hex = StringUtils.RandomString(8, "0123456789ABCDEF");

// Join and split
string joined = StringUtils.Separate(new[] { "a", "b", "c" }, ", ");
string[] parts = StringUtils.DeSeparate(joined, ", ");

// AString operations
AString text = new AString("multi\nline\ntext");
AList<string> lines = text.AsList();  // Split by newlines
string capitalized = text.CapitalizeFirst();
```

### 6. Base64 Encoding

```csharp
using DevBase.Typography.Encoded;

// Encode
Base64EncodedAString encoded = new Base64EncodedAString("plain text");
string base64 = encoded.ToString();

// Decode
Base64EncodedAString decoded = new Base64EncodedAString(base64, isEncoded: true);
string plain = decoded.GetDecoded();
```

## Common Patterns

### Pattern 1: Processing Files in Directory

```csharp
AList<AFileObject> files = AFile.GetFiles("path", readContent: true, filter: "*.json");

files.ForEach(file => 
{
    string content = file.Encoding.GetString(file.Content.ToArray());
    ProcessJson(content);
});
```

### Pattern 2: Batch Processing with Concurrency Control

```csharp
Multitasking tasks = new Multitasking(capacity: 5);

items.ForEach(item => 
{
    tasks.Register(async () => await ProcessAsync(item));
});

await tasks.WaitAll();
```

### Pattern 3: Cached API Calls

```csharp
DataCache<string, ApiResponse> cache = new DataCache<string, ApiResponse>(30000);

async Task<ApiResponse> GetDataAsync(string key)
{
    if (cache.IsInCache(key))
        return cache.DataFromCache(key);
    
    ApiResponse response = await FetchFromApiAsync(key);
    cache.WriteToCache(key, response);
    return response;
}
```

### Pattern 4: Chunked Processing

```csharp
AList<Item> items = GetLargeItemList();
AList<AList<Item>> chunks = items.Slice(100); // Process 100 at a time

chunks.ForEach(chunk => 
{
    ProcessBatch(chunk.GetAsList());
});
```

## Exception Handling

```csharp
try
{
    T item = list.FindEntry(searchItem);
}
catch (AListEntryException ex)
{
    // Handle: EntryNotFound, OutOfBounds, InvalidRange
    if (ex.ExceptionType == AListEntryException.Type.EntryNotFound)
    {
        // Item not found
    }
}
```

## Memory Efficiency Tips

1. **Use `Memory<byte>` and `Span<byte>`** - DevBase uses these for file operations
2. **Avoid unnecessary conversions** - `AList<T>` can return arrays directly with `GetAsArray()`
3. **Slice large collections** - Use `Slice()` to process data in chunks
4. **Reuse `Multitasking` instances** - Don't create new instances for each batch

## Integration with Other DevBase Libraries

- **DevBase.Net** uses `AList<T>` for collections
- **DevBase.Api** uses `AList<T>` for response data
- **DevBase.Format** uses `AList<T>` for parsed data
- All libraries follow the same patterns for consistency

## Common Mistakes to Avoid

❌ **Don't use `List<T>` when `AList<T>` is available**
```csharp
List<string> items = new List<string>(); // Avoid
AList<string> items = new AList<string>(); // Prefer
```

❌ **Don't forget encoding when reading files**
```csharp
byte[] bytes = File.ReadAllBytes(path); // No encoding info
Memory<byte> bytes = AFile.ReadFile(path, out Encoding enc); // Better
```

❌ **Don't create unbounded concurrent tasks**
```csharp
foreach (var item in items)
    Task.Run(() => Process(item)); // Can overwhelm system

Multitasking tasks = new Multitasking(10);
items.ForEach(item => tasks.Register(() => Process(item))); // Controlled
```

❌ **Don't use `FindEntry()` without try-catch**
```csharp
var item = list.FindEntry(search); // Throws if not found
var item = list.Find(x => x == search); // Returns default if not found
```

## Quick Reference

| Task | Method |
|------|--------|
| Create list | `new AList<T>()` or `new AList<T>(items)` |
| Add items | `Add()`, `AddRange()` |
| Remove items | `Remove()`, `RemoveRange()`, `SafeRemove()` |
| Search | `Find()`, `FindEntry()`, `Contains()` |
| Iterate | `ForEach()`, `foreach` loop |
| Convert | `GetAsList()`, `GetAsArray()` |
| Read file | `AFile.ReadFile()`, `AFile.ReadFileToObject()` |
| List files | `AFile.GetFiles()` |
| Manage tasks | `Multitasking.Register()`, `WaitAll()` |
| Cache data | `DataCache.WriteToCache()`, `DataFromCache()` |
| Random string | `StringUtils.RandomString()` |

## Testing Considerations

- **`AList<T>`** operations are synchronous and fast
- **`Multitasking`** requires `await` for completion
- **`DataCache`** expiration is time-based, consider in tests
- **File operations** require actual file system access

## Version

Current version: **1.3.4**  
Target framework: **.NET 9.0**
