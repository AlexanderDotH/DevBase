# DevBase.Logging Agent Guide

## Overview
DevBase.Logging is a lightweight, context-aware logging utility for debug output.

## Key Classes

| Class | Namespace | Purpose |
|-------|-----------|---------|
| `Logger` | `DevBase.Logging` | Main logging class |

## Quick Reference

### Basic Logging
```csharp
using DevBase.Logging;

Logger logger = new Logger();
logger.Debug("Debug message");
logger.Info("Info message");
logger.Warn("Warning message");
logger.Error("Error message");
```

### With Context
```csharp
logger.Info("Processing item", context: "ItemProcessor");
```

## File Structure
```
DevBase.Logging/
└── Logger.cs
```

## Important Notes

1. **Lightweight** - minimal overhead
2. **Context-aware** - pass context for better log organization
3. **Debug output** - primarily for development/debugging
