# DevBase.Format

**DevBase.Format** is a comprehensive file format parsing library for .NET 9.0, specializing in lyrics formats (LRC, SRT, ELRC, etc.) and configuration files (ENV). It provides a unified API for parsing various text-based formats with strong typing and error handling.

## Supported Formats

### Lyrics Formats
- **LRC** - Standard LRC lyrics with timestamps `[mm:ss.xx]`
- **ELRC** - Enhanced LRC with word-level timing
- **RLRC** - Rich LRC format
- **SRT** - SubRip subtitle format
- **Apple XML** - Apple Music lyrics XML format
- **Apple LRC XML** - Apple's LRC-based XML format
- **Apple Rich XML** - Apple's rich-synced lyrics format
- **MML** - Music Markup Language
- **RMML** - Rich Music Markup Language
- **KLyrics** - Karaoke lyrics format

### Configuration Formats
- **ENV** - Environment variable files (`.env`)

## Installation

```bash
dotnet add package DevBase.Format
```

## Features

- **Type-Safe Parsing** - Strongly-typed result objects
- **Error Handling** - Try-parse pattern for safe parsing
- **File & String Input** - Parse from disk or in-memory strings
- **Timestamp Conversion** - Automatic timestamp parsing and conversion
- **Rich Metadata** - Extract metadata tags from lyrics files
- **Revertable Formats** - Convert parsed data back to original format

## Quick Start

### Parsing LRC Lyrics

```csharp
using DevBase.Format;
using DevBase.Format.Formats.LrcFormat;

// Parse from string
string lrcContent = @"
[00:12.00]First line
[00:15.50]Second line
[00:20.00]Third line
";

var parser = new FileParser<LrcFormat, AList<TimeStampedLyric>>();
var lyrics = parser.ParseFromString(lrcContent);

foreach (var line in lyrics)
{
    Console.WriteLine($"[{line.StartTime}] {line.Text}");
}

// Parse from file
var lyricsFromFile = parser.ParseFromDisk("song.lrc");
```

### Parsing SRT Subtitles

```csharp
using DevBase.Format.Formats.SrtFormat;

var parser = new FileParser<SrtFormat, AList<TimeStampedLyric>>();
var subtitles = parser.ParseFromDisk("movie.srt");

foreach (var subtitle in subtitles)
{
    Console.WriteLine($"{subtitle.StartTime}: {subtitle.Text}");
}
```

### Parsing ENV Files

```csharp
using DevBase.Format.Formats.EnvFormat;

var parser = new FileParser<EnvFormat, Dictionary<string, string>>();
var config = parser.ParseFromDisk(".env");

string dbHost = config["DB_HOST"];
string apiKey = config["API_KEY"];
```

### Safe Parsing with Try-Parse

```csharp
var parser = new FileParser<LrcFormat, AList<TimeStampedLyric>>();

if (parser.TryParseFromString(lrcContent, out var lyrics))
{
    Console.WriteLine($"Parsed {lyrics.Length} lines");
}
else
{
    Console.WriteLine("Failed to parse lyrics");
}
```

## Usage Examples

### Working with Timestamps

```csharp
using DevBase.Format.Structure;

var lyric = new TimeStampedLyric
{
    Text = "Hello world",
    StartTime = TimeSpan.FromSeconds(12.5)
};

// Get timestamp in milliseconds
long ms = lyric.StartTimestamp;  // 12500

// Format for display
Console.WriteLine($"[{lyric.StartTime:mm\\:ss\\.ff}] {lyric.Text}");
```

### Rich-Synced Lyrics (Word-Level Timing)

```csharp
using DevBase.Format.Structure;

var richLyric = new RichTimeStampedLyric
{
    StartTime = TimeSpan.FromSeconds(10),
    Words = new AList<RichTimeStampedWord>
    {
        new RichTimeStampedWord { Text = "Hello", StartTime = TimeSpan.FromSeconds(10.0) },
        new RichTimeStampedWord { Text = "world", StartTime = TimeSpan.FromSeconds(10.5) }
    }
};

foreach (var word in richLyric.Words)
{
    Console.WriteLine($"[{word.StartTime}] {word.Text}");
}
```

### Parsing Apple Music Lyrics

