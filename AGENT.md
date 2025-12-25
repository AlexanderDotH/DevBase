# DevBase Agent Guide

This document helps AI agents work effectively with the DevBase solution.

## Solution Overview

DevBase is a multi-project .NET solution providing utilities, API clients, and helpers. The solution targets **.NET 9.0**.

## Project Structure

```
DevBase/
├── DevBase/                    # Core utilities (AList, IO, async tasks)
├── DevBase.Api/                # API clients (Deezer, Tidal, AppleMusic, etc.)
├── DevBase.Avalonia/           # Avalonia UI utilities (color analysis)
├── DevBase.Cryptography/       # Basic cryptography (Blowfish, MD5)
├── DevBase.Cryptography.BouncyCastle/  # BouncyCastle crypto wrappers
├── DevBase.Extensions/         # .NET type extensions
├── DevBase.Format/             # File format parsers (LRC, SRT, ENV)
├── DevBase.Logging/            # Lightweight logging
├── DevBase.Net/                # HTTP client library (main networking)
├── DevBase.Test/               # Unit tests (NUnit)
├── DevBaseLive/                # Console app for testing
└── docs/                       # Documentation
```

## Key Dependencies Between Projects

```
DevBase.Api → DevBase.Net → DevBase → DevBase.Cryptography
DevBase.Format → DevBase
DevBase.Test → All projects
```

## Common Patterns

### 1. HTTP Requests (DevBase.Net)
```csharp
using DevBase.Net.Core;

Response response = await new Request("https://api.example.com")
    .WithHeader("Authorization", "Bearer token")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .SendAsync();

var data = await response.ParseJsonAsync<MyType>();
```

### 2. API Client Error Handling (DevBase.Api)
All API clients extend `ApiClient` and use the `Throw<T>()` method:
```csharp
if (response.StatusCode != HttpStatusCode.OK)
    return Throw<object>(new MyException(ExceptionType.NotFound));
```

For tuple return types, use `ThrowTuple()`:
```csharp
return ThrowTuple(new MyException(ExceptionType.NotFound));
```

### 3. Generic Collections (DevBase)
Use `AList<T>` instead of `List<T>` for enhanced functionality:
```csharp
AList<string> items = new AList<string>();
items.Add("item");
string first = items.Get(0);
bool isEmpty = items.IsEmpty();
```

## Important Classes

| Project | Key Classes |
|---------|-------------|
| DevBase.Net | `Request`, `Response`, `ProxyInfo`, `RetryPolicy` |
| DevBase.Api | `ApiClient`, `Deezer`, `Tidal`, `AppleMusic`, `NetEase` |
| DevBase | `AList<T>`, `AFile`, `AFileObject` |
| DevBase.Format | `LrcParser`, `SrtParser`, `TimeStampedLyric` |

## Namespaces

- **DevBase.Net.Core** - Request/Response classes
- **DevBase.Net.Proxy** - Proxy support
- **DevBase.Api.Apis.[Service]** - API clients
- **DevBase.Generics** - Generic collections
- **DevBase.Format.Formats** - File parsers

## Testing

Run all tests:
```bash
dotnet test
```

Run specific test class:
```bash
dotnet test --filter "FullyQualifiedName~ClassName"
```

## Building NuGet Packages

Packages are generated on build with `GeneratePackageOnBuild=true`:
```bash
dotnet build -c Release
```

Packages output to: `bin/Release/*.nupkg`

## Tips for AI Agents

1. **Use `DevBase.Net.Core.Request`** for HTTP operations, not `HttpClient` directly
2. **Extend `ApiClient`** when creating new API clients
3. **Use `Throw<T>()`** for error handling in API clients
4. **Use `ThrowTuple()`** for methods returning `ValueTuple` types
5. **Check `StrictErrorHandling`** property for exception behavior
6. **Use `AList<T>`** from DevBase.Generics for collections
7. **External API tests** should handle unavailable services gracefully
8. **README.md files** are included in NuGet packages

## Error Handling Pattern

```csharp
// In API client methods
if (condition_failed)
    return Throw<object>(new MyException(ExceptionType.Reason));

// For tuple returns
if (condition_failed)
    return ThrowTuple(new MyException(ExceptionType.Reason));
```

When `StrictErrorHandling = true`, exceptions are thrown.
When `StrictErrorHandling = false`, default values are returned.

# DevBase Project Documentation

This document contains all class, method, and field signatures with their corresponding comments for the DevBase project.

## Table of Contents

