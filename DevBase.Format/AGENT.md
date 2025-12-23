# DevBase.Format - AI Agent Guide

This guide helps AI agents effectively use the DevBase.Format library for parsing lyrics and configuration files.

## Overview

DevBase.Format provides parsers for various text-based formats, primarily focused on lyrics formats (LRC, SRT, ELRC, etc.) and configuration files (ENV).

**Target Framework:** .NET 9.0

## Core Architecture

### Generic Parser Pattern

All parsers follow this pattern:

```csharp
FileParser<FormatType, ResultType>
```

Where:
- `FormatType` - The format parser class (e.g., `LrcFormat`, `SrtFormat`)
- `ResultType` - The parsed result type (e.g., `AList<TimeStampedLyric>`, `Dictionary<string, string>`)

### Base Classes

1. **`FileFormat<TInput, TOutput>`** - Base class for all format parsers
2. **`RevertableFileFormat<TInput, TOutput>`** - For formats that can convert back to string
3. **`FileParser<P, T>`** - Generic parser wrapper

## Usage Patterns for AI Agents

### Pattern 1: Parse LRC Lyrics

```csharp
using DevBase.Format;
using DevBase.Format.Formats.LrcFormat;

var parser = new FileParser<LrcFormat, AList<TimeStampedLyric>>();

// From string
var lyrics = parser.ParseFromString(lrcContent);

// From file
var lyrics = parser.ParseFromDisk("song.lrc");

// Safe parsing
if (parser.TryParseFromString(lrcContent, out var lyrics))
{
    foreach (var line in lyrics)
    {
        Console.WriteLine($"[{line.StartTime}] {line.Text}");
    }
}
```

### Pattern 2: Parse SRT Subtitles

```csharp
using DevBase.Format.Formats.SrtFormat;

var parser = new FileParser<SrtFormat, AList<TimeStampedLyric>>();
var subtitles = parser.ParseFromDisk("movie.srt");

foreach (var subtitle in subtitles)
{
    Console.WriteLine($"{subtitle.StartTime}: {subtitle.Text}");
}
```

### Pattern 3: Parse ENV Configuration

```csharp
using DevBase.Format.Formats.EnvFormat;

var parser = new FileParser<EnvFormat, Dictionary<string, string>>();
var config = parser.ParseFromDisk(".env");

string dbHost = config["DB_HOST"];
string apiKey = config["API_KEY"];
```

### Pattern 4: Multi-Format Detection

```csharp
AList<TimeStampedLyric> ParseLyricsAnyFormat(string content)
{
    // Try LRC
    var lrcParser = new FileParser<LrcFormat, AList<TimeStampedLyric>>();
    if (lrcParser.TryParseFromString(content, out var lrcLyrics))
        return lrcLyrics;
    
    // Try SRT
    var srtParser = new FileParser<SrtFormat, AList<TimeStampedLyric>>();
    if (srtParser.TryParseFromString(content, out var srtLyrics))
        return srtLyrics;
    
    // Try ELRC
    var elrcParser = new FileParser<ElrcFormat, AList<RichTimeStampedLyric>>();
    if (elrcParser.TryParseFromString(content, out var elrcLyrics))
    {
        // Convert to TimeStampedLyric if needed
        return ConvertToTimeStampedLyrics(elrcLyrics);
    }
    
    return null;
}
```

### Pattern 5: Integration with API Clients

```csharp
using DevBase.Api.Apis.Deezer;
using DevBase.Format;
using DevBase.Format.Formats.LrcFormat;

var deezer = new Deezer();
string lyricsString = await deezer.GetLyrics("123456");

if (lyricsString != null)
{
    var parser = new FileParser<LrcFormat, AList<TimeStampedLyric>>();
    if (parser.TryParseFromString(lyricsString, out var lyrics))
    {
        // Process parsed lyrics
        foreach (var line in lyrics)
        {
            Console.WriteLine($"[{line.StartTime}] {line.Text}");
        }
    }
}
```

### Pattern 6: Working with Timestamps

```csharp
using DevBase.Format.Structure;

var lyric = new TimeStampedLyric
{
    Text = "Hello world",
    StartTime = TimeSpan.FromSeconds(12.5)
};

// Get milliseconds
long ms = lyric.StartTimestamp;  // 12500

// Format for display
string formatted = $"[{lyric.StartTime:mm\\:ss\\.ff}] {lyric.Text}";
```

### Pattern 7: Rich-Synced Lyrics (Word-Level)

