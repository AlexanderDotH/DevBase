# DevBase.Format

DevBase.Format provides a robust parsing framework for various file formats. It utilizes a generic `FileFormat<F, T>` base class to standardize parsing logic, supporting both one-way parsing and revertable (two-way) parsing.

## Table of Contents
- [Core Architecture](#core-architecture)
- [Supported Formats](#supported-formats)
  - [LRC (Lyrics)](#lrc-lyrics)
  - [ENV (Environment Variables)](#env-environment-variables)
  - [SRT (Subtitles)](#srt-subtitles)

## Core Architecture

### FileFormat<F, T>
The base abstract class for all parsers.
- **F**: The input format type (usually `string` for text-based formats).
- **T**: The output type (e.g., `AList<TimeStampedLyric>`).
- **StrictErrorHandling**: A boolean property to toggle between throwing exceptions or returning default values on error.

### RevertableFileFormat<F, T>
Extends `FileFormat<F, T>` to support converting the parsed object back to its original format (e.g., saving modified lyrics back to an .lrc string).

## Supported Formats

### LRC (Lyrics)
Parses standard `.lrc` files into a list of timestamped lyrics.

**Class:** `LrcParser`
**Input:** `string` (file content)
**Output:** `AList<TimeStampedLyric>`

**Example:**
```csharp
using DevBase.Format.Formats.LrcFormat;
using DevBase.Format.Structure;

string lrcContent = "[00:12.00]Line 1\n[00:15.30]Line 2";
var parser = new LrcParser();

// Parse
if (parser.TryParse(lrcContent, out var lyrics))
{
    foreach(var line in lyrics)
    {
        Console.WriteLine($"{line.StartTime}: {line.Text}");
    }
}

// Revert (Convert back to string)
string newContent = parser.Revert(lyrics);
```

### ENV (Environment Variables)
Parses `.env` files into key-value pairs.

**Class:** `EnvParser`
**Input:** `string`
**Output:** `ATupleList<string, string>`

**Example:**
```csharp
using DevBase.Format.Formats.EnvFormat;

string envContent = "KEY=Value\nDEBUG=true";
var parser = new EnvParser();

var envVars = parser.Parse(envContent);
string debugValue = envVars.FindEntry("DEBUG"); // Returns "true"
```

### SRT (Subtitles)
Parses `.srt` subtitle files.

**Class:** `SrtParser`
**Input:** `string`
**Output:** `AList<RichTimeStampedLyric>` (Contains StartTime, EndTime, and Text)

**Example:**
```csharp
using DevBase.Format.Formats.SrtFormat;

string srtContent = "..."; // Load SRT content
var parser = new SrtParser();

var subtitles = parser.Parse(srtContent);
```
