# DevBase (Core)

DevBase is the core library providing essential utilities, custom generic collections, IO helpers, and asynchronous task management.

## Table of Contents
- [Generics](#generics)
- [IO Utilities](#io-utilities)
- [Asynchronous Operations](#asynchronous-operations)
- [Typography](#typography)
- [Utilities](#utilities)

## Generics

### AList<T>
`AList<T>` is a custom implementation of a list that wraps a standard array but provides additional utility methods, including performance optimizations for searching based on memory size.

**Key Features:**
- **Memory-Optimized Search**: Uses `MemoryUtils.GetSize()` to quickly filter objects before checking equality.
- **Fluent API**: Methods for slicing, random selection, and safe removal.

**Example:**
```csharp
using DevBase.Generics;

// Create a new list
var list = new AList<string>("Alpha", "Beta", "Gamma");

// Add items
list.Add("Delta");

// Find entry (optimized)
string entry = list.FindEntry("Beta");

// Safe Remove (checks existence first)
list.SafeRemove("Gamma");

// Get Random item
string random = list.GetRandom();
```

**Tip:**
`AList<T>` relies on `MemoryUtils.GetSize()` for some operations. Ensure `Globals.ALLOW_SERIALIZATION` is true (default) for this to work effectively with complex objects.

### ATupleList<T1, T2>
Extends `AList` to handle `Tuple<T1, T2>`, allowing you to search by either the first or second item of the tuple.

**Example:**
```csharp
var tupleList = new ATupleList<int, string>();
tupleList.Add(1, "One");
tupleList.Add(2, "Two");

// Find by Item1
string value = tupleList.FindEntry(1); // Returns "One"

// Find by Item2
int key = tupleList.FindEntry("Two"); // Returns 2
```

## IO Utilities

### AFile
Static helper class for file operations, reading files into memory or `AFileObject`.

**Example:**
```csharp
using DevBase.IO;

// Read file content into memory
Memory<byte> data = AFile.ReadFile("path/to/file.txt");

// Get all files in directory as objects
var files = AFile.GetFiles("path/to/dir", readContent: true, filter: "*.json");
```

### ADirectory
Helper for retrieving directory structures.

## Asynchronous Operations

### Multitasking
Manages a queue of tasks with a constrained capacity, useful for throttling concurrent operations.

**Example:**
```csharp
using DevBase.Async.Task;

// Create a manager that allows 5 concurrent tasks
var taskManager = new Multitasking(capacity: 5);

// Register tasks
for(int i = 0; i < 20; i++) {
    taskManager.Register(() => {
        Console.WriteLine($"Working {i}");
        Thread.Sleep(1000);
    });
}

// Wait for all to finish
await taskManager.WaitAll();
```

## Typography

### AString
A wrapper around `string` providing additional manipulation methods.

**Example:**
```csharp
var aStr = new AString("hello world");
Console.WriteLine(aStr.CapitalizeFirst()); // "Hello world"
```

## Utilities

### MemoryUtils
Provides methods to get the size of objects via serialization.

**Note:** This relies on `BinaryFormatter` which is obsolete in newer .NET versions. Use with caution or ensure `Globals.ALLOW_SERIALIZATION` is managed if security is a concern.