```csharp
using DevBase.Format.Structure;
using DevBase.Format.Formats.ElrcFormat;

var parser = new FileParser<ElrcFormat, AList<RichTimeStampedLyric>>();
var richLyrics = parser.ParseFromString(elrcContent);

foreach (var line in richLyrics)
{
    Console.WriteLine($"Line starts at: {line.StartTime}");
    foreach (var word in line.Words)
    {
        Console.WriteLine($"  [{word.StartTime}] {word.Text}");
    }
}
```

## Important Concepts

### 1. Parser Instantiation

**Always use `FileParser<P, T>` wrapper:**

```csharp
// ✅ Correct
var parser = new FileParser<LrcFormat, AList<TimeStampedLyric>>();
var lyrics = parser.ParseFromString(content);

// ❌ Wrong - don't instantiate format directly
var format = new LrcFormat();
var lyrics = format.Parse(content);  // Works but not recommended
```

### 2. Try-Parse Pattern

**Use Try-Parse for safe parsing:**

```csharp
// ✅ Correct - no exceptions
if (parser.TryParseFromString(content, out var result))
{
    // Success
}
else
{
    // Failed
}

// ❌ Avoid - can throw exceptions
var result = parser.ParseFromString(content);
```

### 3. File vs String Parsing

```csharp
// From file (reads and parses)
var lyrics = parser.ParseFromDisk("song.lrc");

// From string (already in memory)
var lyrics = parser.ParseFromString(lrcContent);

// From FileInfo
var lyrics = parser.ParseFromDisk(new FileInfo("song.lrc"));
```

### 4. Result Types

Different formats return different types:

| Format | Result Type |
|--------|-------------|
| LRC, SRT, Apple XML | `AList<TimeStampedLyric>` |
| ELRC, RLRC | `AList<RichTimeStampedLyric>` |
| ENV | `Dictionary<string, string>` |

### 5. Timestamp Handling

```csharp
var lyric = new TimeStampedLyric
{
    Text = "Line text",
    StartTime = TimeSpan.FromMilliseconds(12500)
};

// Access as TimeSpan
TimeSpan time = lyric.StartTime;

// Access as milliseconds
long ms = lyric.StartTimestamp;

// Create from seconds
lyric.StartTime = TimeSpan.FromSeconds(12.5);
```

## Format-Specific Guidelines

### LRC Format

**Structure:**
```
[mm:ss.xx]Lyric text
```

**Metadata:**
```
[ar:Artist]
[ti:Title]
[al:Album]
[by:Creator]
[offset:+/-ms]
```

**Parsing:**
```csharp
var parser = new FileParser<LrcFormat, AList<TimeStampedLyric>>();
var lyrics = parser.ParseFromString(lrcContent);
```

### SRT Format

**Structure:**
```
1
00:00:12,000 --> 00:00:15,000
Subtitle text
```

**Parsing:**
```csharp
var parser = new FileParser<SrtFormat, AList<TimeStampedLyric>>();
var subtitles = parser.ParseFromDisk("movie.srt");
```

### ENV Format

**Structure:**
```
KEY=value
DATABASE_URL=postgresql://localhost/mydb
# Comments
```

**Parsing:**
```csharp
var parser = new FileParser<EnvFormat, Dictionary<string, string>>();
var config = parser.ParseFromString(envContent);
string value = config["KEY"];
```

### Apple Formats

**Apple XML, Apple LRC XML, Apple Rich XML:**

```csharp
using DevBase.Format.Formats.AppleXmlFormat;
using DevBase.Format.Formats.AppleLrcXmlFormat;
using DevBase.Format.Formats.AppleRichXmlFormat;

// Standard Apple XML
var parser1 = new FileParser<AppleXmlFormat, AList<TimeStampedLyric>>();

// Apple LRC XML
var parser2 = new FileParser<AppleLrcXmlFormat, AList<TimeStampedLyric>>();

// Apple Rich XML (word-level timing)
var parser3 = new FileParser<AppleRichXmlFormat, AList<RichTimeStampedLyric>>();
```

## Common Mistakes to Avoid

### ❌ Mistake 1: Wrong Result Type

```csharp
// Wrong - LrcFormat returns AList<TimeStampedLyric>, not string
var parser = new FileParser<LrcFormat, string>();

// Correct
var parser = new FileParser<LrcFormat, AList<TimeStampedLyric>>();
```

### ❌ Mistake 2: Not Checking Parse Result

```csharp
// Wrong - might be null
var lyrics = parser.ParseFromString(content);
foreach (var line in lyrics)  // NullReferenceException!

// Correct
if (parser.TryParseFromString(content, out var lyrics))
{
    foreach (var line in lyrics)
    {
        // Safe
    }
}
```