```csharp
using DevBase.Format.Formats.AppleXmlFormat;

var parser = new FileParser<AppleXmlFormat, AList<TimeStampedLyric>>();
var lyrics = parser.ParseFromString(appleMusicXml);

// Apple XML includes metadata
foreach (var line in lyrics)
{
    Console.WriteLine($"{line.StartTime}: {line.Text}");
}
```

### Creating Custom Format Parser

```csharp
using DevBase.Format;

public class MyFormat : FileFormat<string, AList<string>>
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

// Use custom format
var parser = new FileParser<MyFormat, AList<string>>();
var result = parser.ParseFromString("line1\nline2\nline3");
```

### Revertable Formats (Convert Back to String)

```csharp
using DevBase.Format;

// Some formats implement RevertableFileFormat
public class LrcFormat : RevertableFileFormat<string, AList<TimeStampedLyric>>
{
    public override string Revert(AList<TimeStampedLyric> parsed)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var lyric in parsed)
        {
            int minutes = (int)lyric.StartTime.TotalMinutes;
            double seconds = lyric.StartTime.TotalSeconds % 60;
            sb.AppendLine($"[{minutes:D2}:{seconds:00.00}]{lyric.Text}");
        }
        return sb.ToString();
    }
}

// Parse and revert
var parser = new FileParser<LrcFormat, AList<TimeStampedLyric>>();
var lyrics = parser.ParseFromString(lrcContent);

var format = new LrcFormat();
string lrcString = format.Revert(lyrics);
```

## Format-Specific Details

### LRC Format

**Structure:**
```
[mm:ss.xx]Lyric text
[mm:ss.xx]Another line
```

**Metadata Tags:**
```
[ar:Artist Name]
[ti:Song Title]
[al:Album Name]
[by:Creator]
[offset:+/-milliseconds]
```

**Example:**
```csharp
string lrc = @"
[ar:Rick Astley]
[ti:Never Gonna Give You Up]
[al:Whenever You Need Somebody]
[00:12.00]We're no strangers to love
[00:16.00]You know the rules and so do I
";

var parser = new FileParser<LrcFormat, AList<TimeStampedLyric>>();
var lyrics = parser.ParseFromString(lrc);
```

### SRT Format

**Structure:**
```
1
00:00:12,000 --> 00:00:15,000
First subtitle

2
00:00:16,000 --> 00:00:20,000
Second subtitle
```

**Example:**
```csharp
var parser = new FileParser<SrtFormat, AList<TimeStampedLyric>>();
var subtitles = parser.ParseFromDisk("movie.srt");
```

### ENV Format

**Structure:**
```
KEY=value
DATABASE_URL=postgresql://localhost/mydb
API_KEY=secret123
# Comments are supported
```

**Example:**
```csharp
var parser = new FileParser<EnvFormat, Dictionary<string, string>>();
var config = parser.ParseFromString("DB_HOST=localhost\nDB_PORT=5432");

Console.WriteLine(config["DB_HOST"]);  // "localhost"
Console.WriteLine(config["DB_PORT"]);  // "5432"
```

## Core Classes

### FileParser<P, T>

Generic parser that works with any format:

```csharp
public class FileParser<P, T> where P : FileFormat<string, T>
{
    T ParseFromString(string content);
    bool TryParseFromString(string content, out T parsed);
    T ParseFromDisk(string filePath);
    bool TryParseFromDisk(string filePath, out T parsed);
}
```

### FileFormat<TInput, TOutput>

Base class for all format parsers:

```csharp
public abstract class FileFormat<TInput, TOutput>
{
    abstract TOutput Parse(TInput input);
    abstract bool TryParse(TInput input, out TOutput parsed);
}
```

### RevertableFileFormat<TInput, TOutput>

For formats that can be converted back:

```csharp
public abstract class RevertableFileFormat<TInput, TOutput> : FileFormat<TInput, TOutput>
{
    abstract TInput Revert(TOutput parsed);
}
```

### Data Structures

| Class | Description |
|-------|-------------|
| `TimeStampedLyric` | Single lyric line with timestamp |
| `RichTimeStampedLyric` | Lyric line with word-level timing |
| `RichTimeStampedWord` | Individual word with timestamp |
| `RawLyric` | Unparsed lyric data |

## Utilities

### LyricsUtils

```csharp
using DevBase.Format.Utilities;

// Utility methods for lyrics manipulation
string cleaned = LyricsUtils.CleanLyrics(rawLyrics);
bool hasTimestamps = LyricsUtils.HasTimestamps(lrcContent);
```

