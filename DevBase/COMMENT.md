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