- [Async](#async)
  - [Task](#task)
  - [Thread](#thread)
- [Cache](#cache)
- [Enums](#enums)
- [Exception](#exception)
- [Extensions](#extensions)
- [Generics](#generics)
- [IO](#io)
- [Typography](#typography)
  - [Encoded](#encoded)
- [Utilities](#utilities)

## Async

### Task

#### Multitasking
```csharp
/// <summary>
/// Manages asynchronous tasks execution with capacity limits and scheduling.
/// </summary>
public class Multitasking
{
    private readonly ConcurrentQueue<(Task, CancellationTokenSource)> _parkedTasks;
    private readonly ConcurrentDictionary<Task, CancellationTokenSource> _activeTasks;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly int _capacity;
    private readonly int _scheduleDelay;
    private bool _disposed;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Multitasking"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of concurrent tasks.</param>
    /// <param name="scheduleDelay">The delay between schedule checks in milliseconds.</param>
    public Multitasking(int capacity, int scheduleDelay = 100)
    
    /// <summary>
    /// Waits for all scheduled tasks to complete.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task WaitAll()
    
    /// <summary>
    /// Cancels all tasks and waits for them to complete.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task KillAll()
    
    /// <summary>
    /// Registers a task to be managed.
    /// </summary>
    /// <param name="task">The task to register.</param>
    /// <returns>The registered task.</returns>
    public Task Register(Task task)
    
    /// <summary>
    /// Registers an action as a task to be managed.
    /// </summary>
    /// <param name="action">The action to register.</param>
    /// <returns>The task created from the action.</returns>
    public Task Register(Action action)
}
```

#### TaskActionEntry
```csharp
/// <summary>
/// Represents an entry for a task action with creation options.
/// </summary>
public class TaskActionEntry
{
    private readonly Action _action;
    private readonly TaskCreationOptions _creationOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskActionEntry"/> class.
    /// </summary>
    /// <param name="action">The action to be executed.</param>
    /// <param name="creationOptions">The task creation options.</param>
    public TaskActionEntry(Action action, TaskCreationOptions creationOptions)
    
    /// <summary>
    /// Gets the action associated with this entry.
    /// </summary>
    public Action Action { get; }
    
    /// <summary>
    /// Gets the task creation options associated with this entry.
    /// </summary>
    public TaskCreationOptions CreationOptions { get; }
}
```

#### TaskRegister
```csharp
/// <summary>
/// Registers and manages tasks, allowing for suspension, resumption, and termination by type.
/// </summary>
public class TaskRegister
{
    private readonly ATupleList<TaskSuspensionToken, Object> _suspensionList;
    private readonly ATupleList<System.Threading.Tasks.Task, Object> _taskList;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskRegister"/> class.
    /// </summary>
    public TaskRegister()
    
    /// <summary>
    /// Registers a task created from an action with a specific type.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="type">The type identifier for the task.</param>
    /// <param name="startAfterCreation">Whether to start the task immediately.</param>
    public void RegisterTask(Action action, Object type, bool startAfterCreation = true)
    
    /// <summary>
    /// Registers an existing task with a specific type.
    /// </summary>
    /// <param name="task">The task to register.</param>
    /// <param name="type">The type identifier for the task.</param>
    /// <param name="startAfterCreation">Whether to start the task immediately if not already started.</param>
    public void RegisterTask(System.Threading.Tasks.Task task, Object type, bool startAfterCreation = true)
    
    /// <summary>
    /// Registers a task created from an action and returns a suspension token.
    /// </summary>
    /// <param name="token">The returned suspension token.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="type">The type identifier for the task.</param>
    /// <param name="startAfterCreation">Whether to start the task immediately.</param>
    public void RegisterTask(out TaskSuspensionToken token, Action action, Object type, bool startAfterCreation = true)
    
    /// <summary>
    /// Registers an existing task and returns a suspension token.
    /// </summary>
    /// <param name="token">The returned suspension token.</param>
    /// <param name="task">The task to register.</param>
    /// <param name="type">The type identifier for the task.</param>
    /// <param name="startAfterCreation">Whether to start the task immediately.</param>
    public void RegisterTask(out TaskSuspensionToken token, System.Threading.Tasks.Task task, Object type, bool startAfterCreation = true)
    
    /// <summary>
    /// Generates or retrieves a suspension token for a specific type.
    /// </summary>
    /// <param name="type">The type identifier.</param>
    /// <returns>The suspension token.</returns>
    public TaskSuspensionToken GenerateNewToken(Object type)
    
    /// <summary>
    /// Gets the suspension token associated with a specific type.
    /// </summary>
    /// <param name="type">The type identifier.</param>
    /// <returns>The suspension token.</returns>
    public TaskSuspensionToken GetTokenByType(Object type)
    
    /// <summary>
    /// Gets the suspension token associated with a specific task.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <returns>The suspension token.</returns>
    public TaskSuspensionToken GetTokenByTask(System.Threading.Tasks.Task task)
    
    /// <summary>
    /// Suspends tasks associated with an array of types.
    /// </summary>
    /// <param name="types">The array of types to suspend.</param>
    public void SuspendByArray(Object[] types)
    
    /// <summary>
    /// Suspends tasks associated with the specified types.
    /// </summary>
    /// <param name="types">The types to suspend.</param>
    public void Suspend(params Object[] types)
    
    /// <summary>
    /// Suspends tasks associated with a specific type.
    /// </summary>
    /// <param name="type">The type to suspend.</param>
    public void Suspend(Object type)
    
    /// <summary>
    /// Resumes tasks associated with an array of types.
    /// </summary>
    /// <param name="types">The array of types to resume.</param>
    public void ResumeByArray(Object[] types)
    
    /// <summary>
    /// Resumes tasks associated with the specified types.
    /// </summary>
    /// <param name="types">The types to resume.</param>
    public void Resume(params Object[] types)
    
    /// <summary>
    /// Resumes tasks associated with a specific type.
    /// </summary>
    /// <param name="type">The type to resume.</param>
    public void Resume(Object type)
    
    /// <summary>
    /// Kills (waits for) tasks associated with the specified types.
    /// </summary>
    /// <param name="types">The types to kill.</param>
    public void Kill(params Object[] types)
    
    /// <summary>
    /// Kills (waits for) tasks associated with a specific type.
    /// </summary>
    /// <param name="type">The type to kill.</param>
    public void Kill(Object type)
}
```

#### TaskSuspensionToken
```csharp
/// <summary>
/// A token that allows for suspending and resuming tasks.
/// </summary>
public class TaskSuspensionToken
{
    private readonly SemaphoreSlim _lock;
    private bool _suspended;
    private TaskCompletionSource<bool> _resumeRequestTcs;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskSuspensionToken"/> class.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token source (not currently used in constructor logic but kept for signature).</param>
    public TaskSuspensionToken(CancellationTokenSource cancellationToken)
    
    /// <summary>
    /// Initializes a new instance of the <see cref="TaskSuspensionToken"/> class with a default cancellation token source.
    /// </summary>
    public TaskSuspensionToken()
    
    /// <summary>
    /// Waits for the suspension to be released if currently suspended.
    /// </summary>
    /// <param name="delay">Optional delay before checking.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>A task representing the wait operation.</returns>
    public async System.Threading.Tasks.Task WaitForRelease(int delay = 0, CancellationToken token = default(CancellationToken))
    
    /// <summary>
    /// Suspends the task associated with this token.
    /// </summary>
    public void Suspend()
    
    /// <summary>
    /// Resumes the task associated with this token.
    /// </summary>
    public void Resume()
}
```

### Thread

#### AThread
```csharp
/// <summary>
/// Wrapper class for System.Threading.Thread to add additional functionality.
/// </summary>
[Serializable]
public class AThread
{
    private readonly System.Threading.Thread _thread;
    private bool _startAfterCreation;

    /// <summary>
    /// Constructs a editable thread
    /// </summary>
    /// <param name="t">Delivers a thread object</param>
    public AThread(System.Threading.Thread t)
    
    /// <summary>
    /// Starts a thread with a given condition
    /// </summary>
    /// <param name="condition">A given condition needs to get delivered which is essential to let this method work</param>
    public void StartIf(bool condition)
    
    /// <summary>
    /// Starts a thread with a given condition
    /// </summary>
    /// <param name="condition">A given condition needs to get delivered which is essential to let this method work</param>
    /// <param name="parameters">A parameter can be used to give a thread some start parameters</param>
    public void StartIf(bool condition, object parameters)
    
    /// <summary>
    /// Returns the given Thread
    /// </summary>
    public System.Threading.Thread Thread { get; }
    
    /// <summary>
    /// Changes the StartAfterCreation status of the thread
    /// </summary>
    public bool StartAfterCreation { get; set; }
}
```

#### Multithreading
```csharp
/// <summary>
/// Manages multiple threads, allowing for queuing and capacity management.
/// </summary>
public class Multithreading
{
    private readonly AList<AThread> _threads;
    private readonly ConcurrentQueue<AThread> _queueThreads;
    private readonly int _capacity;

    /// <summary>
    /// Constructs the base of the multithreading system
    /// </summary>
    /// <param name="capacity">Specifies a limit for active working threads</param>
    public Multithreading(int capacity = 10)
    
    /// <summary>
    /// Adds a thread to the ThreadQueue
    /// </summary>
    /// <param name="t">A delivered thread which will be added to the multithreading queue</param>
    /// <param name="startAfterCreation">Specifies if the thread will be started after dequeueing</param>
    /// <returns>The given thread</returns>
    public AThread CreateThread(System.Threading.Thread t, bool startAfterCreation)
    
    /// <summary>
    /// Adds a thread from object AThread to the ThreadQueue
    /// </summary>
    /// <param name="t">A delivered thread which will be added to the multithreading queue</param>
    /// <param name="startAfterCreation">Specifies if the thread will be started after dequeueing</param>
    /// <returns>The given thread</returns>
    public AThread CreateThread(AThread t, bool startAfterCreation)
    
    /// <summary>
    /// Abort all active running threads
    /// </summary>
    public void AbortAll()
    
    /// <summary>
    /// Dequeues all active queue members
    /// </summary>
    public void DequeueAll()
    
    /// <summary>
    /// Returns the capacity
    /// </summary>
    public int Capacity { get; }
    
    /// <summary>
    /// Returns all active threads
    /// </summary>
    public AList<AThread> Threads { get; }
}
```

## Cache

#### CacheElement
```csharp
/// <summary>
/// Represents an element in the cache with a value and an expiration timestamp.
/// </summary>
/// <typeparam name="TV">The type of the value.</typeparam>
[Serializable]
public class CacheElement<TV>
{
    private TV _value;
    private long _expirationDate;

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheElement{TV}"/> class.
    /// </summary>
    /// <param name="value">The value to cache.</param>
    /// <param name="expirationDate">The expiration timestamp in milliseconds.</param>
    public CacheElement(TV value, long expirationDate)
    
    /// <summary>
    /// Gets or sets the cached value.
    /// </summary>
    public TV Value { get; set; }
    
    /// <summary>
    /// Gets or sets the expiration date in Unix milliseconds.
    /// </summary>
    public long ExpirationDate { get; set; }
}
```

#### DataCache
```csharp
/// <summary>
/// A generic data cache implementation with expiration support.
/// </summary>
/// <typeparam name="K">The type of the key.</typeparam>
/// <typeparam name="V">The type of the value.</typeparam>
public class DataCache<K,V>
{
    private readonly int _expirationMS;
    private readonly ATupleList<K, CacheElement<V>> _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataCache{K, V}"/> class.
    /// </summary>
    /// <param name="expirationMS">The cache expiration time in milliseconds.</param>
    public DataCache(int expirationMS)
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DataCache{K, V}"/> class with a default expiration of 2000ms.
    /// </summary>
    public DataCache()
    
    /// <summary>
    /// Writes a value to the cache with the specified key.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to cache.</param>
    public void WriteToCache(K key, V value)
    
    /// <summary>
    /// Retrieves a value from the cache by key.
    /// Returns default(V) if the key is not found or expired.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <returns>The cached value, or default.</returns>
    public V DataFromCache(K key)
    
    /// <summary>
    /// Retrieves all values associated with a key from the cache as a list.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <returns>A list of cached values.</returns>
    public AList<V> DataFromCacheAsList(K key)
    
    /// <summary>
    /// Checks if a key exists in the cache.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <returns>True if the key exists, false otherwise.</returns>
    public bool IsInCache(K key)
}
```

## Enums

#### EnumAuthType
```csharp
/// <summary>
/// Specifies the authentication type.
/// </summary>
public enum EnumAuthType
{
    /// <summary>
    /// OAuth2 authentication.
    /// </summary>
    OAUTH2,
    
    /// <summary>
    /// Basic authentication.
    /// </summary>
    BASIC
}
```

#### EnumCharsetType
```csharp
/// <summary>
/// Specifies the character set type.
/// </summary>
public enum EnumCharsetType
{
    /// <summary>
    /// UTF-8 character set.
    /// </summary>
    UTF8,
    
    /// <summary>
    /// All character sets.
    /// </summary>
    ALL
}
```

#### EnumContentType
```csharp
/// <summary>
/// Specifies the content type of a request or response.
/// </summary>
public enum EnumContentType
{
    /// <summary>
    /// application/json
    /// </summary>
    APPLICATION_JSON,
    
    /// <summary>
    /// application/x-www-form-urlencoded
    /// </summary>
    APPLICATION_FORM_URLENCODED,
    
    /// <summary>
    /// multipart/form-data
    /// </summary>
    MULTIPART_FORMDATA,
    
    /// <summary>
    /// text/plain
    /// </summary>
    TEXT_PLAIN,
    
    /// <summary>
    /// text/html
    /// </summary>
    TEXT_HTML
}
```

#### EnumRequestMethod
```csharp
/// <summary>
/// Specifies the HTTP request method.
/// </summary>
public enum EnumRequestMethod
{
    /// <summary>
    /// HTTP GET method.
    /// </summary>
    GET, 
    
    /// <summary>
    /// HTTP POST method.
    /// </summary>
    POST
}
```

## Exception

#### EncodingException
```csharp
/// <summary>
/// Exception thrown when an encoding error occurs.
/// </summary>
public class EncodingException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EncodingException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public EncodingException(string message) : base(message)
}
```

#### ErrorStatementException
```csharp
/// <summary>
/// Exception thrown when an exception state is not present.
/// </summary>
public class ErrorStatementException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorStatementException"/> class.
    /// </summary>
    public ErrorStatementException() : base("Exception state not present")
}
```

#### AListEntryException
```csharp
/// <summary>
/// Exception thrown for errors related to AList entries.
/// </summary>
public class AListEntryException : SystemException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AListEntryException"/> class.
    /// </summary>
    /// <param name="type">The type of error.</param>
    public AListEntryException(Type type)
    
    /// <summary>
    /// Specifies the type of list entry error.
    /// </summary>
    public enum Type
    {
        /// <summary>Entry not found.</summary>
        EntryNotFound,
        /// <summary>List sizes are not equal.</summary>
        ListNotEqual,
        /// <summary>Index out of bounds.</summary>
        OutOfBounds,
        /// <summary>Invalid range.</summary>
        InvalidRange
    }
}
```

## Extensions

#### AListExtension
```csharp
/// <summary>
/// Provides extension methods for AList.
/// </summary>
public static class AListExtension
{
    /// <summary>
    /// Converts an array to an AList.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="list">The array to convert.</param>
    /// <returns>An AList containing the elements of the array.</returns>
    public static AList<T> ToAList<T>(this T[] list)
}
```

#### Base64EncodedAStringExtension
```csharp
/// <summary>
/// Provides extension methods for Base64 encoding.
/// </summary>
public static class Base64EncodedAStringExtension
{
    /// <summary>
    /// Converts a string to a Base64EncodedAString.
    /// </summary>
    /// <param name="content">The string content to encode.</param>
    /// <returns>A new instance of Base64EncodedAString.</returns>
    public static Base64EncodedAString ToBase64(this string content)
}
```

#### StringExtension
```csharp
/// <summary>
/// Provides extension methods for strings.
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// Repeats a string a specified number of times.
    /// </summary>
    /// <param name="value">The string to repeat.</param>
    /// <param name="amount">The number of times to repeat.</param>
    /// <returns>The repeated string.</returns>
    public static string Repeat(this string value, int amount)
}
```

## Generics

#### AList
```csharp
/// <summary>
/// A generic list implementation with optimized search and manipulation methods.
/// </summary>
/// <typeparam name="T">The type of elements in the list.</typeparam>
public class AList<T> : IEnumerable<T>
{
    private T[] _array;
    
    /// <summary>
    /// Constructs this class with an empty array
    /// </summary>
    public AList()
    
    /// <summary>
    /// Constructs this class and adds items from the given list
    /// </summary>
    /// <param name="list">The list which will be added</param>
    public AList(List<T> list)
    
    /// <summary>
    /// Constructs this class with the given array
    /// </summary>
    /// <param name="array">The given array</param>
    public AList(params T[] array)
    
    /// <summary>
    /// A faster and optimized way to search entries inside this generic list
    ///
    /// It iterates through the list and firstly checks
    /// the size of the object to the corresponding searchObject.
    /// 
    /// </summary>
    /// <param name="searchObject">The object to search for</param>
    /// <returns></returns>
    public T FindEntry(T searchObject)
    
    /// <summary>
    /// Finds an elements by an given predicate
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <returns>The element matching the predicate</returns>
    public T Find(Predicate<T> predicate)
    
    /// <summary>
    /// Iterates through the list and executes an action
    /// </summary>
    /// <param name="action">The action</param>
    public void ForEach(Action<T> action)
    
    /// <summary>
    /// Sorts this list with an comparer
    /// </summary>
    /// <param name="comparer">The given comparer</param>
    public void Sort(IComparer<T> comparer)
    
    /// <summary>
    /// Sorts this list with an comparer
    /// </summary>
    /// <param name="comparer">The given comparer</param>
    public void Sort(int index, int count, IComparer<T> comparer)
    
    /// <summary>
    /// Checks if this list contains a given item
    /// </summary>
    /// <param name="item">The given item</param>
    /// <returns>True if the item is in the list. False if the item is not in the list</returns>
    public bool Contains(T item)
    
    /// <summary>
    /// Returns a random object from the array 
    /// </summary>
    /// <returns>A random object</returns>
    public T GetRandom()
    
    /// <summary>
    /// Returns a random object from the array with an given random number generator
    /// </summary>
    /// <returns>A random object</returns>
    public T GetRandom(Random random)
    
    /// <summary>
    /// This function slices the list into smaller given pieces.
    /// </summary>
    /// <param name="size">Is the size of the chunks inside the list</param>
    /// <returns>A freshly sliced list</returns>
    public AList<AList<T>> Slice(int size)
    
    /// <summary>
    /// Checks if this list contains a given item
    /// </summary>
    /// <param name="item">The given item</param>
    /// <returns>True if the item is in the list. False if the item is not in the list</returns>
    public bool SafeContains(T item)
    
    /// <summary>
    /// Gets and sets the items with an given index
    /// </summary>
    /// <param name="index">The given index</param>
    /// <returns>A requested item based on the index</returns>
    public T this[int index] { get; set; }
    
    /// <summary>
    /// Gets an T type from an given index
    /// </summary>
    /// <param name="index">The index of the array</param>
    /// <returns>A T-Object from the given index</returns>
    public T Get(int index)
    
    /// <summary>
    /// Sets the value at a given index
    /// </summary>
    /// <param name="index">The given index</param>
    /// <param name="value">The given value</param>
    public void Set(int index, T value)
    
    /// <summary>
    /// Clears the list
    /// </summary>
    public void Clear()
    
    /// <summary>
    /// Gets a range of item as array
    /// </summary>
    /// <param name="min">The minimum range</param>
    /// <param name="max">The maximum range</param>
    /// <returns>An array of type T from the given range</returns>
    /// <exception cref="AListEntryException">When the min value is bigger than the max value</exception>
    public T[] GetRangeAsArray(int min, int max)
    
    /// <summary>
    /// Gets a range of items as AList.
    /// </summary>
    /// <param name="min">The minimum index.</param>
    /// <param name="max">The maximum index.</param>
    /// <returns>An AList of items in the range.</returns>
    public AList<T> GetRangeAsAList(int min, int max)
    
    /// <summary>
    /// Gets a range of item as list
    /// </summary>
    /// <param name="min">The minimum range</param>
    /// <param name="max">The maximum range</param>
    /// <returns>An array of type T from the given range</returns>
    /// <exception cref="AListEntryException">When the min value is bigger than the max value</exception>
    public List<T> GetRangeAsList(int min, int max)
    
    /// <summary>
    /// Adds an item to the array by creating a new array and the new item to it.
    /// </summary>
    /// <param name="item">The new item</param>
    public void Add(T item)
    
    /// <summary>
    /// Adds an array of T values to this collection.
    /// </summary>
    /// <param name="array"></param>
    public void AddRange(params T[] array)
    
    /// <summary>
    /// Adds an array of T values to the array
    /// </summary>
    /// <param name="array">The given array</param>
    public void AddRange(AList<T> array)
    
    /// <summary>
    /// Adds a list if T values to the array
    /// </summary>
    /// <param name="arrayList">The given list</param>
    public void AddRange(List<T> arrayList)
    
    /// <summary>
    /// Removes an item of the array with an given item as type
    /// </summary>
    /// <param name="item">The given item which will be removed</param>
    public void Remove(T item)
    
    /// <summary>
    /// Removes an entry without checking the size before identifying it
    /// </summary>
    /// <param name="item">The item which will be deleted</param>
    public void SafeRemove(T item)
    
    /// <summary>
    /// Removes an item of this list at an given index
    /// </summary>
    /// <param name="index">The given index</param>
    public void Remove(int index)
    
    /// <summary>
    /// Removes items in an given range
    /// </summary>
    /// <param name="min">Minimum range</param>
    /// <param name="max">Maximum range</param>
    /// <exception cref="AListEntryException">Throws if the range is invalid</exception>
    public void RemoveRange(int minIndex, int maxIndex)
    
    /// <summary>
    /// Converts this Generic list array to an List<T>
    /// </summary>
    /// <returns></returns>
    public List<T> GetAsList()
    
    /// <summary>
    /// Returns the internal array for this list
    /// </summary>
    /// <returns>An array from type T</returns>
    public T[] GetAsArray()
    
    /// <summary>
    /// Is empty check
    /// </summary>
    /// <returns>True, if this list is empty, False if not</returns>
    public bool IsEmpty()
    
    /// <summary>
    /// Returns the length of this list
    /// </summary>
    public int Length { get; }
    
    public IEnumerator<T> GetEnumerator()
    IEnumerator IEnumerable.GetEnumerator()
}
```

#### ATupleList
```csharp
/// <summary>
/// A generic list of tuples with specialized search methods.
/// </summary>
/// <typeparam name="T1">The type of the first item in the tuple.</typeparam>
/// <typeparam name="T2">The type of the second item in the tuple.</typeparam>
public class ATupleList<T1, T2> : AList<Tuple<T1, T2>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ATupleList{T1, T2}"/> class.
    /// </summary>
    public ATupleList()
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ATupleList{T1, T2}"/> class by copying elements from another list.
    /// </summary>
    /// <param name="list">The list to copy.</param>
    public ATupleList(ATupleList<T1, T2> list)
    
    /// <summary>
    /// Adds a range of items from another ATupleList.
    /// </summary>
    /// <param name="anotherList">The list to add items from.</param>
    public void AddRange(ATupleList<T1, T2> anotherList)
    
    /// <summary>
    /// Finds the full tuple entry where the first item matches the specified value.
    /// </summary>
    /// <param name="t1">The value of the first item to search for.</param>
    /// <returns>The matching tuple, or null if not found.</returns>
    public Tuple<T1, T2> FindFullEntry(T1 t1)
    
    /// <summary>
    /// Finds the full tuple entry where the second item matches the specified value.
    /// </summary>
    /// <param name="t2">The value of the second item to search for.</param>
    /// <returns>The matching tuple, or null if not found.</returns>
    public Tuple<T1, T2> FindFullEntry(T2 t2)
    
    /// <summary>
    /// Finds the second item of the tuple where the first item matches the specified value.
    /// </summary>
    /// <param name="t1">The value of the first item to search for.</param>
    /// <returns>The second item of the matching tuple, or null if not found.</returns>
    public dynamic FindEntry(T1 t1)
    
    /// <summary>
    /// Finds the first item of the tuple where the second item matches the specified value.
    /// </summary>
    /// <param name="t2">The value of the second item to search for.</param>
    /// <returns>The first item of the matching tuple, or null if not found.</returns>
    public dynamic FindEntry(T2 t2)
    
    /// <summary>
    /// Finds the second item of the tuple where the first item equals the specified value (without size check).
    /// </summary>
    /// <param name="t1">The value of the first item to search for.</param>
    /// <returns>The second item of the matching tuple, or null if not found.</returns>
    public dynamic FindEntrySafe(T1 t1)
    
    /// <summary>
    /// Finds the first item of the tuple where the second item equals the specified value (without size check).
    /// </summary>
    /// <param name="t2">The value of the second item to search for.</param>
    /// <returns>The first item of the matching tuple, or null if not found.</returns>
    public dynamic FindEntrySafe(T2 t2)
    
    /// <summary>
    /// Finds all full tuple entries where the second item matches the specified value.
    /// </summary>
    /// <param name="t2">The value of the second item to search for.</param>
    /// <returns>A list of matching tuples.</returns>
    public AList<Tuple<T1, T2>> FindFullEntries(T2 t2)
    
    /// <summary>
    /// Finds all full tuple entries where the first item matches the specified value.
    /// </summary>
    /// <param name="t1">The value of the first item to search for.</param>
    /// <returns>A list of matching tuples.</returns>
    public AList<Tuple<T1, T2>> FindFullEntries(T1 t1)
    
    /// <summary>
    /// Finds all first items from tuples where the second item matches the specified value.
    /// </summary>
    /// <param name="t2">The value of the second item to search for.</param>
    /// <returns>A list of matching first items.</returns>
    public AList<T1> FindEntries(T2 t2)
    
    /// <summary>
    /// Finds all second items from tuples where the first item matches the specified value.
    /// </summary>
    /// <param name="t1">The value of the first item to search for.</param>
    /// <returns>A list of matching second items.</returns>
    public AList<T2> FindEntries(T1 t1)
    
    /// <summary>
    /// Adds a new tuple with the specified values to the list.
    /// </summary>
    /// <param name="t1">The first item.</param>
    /// <param name="t2">The second item.</param>
    public void Add(T1 t1, T2 t2)
}
```

#### GenericTypeConversion
```csharp
/// <summary>
/// Provides functionality to convert and merge lists of one type into another using a conversion action.
/// </summary>
/// <typeparam name="F">The source type.</typeparam>
/// <typeparam name="T">The target type.</typeparam>
public class GenericTypeConversion<F, T>
{
    /// <summary>
    /// Merges an AList of type F into an AList of type T using the provided action.
    /// </summary>
    /// <param name="inputList">The source list.</param>
    /// <param name="action">The action to perform conversion and addition to the target list.</param>
    /// <returns>The resulting list of type T.</returns>
    public AList<T> MergeToList(AList<F> inputList, Action<F, AList<T>> action)
    
    /// <summary>
    /// Merges a List of type F into an AList of type T using the provided action.
    /// </summary>
    /// <param name="inputList">The source list.</param>
    /// <param name="action">The action to perform conversion and addition to the target list.</param>
    /// <returns>The resulting list of type T.</returns>
    public AList<T> MergeToList(List<F> inputList, Action<F, AList<T>> action)
}
```

## IO

#### ADirectory
```csharp
/// <summary>
/// Provides utility methods for directory operations.
/// </summary>
public class ADirectory
{
    /// <summary>
    /// Gets a list of directory objects from a specified path.
    /// </summary>
    /// <param name="directory">The root directory path.</param>
    /// <param name="filter">The search filter string.</param>
    /// <returns>A list of directory objects.</returns>
    /// <exception cref="SystemException">Thrown if the directory does not exist.</exception>
    public static List<ADirectoryObject> GetDirectories(string directory, string filter = "*.*")
}
```

#### ADirectoryObject
```csharp
/// <summary>
/// Represents a directory object wrapper around DirectoryInfo.
/// </summary>
public class ADirectoryObject
{
    private readonly DirectoryInfo _directoryInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="ADirectoryObject"/> class.
    /// </summary>
    /// <param name="directoryInfo">The DirectoryInfo object.</param>
    public ADirectoryObject(DirectoryInfo directoryInfo)
    
    /// <summary>
    /// Gets the underlying DirectoryInfo.
    /// </summary>
    public DirectoryInfo GetDirectoryInfo { get; }
}
```

#### AFile
```csharp
/// <summary>
/// Provides static utility methods for file operations.
/// </summary>
public static class AFile
{
    /// <summary>
    /// Gets a list of files in a directory matching the specified filter.
    /// </summary>
    /// <param name="directory">The directory to search.</param>
    /// <param name="readContent">Whether to read the content of each file.</param>
    /// <param name="filter">The file filter pattern.</param>
    /// <returns>A list of AFileObject representing the files.</returns>
    /// <exception cref="DirectoryNotFoundException">Thrown if the directory does not exist.</exception>
    public static AList<AFileObject> GetFiles(string directory, bool readContent = false, string filter = "*.txt")
    
    /// <summary>
    /// Reads a file and returns an AFileObject containing its data.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <returns>The AFileObject with file data.</returns>
    public static AFileObject ReadFileToObject(string filePath)
    
    /// <summary>
    /// Reads a file and returns an AFileObject containing its data.
    /// </summary>
    /// <param name="file">The FileInfo of the file.</param>
    /// <returns>The AFileObject with file data.</returns>
    public static AFileObject ReadFileToObject(FileInfo file)
    
    /// <summary>
    /// Reads the content of a file into a memory buffer.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <returns>The file content as a memory buffer.</returns>
    public static Memory<byte> ReadFile(string filePath)
    
    /// <summary>
    /// Reads the content of a file into a memory buffer and detects its encoding.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <param name="encoding">The detected encoding.</param>
    /// <returns>The file content as a memory buffer.</returns>
    public static Memory<byte> ReadFile(string filePath, out Encoding encoding)
    
    /// <summary>
    /// Reads the content of a file into a memory buffer and detects its encoding.
    /// </summary>
    /// <param name="fileInfo">The FileInfo of the file.</param>
    /// <returns>The file content as a memory buffer.</returns>
    public static Memory<byte> ReadFile(FileInfo fileInfo)
    
    /// <summary>
    /// Reads the content of a file into a memory buffer and detects its encoding.
    /// </summary>
    /// <param name="fileInfo">The FileInfo of the file.</param>
    /// <param name="encoding">The detected encoding.</param>
    /// <returns>The file content as a memory buffer.</returns>
    /// <exception cref="IOException">Thrown if the file cannot be fully read.</exception>
    public static Memory<byte> ReadFile(FileInfo fileInfo, out Encoding encoding)
    
    /// <summary>
    /// Checks if a file can be accessed with the specified access rights.
    /// </summary>
    /// <param name="fileInfo">The FileInfo of the file.</param>
    /// <param name="fileAccess">The requested file access.</param>
    /// <returns>True if the file can be accessed, false otherwise.</returns>
    public static bool CanFileBeAccessed(FileInfo fileInfo, FileAccess fileAccess = FileAccess.Read)
}
```

#### AFileObject
```csharp
/// <summary>
/// Represents a file object including its info, content buffer, and encoding.
/// </summary>
public class AFileObject
{
    /// <summary>
    /// Gets or sets the file info.
    /// </summary>
    public FileInfo FileInfo { get; protected set; }
    
    /// <summary>
    /// Gets or sets the memory buffer of the file content.
    /// </summary>
    public Memory<byte> Buffer { get; protected set; }
    
    /// <summary>
    /// Gets or sets the encoding of the file content.
    /// </summary>
    public Encoding Encoding { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AFileObject"/> class.
    /// </summary>
    /// <param name="fileInfo">The file info.</param>
    /// <param name="readFile">Whether to read the file content immediately.</param>
    public AFileObject(FileInfo fileInfo, bool readFile = false)
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AFileObject"/> class with existing data.
    /// Detects encoding from binary data.
    /// </summary>
    /// <param name="fileInfo">The file info.</param>
    /// <param name="binaryData">The binary data.</param>
    public AFileObject(FileInfo fileInfo, Memory<byte> binaryData)
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AFileObject"/> class with existing data and encoding.
    /// </summary>
    /// <param name="fileInfo">The file info.</param>
    /// <param name="binaryData">The binary data.</param>
    /// <param name="encoding">The encoding.</param>
    public AFileObject(FileInfo fileInfo, Memory<byte> binaryData, Encoding encoding)
    
    /// <summary>
    /// Creates an AFileObject from a byte buffer.
    /// </summary>
    /// <param name="buffer">The byte buffer.</param>
    /// <param name="fileName">The mock file name.</param>
    /// <returns>A new AFileObject.</returns>
    public static AFileObject FromBuffer(byte[] buffer, string fileName = "buffer.bin")
    
    /// <summary>
    /// Converts the file content to a list of strings (lines).
    /// </summary>
    /// <returns>An AList of strings.</returns>
    public AList<string> ToList()
    
    /// <summary>
    /// Decodes the buffer to a string using the stored encoding.
    /// </summary>
    /// <returns>The decoded string.</returns>
    public string ToStringData()
    
    /// <summary>
    /// Returns the string representation of the file data.
    /// </summary>
    /// <returns>The file data as string.</returns>
    public override string ToString()
}
```

## Typography

#### AString
```csharp
/// <summary>
/// Represents a string wrapper with utility methods.
/// </summary>
public class AString
{
    protected string _value;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AString"/> class.
    /// </summary>
    /// <param name="value">The string value.</param>
    public AString(string value)
    
    /// <summary>
    /// Converts the string to a list of lines.
    /// </summary>
    /// <returns>An AList of lines.</returns>
    public AList<string> AsList()
    
    /// <summary>
    /// Capitalizes the first letter of the string.
    /// </summary>
    /// <returns>The string with the first letter capitalized.</returns>
    public string CapitalizeFirst()
    
    /// <summary>
    /// Returns the string value.
    /// </summary>
    /// <returns>The string value.</returns>
    public override string ToString()
}
```

### Encoded

#### EncodedAString
```csharp
/// <summary>
/// Abstract base class for encoded strings.
/// </summary>
public abstract class EncodedAString : AString
{
    /// <summary>
    /// Gets the decoded AString.
    /// </summary>
    /// <returns>The decoded AString.</returns>
    public abstract AString GetDecoded()

    /// <summary>
    /// Checks if the string is properly encoded.
    /// </summary>
    /// <returns>True if encoded, false otherwise.</returns>
    public abstract bool IsEncoded()
    
    /// <summary>
    /// Initializes a new instance of the <see cref="EncodedAString"/> class.
    /// </summary>
    /// <param name="value">The encoded string value.</param>
    protected EncodedAString(string value)
}
```

#### Base64EncodedAString
```csharp
/// <summary>
/// Represents a Base64 encoded string.
/// </summary>
public class Base64EncodedAString : EncodedAString
{
    private static Regex ENCODED_REGEX_BASE64;
    private static Regex DECODED_REGEX_BASE64;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Base64EncodedAString"/> class.
    /// Validates and pads the input value.
    /// </summary>
    /// <param name="value">The base64 encoded string.</param>
    /// <exception cref="EncodingException">Thrown if the string is not a valid base64 string.</exception>
    public Base64EncodedAString(string value)
    
    /// <summary>
    /// Decodes the URL-safe Base64 string to standard Base64.
    /// </summary>
    /// <returns>A new Base64EncodedAString instance.</returns>
    public Base64EncodedAString UrlDecoded()
    
    /// <summary>
    /// Encodes the Base64 string to URL-safe Base64.
    /// </summary>
    /// <returns>A new Base64EncodedAString instance.</returns>
    public Base64EncodedAString UrlEncoded()
    
    /// <summary>
    /// Decodes the Base64 string to plain text using UTF-8 encoding.
    /// </summary>
    /// <returns>An AString containing the decoded value.</returns>
    public override AString GetDecoded()
    
    /// <summary>
    /// Decodes the Base64 string to a byte array.
    /// </summary>
    /// <returns>The decoded byte array.</returns>
    public byte[] GetDecodedBuffer()
    
    /// <summary>
    /// Gets the raw string value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Checks if the string is a valid Base64 encoded string.
    /// </summary>
    /// <returns>True if encoded correctly, otherwise false.</returns>
    public override bool IsEncoded()
}
```

## Utilities

#### CollectionUtils
```csharp
/// <summary>
/// Provides utility methods for collections.
/// </summary>
public class CollectionUtils
{
    /// <summary>
    /// Appends to every item inside this list a given item of the other list
    ///
    /// List sizes should be equal or it throws
    /// <see cref="AListEntryException"/>
    /// </summary>
    /// <param name="first">The first list.</param>
    /// <param name="second">The second list to merge with.</param>
    /// <param name="marker">The separator string between merged items.</param>
    /// <returns>Returns a new list with the merged entries</returns>
    public static AList<string> MergeList(List<string> first, List<string> second, string marker = "")
}
```

#### EncodingUtils
```csharp
/// <summary>
/// Provides utility methods for encoding detection.
/// </summary>
public static class EncodingUtils
{
    /// <summary>
    /// Detects the encoding of a byte buffer.
    /// </summary>
    /// <param name="buffer">The memory buffer.</param>
    /// <returns>The detected encoding.</returns>
    public static Encoding GetEncoding(Memory<byte> buffer)
    
    /// <summary>
    /// Detects the encoding of a byte buffer.
    /// </summary>
    /// <param name="buffer">The read-only span buffer.</param>
    /// <returns>The detected encoding.</returns>
    public static Encoding GetEncoding(ReadOnlySpan<byte> buffer)
    
    /// <summary>
    /// Detects the encoding of a byte array using a StreamReader.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <returns>The detected encoding.</returns>
    public static Encoding GetEncoding(byte[] buffer)
}
```

#### MemoryUtils
```csharp
/// <summary>
/// Provides utility methods for memory and serialization operations.
/// </summary>
public class MemoryUtils
{
    /// <summary>
    /// Calculates the approximate size of an object in bytes using serialization.
    /// Returns 0 if serialization is not allowed or object is null.
    /// </summary>
    /// <param name="obj">The object to measure.</param>
    /// <returns>The size in bytes.</returns>
    public static long GetSize(Object obj)
    
    /// <summary>
    /// Reads a stream and converts it to a byte array.
    /// </summary>
    /// <param name="input">The input stream.</param>
    /// <returns>The byte array containing the stream data.</returns>
    public static byte[] StreamToByteArray(Stream input)
}
```

#### StringUtils
```csharp
/// <summary>
/// Provides utility methods for string manipulation.
/// </summary>
public class StringUtils
{
    private static readonly Random _random = new Random();

    protected StringUtils() { }
    
    /// <summary>
    /// Generates a random string of a specified length using a given charset.
    /// </summary>
    /// <param name="length">The length of the random string.</param>
    /// <param name="charset">The characters to use for generation.</param>
    /// <returns>A random string.</returns>
    public static string RandomString(int length, string charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz")
    
    /// <summary>
    /// Joins list elements into a single string using a separator.
    /// </summary>
    /// <param name="elements">The list of strings.</param>
    /// <param name="separator">The separator string.</param>
    /// <returns>The joined string.</returns>
    public static string Separate(AList<string> elements, string separator = ", ")
    
    /// <summary>
    /// Joins array elements into a single string using a separator.
    /// </summary>
    /// <param name="elements">The array of strings.</param>
    /// <param name="separator">The separator string.</param>
    /// <returns>The joined string.</returns>
    public static string Separate(string[] elements, string separator = ", ")
    
    /// <summary>
    /// Splits a string into an array using a separator.
    /// </summary>
    /// <param name="elements">The joined string.</param>
    /// <param name="separator">The separator string.</param>
    /// <returns>The array of strings.</returns>
    public static string[] DeSeparate(string elements, string separator = ", ")
}
```

## Globals

```csharp
/// <summary>
/// Global configuration class for the DevBase library.
/// </summary>
public class Globals
{
    /// <summary>
    /// Gets or sets whether serialization is allowed for memory size calculations.
    /// </summary>
    public static bool ALLOW_SERIALIZATION { get; set; } = true;
}
```

# DevBase.Api Project Documentation

This document contains all class, method, and field signatures with their corresponding comments for the DevBase.Api project.

## Table of Contents

- [Apis](#apis)
  - [ApiClient](#apiclient)
  - [AppleMusic](#applemusic)
  - [BeautifulLyrics](#beautifullyrics)
  - [Deezer](#deezer)
- [Enums](#enums)
- [Exceptions](#exceptions)
- [Serializer](#serializer)
- [Structure](#structure)

## Apis

### ApiClient

```csharp
/// <summary>
/// Base class for API clients, providing common error handling and type conversion utilities.
/// </summary>
public class ApiClient
{
    /// <summary>
    /// Gets or sets a value indicating whether to throw exceptions on errors or return default values.
    /// </summary>
    public bool StrictErrorHandling { get; set; }
    
    /// <summary>
    /// Throws an exception if strict error handling is enabled, otherwise returns a default value for type T.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="exception">The exception to throw.</param>
    /// <param name="callerMember">The calling member name.</param>
    /// <param name="callerFilePath">The calling file path.</param>
    /// <param name="callerLineNumber">The calling line number.</param>
    /// <returns>The default value of T if exception is not thrown.</returns>
    protected dynamic Throw<T>(
        System.Exception exception,
        [CallerMemberName] string callerMember = "", 
        [CallerFilePath] string callerFilePath = "", 
        [CallerLineNumber] int callerLineNumber = 0)
    
    /// <summary>
    /// Throws an exception if strict error handling is enabled, otherwise returns a default tuple (empty string, false).
    /// </summary>
    /// <param name="exception">The exception to throw.</param>
    /// <param name="callerMember">The calling member name.</param>
    /// <param name="callerFilePath">The calling file path.</param>
    /// <param name="callerLineNumber">The calling line number.</param>
    /// <returns>A tuple (string.Empty, false) if exception is not thrown.</returns>
    protected (string, bool) ThrowTuple(
        System.Exception exception,
        [CallerMemberName] string callerMember = "", 
        [CallerFilePath] string callerFilePath = "", 
        [CallerLineNumber] int callerLineNumber = 0)
}
```

### AppleMusic

```csharp
/// <summary>
/// Apple Music API client for searching tracks and retrieving lyrics.
/// </summary>
public class AppleMusic : ApiClient
{
    private readonly string _baseUrl;
    private readonly AuthenticationToken _apiToken;
    private GenericAuthenticationToken _userMediaToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppleMusic"/> class.
    /// </summary>
    /// <param name="apiToken">The API token for authentication.</param>
    public AppleMusic(string apiToken)
    
    /// <summary>
    /// Sets the user media token for authenticated requests.
    /// </summary>
    /// <param name="userMediaToken">The user media token.</param>
    /// <returns>The current AppleMusic instance.</returns>
    public AppleMusic WithMediaUserToken(GenericAuthenticationToken userMediaToken)
    
    /// <summary>
    /// Sets the user media token from a cookie.
    /// </summary>
    /// <param name="myacinfoCookie">The myacinfo cookie value.</param>
    public async Task WithMediaUserTokenFromCookie(string myacinfoCookie)
    
    /// <summary>
    /// Creates an AppleMusic instance with an access token extracted from the website.
    /// </summary>
    /// <returns>A new AppleMusic instance or null if token extraction fails.</returns>
    public static async Task<AppleMusic> WithAccessToken()
    
    /// <summary>
    /// Searches for tracks on Apple Music.
    /// </summary>
    /// <param name="searchTerm">The search term.</param>
    /// <param name="limit">The maximum number of results.</param>
    /// <returns>A list of AppleMusicTrack objects.</returns>
    public async Task<List<AppleMusicTrack>> Search(string searchTerm, int limit = 10)
    
    /// <summary>
    /// Performs a raw search and returns the JSON response.
    /// </summary>
    /// <param name="searchTerm">The search term.</param>
    /// <param name="limit">The maximum number of results.</param>
    /// <returns>The raw JSON search response.</returns>
    public async Task<JsonAppleMusicSearchResult> RawSearch(string searchTerm, int limit = 10)
    
    /// <summary>
    /// Gets lyrics for a specific track.
    /// </summary>
    /// <param name="trackId">The track ID.</param>
    /// <returns>The lyrics response.</returns>
    public async Task<JsonAppleMusicLyricsResponse> GetLyrics(string trackId)
    
    /// <summary>
    /// Gets the API token.
    /// </summary>
    public AuthenticationToken ApiToken { get; }
}
```

### BeautifulLyrics

```csharp
/// <summary>
/// Beautiful Lyrics API client for retrieving song lyrics.
/// </summary>
public class BeautifulLyrics : ApiClient
{
    private readonly string _baseUrl;

    /// <summary>
    /// Initializes a new instance of the <see cref="BeautifulLyrics"/> class.
    /// </summary>
    public BeautifulLyrics()
    
    /// <summary>
    /// Gets lyrics for a song by ISRC.
    /// </summary>
    /// <param name="isrc">The ISRC code.</param>
    /// <returns>Either TimeStampedLyric list or RichTimeStampedLyric list depending on lyrics type.</returns>
    public async Task<dynamic> GetLyrics(string isrc)
    
    /// <summary>
    /// Gets raw lyrics data for a song by ISRC.
    /// </summary>
    /// <param name="isrc">The ISRC code.</param>
    /// <returns>A tuple containing raw lyrics and a boolean indicating if lyrics are rich sync.</returns>
    public async Task<(string RawLyrics, bool IsRichSync)> GetRawLyrics(string isrc)
}
```

### Deezer

```csharp
/// <summary>
/// Deezer API client for searching tracks, retrieving lyrics, and downloading music.
/// </summary>
public class Deezer : ApiClient
{
    private readonly string _authEndpoint;
    private readonly string _apiEndpoint;
    private readonly string _pipeEndpoint;
    private readonly string _websiteEndpoint;
    private readonly string _mediaEndpoint;
    private readonly CookieContainer _cookieContainer;

    /// <summary>
    /// Initializes a new instance of the <see cref="Deezer"/> class.
    /// </summary>
    /// <param name="arlToken">Optional ARL token for authentication.</param>
    public Deezer(string arlToken = "")
    
    /// <summary>
    /// Gets a JWT token for API authentication.
    /// </summary>
    /// <returns>The JWT token response.</returns>
    public async Task<JsonDeezerJwtToken> GetJwtToken()
    
    /// <summary>
    /// Gets an access token for unlogged requests.
    /// </summary>
    /// <param name="appID">The application ID.</param>
    /// <returns>The access token response.</returns>
    public async Task<JsonDeezerAuthTokenResponse> GetAccessToken(string appID = "457142")
    
    /// <summary>
    /// Gets an access token for a session.
    /// </summary>
    /// <param name="sessionID">The session ID.</param>
    /// <param name="appID">The application ID.</param>
    /// <returns>The access token response.</returns>
    public async Task<JsonDeezerAuthTokenResponse> GetAccessToken(string sessionID, string appID = "457142")
    
    /// <summary>
    /// Gets an ARL token from a session.
    /// </summary>
    /// <param name="sessionID">The session ID.</param>
    /// <returns>The ARL token.</returns>
    public async Task<string> GetArlTokenFromSession(string sessionID)
    
    /// <summary>
    /// Gets lyrics for a track.
    /// </summary>
    /// <param name="trackID">The track ID.</param>
    /// <returns>A tuple containing raw lyrics and a list of timestamped lyrics.</returns>
    public async Task<(string RawLyrics, AList<TimeStampedLyric> TimeStampedLyrics)> GetLyrics(string trackID)
    
    /// <summary>
    /// Gets lyrics using the AJAX endpoint.
    /// </summary>
    /// <param name="trackID">The track ID.</param>
    /// <returns>The raw lyrics response.</returns>
    public async Task<JsonDeezerRawLyricsResponse> GetLyricsAjax(string trackID)
    
    /// <summary>
    /// Gets lyrics using the GraphQL endpoint.
    /// </summary>
    /// <param name="trackID">The track ID.</param>
    /// <returns>The lyrics response.</returns>
    public async Task<JsonDeezerLyricsResponse> GetLyricsGraph(string trackID)
    
    /// <summary>
    /// Gets the CSRF token.
    /// </summary>
    /// <returns>The CSRF token.</returns>
    public async Task<string> GetCsrfToken()
    
    /// <summary>
    /// Gets user data.
    /// </summary>
    /// <param name="retries">Number of retries.</param>
    /// <returns>The user data.</returns>
    public async Task<JsonDeezerUserData> GetUserData(int retries = 5)
    
    /// <summary>
    /// Gets raw user data.
    /// </summary>
    /// <param name="retries">Number of retries.</param>
    /// <returns>The raw user data.</returns>
    public async Task<string> GetUserDataRaw(int retries = 5)
    
    /// <summary>
    /// Gets song details.
    /// </summary>
    /// <param name="trackID">The track ID.</param>
    /// <returns>The DeezerTrack object.</returns>
    public async Task<DeezerTrack> GetSong(string trackID)
    
    /// <summary>
    /// Gets detailed song information.
    /// </summary>
    /// <param name="trackID">The track ID.</param>
    /// <param name="csrfToken">The CSRF token.</param>
    /// <param name="retries">Number of retries.</param>
    /// <returns>The song details.</returns>
    public async Task<JsonDeezerSongDetails> GetSongDetails(string trackID, string csrfToken, int retries = 5)
    
    /// <summary>
    /// Gets song URLs for downloading.
    /// </summary>
    /// <param name="trackToken">The track token.</param>
    /// <param name="licenseToken">The license token.</param>
    /// <returns>The song source information.</returns>
    public async Task<JsonDeezerSongSource> GetSongUrls(string trackToken, string licenseToken)
    
    /// <summary>
    /// Downloads a song.
    /// </summary>
    /// <param name="trackID">The track ID.</param>
    /// <returns>The decrypted song data.</returns>
    public async Task<byte[]> DownloadSong(string trackID)
    
    /// <summary>
    /// Searches for content.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <returns>The search response.</returns>
    public async Task<JsonDeezerSearchResponse> Search(string query)
    
    /// <summary>
    /// Searches for songs with specific parameters.
    /// </summary>
    /// <param name="track">Track name.</param>
    /// <param name="artist">Artist name.</param>
    /// <param name="album">Album name.</param>
    /// <param name="strict">Whether to use strict search.</param>
    /// <returns>The search response.</returns>
    public async Task<JsonDeezerSearchResponse> Search(string track = "", string artist = "", string album = "", bool strict = false)
    
    /// <summary>
    /// Searches for songs and returns track data.
    /// </summary>
    /// <param name="track">Track name.</param>
    /// <param name="artist">Artist name.</param>
    /// <param name="album">Album name.</param>
    /// <param name="strict">Whether to use strict search.</param>
    /// <param name="limit">Maximum number of results.</param>
    /// <returns>A list of DeezerTrack objects.</returns>
    public async Task<List<DeezerTrack>> SearchSongData(string track = "", string artist = "", string album = "", bool strict = false, int limit = 10)
}
```

## Enums

### EnumAppleMusicExceptionType
```csharp
/// <summary>
/// Specifies the type of Apple Music exception.
/// </summary>
public enum EnumAppleMusicExceptionType
{
    /// <summary>User media token is not provided.</summary>
    UnprovidedUserMediaToken,
    /// <summary>Access token is unavailable.</summary>
    AccessTokenUnavailable,
    /// <summary>Search results are empty.</summary>
    SearchResultsEmpty
}
```

### EnumBeautifulLyricsExceptionType
```csharp
/// <summary>
/// Specifies the type of Beautiful Lyrics exception.
/// </summary>
public enum EnumBeautifulLyricsExceptionType
{
    /// <summary>Lyrics not found.</summary>
    LyricsNotFound,
    /// <summary>Failed to parse lyrics.</summary>
    LyricsParsed
}
```

### EnumDeezerExceptionType
```csharp
/// <summary>
/// Specifies the type of Deezer exception.
/// </summary>
public enum EnumDeezerExceptionType
{
    /// <summary>ARL token is missing or invalid.</summary>
    ArlToken, 
    /// <summary>App ID is invalid.</summary>
    AppId, 
    /// <summary>App session ID is invalid.</summary>
    AppSessionId, 
    /// <summary>Session ID is invalid.</summary>
    SessionId, 
    /// <summary>No CSRF token available.</summary>
    NoCsrfToken, 
    /// <summary>CSRF token is invalid.</summary>
    InvalidCsrfToken, 
    /// <summary>JWT token has expired.</summary>
    JwtExpired, 
    /// <summary>Song details are missing.</summary>
    MissingSongDetails,
    /// <summary>Failed to receive song details.</summary>
    FailedToReceiveSongDetails,
    /// <summary>Wrong parameter provided.</summary>
    WrongParameter, 
    /// <summary>Lyrics not found.</summary>
    LyricsNotFound,
    /// <summary>Failed to parse CSRF token.</summary>
    CsrfParsing,
    /// <summary>User data error.</summary>
    UserData,
    /// <summary>URL data error.</summary>
    UrlData
}
```

## Exceptions

### AppleMusicException
```csharp
/// <summary>
/// Exception thrown for Apple Music API related errors.
/// </summary>
public class AppleMusicException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppleMusicException"/> class.
    /// </summary>
    /// <param name="type">The type of error.</param>
    public AppleMusicException(EnumAppleMusicExceptionType type)
}
```

### BeautifulLyricsException
```csharp
/// <summary>
/// Exception thrown for Beautiful Lyrics API related errors.
/// </summary>
public class BeautifulLyricsException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BeautifulLyricsException"/> class.
    /// </summary>
    /// <param name="type">The type of error.</param>
    public BeautifulLyricsException(EnumBeautifulLyricsExceptionType type)
}
```

### DeezerException
```csharp
/// <summary>
/// Exception thrown for Deezer API related errors.
/// </summary>
public class DeezerException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeezerException"/> class.
    /// </summary>
    /// <param name="type">The type of error.</param>
    public DeezerException(EnumDeezerExceptionType type)
}
```

## Serializer

### JsonDeserializer
```csharp
/// <summary>
/// A generic JSON deserializer helper that captures serialization errors.
/// </summary>
/// <typeparam name="T">The type to deserialize into.</typeparam>
public class JsonDeserializer<T>
{
    private JsonSerializerSettings _serializerSettings;
    private AList<string> _errorList;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonDeserializer{T}"/> class.
    /// </summary>
    public JsonDeserializer()
    
    /// <summary>
    /// Deserializes the JSON string into an object of type T.
    /// </summary>
    /// <param name="input">The JSON string.</param>
    /// <returns>The deserialized object.</returns>
    public T Deserialize(string input)
    
    /// <summary>
    /// Deserializes the JSON string into an object of type T asynchronously.
    /// </summary>
    /// <param name="input">The JSON string.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object.</returns>
    public Task<T> DeserializeAsync(string input)
    
    /// <summary>
    /// Gets or sets the list of errors encountered during deserialization.
    /// </summary>
    public AList<string> ErrorList { get; set; }
}
```

## Structure

### AppleMusicTrack
```csharp
/// <summary>
/// Represents a track from Apple Music.
/// </summary>
public class AppleMusicTrack
{
    /// <summary>Gets or sets the track title.</summary>
    public string Title { get; set; }
    /// <summary>Gets or sets the album name.</summary>
    public string Album { get; set; }
    /// <summary>Gets or sets the duration in milliseconds.</summary>
    public int Duration { get; set; }
    /// <summary>Gets or sets the array of artists.</summary>
    public string[] Artists { get; set; }
    /// <summary>Gets or sets the array of artwork URLs.</summary>
    public string[] ArtworkUrls { get; set; }
    /// <summary>Gets or sets the service internal ID.</summary>
    public string ServiceInternalId { get; set; }
    /// <summary>Gets or sets the ISRC code.</summary>
    public string Isrc { get; set; }

    /// <summary>
    /// Creates an AppleMusicTrack from a JSON response.
    /// </summary>
    /// <param name="response">The JSON response.</param>
    /// <returns>An AppleMusicTrack instance.</returns>
    public static AppleMusicTrack FromResponse(JsonAppleMusicSearchResultResultsSongData response)
}
```

### DeezerTrack
```csharp
/// <summary>
/// Represents a track from Deezer.
/// </summary>
public class DeezerTrack
{
    /// <summary>Gets or sets the track title.</summary>
    public string Title { get; set; }
    /// <summary>Gets or sets the album name.</summary>
    public string Album { get; set; }
    /// <summary>Gets or sets the duration in milliseconds.</summary>
    public int Duration { get; set; }
    /// <summary>Gets or sets the array of artists.</summary>
    public string[] Artists { get; set; }
    /// <summary>Gets or sets the array of artwork URLs.</summary>
    public string[] ArtworkUrls { get; set; }
    /// <summary>Gets or sets the service internal ID.</summary>
    public string ServiceInternalId { get; set; }
}
```

### JSON Structure Classes
The project contains numerous JSON structure classes for deserializing API responses:

#### Apple Music JSON Structures
- `JsonAppleMusicLyricsResponse`
- `JsonAppleMusicLyricsResponseData`
- `JsonAppleMusicLyricsResponseDataAttributes`
- `JsonAppleMusicSearchResult`
- `JsonAppleMusicSearchResultResultsSong`
- And many more...

#### Beautiful Lyrics JSON Structures
- `JsonBeautifulLyricsLineLyricsResponse`
- `JsonBeautifulLyricsRichLyricsResponse`
- And related vocal group classes...

#### Deezer JSON Structures
- `JsonDeezerArlTokenResponse`
- `JsonDeezerAuthTokenResponse`
- `JsonDeezerJwtToken`
- `JsonDeezerLyricsResponse`
- `JsonDeezerRawLyricsResponse`
- `JsonDeezerSearchResponse`
- `JsonDeezerSongDetails`
- `JsonDeezerSongSource`
- `JsonDeezerUserData`
- And many more...

# DevBase.Avalonia Project Documentation

This document contains all class, method, and field signatures with their corresponding comments for the DevBase.Avalonia project.

## Table of Contents

- [Color](#color)
  - [Extensions](#extensions)
    - [ColorExtension](#colorextension)
    - [ColorNormalizerExtension](#colornormalizerextension)
    - [LockedFramebufferExtensions](#lockedframebufferextensions)
  - [Image](#image)
    - [BrightestColorCalculator](#brightestcolorcalculator)
    - [GroupColorCalculator](#groupcolorcalculator)
    - [NearestColorCalculator](#nearestcolorcalculator)
  - [Utils](#utils)
    - [ColorUtils](#colorutils)
- [Data](#data)
  - [ClusterData](#clusterdata)

## Color

### Extensions

#### ColorExtension

```csharp
/// <summary>
/// Provides extension methods for <see cref="global::Avalonia.Media.Color"/>.
/// </summary>
public static class ColorExtension
{
    /// <summary>
    /// Shifts the RGB components of the color based on their relative intensity.
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <param name="smallShift">The multiplier for non-dominant color components.</param>
    /// <param name="bigShift">The multiplier for the dominant color component.</param>
    /// <returns>A new <see cref="global::Avalonia.Media.Color"/> with shifted values.</returns>
    public static global::Avalonia.Media.Color Shift(
        this global::Avalonia.Media.Color color, 
        double smallShift,
        double bigShift)
    
    /// <summary>
    /// Adjusts the brightness of the color by a percentage.
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <param name="percentage">The percentage to adjust brightness (e.g., 50 for 50%).</param>
    /// <returns>A new <see cref="global::Avalonia.Media.Color"/> with adjusted brightness.</returns>
    public static global::Avalonia.Media.Color AdjustBrightness(
        this global::Avalonia.Media.Color color,
        double percentage)
    
    /// <summary>
    /// Calculates the saturation of the color (0.0 to 1.0).
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <returns>The saturation value.</returns>
    public static double Saturation(this global::Avalonia.Media.Color color)
    
    /// <summary>
    /// Calculates the saturation percentage of the color (0.0 to 100.0).
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <returns>The saturation percentage.</returns>
    public static double SaturationPercentage(this global::Avalonia.Media.Color color)
    
    /// <summary>
    /// Calculates the brightness of the color using weighted RGB values.
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <returns>The brightness value.</returns>
    public static double Brightness(this global::Avalonia.Media.Color color)
    
    /// <summary>
    /// Calculates the brightness percentage of the color (0.0 to 100.0).
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <returns>The brightness percentage.</returns>
    public static double BrightnessPercentage(this global::Avalonia.Media.Color color)
    
    /// <summary>
    /// Calculates the similarity between two colors as a percentage.
    /// </summary>
    /// <param name="color">The first color.</param>
    /// <param name="otherColor">The second color.</param>
    /// <returns>The similarity percentage (0.0 to 100.0).</returns>
    public static double Similarity(this global::Avalonia.Media.Color color, global::Avalonia.Media.Color otherColor)
    
    /// <summary>
    /// Corrects the color component values to ensure they are within the valid range (0-255).
    /// </summary>
    /// <param name="color">The color to correct.</param>
    /// <returns>A corrected <see cref="global::Avalonia.Media.Color"/>.</returns>
    public static global::Avalonia.Media.Color Correct(this global::Avalonia.Media.Color color)
    
    /// <summary>
    /// Calculates the average color from a list of colors.
    /// </summary>
    /// <param name="colors">The list of colors.</param>
    /// <returns>The average color.</returns>
    public static global::Avalonia.Media.Color Average(this AList<global::Avalonia.Media.Color> colors)
    
    /// <summary>
    /// Filters a list of colors, returning only those with saturation greater than the specified value.
    /// </summary>
    /// <param name="colors">The source list of colors.</param>
    /// <param name="value">The minimum saturation percentage threshold.</param>
    /// <returns>A filtered list of colors.</returns>
    public static AList<global::Avalonia.Media.Color> FilterSaturation(this AList<global::Avalonia.Media.Color> colors, double value)
    
    /// <summary>
    /// Filters a list of colors, returning only those with brightness greater than the specified percentage.
    /// </summary>
    /// <param name="colors">The source list of colors.</param>
    /// <param name="percentage">The minimum brightness percentage threshold.</param>
    /// <returns>A filtered list of colors.</returns>
    public static AList<global::Avalonia.Media.Color> FilterBrightness(this AList<global::Avalonia.Media.Color> colors, double percentage)
    
    /// <summary>
    /// Removes transparent colors (alpha=0, rgb=0) from the array.
    /// </summary>
    /// <param name="colors">The source array of colors.</param>
    /// <returns>A new array with null/empty values removed.</returns>
    public static global::Avalonia.Media.Color[] RemoveNullValues(this global::Avalonia.Media.Color[] colors)
}
```

#### ColorNormalizerExtension

```csharp
/// <summary>
/// Provides extension methods for normalizing color values.
/// </summary>
public static class ColorNormalizerExtension
{
    /// <summary>
    /// Normalizes the color components to a range of 0.0 to 1.0.
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <returns>An array containing normalized [A, R, G, B] values.</returns>
    public static double[] Normalize(this global::Avalonia.Media.Color color)
    
    /// <summary>
    /// Denormalizes an array of [A, R, G, B] (or [R, G, B]) values back to a Color.
    /// </summary>
    /// <param name="normalized">The normalized color array (values 0.0 to 1.0).</param>
    /// <returns>A new <see cref="global::Avalonia.Media.Color"/>.</returns>
    public static global::Avalonia.Media.Color DeNormalize(this double[] normalized)
}
```

#### LockedFramebufferExtensions

```csharp
/// <summary>
/// Provides extension methods for accessing pixel data from a <see cref="ILockedFramebuffer"/>.
/// </summary>
public static class LockedFramebufferExtensions
{
    /// <summary>
    /// Gets the pixel data at the specified coordinates as a span of bytes.
    /// </summary>
    /// <param name="framebuffer">The locked framebuffer.</param>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <returns>A span of bytes representing the pixel.</returns>
    public static Span<byte> GetPixel(this ILockedFramebuffer framebuffer, int x, int y)
}
```

### Image

#### BrightestColorCalculator

```csharp
/// <summary>
/// Calculates the brightest color from a bitmap.
/// </summary>
public class BrightestColorCalculator
{
    private global::Avalonia.Media.Color _brightestColor;
    private double _colorRange;
    private double _bigShift;
    private double _smallShift;
    private int _pixelSteps;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="BrightestColorCalculator"/> class with default settings.
    /// </summary>
    public BrightestColorCalculator()
    
    /// <summary>
    /// Initializes a new instance of the <see cref="BrightestColorCalculator"/> class with custom shift values.
    /// </summary>
    /// <param name="bigShift">The multiplier for dominant color components.</param>
    /// <param name="smallShift">The multiplier for non-dominant color components.</param>
    public BrightestColorCalculator(double bigShift, double smallShift)
    
    /// <summary>
    /// Calculates the brightest color from the provided bitmap.
    /// </summary>
    /// <param name="bitmap">The source bitmap.</param>
    /// <returns>The calculated brightest color.</returns>
    public unsafe global::Avalonia.Media.Color GetColorFromBitmap(Bitmap bitmap)
    
    /// <summary>
    /// Gets or sets the range within which colors are considered similar to the brightest color.
    /// </summary>
    public double ColorRange { get; set; }
    
    /// <summary>
    /// Gets or sets the multiplier for dominant color components.
    /// </summary>
    public double BigShift { get; set; }
    
    /// <summary>
    /// Gets or sets the multiplier for non-dominant color components.
    /// </summary>
    public double SmallShift { get; set; }
    
    /// <summary>
    /// Gets or sets the step size for pixel sampling.
    /// </summary>
    public int PixelSteps { get; set; }
}
```

#### GroupColorCalculator

```csharp
/// <summary>
/// Calculates the dominant color by grouping similar colors together.
/// </summary>
public class GroupColorCalculator
{
    private double _colorRange;
    private double _bigShift;
    private double _smallShift;
    private int _pixelSteps;
    private int _brightness;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupColorCalculator"/> class with default settings.
    /// </summary>
    public GroupColorCalculator()
    
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupColorCalculator"/> class with custom shift values.
    /// </summary>
    /// <param name="bigShift">The multiplier for dominant color components.</param>
    /// <param name="smallShift">The multiplier for non-dominant color components.</param>
    public GroupColorCalculator(double bigShift, double smallShift)
    
    /// <summary>
    /// Calculates the dominant color from the provided bitmap using color grouping.
    /// </summary>
    /// <param name="bitmap">The source bitmap.</param>
    /// <returns>The calculated dominant color.</returns>
    public global::Avalonia.Media.Color GetColorFromBitmap(Bitmap bitmap)
    
    /// <summary>
    /// Gets or sets the color range to group colors.
    /// </summary>
    public double ColorRange { get; set; }
    
    /// <summary>
    /// Gets or sets the multiplier for dominant color components.
    /// </summary>
    public double BigShift { get; set; }
    
    /// <summary>
    /// Gets or sets the multiplier for non-dominant color components.
    /// </summary>
    public double SmallShift { get; set; }
    
    /// <summary>
    /// Gets or sets the step size for pixel sampling.
    /// </summary>
    public int PixelSteps { get; set; }
    
    /// <summary>
    /// Gets or sets the minimum brightness threshold.
    /// </summary>
    public int Brightness { get; set; }
}
```

#### NearestColorCalculator

```csharp
/// <summary>
/// Calculates the nearest color based on difference logic.
/// </summary>
public class NearestColorCalculator
{
    private global::Avalonia.Media.Color _smallestDiff;
    private global::Avalonia.Media.Color _brightestColor;
    private double _colorRange;
    private double _bigShift;
    private double _smallShift;
    private int _pixelSteps;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="NearestColorCalculator"/> class with default settings.
    /// </summary>
    public NearestColorCalculator()
    
    /// <summary>
    /// Initializes a new instance of the <see cref="NearestColorCalculator"/> class with custom shift values.
    /// </summary>
    /// <param name="bigShift">The multiplier for dominant color components.</param>
    /// <param name="smallShift">The multiplier for non-dominant color components.</param>
    public NearestColorCalculator(double bigShift, double smallShift)
    
    /// <summary>
    /// Calculates the nearest color from the provided bitmap.
    /// </summary>
    /// <param name="bitmap">The source bitmap.</param>
    /// <returns>The calculated color.</returns>
    public unsafe global::Avalonia.Media.Color GetColorFromBitmap(Bitmap bitmap)
    
    /// <summary>
    /// Gets or sets the color with the smallest difference found.
    /// </summary>
    public global::Avalonia.Media.Color SmallestDiff { get; set; }
    
    /// <summary>
    /// Gets or sets the range within which colors are considered similar.
    /// </summary>
    public double ColorRange { get; set; }
    
    /// <summary>
    /// Gets or sets the multiplier for dominant color components.
    /// </summary>
    public double BigShift { get; set; }
    
    /// <summary>
    /// Gets or sets the multiplier for non-dominant color components.
    /// </summary>
    public double SmallShift { get; set; }
    
    /// <summary>
    /// Gets or sets the step size for pixel sampling.
    /// </summary>
    public int PixelSteps { get; set; }
}
```

### Utils

#### ColorUtils

```csharp
/// <summary>
/// Provides utility methods for handling colors.
/// </summary>
public class ColorUtils
{
    /// <summary>
    /// Extracts all pixels from a bitmap as a list of colors.
    /// </summary>
    /// <param name="bitmap">The source bitmap.</param>
    /// <returns>A list of colors, excluding fully transparent ones.</returns>
    public static AList<Color> GetPixels(Bitmap bitmap)
}
```

## Data

### ClusterData

```csharp
/// <summary>
/// Contains static data for color clustering.
/// </summary>
public class ClusterData
{
    /// <summary>
    /// A pre-defined set of colors used for clustering or comparison.
    /// </summary>
    public static Color[] RGB_DATA
}
```

# DevBase.Avalonia.Extension Project Documentation

This document contains all class, method, and field signatures with their corresponding comments for the DevBase.Avalonia.Extension project.

## Table of Contents

- [Color](#color)
  - [Image](#image)
    - [ClusterColorCalculator](#clustercolorcalculator)
    - [LabClusterColorCalculator](#labclustercolorcalculator)
- [Configuration](#configuration)
  - [BrightnessConfiguration](#brightnessconfiguration)
  - [ChromaConfiguration](#chromaconfiguration)
  - [FilterConfiguration](#filterconfiguration)
  - [PostProcessingConfiguration](#postprocessingconfiguration)
  - [PreProcessingConfiguration](#preprocessingconfiguration)
- [Converter](#converter)
  - [RGBToLabConverter](#rgbtolabconverter)
- [Extension](#extension)
  - [BitmapExtension](#bitmapextension)
  - [ColorNormalizerExtension](#colornormalizerextension)
  - [LabColorExtension](#labcolorextension)
- [Processing](#processing)
  - [ImagePreProcessor](#imagepreprocessor)

## Color

### Image

#### ClusterColorCalculator

```csharp
/// <summary>
/// Calculates dominant colors from an image using KMeans clustering on RGB values.
/// </summary>
[Obsolete("Use LabClusterColorCalculator instead")]
public class ClusterColorCalculator
{
    /// <summary>
    /// Gets or sets the minimum saturation threshold for filtering colors.
    /// </summary>
    public double MinSaturation { get; set; }
    
    /// <summary>
    /// Gets or sets the minimum brightness threshold for filtering colors.
    /// </summary>
    public double MinBrightness { get; set; }
    
    /// <summary>
    /// Gets or sets the small shift value.
    /// </summary>
    public double SmallShift { get; set; }
    
    /// <summary>
    /// Gets or sets the big shift value.
    /// </summary>
    public double BigShift { get; set; }
    
    /// <summary>
    /// Gets or sets the tolerance for KMeans clustering.
    /// </summary>
    public double Tolerance { get; set; }
    
    /// <summary>
    /// Gets or sets the number of clusters to find.
    /// </summary>
    public int Clusters { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum range of clusters to consider for the result.
    /// </summary>
    public int MaxRange { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use a predefined dataset.
    /// </summary>
    public bool PredefinedDataset { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to filter by saturation.
    /// </summary>
    public bool FilterSaturation { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to filter by brightness.
    /// </summary>
    public bool FilterBrightness { get; set; }

    /// <summary>
    /// Gets or sets additional colors to include in the clustering dataset.
    /// </summary>
    public AList<Color> AdditionalColorDataset { get; set; }

    /// <summary>
    /// Calculates the dominant color from the provided bitmap.
    /// </summary>
    /// <param name="bitmap">The source bitmap.</param>
    /// <returns>The calculated dominant color.</returns>
    public Color GetColorFromBitmap(Bitmap bitmap)
}
```

#### LabClusterColorCalculator

```csharp
/// <summary>
/// Calculates dominant colors from an image using KMeans clustering on Lab values.
/// This is the preferred calculator for better color accuracy closer to human perception.
/// </summary>
public class LabClusterColorCalculator
{
    /// <summary>
    /// Gets or sets the small shift value for post-processing.
    /// </summary>
    public double SmallShift { get; set; }
    
    /// <summary>
    /// Gets or sets the big shift value for post-processing.
    /// </summary>
    public double BigShift { get; set; }
    
    /// <summary>
    /// Gets or sets the tolerance for KMeans clustering.
    /// </summary>
    public double Tolerance { get; set; }
    
    /// <summary>
    /// Gets or sets the number of clusters to find.
    /// </summary>
    public int Clusters { get; set; }

    /// <summary>
    /// Gets or sets the maximum range of clusters to consider for the result.
    /// </summary>
    public int MaxRange { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use a predefined dataset of colors.
    /// </summary>
    public bool UsePredefinedSet { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to return a fallback result if filtering removes all colors.
    /// </summary>
    public bool AllowEdgeCase { get; set; }
    
    /// <summary>
    /// Gets or sets the pre-processing configuration (e.g. blur).
    /// </summary>
    public PreProcessingConfiguration PreProcessing { get; set; }
    
    /// <summary>
    /// Gets or sets the filtering configuration (chroma, brightness).
    /// </summary>
    public FilterConfiguration Filter { get; set; }

    /// <summary>
    /// Gets or sets the post-processing configuration (pastel, shifting).
    /// </summary>
    public PostProcessingConfiguration PostProcessing { get; set; }
    
    /// <summary>
    /// Gets or sets additional Lab colors to include in the clustering dataset.
    /// </summary>
    public AList<LabColor> AdditionalColorDataset { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LabClusterColorCalculator"/> class.
    /// </summary>
    public LabClusterColorCalculator()
    
    /// <summary>
    /// Calculates the dominant color from the provided bitmap.
    /// </summary>
    /// <param name="bitmap">The source bitmap.</param>
    /// <returns>The calculated dominant color.</returns>
    public Color GetColorFromBitmap(Bitmap bitmap)
    
    /// <summary>
    /// Calculates a list of dominant colors from the provided bitmap.
    /// </summary>
    /// <param name="bitmap">The source bitmap.</param>
    /// <returns>A list of calculated colors.</returns>
    public AList<Color> GetColorListFromBitmap(Bitmap bitmap)
}
```

## Configuration

### BrightnessConfiguration

```csharp
/// <summary>
/// Configuration for brightness filtering.
/// </summary>
public class BrightnessConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether brightness filtering is enabled.
    /// </summary>
    public bool FilterBrightness { get; set; }
    
    /// <summary>
    /// Gets or sets the minimum brightness threshold (0-100).
    /// </summary>
    public double MinBrightness { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum brightness threshold (0-100).
    /// </summary>
    public double MaxBrightness { get; set; }
}
```

### ChromaConfiguration

```csharp
/// <summary>
/// Configuration for chroma (color intensity) filtering.
/// </summary>
public class ChromaConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether chroma filtering is enabled.
    /// </summary>
    public bool FilterChroma { get; set; }
    
    /// <summary>
    /// Gets or sets the minimum chroma threshold.
    /// </summary>
    public double MinChroma { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum chroma threshold.
    /// </summary>
    public double MaxChroma { get; set; }
}
```

### FilterConfiguration

```csharp
/// <summary>
/// Configuration for color filtering settings.
/// </summary>
public class FilterConfiguration
{
    /// <summary>
    /// Gets or sets the chroma configuration.
    /// </summary>
    public ChromaConfiguration ChromaConfiguration { get; set; }
    
    /// <summary>
    /// Gets or sets the brightness configuration.
    /// </summary>
    public BrightnessConfiguration BrightnessConfiguration { get; set; }
}
```

### PostProcessingConfiguration

```csharp
/// <summary>
/// Configuration for post-processing of calculated colors.
/// </summary>
public class PostProcessingConfiguration
{
    /// <summary>
    /// Gets or sets the small shift value for color shifting.
    /// </summary>
    public double SmallShift { get; set; }
    
    /// <summary>
    /// Gets or sets the big shift value for color shifting.
    /// </summary>
    public double BigShift { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether color shifting post-processing is enabled.
    /// </summary>
    public bool ColorShiftingPostProcessing { get; set; }
    
    /// <summary>
    /// Gets or sets the target lightness for pastel processing.
    /// </summary>
    public double PastelLightness { get; set; }
    
    /// <summary>
    /// Gets or sets the lightness subtractor value for pastel processing when lightness is above guidance.
    /// </summary>
    public double PastelLightnessSubtractor { get; set; }
    
    /// <summary>
    /// Gets or sets the saturation multiplier for pastel processing.
    /// </summary>
    public double PastelSaturation { get; set; }
    
    /// <summary>
    /// Gets or sets the lightness threshold to decide how to adjust pastel lightness.
    /// </summary>
    public double PastelGuidance { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether pastel post-processing is enabled.
    /// </summary>
    public bool PastelPostProcessing { get; set; }
}
```

### PreProcessingConfiguration

```csharp
/// <summary>
/// Configuration for image pre-processing.
/// </summary>
public class PreProcessingConfiguration
{
    /// <summary>
    /// Gets or sets the sigma value for blur.
    /// </summary>
    public float BlurSigma { get; set; }
    
    /// <summary>
    /// Gets or sets the number of blur rounds.
    /// </summary>
    public int BlurRounds { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether blur pre-processing is enabled.
    /// </summary>
    public bool BlurPreProcessing { get; set; }
}
```

## Converter

### RGBToLabConverter

```csharp
/// <summary>
/// Converter for transforming between RGB and LAB color spaces.
/// </summary>
public class RGBToLabConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RGBToLabConverter"/> class.
    /// Configures converters using sRGB working space and D65 illuminant.
    /// </summary>
    public RGBToLabConverter()
    
    /// <summary>
    /// Converts an RGB color to Lab color.
    /// </summary>
    /// <param name="color">The RGB color.</param>
    /// <returns>The Lab color.</returns>
    public LabColor ToLabColor(RGBColor color)
    
    /// <summary>
    /// Converts a Lab color to RGB color.
    /// </summary>
    /// <param name="color">The Lab color.</param>
    /// <returns>The RGB color.</returns>
    public RGBColor ToRgbColor(LabColor color)
}
```

## Extension

### BitmapExtension

```csharp
/// <summary>
/// Provides extension methods for converting between different Bitmap types.
/// </summary>
public static class BitmapExtension
{
    /// <summary>
    /// Converts an Avalonia Bitmap to a System.Drawing.Bitmap.
    /// </summary>
    /// <param name="bitmap">The Avalonia bitmap.</param>
    /// <returns>The System.Drawing.Bitmap.</returns>
    public static Bitmap ToBitmap(this global::Avalonia.Media.Imaging.Bitmap bitmap)
    
    /// <summary>
    /// Converts a System.Drawing.Bitmap to an Avalonia Bitmap.
    /// </summary>
    /// <param name="bitmap">The System.Drawing.Bitmap.</param>
    /// <returns>The Avalonia Bitmap.</returns>
    public static global::Avalonia.Media.Imaging.Bitmap ToBitmap(this Bitmap bitmap)
    
    /// <summary>
    /// Converts a SixLabors ImageSharp Image to an Avalonia Bitmap.
    /// </summary>
    /// <param name="image">The ImageSharp Image.</param>
    /// <returns>The Avalonia Bitmap.</returns>
    public static global::Avalonia.Media.Imaging.Bitmap ToBitmap(this SixLabors.ImageSharp.Image image)
    
    /// <summary>
    /// Converts an Avalonia Bitmap to a SixLabors ImageSharp Image.
    /// </summary>
    /// <param name="bitmap">The Avalonia Bitmap.</param>
    /// <returns>The ImageSharp Image.</returns>
    public static SixLabors.ImageSharp.Image ToImage(this global::Avalonia.Media.Imaging.Bitmap bitmap)
}
```

### ColorNormalizerExtension

```csharp
/// <summary>
/// Provides extension methods for color normalization.
/// </summary>
public static class ColorNormalizerExtension
{
    /// <summary>
    /// Denormalizes an RGBColor (0-1 range) to an Avalonia Color (0-255 range).
    /// </summary>
    /// <param name="normalized">The normalized RGBColor.</param>
    /// <returns>The denormalized Avalonia Color.</returns>
    public static global::Avalonia.Media.Color DeNormalize(this RGBColor normalized)
}
```

### LabColorExtension

```csharp
/// <summary>
/// Provides extension methods for LabColor operations.
/// </summary>
public static class LabColorExtension
{
    /// <summary>
    /// Filters a list of LabColors based on lightness (L) values.
    /// </summary>
    /// <param name="colors">The list of LabColors.</param>
    /// <param name="min">Minimum lightness.</param>
    /// <param name="max">Maximum lightness.</param>
    /// <returns>A filtered list of LabColors.</returns>
    public static AList<LabColor> FilterBrightness(this AList<LabColor> colors, double min, double max)
    
    /// <summary>
    /// Calculates the chroma of a LabColor.
    /// </summary>
    /// <param name="color">The LabColor.</param>
    /// <returns>The chroma value.</returns>
    public static double Chroma(this LabColor color)
    
    /// <summary>
    /// Calculates the chroma percentage relative to a max chroma of 128.
    /// </summary>
    /// <param name="color">The LabColor.</param>
    /// <returns>The chroma percentage.</returns>
    public static double ChromaPercentage(this LabColor color)
    
    /// <summary>
    /// Filters a list of LabColors based on chroma percentage.
    /// </summary>
    /// <param name="colors">The list of LabColors.</param>
    /// <param name="min">Minimum chroma percentage.</param>
    /// <param name="max">Maximum chroma percentage.</param>
    /// <returns>A filtered list of LabColors.</returns>
    public static AList<LabColor> FilterChroma(this AList<LabColor> colors, double min, double max)
    
    /// <summary>
    /// Converts a normalized double array to an RGBColor.
    /// </summary>
    /// <param name="normalized">Normalized array [A, R, G, B] or similar.</param>
    /// <returns>The RGBColor.</returns>
    public static RGBColor ToRgbColor(this double[] normalized)
    
    /// <summary>
    /// Converts an RGBColor to LabColor using the provided converter.
    /// </summary>
    /// <param name="color">The RGBColor.</param>
    /// <param name="converter">The converter instance.</param>
    /// <returns>The LabColor.</returns>
    public static LabColor ToLabColor(this RGBColor color, RGBToLabConverter converter)
    
    /// <summary>
    /// Converts a LabColor to RGBColor using the provided converter.
    /// </summary>
    /// <param name="color">The LabColor.</param>
    /// <param name="converter">The converter instance.</param>
    /// <returns>The RGBColor.</returns>
    public static RGBColor ToRgbColor(this LabColor color, RGBToLabConverter converter)
    
    /// <summary>
    /// Adjusts a LabColor to be more pastel-like by modifying lightness and saturation.
    /// </summary>
    /// <param name="color">The original LabColor.</param>
    /// <param name="lightness">The lightness to add.</param>
    /// <param name="saturation">The saturation multiplier.</param>
    /// <returns>The pastel LabColor.</returns>
    public static LabColor ToPastel(this LabColor color, double lightness = 20.0d, double saturation = 0.5d)
    
    /// <summary>
    /// Converts a list of Avalonia Colors to RGBColors.
    /// </summary>
    /// <param name="color">The list of Avalonia Colors.</param>
    /// <returns>A list of RGBColors.</returns>
    public static AList<RGBColor> ToRgbColor(this AList<global::Avalonia.Media.Color> color)
    
    /// <summary>
    /// Converts a list of RGBColors to LabColors using the provided converter.
    /// </summary>
    /// <param name="colors">The list of RGBColors.</param>
    /// <param name="converter">The converter instance.</param>
    /// <returns>A list of LabColors.</returns>
    public static AList<LabColor> ToLabColor(this AList<RGBColor> colors, RGBToLabConverter converter)
    
    /// <summary>
    /// Removes default LabColor (0,0,0) values from an array.
    /// </summary>
    /// <param name="colors">The source array.</param>
    /// <returns>An array with default values removed.</returns>
    public static LabColor[] RemoveNullValues(this LabColor[] colors)
}
```

## Processing

### ImagePreProcessor

```csharp
/// <summary>
/// Provides image pre-processing functionality, such as blurring.
/// </summary>
public class ImagePreProcessor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImagePreProcessor"/> class.
    /// </summary>
    /// <param name="sigma">The Gaussian blur sigma value.</param>
    /// <param name="rounds">The number of blur iterations.</param>
    public ImagePreProcessor(float sigma, int rounds = 10)
    
    /// <summary>
    /// Processes an Avalonia Bitmap by applying Gaussian blur.
    /// </summary>
    /// <param name="bitmap">The source bitmap.</param>
    /// <returns>The processed bitmap.</returns>
    public global::Avalonia.Media.Imaging.Bitmap Process(global::Avalonia.Media.Imaging.Bitmap bitmap)
}
```

# DevBase.Cryptography Project Documentation

This document contains all class, method, and field signatures with their corresponding comments for the DevBase.Cryptography project.

## Table of Contents

- [Blowfish](#blowfish)
  - [Blowfish](#blowfish-class)
  - [Codec](#codec)
  - [Extensions](#extensions)
  - [Init](#init)
- [MD5](#md5)
  - [MD5](#md5-class)

## Blowfish

### Blowfish (class)

```csharp
// This is the Blowfish CBC implementation from https://github.com/jdvor/encryption-blowfish
// This is NOT my code I just want to add it to my ecosystem to avoid too many libraries.

/// <summary>
/// Blowfish in CBC (cipher block chaining) block mode.
/// </summary>
public sealed class Blowfish
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Blowfish"/> class using a pre-configured codec.
    /// </summary>
    /// <param name="codec">The codec instance to use for encryption/decryption.</param>
    public Blowfish(Codec codec)
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Blowfish"/> class with the specified key.
    /// </summary>
    /// <param name="key">The encryption key.</param>
    public Blowfish(byte[] key)
    
    /// <summary>
    /// Encrypt data.
    /// </summary>
    /// <param name="data">the length must be in multiples of 8</param>
    /// <param name="initVector">IV; the length must be exactly 8</param>
    /// <returns><code>true</code> if data has been encrypted; otherwise <code>false</code>.</returns>
    public bool Encrypt(Span<byte> data, ReadOnlySpan<byte> initVector)
    
    /// <summary>
    /// Decrypt data.
    /// </summary>
    /// <param name="data">the length must be in multiples of 8</param>
    /// <param name="initVector">IV; the length must be exactly 8</param>
    /// <returns><code>true</code> if data has been decrypted; otherwise <code>false</code>.</returns>
    public bool Decrypt(Span<byte> data, ReadOnlySpan<byte> initVector)
}
```

### Codec

```csharp
/// <summary>
/// Blowfish encryption and decryption on fixed size (length = 8) data block.
/// Codec is a relatively expensive object, because it must construct P-array and S-blocks from provided key.
/// It is expected to be used many times and it is thread-safe.
/// </summary>
public sealed class Codec
{
    /// <summary>
    /// Create codec instance and compute P-array and S-blocks.
    /// </summary>
    /// <param name="key">cipher key; valid size is &lt;8, 448&gt;</param>
    /// <exception cref="ArgumentException">on invalid input</exception>
    public Codec(byte[] key)
    
    /// <summary>
    /// Encrypt data block.
    /// There are no range checks within the method and it is expected that the caller will ensure big enough block.
    /// </summary>
    /// <param name="block">only first 8 bytes are encrypted</param>
    public void Encrypt(Span<byte> block)
    
    /// <summary>
    /// Encrypt data block.
    /// There are no range checks within the method and it is expected that the caller will ensure big enough block.
    /// </summary>
    /// <param name="offset">start encryption at this index of the data buffer</param>
    /// <param name="data">only first 8 bytes are encrypted from the offset</param>
    public void Encrypt(int offset, byte[] data)
    
    /// <summary>
    /// Decrypt data block.
    /// There are no range checks within the method and it is expected that the caller will ensure big enough block.
    /// </summary>
    /// <param name="block">only first 8 bytes are decrypted</param>
    public void Decrypt(Span<byte> block)
    
    /// <summary>
    /// Decrypt data block.
    /// There are no range checks within the method and it is expected that the caller will ensure big enough block.
    /// </summary>
    /// <param name="offset">start decryption at this index of the data buffer</param>
    /// <param name="data">only first 8 bytes are decrypted from the offset</param>
    public void Decrypt(int offset, byte[] data)
}
```

### Extensions

```csharp
public static class Extensions
{
    /// <summary>
    /// Return closest number divisible by 8 without remainder, which is equal or larger than original length.
    /// </summary>
    public static int PaddedLength(int originalLength)
    
    /// <summary>
    /// Return if the data block has length in multiples of 8.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmptyOrNotPadded(Span<byte> data)
    
    /// <summary>
    /// Return same array if its length is multiple of 8; otherwise create new array with adjusted length
    /// and copy original array at the beginning.
    /// </summary>
    public static byte[] CopyAndPadIfNotAlreadyPadded(this byte[] data)
    
    /// <summary>
    /// Format data block as hex string with optional formatting. Each byte is represented as two characters [0-9A-F].
    /// </summary>
    /// <param name="data">the data block</param>
    /// <param name="pretty">
    /// if <code>true</code> it will enable additional formatting; otherwise the bytes are placed on one line
    /// without separator. The default is <code>true</code>.
    /// </param>
    /// <param name="bytesPerLine">how many bytes to put on a line</param>
    /// <param name="byteSep">separate bytes with this string</param>
    /// <returns></returns>
    public static string ToHexString(
        this Span<byte> data, bool pretty = true, int bytesPerLine = 8, string byteSep = "")
}
```

### Init

```csharp
internal static class Init
{
    /// <summary>
    /// The 18-entry P-array.
    /// </summary>
    internal static uint[] P()
    
    /// <summary>
    /// The 256-entry S0 box.
    /// </summary>
    internal static uint[] S0()
    
    /// <summary>
    /// The 256-entry S1 box.
    /// </summary>
    internal static uint[] S1()
    
    /// <summary>
    /// The 256-entry S2 box.
    /// </summary>
    internal static uint[] S2()
    
    /// <summary>
    /// The 256-entry S3 box.
    /// </summary>
    internal static uint[] S3()
}
```

## MD5

### MD5 (class)

```csharp
/// <summary>
/// Provides methods for calculating MD5 hashes.
/// </summary>
public class MD5
{
    /// <summary>
    /// Computes the MD5 hash of the given string and returns it as a byte array.
    /// </summary>
    /// <param name="data">The input string to hash.</param>
    /// <returns>The MD5 hash as a byte array.</returns>
    public static byte[] ToMD5Binary(string data)
    
    /// <summary>
    /// Computes the MD5 hash of the given string and returns it as a hexadecimal string.
    /// </summary>
    /// <param name="data">The input string to hash.</param>
    /// <returns>The MD5 hash as a hexadecimal string.</returns>
    public static string ToMD5String(string data)
    
    /// <summary>
    /// Computes the MD5 hash of the given byte array and returns it as a hexadecimal string.
    /// </summary>
    /// <param name="data">The input byte array to hash.</param>
    /// <returns>The MD5 hash as a hexadecimal string.</returns>
    public static string ToMD5(byte[] data)
}
```

# DevBase.Cryptography.BouncyCastle Project Documentation

This document contains all class, method, and field signatures with their corresponding comments for the DevBase.Cryptography.BouncyCastle project.

## Table of Contents

- [AES](#aes)
  - [AESBuilderEngine](#aesbuilderengine)
- [ECDH](#ecdh)
  - [EcdhEngineBuilder](#ecdhenginebuilder)
- [Exception](#exception)
  - [KeypairNotFoundException](#keypairnotfoundexception)
- [Extensions](#extensions)
  - [AsymmetricKeyParameterExtension](#asymmetrickeyparameterextension)
- [Hashing](#hashing)
  - [AsymmetricTokenVerifier](#asymmetrictokenverifier)
  - [SymmetricTokenVerifier](#symmetrictokenverifier)
  - [Verification](#verification)
    - [EsTokenVerifier](#estokenverifier)
    - [PsTokenVerifier](#pstokenverifier)
    - [RsTokenVerifier](#rstokenverifier)
    - [ShaTokenVerifier](#shatokenverifier)
- [Identifier](#identifier)
  - [Identification](#identification)
- [Random](#random)
  - [Random](#random-class)
- [Sealing](#sealing)
  - [Sealing](#sealing-class)

## AES

### AESBuilderEngine

```csharp
/// <summary>
/// Provides AES encryption and decryption functionality using GCM mode.
/// </summary>
public class AESBuilderEngine
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AESBuilderEngine"/> class with a random key.
    /// </summary>
    public AESBuilderEngine()
    
    /// <summary>
    /// Encrypts the specified buffer using AES-GCM.
    /// </summary>
    /// <param name="buffer">The data to encrypt.</param>
    /// <returns>A byte array containing the nonce followed by the encrypted data.</returns>
    public byte[] Encrypt(byte[] buffer)
    
    /// <summary>
    /// Decrypts the specified buffer using AES-GCM.
    /// </summary>
    /// <param name="buffer">The data to decrypt, expected to contain the nonce followed by the ciphertext.</param>
    /// <returns>The decrypted data.</returns>
    public byte[] Decrypt(byte[] buffer)
    
    /// <summary>
    /// Encrypts the specified string using AES-GCM and returns the result as a Base64 string.
    /// </summary>
    /// <param name="data">The string to encrypt.</param>
    /// <returns>The encrypted data as a Base64 string.</returns>
    public string EncryptString(string data)
    
    /// <summary>
    /// Decrypts the specified Base64 encoded string using AES-GCM.
    /// </summary>
    /// <param name="encryptedData">The Base64 encoded encrypted data.</param>
    /// <returns>The decrypted string.</returns>
    public string DecryptString(string encryptedData)
    
    /// <summary>
    /// Sets the encryption key.
    /// </summary>
    /// <param name="key">The key as a byte array.</param>
    /// <returns>The current instance of <see cref="AESBuilderEngine"/>.</returns>
    public AESBuilderEngine SetKey(byte[] key)
    
    /// <summary>
    /// Sets the encryption key from a Base64 encoded string.
    /// </summary>
    /// <param name="key">The Base64 encoded key.</param>
    /// <returns>The current instance of <see cref="AESBuilderEngine"/>.</returns>
    public AESBuilderEngine SetKey(string key)
    
    /// <summary>
    /// Sets a random encryption key.
    /// </summary>
    /// <returns>The current instance of <see cref="AESBuilderEngine"/>.</returns>
    public AESBuilderEngine SetRandomKey()
    
    /// <summary>
    /// Sets the seed for the random number generator.
    /// </summary>
    /// <param name="seed">The seed as a byte array.</param>
    /// <returns>The current instance of <see cref="AESBuilderEngine"/>.</returns>
    public AESBuilderEngine SetSeed(byte[] seed)
    
    /// <summary>
    /// Sets the seed for the random number generator from a string.
    /// </summary>
    /// <param name="seed">The seed string.</param>
    /// <returns>The current instance of <see cref="AESBuilderEngine"/>.</returns>
    public AESBuilderEngine SetSeed(string seed)
    
    /// <summary>
    /// Sets a random seed for the random number generator.
    /// </summary>
    /// <returns>The current instance of <see cref="AESBuilderEngine"/>.</returns>
    public AESBuilderEngine SetRandomSeed()
}
```

## ECDH

### EcdhEngineBuilder

```csharp
/// <summary>
/// Provides functionality for building and managing ECDH (Elliptic Curve Diffie-Hellman) key pairs and shared secrets.
/// </summary>
public class EcdhEngineBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EcdhEngineBuilder"/> class.
    /// </summary>
    public EcdhEngineBuilder()
    
    /// <summary>
    /// Generates a new ECDH key pair using the secp256r1 curve.
    /// </summary>
    /// <returns>The current instance of <see cref="EcdhEngineBuilder"/>.</returns>
    public EcdhEngineBuilder GenerateKeyPair()
    
    /// <summary>
    /// Loads an existing ECDH key pair from byte arrays.
    /// </summary>
    /// <param name="publicKey">The public key bytes.</param>
    /// <param name="privateKey">The private key bytes.</param>
    /// <returns>The current instance of <see cref="EcdhEngineBuilder"/>.</returns>
    public EcdhEngineBuilder FromExistingKeyPair(byte[] publicKey, byte[] privateKey)
    
    /// <summary>
    /// Loads an existing ECDH key pair from Base64 encoded strings.
    /// </summary>
    /// <param name="publicKey">The Base64 encoded public key.</param>
    /// <param name="privateKey">The Base64 encoded private key.</param>
    /// <returns>The current instance of <see cref="EcdhEngineBuilder"/>.</returns>
    public EcdhEngineBuilder FromExistingKeyPair(string publicKey, string privateKey)
    
    /// <summary>
    /// Derives a shared secret from the current private key and the provided public key.
    /// </summary>
    /// <param name="publicKey">The other party's public key.</param>
    /// <returns>The derived shared secret as a byte array.</returns>
    /// <exception cref="KeypairNotFoundException">Thrown if no key pair has been generated or loaded.</exception>
    public byte[] DeriveKeyPairs(AsymmetricKeyParameter publicKey)
    
    /// <summary>
    /// Gets the public key of the current key pair.
    /// </summary>
    public AsymmetricKeyParameter PublicKey { get; }
    
    /// <summary>
    /// Gets the private key of the current key pair.
    /// </summary>
    public AsymmetricKeyParameter PrivateKey { get; }
}
```

## Exception

### KeypairNotFoundException

```csharp
/// <summary>
/// Exception thrown when a key pair operation is attempted but no key pair is found.
/// </summary>
public class KeypairNotFoundException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeypairNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public KeypairNotFoundException(string message)
}
```

## Extensions

### AsymmetricKeyParameterExtension

```csharp
/// <summary>
/// Provides extension methods for converting asymmetric key parameters to and from byte arrays.
/// </summary>
public static class AsymmetricKeyParameterExtension
{
    /// <summary>
    /// Converts an asymmetric public key parameter to its DER encoded byte array representation.
    /// </summary>
    /// <param name="keyParameter">The public key parameter.</param>
    /// <returns>The DER encoded byte array.</returns>
    /// <exception cref="ArgumentException">Thrown if the public key type is not supported.</exception>
    public static byte[] PublicKeyToArray(this AsymmetricKeyParameter keyParameter)
    
    /// <summary>
    /// Converts an asymmetric private key parameter to its unsigned byte array representation.
    /// </summary>
    /// <param name="keyParameter">The private key parameter.</param>
    /// <returns>The unsigned byte array representation of the private key.</returns>
    /// <exception cref="ArgumentException">Thrown if the private key type is not supported.</exception>
    public static byte[] PrivateKeyToArray(this AsymmetricKeyParameter keyParameter)
    
    /// <summary>
    /// Converts a byte array to an ECDH public key parameter using the secp256r1 curve.
    /// </summary>
    /// <param name="keySequence">The byte array representing the public key.</param>
    /// <returns>The ECDH public key parameter.</returns>
    /// <exception cref="ArgumentException">Thrown if the byte array is invalid.</exception>
    public static AsymmetricKeyParameter ToEcdhPublicKey(this byte[] keySequence)
    
    /// <summary>
    /// Converts a byte array to an ECDH private key parameter using the secp256r1 curve.
    /// </summary>
    /// <param name="keySequence">The byte array representing the private key.</param>
    /// <returns>The ECDH private key parameter.</returns>
    /// <exception cref="ArgumentException">Thrown if the byte array is invalid.</exception>
    public static AsymmetricKeyParameter ToEcdhPrivateKey(this byte[] keySequence)
}
```

## Hashing

### AsymmetricTokenVerifier

```csharp
/// <summary>
/// Abstract base class for verifying asymmetric signatures of tokens.
/// </summary>
public abstract class AsymmetricTokenVerifier
{
    /// <summary>
    /// Gets or sets the encoding used for the token parts. Defaults to UTF-8.
    /// </summary>
    public Encoding Encoding { get; set; }
    
    /// <summary>
    /// Verifies the signature of a token.
    /// </summary>
    /// <param name="header">The token header.</param>
    /// <param name="payload">The token payload.</param>
    /// <param name="signature">The token signature (Base64Url encoded).</param>
    /// <param name="publicKey">The public key to use for verification.</param>
    /// <returns><c>true</c> if the signature is valid; otherwise, <c>false</c>.</returns>
    public bool VerifySignature(string header, string payload, string signature, string publicKey)
    
    /// <summary>
    /// Verifies the signature of the content bytes using the provided public key.
    /// </summary>
    /// <param name="content">The content bytes (header + "." + payload).</param>
    /// <param name="signature">The signature bytes.</param>
    /// <param name="publicKey">The public key.</param>
    /// <returns><c>true</c> if the signature is valid; otherwise, <c>false</c>.</returns>
    protected abstract bool VerifySignature(byte[] content, byte[] signature, string publicKey);
}
```

### SymmetricTokenVerifier

```csharp
/// <summary>
/// Abstract base class for verifying symmetric signatures of tokens.
/// </summary>
public abstract class SymmetricTokenVerifier
{
    /// <summary>
    /// Gets or sets the encoding used for the token parts. Defaults to UTF-8.
    /// </summary>
    public Encoding Encoding { get; set; }
    
    /// <summary>
    /// Verifies the signature of a token.
    /// </summary>
    /// <param name="header">The token header.</param>
    /// <param name="payload">The token payload.</param>
    /// <param name="signature">The token signature (Base64Url encoded).</param>
    /// <param name="secret">The shared secret used for verification.</param>
    /// <param name="isSecretEncoded">Indicates whether the secret string is Base64Url encoded.</param>
    /// <returns><c>true</c> if the signature is valid; otherwise, <c>false</c>.</returns>
    public bool VerifySignature(string header, string payload, string signature, string secret, bool isSecretEncoded = false)
    
    /// <summary>
    /// Verifies the signature of the content bytes using the provided secret.
    /// </summary>
    /// <param name="content">The content bytes (header + "." + payload).</param>
    /// <param name="signature">The signature bytes.</param>
    /// <param name="secret">The secret bytes.</param>
    /// <returns><c>true</c> if the signature is valid; otherwise, <c>false</c>.</returns>
    protected abstract bool VerifySignature(byte[] content, byte[] signature, byte[] secret);
}
```

### Verification

#### EsTokenVerifier

```csharp
/// <summary>
/// Verifies ECDSA signatures for tokens.
/// </summary>
/// <typeparam name="T">The digest algorithm to use (e.g., SHA256).</typeparam>
public class EsTokenVerifier<T> : AsymmetricTokenVerifier where T : IDigest
{
    /// <inheritdoc />
    protected override bool VerifySignature(byte[] content, byte[] signature, string publicKey)
    
    /// <summary>
    /// Converts a P1363 signature format to ASN.1 DER format.
    /// </summary>
    /// <param name="p1363Signature">The P1363 signature bytes.</param>
    /// <returns>The ASN.1 DER encoded signature.</returns>
    private byte[] ToAsn1Der(byte[] p1363Signature)
}
```

#### PsTokenVerifier

```csharp
/// <summary>
/// Verifies RSASSA-PSS signatures for tokens.
/// </summary>
/// <typeparam name="T">The digest algorithm to use (e.g., SHA256).</typeparam>
public class PsTokenVerifier<T> : AsymmetricTokenVerifier where T : IDigest
{
    /// <inheritdoc />
    protected override bool VerifySignature(byte[] content, byte[] signature, string publicKey)
}
```

#### RsTokenVerifier

```csharp
/// <summary>
/// Verifies RSASSA-PKCS1-v1_5 signatures for tokens.
/// </summary>
/// <typeparam name="T">The digest algorithm to use (e.g., SHA256).</typeparam>
public class RsTokenVerifier<T> : AsymmetricTokenVerifier where T : IDigest
{
    /// <inheritdoc />
    protected override bool VerifySignature(byte[] content, byte[] signature, string publicKey)
}
```

#### ShaTokenVerifier

```csharp
/// <summary>
/// Verifies HMAC-SHA signatures for tokens.
/// </summary>
/// <typeparam name="T">The digest algorithm to use (e.g., SHA256).</typeparam>
public class ShaTokenVerifier<T> : SymmetricTokenVerifier where T : IDigest
{
    /// <inheritdoc />
    protected override bool VerifySignature(byte[] content, byte[] signature, byte[] secret)
}
```

## Identifier

### Identification

```csharp
/// <summary>
/// Provides methods for generating random identification strings.
/// </summary>
public class Identification
{
    /// <summary>
    /// Generates a random hexadecimal ID string.
    /// </summary>
    /// <param name="size">The number of bytes to generate for the ID. Defaults to 20.</param>
    /// <param name="seed">Optional seed for the random number generator.</param>
    /// <returns>A random hexadecimal string.</returns>
    public static string GenerateRandomId(int size = 20, byte[] seed = null)
}
```

## Random

### Random (class)

```csharp
/// <summary>
/// Provides secure random number generation functionality.
/// </summary>
public class Random
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Random"/> class.
    /// </summary>
    public Random()
    
    /// <summary>
    /// Generates a specified number of random bytes.
    /// </summary>
    /// <param name="size">The number of bytes to generate.</param>
    /// <returns>An array containing the random bytes.</returns>
    public byte[] GenerateRandomBytes(int size)
    
    /// <summary>
    /// Generates a random string of the specified length using a given character set.
    /// </summary>
    /// <param name="length">The length of the string to generate.</param>
    /// <param name="charset">The character set to use. Defaults to alphanumeric characters and some symbols.</param>
    /// <returns>The generated random string.</returns>
    public string RandomString(int length, string charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_")
    
    /// <summary>
    /// Generates a random Base64 string of a specified byte length.
    /// </summary>
    /// <param name="length">The number of random bytes to generate before encoding.</param>
    /// <returns>A Base64 encoded string of random bytes.</returns>
    public string RandomBase64(int length)
    
    /// <summary>
    /// Generates a random integer.
    /// </summary>
    /// <returns>A random integer.</returns>
    public int RandomInt()
    
    /// <summary>
    /// Sets the seed for the random number generator using a long value.
    /// </summary>
    /// <param name="seed">The seed value.</param>
    /// <returns>The current instance of <see cref="Random"/>.</returns>
    public Random SetSeed(long seed)
    
    /// <summary>
    /// Sets the seed for the random number generator using a byte array.
    /// </summary>
    /// <param name="seed">The seed bytes.</param>
    /// <returns>The current instance of <see cref="Random"/>.</returns>
    public Random SetSeed(byte[] seed)
}
```

## Sealing

### Sealing (class)

```csharp
/// <summary>
/// Provides functionality for sealing and unsealing messages using hybrid encryption (ECDH + AES).
/// </summary>
public class Sealing
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Sealing"/> class for sealing messages to a recipient.
    /// </summary>
    /// <param name="othersPublicKey">The recipient's public key.</param>
    public Sealing(byte[] othersPublicKey)
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Sealing"/> class for sealing messages to a recipient using Base64 encoded public key.
    /// </summary>
    /// <param name="othersPublicKey">The recipient's Base64 encoded public key.</param>
    public Sealing(string othersPublicKey)
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Sealing"/> class for unsealing messages.
    /// </summary>
    /// <param name="publicKey">The own public key.</param>
    /// <param name="privateKey">The own private key.</param>
    public Sealing(byte[] publicKey, byte[] privateKey)
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Sealing"/> class for unsealing messages using Base64 encoded keys.
    /// </summary>
    /// <param name="publicKey">The own Base64 encoded public key.</param>
    /// <param name="privateKey">The own Base64 encoded private key.</param>
    public Sealing(string publicKey, string privateKey)
    
    /// <summary>
    /// Seals (encrypts) a message.
    /// </summary>
    /// <param name="unsealedMessage">The message to seal.</param>
    /// <returns>A byte array containing the sender's public key length, public key, and the encrypted message.</returns>
    public byte[] Seal(byte[] unsealedMessage)
    
    /// <summary>
    /// Seals (encrypts) a string message.
    /// </summary>
    /// <param name="unsealedMessage">The string message to seal.</param>
    /// <returns>A Base64 string containing the sealed message.</returns>
    public string Seal(string unsealedMessage)
    
    /// <summary>
    /// Unseals (decrypts) a message.
    /// </summary>
    /// <param name="sealedMessage">The sealed message bytes.</param>
    /// <returns>The unsealed (decrypted) message bytes.</returns>
    public byte[] UnSeal(byte[] sealedMessage)
    
    /// <summary>
    /// Unseals (decrypts) a Base64 encoded message string.
    /// </summary>
    /// <param name="sealedMessage">The Base64 encoded sealed message.</param>
    /// <returns>The unsealed (decrypted) string message.</returns>
    public string UnSeal(string sealedMessage)
}
```

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

# DevBase.Format Project Documentation

This document contains all class, method, and field signatures with their corresponding comments for the DevBase.Format project.

## Table of Contents

- [Exceptions](#exceptions)
  - [ParsingException](#parsingexception)
- [Core](#core)
  - [FileFormat&lt;F, T&gt;](#fileformatf-t)
  - [FileParser&lt;P, T&gt;](#fileparserp-t)
- [Extensions](#extensions)
  - [LyricsExtensions](#lyricsextensions)
- [Structure](#structure)
  - [RawLyric](#rawlyric)
  - [RegexHolder](#regexholder)
  - [RichTimeStampedLyric](#richtimestampedlyric)
  - [RichTimeStampedWord](#richtimestampedword)
  - [TimeStampedLyric](#timestampedlyric)
- [Formats](#formats)
  - [Format Parsers Overview](#format-parsers-overview)

## Exceptions

### ParsingException

```csharp
/// <summary>
/// Exception thrown when a parsing error occurs.
/// </summary>
public class ParsingException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParsingException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ParsingException(string message)
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ParsingException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public ParsingException(string message, System.Exception innerException)
}
```

## Core

### FileFormat&lt;F, T&gt;

```csharp
/// <summary>
/// Base class for defining file formats and their parsing logic.
/// </summary>
/// <typeparam name="F">The type of the input format (e.g., string, byte[]).</typeparam>
/// <typeparam name="T">The type of the parsed result.</typeparam>
public abstract class FileFormat<F, T>
{
    /// <summary>
    /// Gets or sets a value indicating whether strict error handling is enabled.
    /// If true, exceptions are thrown on errors; otherwise, default values are returned.
    /// </summary>
    public bool StrictErrorHandling { get; set; }
    
    /// <summary>
    /// Parses the input into the target type.
    /// </summary>
    /// <param name="from">The input data to parse.</param>
    /// <returns>The parsed object of type <typeparamref name="T"/>.</returns>
    public abstract T Parse(F from)
    
    /// <summary>
    /// Attempts to parse the input into the target type.
    /// </summary>
    /// <param name="from">The input data to parse.</param>
    /// <param name="parsed">The parsed object, or default if parsing fails.</param>
    /// <returns>True if parsing was successful; otherwise, false.</returns>
    public abstract bool TryParse(F from, out T parsed)
    
    /// <summary>
    /// Handles errors during parsing. Throws an exception if strict error handling is enabled.
    /// </summary>
    /// <typeparam name="TX">The return type (usually nullable or default).</typeparam>
    /// <param name="message">The error message.</param>
    /// <param name="callerMember">The calling member name.</param>
    /// <param name="callerFilePath">The source file path.</param>
    /// <param name="callerLineNumber">The source line number.</param>
    /// <returns>The default value of <typeparamref name="TX"/> if strict error handling is disabled.</returns>
    protected dynamic Error<TX>(string message, [CallerMemberName] string callerMember = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
    
    /// <summary>
    /// Handles exceptions during parsing. Rethrows wrapped in a ParsingException if strict error handling is enabled.
    /// </summary>
    /// <typeparam name="TX">The return type.</typeparam>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="callerMember">The calling member name.</param>
    /// <param name="callerFilePath">The source file path.</param>
    /// <param name="callerLineNumber">The source line number.</param>
    /// <returns>The default value of <typeparamref name="TX"/> if strict error handling is disabled.</returns>
    protected dynamic Error<TX>(System.Exception exception, [CallerMemberName] string callerMember = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
}
```

### FileParser&lt;P, T&gt;

```csharp
/// <summary>
/// Provides high-level parsing functionality using a specific file format.
/// </summary>
/// <typeparam name="P">The specific file format implementation.</typeparam>
/// <typeparam name="T">The result type of the parsing.</typeparam>
public class FileParser<P, T> where P : FileFormat<string, T>
{
    /// <summary>
    /// Parses content from a string.
    /// </summary>
    /// <param name="content">The string content to parse.</param>
    /// <returns>The parsed object.</returns>
    public T ParseFromString(string content)
    
    /// <summary>
    /// Attempts to parse content from a string.
    /// </summary>
    /// <param name="content">The string content to parse.</param>
    /// <param name="parsed">The parsed object, or default on failure.</param>
    /// <returns>True if parsing was successful; otherwise, false.</returns>
    public bool TryParseFromString(string content, out T parsed)
    
    /// <summary>
    /// Parses content from a file on disk.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <returns>The parsed object.</returns>
    public T ParseFromDisk(string filePath)
    
    /// <summary>
    /// Attempts to parse content from a file on disk.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <param name="parsed">The parsed object, or default on failure.</param>
    /// <returns>True if parsing was successful; otherwise, false.</returns>
    public bool TryParseFromDisk(string filePath, out T parsed)
    
    /// <summary>
    /// Parses content from a file on disk using a FileInfo object.
    /// </summary>
    /// <param name="fileInfo">The FileInfo object representing the file.</param>
    /// <returns>The parsed object.</returns>
    public T ParseFromDisk(FileInfo fileInfo)
}
```

## Extensions

### LyricsExtensions

```csharp
/// <summary>
/// Provides extension methods for converting between different lyric structures and text formats.
/// </summary>
public static class LyricsExtensions
{
    /// <summary>
    /// Converts a list of raw lyrics to a plain text string.
    /// </summary>
    /// <param name="rawElements">The list of raw lyrics.</param>
    /// <returns>A string containing the lyrics.</returns>
    public static string ToPlainText(this AList<RawLyric> rawElements)
    
    /// <summary>
    /// Converts a list of time-stamped lyrics to a plain text string.
    /// </summary>
    /// <param name="elements">The list of time-stamped lyrics.</param>
    /// <returns>A string containing the lyrics.</returns>
    public static string ToPlainText(this AList<TimeStampedLyric> elements)
    
    /// <summary>
    /// Converts a list of rich time-stamped lyrics to a plain text string.
    /// </summary>
    /// <param name="richElements">The list of rich time-stamped lyrics.</param>
    /// <returns>A string containing the lyrics.</returns>
    public static string ToPlainText(this AList<RichTimeStampedLyric> richElements)
    
    /// <summary>
    /// Converts a list of time-stamped lyrics to raw lyrics (removing timestamps).
    /// </summary>
    /// <param name="timeStampedLyrics">The list of time-stamped lyrics.</param>
    /// <returns>A list of raw lyrics.</returns>
    public static AList<RawLyric> ToRawLyrics(this AList<TimeStampedLyric> timeStampedLyrics)
    
    /// <summary>
    /// Converts a list of rich time-stamped lyrics to raw lyrics (removing timestamps and extra data).
    /// </summary>
    /// <param name="richTimeStampedLyrics">The list of rich time-stamped lyrics.</param>
    /// <returns>A list of raw lyrics.</returns>
    public static AList<RawLyric> ToRawLyrics(this AList<RichTimeStampedLyric> richTimeStampedLyrics)
    
    /// <summary>
    /// Converts a list of rich time-stamped lyrics to standard time-stamped lyrics (simplifying the structure).
    /// </summary>
    /// <param name="richElements">The list of rich time-stamped lyrics.</param>
    /// <returns>A list of time-stamped lyrics.</returns>
    public static AList<TimeStampedLyric> ToTimeStampedLyrics(this AList<RichTimeStampedLyric> richElements)
}
```

## Structure

### RawLyric

```csharp
/// <summary>
/// Represents a basic lyric line without timestamps.
/// </summary>
public class RawLyric
{
    /// <summary>
    /// Gets or sets the text of the lyric line.
    /// </summary>
    public string Text { get; set; }
}
```

### RegexHolder

```csharp
/// <summary>
/// Holds compiled Regular Expressions for various lyric formats.
/// </summary>
public class RegexHolder
{
    /// <summary>Regex pattern for standard LRC format.</summary>
    public const string REGEX_LRC = "((\\[)([0-9]*)([:])([0-9]*)([:]|[.])(\\d+\\.\\d+|\\d+)(\\]))((\\s|.).*$)";
    /// <summary>Regex pattern for garbage/metadata lines.</summary>
    public const string REGEX_GARBAGE = "\\D(\\?{0,2}).([:]).([\\w /]*)";
    /// <summary>Regex pattern for environment variables/metadata.</summary>
    public const string REGEX_ENV = "(\\w*)\\=\"(\\w*)";
    /// <summary>Regex pattern for SRT timestamps.</summary>
    public const string REGEX_SRT_TIMESTAMPS = "([0-9:,]*)(\\W(-->)\\W)([0-9:,]*)";
    /// <summary>Regex pattern for Enhanced LRC (ELRC) format data.</summary>
    public const string REGEX_ELRC_DATA = "(\\[)([0-9]*)([:])([0-9]*)([:])(\\d+\\.\\d+|\\d+)(\\])(\\s-\\s)(\\[)([0-9]*)([:])([0-9]*)([:])(\\d+\\.\\d+|\\d+)(\\])\\s(.*$)";
    /// <summary>Regex pattern for KLyrics word format.</summary>
    public const string REGEX_KLYRICS_WORD = "(\\()([0-9]*)(\\,)([0-9]*)(\\))([^\\(\\)\\[\\]\\n]*)";
    /// <summary>Regex pattern for KLyrics timestamp format.</summary>
    public const string REGEX_KLYRICS_TIMESTAMPS = "(\\[)([0-9]*)(\\,)([0-9]*)(\\])";

    /// <summary>Compiled Regex for standard LRC format.</summary>
    public static Regex RegexLrc { get; }
    /// <summary>Compiled Regex for garbage/metadata lines.</summary>
    public static Regex RegexGarbage { get; }
    /// <summary>Compiled Regex for environment variables/metadata.</summary>
    public static Regex RegexEnv { get; }
    /// <summary>Compiled Regex for SRT timestamps.</summary>
    public static Regex RegexSrtTimeStamps { get; }
    /// <summary>Compiled Regex for Enhanced LRC (ELRC) format data.</summary>
    public static Regex RegexElrc { get; }
    /// <summary>Compiled Regex for KLyrics word format.</summary>
    public static Regex RegexKlyricsWord { get; }
    /// <summary>Compiled Regex for KLyrics timestamp format.</summary>
    public static Regex RegexKlyricsTimeStamps { get; }
}
```

### RichTimeStampedLyric

```csharp
/// <summary>
/// Represents a lyric line with start/end times and individual word timestamps.
/// </summary>
public class RichTimeStampedLyric
{
    /// <summary>
    /// Gets or sets the full text of the lyric line.
    /// </summary>
    public string Text { get; set; }
    
    /// <summary>
    /// Gets or sets the start time of the lyric line.
    /// </summary>
    public TimeSpan StartTime { get; set; }
    
    /// <summary>
    /// Gets or sets the end time of the lyric line.
    /// </summary>
    public TimeSpan EndTime { get; set; }
    
    /// <summary>
    /// Gets the start timestamp in total milliseconds.
    /// </summary>
    public long StartTimestamp { get; }
    
    /// <summary>
    /// Gets the end timestamp in total milliseconds.
    /// </summary>
    public long EndTimestamp { get; }
    
    /// <summary>
    /// Gets or sets the list of words with their own timestamps within this line.
    /// </summary>
    public AList<RichTimeStampedWord> Words { get; set; }
}
```

### RichTimeStampedWord

```csharp
/// <summary>
/// Represents a single word in a lyric with start and end times.
/// </summary>
public class RichTimeStampedWord
{
    /// <summary>
    /// Gets or sets the word text.
    /// </summary>
    public string Word { get; set; }
    
    /// <summary>
    /// Gets or sets the start time of the word.
    /// </summary>
    public TimeSpan StartTime { get; set; }
    
    /// <summary>
    /// Gets or sets the end time of the word.
    /// </summary>
    public TimeSpan EndTime { get; set; }
    
    /// <summary>
    /// Gets the start timestamp in total milliseconds.
    /// </summary>
    public long StartTimestamp { get; }
    
    /// <summary>
    /// Gets the end timestamp in total milliseconds.
    /// </summary>
    public long EndTimestamp { get; }
}
```

### TimeStampedLyric

```csharp
/// <summary>
/// Represents a lyric line with a start time.
/// </summary>
public class TimeStampedLyric
{
    /// <summary>
    /// Gets or sets the text of the lyric line.
    /// </summary>
    public string Text { get; set; }
    
    /// <summary>
    /// Gets or sets the start time of the lyric line.
    /// </summary>
    public TimeSpan StartTime { get; set; }

    /// <summary>
    /// Gets the start timestamp in total milliseconds.
    /// </summary>
    public long StartTimestamp { get; }
}
```

## Formats

### Format Parsers Overview

The DevBase.Format project includes various format parsers:

- **LrcParser** - Parses standard LRC format into `AList<TimeStampedLyric>`
- **ElrcParser** - Parses enhanced LRC format into `AList<RichTimeStampedLyric>`
- **KLyricsParser** - Parses KLyrics format into `AList<RichTimeStampedLyric>`
- **SrtParser** - Parses SRT subtitle format into `AList<RichTimeStampedLyric>`
- **AppleLrcXmlParser** - Parses Apple's Line-timed TTML XML into `AList<TimeStampedLyric>`
- **AppleRichXmlParser** - Parses Apple's Word-timed TTML XML into `AList<RichTimeStampedLyric>`
- **AppleXmlParser** - Parses Apple's non-timed TTML XML into `AList<RawLyric>`
- **MmlParser** - Parses Musixmatch JSON format into `AList<TimeStampedLyric>`
- **RmmlParser** - Parses Rich Musixmatch JSON format into `AList<RichTimeStampedLyric>`
- **EnvParser** - Parses KEY=VALUE style content
- **RlrcParser** - Parses raw lines as lyrics

Each parser extends the `FileFormat<string, T>` base class and implements the `Parse` and `TryParse` methods for their specific format types.

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

# DevBase.Net Project Documentation

This document contains all class, method, and field signatures with their corresponding comments for the DevBase.Net project.

## Table of Contents

- [Abstract](#abstract)
  - [GenericBuilder&lt;T&gt;](#genericbuildert)
  - [HttpHeaderBuilder&lt;T&gt;](#httpheaderbuildert)
  - [HttpBodyBuilder&lt;T&gt;](#httpbodybuildert)
  - [HttpFieldBuilder&lt;T&gt;](#httpfieldbuildert)
  - [BogusHttpHeaderBuilder](#bogushttpheaderbuilder)
  - [HttpKeyValueListBuilder&lt;T, K, V&gt;](#httpkeyvaluelistbuildert-k-v)
  - [RequestContent](#requestcontent)
  - [TypographyRequestContent](#typographyrequestcontent)
- [Batch](#batch)
  - [BatchRequests](#batchrequests)
  - [Batch](#batch)
  - [BatchProgressInfo](#batchprogressinfo)
  - [BatchStatistics](#batchstatistics)
  - [RequeueDecision](#requeuedecision)
  - [ProxiedBatchRequests](#proxiedbatchrequests)
  - [ProxiedBatch](#proxiedbatch)
  - [ProxiedBatchStatistics](#proxiedbatchstatistics)
  - [ProxyFailureContext](#proxyfailurecontext)
  - [Proxy Rotation Strategies](#proxy-rotation-strategies)
- [Cache](#cache)
  - [CachedResponse](#cachedresponse)
  - [ResponseCache](#responsecache)
- [Configuration](#configuration)
  - [Enums](#enums)
  - [Configuration Classes](#configuration-classes)
- [Constants](#constants)
- [Core](#core)
  - [BaseRequest](#baserequest)
  - [Request](#request)
  - [BaseResponse](#baseresponse)
  - [Response](#response)
- [Data](#data)
- [Exceptions](#exceptions)
- [Interfaces](#interfaces)
- [Parsing](#parsing)
- [Proxy](#proxy)
- [Security](#security)
- [Utils](#utils)
- [Validation](#validation)

## Abstract

### GenericBuilder&lt;T&gt;

```csharp
/// <summary>
/// Abstract base class for generic builders.
/// </summary>
/// <typeparam name="T">The specific builder type.</typeparam>
public abstract class GenericBuilder<T> where T : GenericBuilder<T>
{
    private bool AlreadyBuilt { get; set; }
    
    /// <summary>
    /// Gets a value indicating whether the builder result is usable (already built).
    /// </summary>
    public bool Usable { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericBuilder{T}"/> class.
    /// </summary>
    protected GenericBuilder()
    
    /// <summary>
    /// Gets the action to perform when building.
    /// </summary>
    protected abstract Action BuildAction { get; }

    /// <summary>
    /// Builds the object.
    /// </summary>
    /// <returns>The builder instance.</returns>
    /// <exception cref="HttpHeaderException">Thrown if the object has already been built.</exception>
    public T Build()
    
    /// <summary>
    /// Attempts to build the object.
    /// </summary>
    /// <returns>True if the build was successful; otherwise, false (if already built).</returns>
    public bool TryBuild()
}
```

### HttpHeaderBuilder&lt;T&gt;

```csharp
/// <summary>
/// Abstract base class for HTTP header builders.
/// </summary>
/// <typeparam name="T">The specific builder type.</typeparam>
public abstract class HttpHeaderBuilder<T> where T : HttpHeaderBuilder<T>
{
    /// <summary>
    /// Gets the StringBuilder used to construct the header.
    /// </summary>
    protected StringBuilder HeaderStringBuilder { get; private set; }
    
    private bool AlreadyBuilt { get; set; }
    
    /// <summary>
    /// Gets a value indicating whether the builder result is usable (built or has content).
    /// </summary>
    public bool Usable { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpHeaderBuilder{T}"/> class.
    /// </summary>
    protected HttpHeaderBuilder()
    
    /// <summary>
    /// Gets the action to perform when building the header.
    /// </summary>
    protected abstract Action BuildAction { get; }

    /// <summary>
    /// Builds the HTTP header.
    /// </summary>
    /// <returns>The builder instance.</returns>
    /// <exception cref="HttpHeaderException">Thrown if the header has already been built.</exception>
    public T Build()
}
```

### HttpBodyBuilder&lt;T&gt;

```csharp
/// <summary>
/// Base class for builders that construct HTTP request bodies.
/// </summary>
/// <typeparam name="T">The specific builder type.</typeparam>
public abstract class HttpBodyBuilder<T> where T : HttpBodyBuilder<T>
{
    /// <summary>
    /// Gets the content type of the body.
    /// </summary>
    public abstract string ContentType { get; }
    
    /// <summary>
    /// Gets the content length of the body.
    /// </summary>
    public abstract long ContentLength { get; }
    
    /// <summary>
    /// Gets whether the body is built.
    /// </summary>
    public abstract bool IsBuilt { get; }

    /// <summary>
    /// Builds the body content.
    /// </summary>
    /// <returns>The builder instance.</returns>
    public abstract T Build()
    
    /// <summary>
    /// Writes the body content to a stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public abstract Task WriteToAsync(Stream stream, CancellationToken cancellationToken = default)
}
```

### HttpFieldBuilder&lt;T&gt;

```csharp
/// <summary>
/// Base class for builders that construct single HTTP fields.
/// </summary>
/// <typeparam name="T">The specific builder type.</typeparam>
public abstract class HttpFieldBuilder<T> where T : HttpFieldBuilder<T>, new()
{
    /// <summary>
    /// Gets whether the field is built.
    /// </summary>
    public bool IsBuilt { get; protected set; }

    /// <summary>
    /// Builds the field.
    /// </summary>
    /// <returns>The builder instance.</returns>
    public abstract T Build()
}
```

### BogusHttpHeaderBuilder

```csharp
/// <summary>
/// Extended header builder with support for fake data generation.
/// </summary>
public class BogusHttpHeaderBuilder : HttpHeaderBuilder<BogusHttpHeaderBuilder>
{
    // Implementation for generating bogus HTTP headers
}
```

### HttpKeyValueListBuilder&lt;T, K, V&gt;

```csharp
/// <summary>
/// Base for key-value pair based body builders (e.g. form-urlencoded).
/// </summary>
/// <typeparam name="T">The specific builder type.</typeparam>
/// <typeparam name="K">The key type.</typeparam>
/// <typeparam name="V">The value type.</typeparam>
public abstract class HttpKeyValueListBuilder<T, K, V> : HttpBodyBuilder<T> 
    where T : HttpKeyValueListBuilder<T, K, V>, new()
{
    /// <summary>
    /// Adds a key-value pair.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>The builder instance.</returns>
    public abstract T Add(K key, V value)
}
```

### RequestContent

```csharp
/// <summary>
/// Abstract base for request content validation.
/// </summary>
public abstract class RequestContent
{
    /// <summary>
    /// Validates the request content.
    /// </summary>
    /// <param name="content">The content to validate.</param>
    /// <returns>True if valid; otherwise, false.</returns>
    public abstract bool Validate(string content)
}
```

### TypographyRequestContent

```csharp
/// <summary>
/// Text-based request content validation with encoding.
/// </summary>
public class TypographyRequestContent : RequestContent
{
    /// <summary>
    /// Gets or sets the encoding to use.
    /// </summary>
    public Encoding Encoding { get; set; }
    
    /// <summary>
    /// Validates the text content.
    /// </summary>
    /// <param name="content">The content to validate.</param>
    /// <returns>True if valid; otherwise, false.</returns>
    public override bool Validate(string content)
}
```

## Batch

### BatchRequests

```csharp
/// <summary>
/// High-performance batch request execution engine.
/// </summary>
public sealed class BatchRequests : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the number of batches.
    /// </summary>
    public int BatchCount { get; }
    
    /// <summary>
    /// Gets the total queue count across all batches.
    /// </summary>
    public int TotalQueueCount { get; }
    
    /// <summary>
    /// Gets the response queue count.
    /// </summary>
    public int ResponseQueueCount { get; }
    
    /// <summary>
    /// Gets the rate limit.
    /// </summary>
    public int RateLimit { get; }
    
    /// <summary>
    /// Gets whether cookies are persisted.
    /// </summary>
    public bool PersistCookies { get; }
    
    /// <summary>
    /// Gets whether referer is persisted.
    /// </summary>
    public bool PersistReferer { get; }
    
    /// <summary>
    /// Gets whether processing is active.
    /// </summary>
    public bool IsProcessing { get; }
    
    /// <summary>
    /// Gets the processed count.
    /// </summary>
    public int ProcessedCount { get; }
    
    /// <summary>
    /// Gets the error count.
    /// </summary>
    public int ErrorCount { get; }
    
    /// <summary>
    /// Gets the batch names.
    /// </summary>
    public IReadOnlyList<string> BatchNames { get; }

    /// <summary>
    /// Initializes a new instance of the BatchRequests class.
    /// </summary>
    public BatchRequests()
    
    /// <summary>
    /// Sets the rate limit.
    /// </summary>
    /// <param name="requestsPerWindow">Requests per window.</param>
    /// <param name="window">Time window.</param>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests WithRateLimit(int requestsPerWindow, TimeSpan? window = null)
    
    /// <summary>
    /// Enables cookie persistence.
    /// </summary>
    /// <param name="persist">Whether to persist.</param>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests WithCookiePersistence(bool persist = true)
    
    /// <summary>
    /// Enables referer persistence.
    /// </summary>
    /// <param name="persist">Whether to persist.</param>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests WithRefererPersistence(bool persist = true)
    
    /// <summary>
    /// Creates a new batch.
    /// </summary>
    /// <param name="name">Batch name.</param>
    /// <returns>The created batch.</returns>
    public Batch CreateBatch(string name)
    
    /// <summary>
    /// Gets or creates a batch.
    /// </summary>
    /// <param name="name">Batch name.</param>
    /// <returns>The batch.</returns>
    public Batch GetOrCreateBatch(string name)
    
    /// <summary>
    /// Gets a batch by name.
    /// </summary>
    /// <param name="name">Batch name.</param>
    /// <returns>The batch, or null if not found.</returns>
    public Batch? GetBatch(string name)
    
    /// <summary>
    /// Removes a batch.
    /// </summary>
    /// <param name="name">Batch name.</param>
    /// <returns>True if removed; otherwise, false.</returns>
    public bool RemoveBatch(string name)
    
    /// <summary>
    /// Clears all batches.
    /// </summary>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests ClearAllBatches()
    
    /// <summary>
    /// Adds a response callback.
    /// </summary>
    /// <param name="callback">The callback function.</param>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests OnResponse(Func<Response, Task> callback)
    
    /// <summary>
    /// Adds a response callback.
    /// </summary>
    /// <param name="callback">The callback action.</param>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests OnResponse(Action<Response> callback)
    
    /// <summary>
    /// Adds an error callback.
    /// </summary>
    /// <param name="callback">The callback function.</param>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests OnError(Func<Request, System.Exception, Task> callback)
    
    /// <summary>
    /// Adds an error callback.
    /// </summary>
    /// <param name="callback">The callback action.</param>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests OnError(Action<Request, System.Exception> callback)
    
    /// <summary>
    /// Adds a progress callback.
    /// </summary>
    /// <param name="callback">The callback function.</param>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests OnProgress(Func<BatchProgressInfo, Task> callback)
    
    /// <summary>
    /// Adds a progress callback.
    /// </summary>
    /// <param name="callback">The callback action.</param>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests OnProgress(Action<BatchProgressInfo> callback)
    
    /// <summary>
    /// Adds a response requeue callback.
    /// </summary>
    /// <param name="callback">The callback function.</param>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests OnResponseRequeue(Func<Response, Request, RequeueDecision> callback)
    
    /// <summary>
    /// Adds an error requeue callback.
    /// </summary>
    /// <param name="callback">The callback function.</param>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests OnErrorRequeue(Func<Request, System.Exception, RequeueDecision> callback)
    
    /// <summary>
    /// Attempts to dequeue a response.
    /// </summary>
    /// <param name="response">The dequeued response.</param>
    /// <returns>True if dequeued; otherwise, false.</returns>
    public bool TryDequeueResponse(out Response? response)
    
    /// <summary>
    /// Dequeues all responses.
    /// </summary>
    /// <returns>List of responses.</returns>
    public List<Response> DequeueAllResponses()
    
    /// <summary>
    /// Starts processing.
    /// </summary>
    public void StartProcessing()
    
    /// <summary>
    /// Stops processing.
    /// </summary>
    public Task StopProcessingAsync()
    
    /// <summary>
    /// Executes all batches.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of responses.</returns>
    public async Task<List<Response>> ExecuteAllAsync(CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Executes a specific batch.
    /// </summary>
    /// <param name="batchName">Batch name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of responses.</returns>
    public async Task<List<Response>> ExecuteBatchAsync(string batchName, CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Executes all batches as async enumerable.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async enumerable of responses.</returns>
    public async IAsyncEnumerable<Response> ExecuteAllAsyncEnumerable(CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Resets counters.
    /// </summary>
    public void ResetCounters()
    
    /// <summary>
    /// Gets statistics.
    /// </summary>
    /// <returns>Batch statistics.</returns>
    public BatchStatistics GetStatistics()
    
    /// <summary>
    /// Disposes resources.
    /// </summary>
    public void Dispose()
    
    /// <summary>
    /// Disposes resources asynchronously.
    /// </summary>
    public async ValueTask DisposeAsync()
}
```

### Batch

```csharp
/// <summary>
/// Represents a named batch of requests within a BatchRequests engine.
/// </summary>
public sealed class Batch
{
    /// <summary>
    /// Gets the name of the batch.
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// Gets the number of items in the queue.
    /// </summary>
    public int QueueCount { get; }

    /// <summary>
    /// Adds a request to the batch.
    /// </summary>
    /// <param name="request">The request to add.</param>
    /// <returns>The current batch instance.</returns>
    public Batch Add(Request request)
    
    /// <summary>
    /// Adds a collection of requests.
    /// </summary>
    /// <param name="requests">The requests to add.</param>
    /// <returns>The current batch instance.</returns>
    public Batch Add(IEnumerable<Request> requests)
    
    /// <summary>
    /// Adds a request by URL.
    /// </summary>
    /// <param name="url">The URL to request.</param>
    /// <returns>The current batch instance.</returns>
    public Batch Add(string url)
    
    /// <summary>
    /// Adds a collection of URLs.
    /// </summary>
    /// <param name="urls">The URLs to add.</param>
    /// <returns>The current batch instance.</returns>
    public Batch Add(IEnumerable<string> urls)
    
    /// <summary>
    /// Enqueues a request (alias for Add).
    /// </summary>
    public Batch Enqueue(Request request)
    
    /// <summary>
    /// Enqueues a request by URL (alias for Add).
    /// </summary>
    public Batch Enqueue(string url)
    
    /// <summary>
    /// Enqueues a collection of requests (alias for Add).
    /// </summary>
    public Batch Enqueue(IEnumerable<Request> requests)
    
    /// <summary>
    /// Enqueues a collection of URLs (alias for Add).
    /// </summary>
    public Batch Enqueue(IEnumerable<string> urls)
    
    /// <summary>
    /// Enqueues a request with configuration.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="configure">Action to configure the request.</param>
    /// <returns>The current batch instance.</returns>
    public Batch Enqueue(string url, Action<Request> configure)
    
    /// <summary>
    /// Enqueues a request from a factory.
    /// </summary>
    /// <param name="requestFactory">The factory function.</param>
    /// <returns>The current batch instance.</returns>
    public Batch Enqueue(Func<Request> requestFactory)
    
    /// <summary>
    /// Attempts to dequeue a request.
    /// </summary>
    /// <param name="request">The dequeued request.</param>
    /// <returns>True if dequeued; otherwise, false.</returns>
    public bool TryDequeue(out Request? request)
    
    /// <summary>
    /// Clears all requests.
    /// </summary>
    public void Clear()
    
    /// <summary>
    /// Returns to the parent BatchRequests.
    /// </summary>
    /// <returns>The parent engine.</returns>
    public BatchRequests EndBatch()
}
```

### BatchProgressInfo

```csharp
/// <summary>
/// Information about batch processing progress.
/// </summary>
public class BatchProgressInfo
{
    /// <summary>
    /// Gets the batch name.
    /// </summary>
    public string BatchName { get; }
    
    /// <summary>
    /// Gets the completed count.
    /// </summary>
    public int Completed { get; }
    
    /// <summary>
    /// Gets the total count.
    /// </summary>
    public int Total { get; }
    
    /// <summary>
    /// Gets the error count.
    /// </summary>
    public int Errors { get; }
    
    /// <summary>
    /// Gets the progress percentage.
    /// </summary>
    public double ProgressPercentage { get; }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="batchName">Batch name.</param>
    /// <param name="completed">Completed count.</param>
    /// <param name="total">Total count.</param>
    /// <param name="errors">Error count.</param>
    public BatchProgressInfo(string batchName, int completed, int total, int errors)
}
```

### BatchStatistics

```csharp
/// <summary>
/// Statistics for batch processing.
/// </summary>
public class BatchStatistics
{
    /// <summary>
    /// Gets the batch count.
    /// </summary>
    public int BatchCount { get; }
    
    /// <summary>
    /// Gets the total queue count.
    /// </summary>
    public int TotalQueueCount { get; }
    
    /// <summary>
    /// Gets the processed count.
    /// </summary>
    public int ProcessedCount { get; }
    
    /// <summary>
    /// Gets the error count.
    /// </summary>
    public int ErrorCount { get; }
    
    /// <summary>
    /// Gets the batch queue counts.
    /// </summary>
    public IReadOnlyDictionary<string, int> BatchQueueCounts { get; }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="batchCount">Batch count.</param>
    /// <param name="totalQueueCount">Total queue count.</param>
    /// <param name="processedCount">Processed count.</param>
    /// <param name="errorCount">Error count.</param>
    /// <param name="batchQueueCounts">Batch queue counts.</param>
    public BatchStatistics(int batchCount, int totalQueueCount, int processedCount, int errorCount, IReadOnlyDictionary<string, int> batchQueueCounts)
}
```

### RequeueDecision

```csharp
/// <summary>
/// Decision for requeuing a request.
/// </summary>
public class RequeueDecision
{
    /// <summary>
    /// Gets whether to requeue.
    /// </summary>
    public bool ShouldRequeue { get; }
    
    /// <summary>
    /// Gets the modified request (if any).
    /// </summary>
    public Request? ModifiedRequest { get; }
    
    /// <summary>
    /// Gets a decision to not requeue.
    /// </summary>
    public static RequeueDecision NoRequeue { get; }
    
    /// <summary>
    /// Gets a decision to requeue.
    /// </summary>
    /// <param name="modifiedRequest">Optional modified request.</param>
    /// <returns>The requeue decision.</returns>
    public static RequeueDecision Requeue(Request? modifiedRequest = null)
}
```

### ProxiedBatchRequests

```csharp
/// <summary>
/// Extension of BatchRequests with built-in proxy support.
/// </summary>
public sealed class ProxiedBatchRequests : BatchRequests
{
    /// <summary>
    /// Gets the proxy rotation strategy.
    /// </summary>
    public IProxyRotationStrategy ProxyRotationStrategy { get; }
    
    /// <summary>
    /// Gets the proxy pool.
    /// </summary>
    public IReadOnlyList<TrackedProxyInfo> ProxyPool { get; }

    /// <summary>
    /// Configures the proxy pool.
    /// </summary>
    /// <param name="proxies">List of proxies.</param>
    /// <returns>The ProxiedBatchRequests instance.</returns>
    public ProxiedBatchRequests WithProxies(IList<TrackedProxyInfo> proxies)
    
    /// <summary>
    /// Sets the proxy rotation strategy.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    /// <returns>The ProxiedBatchRequests instance.</returns>
    public ProxiedBatchRequests WithProxyRotationStrategy(IProxyRotationStrategy strategy)
    
    /// <summary>
    /// Gets proxy statistics.
    /// </summary>
    /// <returns>Proxy statistics.</returns>
    public ProxiedBatchStatistics GetProxyStatistics()
}
```

### ProxiedBatch

```csharp
/// <summary>
/// A batch with proxy support.
/// </summary>
public sealed class ProxiedBatch : Batch
{
    /// <summary>
    /// Gets the assigned proxy.
    /// </summary>
    public TrackedProxyInfo? AssignedProxy { get; }
    
    /// <summary>
    /// Gets the proxy failure count.
    /// </summary>
    public int ProxyFailureCount { get; }
    
    /// <summary>
    /// Marks proxy as failed.
    /// </summary>
    public void MarkProxyAsFailed()
    
    /// <summary>
    /// Resets proxy failure count.
    /// </summary>
    public void ResetProxyFailureCount()
}
```

### ProxiedBatchStatistics

```csharp
/// <summary>
/// Statistics for proxied batch processing.
/// </summary>
public class ProxiedBatchStatistics : BatchStatistics
{
    /// <summary>
    /// Gets the total proxy count.
    /// </summary>
    public int TotalProxyCount { get; }
    
    /// <summary>
    /// Gets the active proxy count.
    /// </summary>
    public int ActiveProxyCount { get; }
    
    /// <summary>
    /// Gets the failed proxy count.
    /// </summary>
    public int FailedProxyCount { get; }
    
    /// <summary>
    /// Gets proxy failure details.
    /// </summary>
    public IReadOnlyDictionary<string, int> ProxyFailureCounts { get; }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="baseStats">Base statistics.</param>
    /// <param name="totalProxyCount">Total proxy count.</param>
    /// <param name="activeProxyCount">Active proxy count.</param>
    /// <param name="failedProxyCount">Failed proxy count.</param>
    /// <param name="proxyFailureCounts">Proxy failure counts.</param>
    public ProxiedBatchStatistics(BatchStatistics baseStats, int totalProxyCount, int activeProxyCount, int failedProxyCount, IReadOnlyDictionary<string, int> proxyFailureCounts)
}
```

### ProxyFailureContext

```csharp
/// <summary>
/// Context for proxy failure.
/// </summary>
public class ProxyFailureContext
{
    /// <summary>
    /// Gets the failed proxy.
    /// </summary>
    public TrackedProxyInfo Proxy { get; }
    
    /// <summary>
    /// Gets the exception.
    /// </summary>
    public System.Exception Exception { get; }
    
    /// <summary>
    /// Gets the failure count.
    /// </summary>
    public int FailureCount { get; }
    
    /// <summary>
    /// Gets the timestamp of failure.
    /// </summary>
    public DateTime FailureTimestamp { get; }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="proxy">The proxy.</param>
    /// <param name="exception">The exception.</param>
    /// <param name="failureCount">Failure count.</param>
    public ProxyFailureContext(TrackedProxyInfo proxy, System.Exception exception, int failureCount)
}
```

### Proxy Rotation Strategies

```csharp
/// <summary>
/// Interface for proxy rotation strategies.
/// </summary>
public interface IProxyRotationStrategy
{
    /// <summary>
    /// Selects the next proxy.
    /// </summary>
    /// <param name="availableProxies">Available proxies.</param>
    /// <param name="failureContexts">Failure contexts.</param>
    /// <returns>The selected proxy, or null if none available.</returns>
    TrackedProxyInfo? SelectNextProxy(IReadOnlyList<TrackedProxyInfo> availableProxies, IReadOnlyDictionary<string, ProxyFailureContext> failureContexts)
}

/// <summary>
/// Round-robin proxy rotation strategy.
/// </summary>
public class RoundRobinStrategy : IProxyRotationStrategy
{
    /// <summary>
    /// Selects the next proxy in round-robin order.
    /// </summary>
    public TrackedProxyInfo? SelectNextProxy(IReadOnlyList<TrackedProxyInfo> availableProxies, IReadOnlyDictionary<string, ProxyFailureContext> failureContexts)
}

/// <summary>
/// Random proxy rotation strategy.
/// </summary>
public class RandomStrategy : IProxyRotationStrategy
{
    /// <summary>
    /// Selects a random proxy.
    /// </summary>
    public TrackedProxyInfo? SelectNextProxy(IReadOnlyList<TrackedProxyInfo> availableProxies, IReadOnlyDictionary<string, ProxyFailureContext> failureContexts)
}

/// <summary>
/// Least failures proxy rotation strategy.
/// </summary>
public class LeastFailuresStrategy : IProxyRotationStrategy
{
    /// <summary>
    /// Selects the proxy with the least failures.
    /// </summary>
    public TrackedProxyInfo? SelectNextProxy(IReadOnlyList<TrackedProxyInfo> availableProxies, IReadOnlyDictionary<string, ProxyFailureContext> failureContexts)
}

/// <summary>
/// Sticky proxy rotation strategy (keeps using the same proxy until it fails).
/// </summary>
public class StickyStrategy : IProxyRotationStrategy
{
    /// <summary>
    /// Gets or sets the current sticky proxy.
    /// </summary>
    public TrackedProxyInfo? CurrentProxy { get; set; }
    
    /// <summary>
    /// Selects the current proxy if available, otherwise selects a new one.
    /// </summary>
    public TrackedProxyInfo? SelectNextProxy(IReadOnlyList<TrackedProxyInfo> availableProxies, IReadOnlyDictionary<string, ProxyFailureContext> failureContexts)
}
```

## Cache

### CachedResponse

```csharp
/// <summary>
/// Represents a cached HTTP response.
/// </summary>
public class CachedResponse
{
    /// <summary>
    /// Gets the cached response data.
    /// </summary>
    public Response Response { get; }
    
    /// <summary>
    /// Gets the cache timestamp.
    /// </summary>
    public DateTime CachedAt { get; }
    
    /// <summary>
    /// Gets the expiration time.
    /// </summary>
    public DateTime? ExpiresAt { get; }
    
    /// <summary>
    /// Gets whether the response is expired.
    /// </summary>
    public bool IsExpired => ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <param name="expiresAt">Expiration time.</param>
    public CachedResponse(Response response, DateTime? expiresAt = null)
}
```

### ResponseCache

```csharp
/// <summary>
/// Integrated caching system using FusionCache.
/// </summary>
public class ResponseCache : IDisposable
{
    private readonly FusionCache _cache;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="options">Cache options.</param>
    public ResponseCache(FusionCacheOptions? options = null)
    
    /// <summary>
    /// Gets a cached response.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cached response, or null if not found.</returns>
    public async Task<CachedResponse?> GetAsync(Request request, CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Sets a response in cache.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="response">The response.</param>
    /// <param name="expiration">Expiration time.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task SetAsync(Request request, Response response, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Removes a cached response.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task RemoveAsync(Request request, CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Clears the cache.
    /// </summary>
    public void Clear()
    
    /// <summary>
    /// Disposes resources.
    /// </summary>
    public void Dispose()
}
```

## Configuration

### Enums

#### EnumBackoffStrategy

```csharp
/// <summary>
/// Strategy for retry backoff.
/// </summary>
public enum EnumBackoffStrategy
{
    /// <summary>Fixed delay between retries.</summary>
    Fixed,
    /// <summary>Linear increase in delay.</summary>
    Linear,
    /// <summary>Exponential increase in delay.</summary>
    Exponential
}
```

#### EnumBrowserProfile

```csharp
/// <summary>
/// Browser profile for emulating specific browsers.
/// </summary>
public enum EnumBrowserProfile
{
    /// <summary>No specific profile.</summary>
    None,
    /// <summary>Chrome browser.</summary>
    Chrome,
    /// <summary>Firefox browser.</summary>
    Firefox,
    /// <summary>Edge browser.</summary>
    Edge,
    /// <summary>Safari browser.</summary>
    Safari
}
```

#### EnumHostCheckMethod

```csharp
/// <summary>
/// Method for checking host availability.
/// </summary>
public enum EnumHostCheckMethod
{
    /// <summary>Use ICMP ping.</summary>
    Ping,
    /// <summary>Use TCP connection.</summary>
    TcpConnect
}
```

#### EnumRefererStrategy

```csharp
/// <summary>
/// Strategy for handling referer headers.
/// </summary>
public enum EnumRefererStrategy
{
    /// <summary>No referer.</summary>
    None,
    /// <summary>Use previous URL as referer.</summary>
    PreviousUrl,
    /// <summary>Use domain root as referer.</summary>
    DomainRoot,
    /// <summary>Use custom referer.</summary>
    Custom
}
```

#### EnumRequestLogLevel

```csharp
/// <summary>
/// Log level for request/response logging.
/// </summary>
public enum EnumRequestLogLevel
{
    /// <summary>No logging.</summary>
    None,
    /// <summary>Log only basic info.</summary>
    Basic,
    /// <summary>Log headers.</summary>
    Headers,
    /// <summary>Log full content.</summary>
    Full
}
```

### Configuration Classes

#### RetryPolicy

```csharp
/// <summary>
/// Configuration for request retry policies.
/// </summary>
public sealed class RetryPolicy
{
    /// <summary>
    /// Gets the maximum number of retries. Defaults to 3.
    /// </summary>
    public int MaxRetries { get; init; } = 3;
    
    /// <summary>
    /// Gets the backoff strategy. Defaults to Exponential.
    /// </summary>
    public EnumBackoffStrategy BackoffStrategy { get; init; } = EnumBackoffStrategy.Exponential;
    
    /// <summary>
    /// Gets the initial delay. Defaults to 500ms.
    /// </summary>
    public TimeSpan InitialDelay { get; init; } = TimeSpan.FromMilliseconds(500);
    
    /// <summary>
    /// Gets the maximum delay. Defaults to 30 seconds.
    /// </summary>
    public TimeSpan MaxDelay { get; init; } = TimeSpan.FromSeconds(30);
    
    /// <summary>
    /// Gets the backoff multiplier. Defaults to 2.0.
    /// </summary>
    public double BackoffMultiplier { get; init; } = 2.0;

    /// <summary>
    /// Calculates delay for a specific attempt.
    /// </summary>
    /// <param name="attemptNumber">Attempt number (1-based).</param>
    /// <returns>Time to wait.</returns>
    public TimeSpan GetDelay(int attemptNumber)
    
    /// <summary>
    /// Gets the default retry policy.
    /// </summary>
    public static RetryPolicy Default { get; }
    
    /// <summary>
    /// Gets a policy with no retries.
    /// </summary>
    public static RetryPolicy None { get; }
    
    /// <summary>
    /// Gets an aggressive retry policy.
    /// </summary>
    public static RetryPolicy Aggressive { get; }
}
```

#### HostCheckConfig

```csharp
/// <summary>
/// Configuration for host availability checks.
/// </summary>
public class HostCheckConfig
{
    /// <summary>
    /// Gets or sets the check method.
    /// </summary>
    public EnumHostCheckMethod Method { get; set; } = EnumHostCheckMethod.Ping;
    
    /// <summary>
    /// Gets or sets the timeout for checks.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
    
    /// <summary>
    /// Gets or sets the port for TCP checks.
    /// </summary>
    public int Port { get; set; } = 80;
    
    /// <summary>
    /// Gets or sets whether to enable checks.
    /// </summary>
    public bool Enabled { get; set; } = false;
}
```

#### JsonPathConfig

```csharp
/// <summary>
/// Configuration for JSON path extraction.
/// </summary>
public class JsonPathConfig
{
    /// <summary>
    /// Gets or sets whether to use fast streaming parser.
    /// </summary>
    public bool UseStreamingParser { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the buffer size for streaming.
    /// </summary>
    public int BufferSize { get; set; } = 8192;
    
    /// <summary>
    /// Gets or sets whether to cache compiled paths.
    /// </summary>
    public bool CacheCompiledPaths { get; set; } = true;
}
```

#### LoggingConfig

```csharp
/// <summary>
/// Configuration for request/response logging.
/// </summary>
public class LoggingConfig
{
    /// <summary>
    /// Gets or sets the log level.
    /// </summary>
    public EnumRequestLogLevel LogLevel { get; set; } = EnumRequestLogLevel.Basic;
    
    /// <summary>
    /// Gets or sets whether to log request body.
    /// </summary>
    public bool LogRequestBody { get; set; } = false;
    
    /// <summary>
    /// Gets or sets whether to log response body.
    /// </summary>
    public bool LogResponseBody { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the maximum body size to log.
    /// </summary>
    public int MaxBodySizeToLog { get; set; } = 1024 * 1024; // 1MB
    
    /// <summary>
    /// Gets or sets whether to sanitize headers.
    /// </summary>
    public bool SanitizeHeaders { get; set; } = true;
}
```

#### MultiSelectorConfig

```csharp
/// <summary>
/// Configuration for selecting multiple JSON paths.
/// </summary>
public class MultiSelectorConfig
{
    /// <summary>
    /// Gets the selectors.
    /// </summary>
    public IReadOnlyList<(string name, string path)> Selectors { get; }
    
    /// <summary>
    /// Gets whether to use optimized parsing.
    /// </summary>
    public bool UseOptimizedParsing { get; }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="selectors">The selectors.</param>
    /// <param name="useOptimizedParsing">Whether to use optimized parsing.</param>
    public MultiSelectorConfig(IReadOnlyList<(string name, string path)> selectors, bool useOptimizedParsing = true)
}
```

#### ScrapingBypassConfig

```csharp
/// <summary>
/// Configuration for anti-scraping bypass.
/// </summary>
public class ScrapingBypassConfig
{
    /// <summary>
    /// Gets or sets the referer strategy.
    /// </summary>
    public EnumRefererStrategy RefererStrategy { get; set; } = EnumRefererStrategy.None;
    
    /// <summary>
    /// Gets or sets the custom referer.
    /// </summary>
    public string? CustomReferer { get; set; }
    
    /// <summary>
    /// Gets or sets the browser profile.
    /// </summary>
    public EnumBrowserProfile BrowserProfile { get; set; } = EnumBrowserProfile.None;
    
    /// <summary>
    /// Gets or sets whether to randomize user agent.
    /// </summary>
    public bool RandomizeUserAgent { get; set; } = false;
    
    /// <summary>
    /// Gets or sets additional headers to add.
    /// </summary>
    public Dictionary<string, string> AdditionalHeaders { get; set; } = new();
}
```

## Constants

### AuthConstants

```csharp
/// <summary>
/// Constants for authentication.
/// </summary>
public static class AuthConstants
{
    /// <summary>Bearer authentication scheme.</summary>
    public const string Bearer = "Bearer";
    /// <summary>Basic authentication scheme.</summary>
    public const string Basic = "Basic";
    /// <summary>Digest authentication scheme.</summary>
    public const string Digest = "Digest";
}
```

### EncodingConstants

```csharp
/// <summary>
/// Constants for encoding.
/// </summary>
public static class EncodingConstants
{
    /// <summary>UTF-8 encoding name.</summary>
    public const string Utf8 = "UTF-8";
    /// <summary>ASCII encoding name.</summary>
    public const string Ascii = "ASCII";
    /// <summary>ISO-8859-1 encoding name.</summary>
    public const string Iso88591 = "ISO-8859-1";
}
```

### HeaderConstants

```csharp
/// <summary>
/// Constants for HTTP headers.
/// </summary>
public static class HeaderConstants
{
    /// <summary>Content-Type header.</summary>
    public const string ContentType = "Content-Type";
    /// <summary>Content-Length header.</summary>
    public const string ContentLength = "Content-Length";
    /// <summary>User-Agent header.</summary>
    public const string UserAgent = "User-Agent";
    /// <summary>Authorization header.</summary>
    public const string Authorization = "Authorization";
    /// <summary>Accept header.</summary>
    public const string Accept = "Accept";
    /// <summary>Cookie header.</summary>
    public const string Cookie = "Cookie";
    /// <summary>Set-Cookie header.</summary>
    public const string SetCookie = "Set-Cookie";
    /// <summary>Referer header.</summary>
    public const string Referer = "Referer";
}
```

### HttpConstants

```csharp
/// <summary>
/// Constants for HTTP.
/// </summary>
public static class HttpConstants
{
    /// <summary>HTTP/1.1 version.</summary>
    public const string Http11 = "HTTP/1.1";
    /// <summary>HTTP/2 version.</summary>
    public const string Http2 = "HTTP/2";
    /// <summary>HTTP/3 version.</summary>
    public const string Http3 = "HTTP/3";
}
```

### MimeConstants

```csharp
/// <summary>
/// Constants for MIME types.
/// </summary>
public static class MimeConstants
{
    /// <summary>JSON MIME type.</summary>
    public const string ApplicationJson = "application/json";
    /// <summary>XML MIME type.</summary>
    public const string ApplicationXml = "application/xml";
    /// <summary>Form URL-encoded MIME type.</summary>
    public const string ApplicationFormUrlEncoded = "application/x-www-form-urlencoded";
    /// <summary>Multipart form-data MIME type.</summary>
    public const string MultipartFormData = "multipart/form-data";
    /// <summary>Text HTML MIME type.</summary>
    public const string TextHtml = "text/html";
    /// <summary>Text plain MIME type.</summary>
    public const string TextPlain = "text/plain";
}
```

### PlatformConstants

```csharp
/// <summary>
/// Constants for platforms.
/// </summary>
public static class PlatformConstants
{
    /// <summary>Windows platform.</summary>
    public const string Windows = "Windows";
    /// <summary>Linux platform.</summary>
    public const string Linux = "Linux";
    /// <summary>macOS platform.</summary>
    public const string MacOS = "macOS";
}
```

### ProtocolConstants

```csharp
/// <summary>
/// Constants for protocols.
/// </summary>
public static class ProtocolConstants
{
    /// <summary>HTTP protocol.</summary>
    public const string Http = "http";
    /// <summary>HTTPS protocol.</summary>
    public const string Https = "https";
    /// <summary>WebSocket protocol.</summary>
    public const string Ws = "ws";
    /// <summary>WebSocket Secure protocol.</summary>
    public const string Wss = "wss";
}
```

### UserAgentConstants

```csharp
/// <summary>
/// Constants for user agents.
/// </summary>
public static class UserAgentConstants
{
    /// <summary>Chrome user agent.</summary>
    public const string Chrome = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";
    /// <summary>Firefox user agent.</summary>
    public const string Firefox = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:89.0) Gecko/20100101 Firefox/89.0";
    /// <summary>Edge user agent.</summary>
    public const string Edge = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36 Edg/91.0.864.59";
}
```

## Core

### BaseRequest

```csharp
/// <summary>
/// Abstract base class for HTTP requests providing core properties and lifecycle management.
/// </summary>
public abstract class BaseRequest : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the HTTP method.
    /// </summary>
    public HttpMethod Method { get; }
    
    /// <summary>
    /// Gets the timeout duration.
    /// </summary>
    public TimeSpan Timeout { get; }
    
    /// <summary>
    /// Gets the cancellation token.
    /// </summary>
    public CancellationToken CancellationToken { get; }
    
    /// <summary>
    /// Gets the proxy configuration.
    /// </summary>
    public TrackedProxyInfo? Proxy { get; }
    
    /// <summary>
    /// Gets the retry policy.
    /// </summary>
    public RetryPolicy RetryPolicy { get; }
    
    /// <summary>
    /// Gets whether certificate validation is enabled.
    /// </summary>
    public bool ValidateCertificates { get; }
    
    /// <summary>
    /// Gets whether redirects are followed.
    /// </summary>
    public bool FollowRedirects { get; }
    
    /// <summary>
    /// Gets the maximum redirects.
    /// </summary>
    public int MaxRedirects { get; }
    
    /// <summary>
    /// Gets whether the request is built.
    /// </summary>
    public bool IsBuilt { get; }
    
    /// <summary>
    /// Gets the request interceptors.
    /// </summary>
    public IReadOnlyList<IRequestInterceptor> RequestInterceptors { get; }
    
    /// <summary>
    /// Gets the response interceptors.
    /// </summary>
    public IReadOnlyList<IResponseInterceptor> ResponseInterceptors { get; }

    /// <summary>
    /// Gets the request URI.
    /// </summary>
    public abstract ReadOnlySpan<char> Uri { get; }
    
    /// <summary>
    /// Gets the request body.
    /// </summary>
    public abstract ReadOnlySpan<byte> Body { get; }

    /// <summary>
    /// Builds the request.
    /// </summary>
    /// <returns>The built request.</returns>
    public abstract BaseRequest Build()
    
    /// <summary>
    /// Sends the request asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response.</returns>
    public abstract Task<Response> SendAsync(CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Disposes resources.
    /// </summary>
    public virtual void Dispose()
    
    /// <summary>
    /// Disposes resources asynchronously.
    /// </summary>
    public virtual ValueTask DisposeAsync()
}
```

### Request

```csharp
/// <summary>
/// HTTP request class with full request building and execution capabilities.
/// Split across partial classes: Request.cs (core), RequestConfiguration.cs (fluent API), 
/// RequestHttp.cs (HTTP execution), RequestContent.cs (content handling), RequestBuilder.cs (file uploads).
/// </summary>
public partial class Request : BaseRequest
{
    /// <summary>
    /// Gets the request URI.
    /// </summary>
    public override ReadOnlySpan<char> Uri { get; }
    
    /// <summary>
    /// Gets the request body.
    /// </summary>
    public override ReadOnlySpan<byte> Body { get; }
    
    /// <summary>
    /// Gets the request URI as Uri object.
    /// </summary>
    public Uri? GetUri()
    
    /// <summary>
    /// Gets the scraping bypass configuration.
    /// </summary>
    public ScrapingBypassConfig? ScrapingBypass { get; }
    
    /// <summary>
    /// Gets the JSON path configuration.
    /// </summary>
    public JsonPathConfig? JsonPathConfig { get; }
    
    /// <summary>
    /// Gets the host check configuration.
    /// </summary>
    public HostCheckConfig? HostCheckConfig { get; }
    
    /// <summary>
    /// Gets the logging configuration.
    /// </summary>
    public LoggingConfig? LoggingConfig { get; }
    
    /// <summary>
    /// Gets whether header validation is enabled.
    /// </summary>
    public bool HeaderValidationEnabled { get; }
    
    /// <summary>
    /// Gets the header builder.
    /// </summary>
    public RequestHeaderBuilder? HeaderBuilder { get; }
    
    /// <summary>
    /// Gets the request interceptors.
    /// </summary>
    public new IReadOnlyList<IRequestInterceptor> RequestInterceptors { get; }
    
    /// <summary>
    /// Gets the response interceptors.
    /// </summary>
    public new IReadOnlyList<IResponseInterceptor> ResponseInterceptors { get; }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public Request()
    
    /// <summary>
    /// Initializes with URL.
    /// </summary>
    /// <param name="url">The URL.</param>
    public Request(ReadOnlyMemory<char> url)
    
    /// <summary>
    /// Initializes with URL.
    /// </summary>
    /// <param name="url">The URL.</param>
    public Request(string url)
    
    /// <summary>
    /// Initializes with URI.
    /// </summary>
    /// <param name="uri">The URI.</param>
    public Request(Uri uri)
    
    /// <summary>
    /// Initializes with URL and method.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="method">The HTTP method.</param>
    public Request(string url, HttpMethod method)
    
    /// <summary>
    /// Initializes with URI and method.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="method">The HTTP method.</param>
    public Request(Uri uri, HttpMethod method)
    
    /// <summary>
    /// Builds the request.
    /// </summary>
    /// <returns>The built request.</returns>
    public override BaseRequest Build()
    
    /// <summary>
    /// Sends the request asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response.</returns>
    public override async Task<Response> SendAsync(CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Disposes resources.
    /// </summary>
    public override void Dispose()
    
    /// <summary>
    /// Disposes resources asynchronously.
    /// </summary>
    public override ValueTask DisposeAsync()
}
```

### BaseResponse

```csharp
/// <summary>
/// Abstract base class for HTTP responses providing core properties and content access.
/// </summary>
public abstract class BaseResponse : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the HTTP status code.
    /// </summary>
    public HttpStatusCode StatusCode { get; }
    
    /// <summary>
    /// Gets whether the response indicates success.
    /// </summary>
    public bool IsSuccessStatusCode { get; }
    
    /// <summary>
    /// Gets the response headers.
    /// </summary>
    public HttpResponseHeaders Headers { get; }
    
    /// <summary>
    /// Gets the content headers.
    /// </summary>
    public HttpContentHeaders? ContentHeaders { get; }
    
    /// <summary>
    /// Gets the content type.
    /// </summary>
    public string? ContentType { get; }
    
    /// <summary>
    /// Gets the content length.
    /// </summary>
    public long? ContentLength { get; }
    
    /// <summary>
    /// Gets the HTTP version.
    /// </summary>
    public Version HttpVersion { get; }
    
    /// <summary>
    /// Gets the reason phrase.
    /// </summary>
    public string? ReasonPhrase { get; }
    
    /// <summary>
    /// Gets whether this is a redirect response.
    /// </summary>
    public bool IsRedirect { get; }
    
    /// <summary>
    /// Gets whether this is a client error (4xx).
    /// </summary>
    public bool IsClientError { get; }
    
    /// <summary>
    /// Gets whether this is a server error (5xx).
    /// </summary>
    public bool IsServerError { get; }
    
    /// <summary>
    /// Gets whether this response indicates rate limiting.
    /// </summary>
    public bool IsRateLimited { get; }

    /// <summary>
    /// Gets the response content as bytes.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The content bytes.</returns>
    public virtual async Task<byte[]> GetBytesAsync(CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Gets the response content as string.
    /// </summary>
    /// <param name="encoding">The encoding to use.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The content string.</returns>
    public virtual async Task<string> GetStringAsync(Encoding? encoding = null, CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Gets the response content stream.
    /// </summary>
    /// <returns>The content stream.</returns>
    public virtual Stream GetStream()
    
    /// <summary>
    /// Gets cookies from the response.
    /// </summary>
    /// <returns>The cookie collection.</returns>
    public virtual CookieCollection GetCookies()
    
    /// <summary>
    /// Gets a header value by name.
    /// </summary>
    /// <param name="name">The header name.</param>
    /// <returns>The header value.</returns>
    public virtual string? GetHeader(string name)
    
    /// <summary>
    /// Throws if the response does not indicate success.
    /// </summary>
    public virtual void EnsureSuccessStatusCode()
    
    /// <summary>
    /// Disposes resources.
    /// </summary>
    public virtual void Dispose()
    
    /// <summary>
    /// Disposes resources asynchronously.
    /// </summary>
    public virtual async ValueTask DisposeAsync()
}
```

### Response

```csharp
/// <summary>
/// HTTP response class with parsing and streaming capabilities.
/// </summary>
public sealed class Response : BaseResponse
{
    /// <summary>
    /// Gets the request metrics.
    /// </summary>
    public RequestMetrics Metrics { get; }
    
    /// <summary>
    /// Gets whether this response was served from cache.
    /// </summary>
    public bool FromCache { get; }
    
    /// <summary>
    /// Gets the original request URI.
    /// </summary>
    public Uri? RequestUri { get; }

    /// <summary>
    /// Gets the response as specified type.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed response.</returns>
    public async Task<T> GetAsync<T>(CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Parses JSON response.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="useSystemTextJson">Whether to use System.Text.Json.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed object.</returns>
    public async Task<T> ParseJsonAsync<T>(bool useSystemTextJson = true, CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Parses JSON document.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The JsonDocument.</returns>
    public async Task<JsonDocument> ParseJsonDocumentAsync(CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Parses XML response.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The XDocument.</returns>
    public async Task<XDocument> ParseXmlAsync(CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Parses HTML response.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The IDocument.</returns>
    public async Task<IDocument> ParseHtmlAsync(CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Parses JSON path.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="path">The JSON path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed value.</returns>
    public async Task<T> ParseJsonPathAsync<T>(string path, CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Parses JSON path list.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="path">The JSON path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed list.</returns>
    public async Task<List<T>> ParseJsonPathListAsync<T>(string path, CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Parses multiple JSON paths.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The multi-selector result.</returns>
    public async Task<MultiSelectorResult> ParseMultipleJsonPathsAsync(MultiSelectorConfig config, CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Parses multiple JSON paths.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="selectors">The selectors.</param>
    /// <returns>The multi-selector result.</returns>
    public async Task<MultiSelectorResult> ParseMultipleJsonPathsAsync(CancellationToken cancellationToken = default, params (string name, string path)[] selectors)
    
    /// <summary>
    /// Parses multiple JSON paths optimized.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="selectors">The selectors.</param>
    /// <returns>The multi-selector result.</returns>
    public async Task<MultiSelectorResult> ParseMultipleJsonPathsOptimizedAsync(CancellationToken cancellationToken = default, params (string name, string path)[] selectors)
    
    /// <summary>
    /// Streams response lines.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async enumerable of lines.</returns>
    public async IAsyncEnumerable<string> StreamLinesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Streams response chunks.
    /// </summary>
    /// <param name="chunkSize">Chunk size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async enumerable of chunks.</returns>
    public async IAsyncEnumerable<byte[]> StreamChunksAsync(int chunkSize = 4096, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Gets header values.
    /// </summary>
    /// <param name="name">Header name.</param>
    /// <returns>The header values.</returns>
    public IEnumerable<string> GetHeaderValues(string name)
    
    /// <summary>
    /// Parses bearer token.
    /// </summary>
    /// <returns>The authentication token.</returns>
    public AuthenticationToken? ParseBearerToken()
    
    /// <summary>
    /// Parses and verifies bearer token.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>The authentication token.</returns>
    public AuthenticationToken? ParseAndVerifyBearerToken(string secret)
    
    /// <summary>
    /// Validates content length.
    /// </summary>
    /// <returns>The validation result.</returns>
    public ValidationResult ValidateContentLength()
}
```

## Data

The Data namespace contains various data structures for HTTP requests and responses:

- **Body**: Classes for different body types (JsonBody, FormBody, MultipartBody, etc.)
- **Header**: Classes for HTTP headers (RequestHeaderBuilder, ResponseHeaders, etc.)
- **Query**: Classes for query string handling
- **Cookie**: Classes for cookie handling
- **Mime**: Classes for MIME type handling

## Exceptions

The Exceptions namespace contains custom exceptions:

- **HttpHeaderException**: Thrown for HTTP header errors
- **RequestException**: Base class for request errors
- **ResponseException**: Base class for response errors
- **ProxyException**: Thrown for proxy-related errors
- **ValidationException**: Thrown for validation errors

## Interfaces

The Interfaces namespace defines contracts for:

- **IRequestInterceptor**: Interface for request interceptors
- **IResponseInterceptor**: Interface for response interceptors
- **IHttpClient**: Interface for HTTP clients
- **IProxyProvider**: Interface for proxy providers

## Parsing

The Parsing namespace provides parsers for:

- **JsonPathParser**: JSON path extraction
- **StreamingJsonPathParser**: Fast streaming JSON path parser
- **MultiSelectorParser**: Multiple JSON path selector
- **HtmlParser**: HTML parsing utilities
- **XmlParser**: XML parsing utilities

## Proxy

The Proxy namespace contains:

- **TrackedProxyInfo**: Information about a proxy with tracking
- **ProxyValidator**: Proxy validation utilities
- **ProxyPool**: Pool of proxies
- **ProxyRotator**: Proxy rotation logic

## Security

The Security namespace provides:

- **Token**: JWT token handling
- **AuthenticationToken**: Authentication token structure
- **JwtValidator**: JWT validation utilities
- **CertificateValidator**: Certificate validation

## Utils

The Utils namespace contains utility classes:

- **BogusUtils**: Fake data generation
- **JsonUtils**: JSON manipulation helpers
- **ContentDispositionUtils**: Content-Disposition parsing
- **UriUtils**: URI manipulation utilities
- **StringBuilderPool**: Pool for StringBuilder instances

## Validation

The Validation namespace provides:

- **HeaderValidator**: HTTP header validation
- **RequestValidator**: Request validation
- **ResponseValidator**: Response validation
- **ValidationResult**: Result of validation