### TimeUtils

```csharp
using DevBase.Format.Utilities;

// Time conversion utilities
TimeSpan time = TimeUtils.ParseLrcTimestamp("[00:12.50]");
string formatted = TimeUtils.FormatLrcTimestamp(TimeSpan.FromSeconds(12.5));
```

## Extensions

### LyricsExtensions

```csharp
using DevBase.Format.Extensions;

var lyrics = new AList<TimeStampedLyric>();
// Extension methods for lyrics manipulation
var sorted = lyrics.SortByTimestamp();
var filtered = lyrics.FilterByTimeRange(TimeSpan.Zero, TimeSpan.FromMinutes(2));
```

## Error Handling

### Using Try-Parse Pattern

```csharp
var parser = new FileParser<LrcFormat, AList<TimeStampedLyric>>();

if (parser.TryParseFromDisk("song.lrc", out var lyrics))
{
    // Success
    ProcessLyrics(lyrics);
}
else
{
    // Handle parsing failure
    Console.WriteLine("Invalid LRC format");
}
```

### Catching Exceptions

```csharp
try
{
    var lyrics = parser.ParseFromString(content);
}
catch (ParsingException ex)
{
    Console.WriteLine($"Parsing error: {ex.Message}");
}
catch (FileNotFoundException ex)
{
    Console.WriteLine($"File not found: {ex.Message}");
}
```

## Integration with DevBase.Api

Many API clients return lyrics that can be parsed:

```csharp
using DevBase.Api.Apis.Deezer;
using DevBase.Format;
using DevBase.Format.Formats.LrcFormat;

var deezer = new Deezer();
var lyricsString = await deezer.GetLyrics("123456");

var parser = new FileParser<LrcFormat, AList<TimeStampedLyric>>();
var lyrics = parser.ParseFromString(lyricsString);

foreach (var line in lyrics)
{
    Console.WriteLine($"[{line.StartTime}] {line.Text}");
}
```

## Performance Tips

1. **Reuse parser instances** for multiple files
2. **Use Try-Parse** for better performance (no exceptions)
3. **Parse from string** when data is already in memory
4. **Use appropriate format** - don't parse LRC as SRT
5. **Cache parsed results** for frequently accessed files

## Common Patterns

### Pattern 1: Multi-Format Lyrics Parser

```csharp
AList<TimeStampedLyric> ParseLyrics(string content)
{
    // Try LRC first
    var lrcParser = new FileParser<LrcFormat, AList<TimeStampedLyric>>();
    if (lrcParser.TryParseFromString(content, out var lrcLyrics))
        return lrcLyrics;
    
    // Try SRT
    var srtParser = new FileParser<SrtFormat, AList<TimeStampedLyric>>();
    if (srtParser.TryParseFromString(content, out var srtLyrics))
        return srtLyrics;
    
    // Try Apple XML
    var appleParser = new FileParser<AppleXmlFormat, AList<TimeStampedLyric>>();
    if (appleParser.TryParseFromString(content, out var appleLyrics))
        return appleLyrics;
    
    return null;
}
```

### Pattern 2: Lyrics Synchronization

```csharp
void SyncLyricsToAudio(AList<TimeStampedLyric> lyrics, TimeSpan currentTime)
{
    var currentLyric = lyrics.Find(l => 
        l.StartTime <= currentTime && 
        (lyrics.IndexOf(l) == lyrics.Length - 1 || 
         lyrics[lyrics.IndexOf(l) + 1].StartTime > currentTime)
    );
    
    if (currentLyric != null)
        DisplayLyric(currentLyric.Text);
}
```

### Pattern 3: Format Conversion

```csharp
// Convert LRC to SRT
var lrcParser = new FileParser<LrcFormat, AList<TimeStampedLyric>>();
var lyrics = lrcParser.ParseFromDisk("song.lrc");

var srtFormat = new SrtFormat();
string srtContent = srtFormat.Revert(lyrics);
File.WriteAllText("song.srt", srtContent);
```

## Target Framework

- **.NET 9.0**

## Dependencies

- **DevBase** - Core utilities and collections
- **System.Text.RegularExpressions** - Pattern matching

## License

MIT License - See LICENSE file for details

## Author

AlexanderDotH

## Repository

https://github.com/AlexanderDotH/DevBase