### ❌ Mistake 3: Using Wrong Format Parser

```csharp
// Wrong - trying to parse SRT with LRC parser
var parser = new FileParser<LrcFormat, AList<TimeStampedLyric>>();
var subtitles = parser.ParseFromString(srtContent);  // Will fail!

// Correct
var parser = new FileParser<SrtFormat, AList<TimeStampedLyric>>();
var subtitles = parser.ParseFromString(srtContent);
```

### ❌ Mistake 4: Not Handling Empty Results

```csharp
// Wrong
var lyrics = parser.ParseFromString(content);
var firstLine = lyrics[0];  // Might be empty!

// Correct
var lyrics = parser.ParseFromString(content);
if (lyrics != null && !lyrics.IsEmpty())
{
    var firstLine = lyrics[0];
}
```

### ❌ Mistake 5: Incorrect Timestamp Format

```csharp
// Wrong - using wrong time format
var lyric = new TimeStampedLyric
{
    StartTime = TimeSpan.Parse("12:50")  // Hours:Minutes, not Minutes:Seconds!
};

// Correct
var lyric = new TimeStampedLyric
{
    StartTime = TimeSpan.FromSeconds(12.5)  // 12.5 seconds
};
```

## Advanced Usage

### Creating Custom Format Parser

```csharp
using DevBase.Format;

public class CustomFormat : FileFormat<string, AList<string>>
{
    public override AList<string> Parse(string input)
    {
        var lines = new AList<string>();
        foreach (var line in input.Split('\n'))
        {
            if (!string.IsNullOrWhiteSpace(line))
                lines.Add(line.Trim());
        }
        return lines;
    }

    public override bool TryParse(string input, out AList<string> parsed)
    {
        try
        {
            parsed = Parse(input);
            return true;
        }
        catch
        {
            parsed = null;
            return false;
        }
    }
}

// Use it
var parser = new FileParser<CustomFormat, AList<string>>();
var result = parser.ParseFromString("line1\nline2");
```

### Revertable Formats

Some formats can convert back to string:

```csharp
// Parse
var parser = new FileParser<LrcFormat, AList<TimeStampedLyric>>();
var lyrics = parser.ParseFromString(lrcContent);

// Modify
lyrics.Add(new TimeStampedLyric 
{ 
    Text = "New line", 
    StartTime = TimeSpan.FromSeconds(30) 
});

// Convert back
var format = new LrcFormat();
string newLrcContent = format.Revert(lyrics);
```

## Performance Tips

1. **Reuse parser instances** for multiple files
2. **Use Try-Parse** to avoid exception overhead
3. **Parse from string** when data is already in memory
4. **Cache parsed results** for frequently accessed files
5. **Use appropriate format** - don't try all formats if you know the type

## Integration Examples

### With DevBase.Api

```csharp
// Get lyrics from API
var deezer = new Deezer();
string lyricsString = await deezer.GetLyrics("123456");

// Parse
var parser = new FileParser<LrcFormat, AList<TimeStampedLyric>>();
var lyrics = parser.ParseFromString(lyricsString);
```

### With DevBase.IO

```csharp
using DevBase.IO;

// Read file
AFileObject file = AFile.ReadFileToObject("song.lrc");
string content = file.ToStringData();

// Parse
var parser = new FileParser<LrcFormat, AList<TimeStampedLyric>>();
var lyrics = parser.ParseFromString(content);
```

## Quick Reference

| Task | Code |
|------|------|
| Parse LRC | `new FileParser<LrcFormat, AList<TimeStampedLyric>>()` |
| Parse SRT | `new FileParser<SrtFormat, AList<TimeStampedLyric>>()` |
| Parse ENV | `new FileParser<EnvFormat, Dictionary<string, string>>()` |
| Parse ELRC | `new FileParser<ElrcFormat, AList<RichTimeStampedLyric>>()` |
| From string | `parser.ParseFromString(content)` |
| From file | `parser.ParseFromDisk("file.lrc")` |
| Safe parse | `parser.TryParseFromString(content, out var result)` |
| Get timestamp (ms) | `lyric.StartTimestamp` |
| Get timestamp (TimeSpan) | `lyric.StartTime` |

## Testing Considerations

- Test with valid and invalid formats
- Test with empty files
- Test with malformed timestamps
- Test with special characters in lyrics
- Test with different encodings
- Test with large files

## Version

Current version: **1.0.0**  
Target framework: **.NET 9.0**

## Dependencies

- DevBase (core utilities)
- System.Text.RegularExpressions
