# DevBase (Core) Agent Guide

## Overview
DevBase is the core library providing generic collections, IO utilities, and async task management.

## Key Classes

| Class | Namespace | Purpose |
|-------|-----------|---------|
| `AList<T>` | `DevBase.Generics` | Enhanced generic list |
| `AFile` | `DevBase.IO` | File operations utility |
| `AFileObject` | `DevBase.IO` | File wrapper object |
| `AString` | `DevBase.Typography` | String utilities |

## Quick Reference

### AList<T> - Enhanced List
```csharp
using DevBase.Generics;

AList<string> list = new AList<string>();
list.Add("item");
string item = list.Get(0);
bool empty = list.IsEmpty();
int length = list.Length;
string[] array = list.GetAsArray();
List<string> asList = list.GetAsList();
string random = list.GetRandom();
```

### AFile - File Operations
```csharp
using DevBase.IO;

// Get files recursively
AList<AFileObject> files = AFile.GetFiles("path", recursive: true, "*.txt");

// Read file
AFileObject file = files.Get(0);
string content = file.ToStringData();
byte[] bytes = file.ToByteData();
```

### AString - String Utilities
```csharp
using DevBase.Typography;

AString str = new AString("line1\nline2\nline3");
AList<string> lines = str.AsList();
```

## File Structure
```
DevBase/
├── Cache/                   # Caching utilities
├── Generics/
│   └── AList.cs            # Enhanced generic list
├── IO/
│   ├── AFile.cs            # File operations
│   └── AFileObject.cs      # File wrapper
├── Typography/
│   └── AString.cs          # String utilities
└── Utilities/              # Helper utilities
```

## Important Notes

1. **Prefer `AList<T>` over `List<T>`** for DevBase projects
2. **Use `Get(index)` instead of indexer `[index]`** for AList
3. **`IsEmpty()` checks for empty list**
4. **`Length` property for count** (not `Count`)
5. **`GetAsArray()` converts to array**

## Common Patterns

### Iterate AList
```csharp
for (int i = 0; i < list.Length; i++)
{
    var item = list.Get(i);
}

// Or use GetAsList() for foreach
foreach (var item in list.GetAsList())
{
    // process
}
```

### Check Empty
```csharp
if (list.IsEmpty())
    return;
```
