# DevBase.Extensions Agent Guide

## Overview
DevBase.Extensions provides extension methods for standard .NET types.

## Key Classes

| Class | Namespace | Purpose |
|-------|-----------|---------|
| `StopwatchExtensions` | `DevBase.Extensions` | Stopwatch formatting |

## Quick Reference

### Stopwatch Formatting
```csharp
using DevBase.Extensions;
using System.Diagnostics;

Stopwatch sw = Stopwatch.StartNew();
// ... work ...
sw.Stop();

string formatted = sw.ToFormattedString();
// Output: "1.234s" or "123ms" depending on duration
```

## File Structure
```
DevBase.Extensions/
└── StopwatchExtensions.cs
```

## Important Notes

1. **Import `DevBase.Extensions`** namespace to use extensions
2. **Stopwatch formatter** auto-selects appropriate unit (ms, s, min)
