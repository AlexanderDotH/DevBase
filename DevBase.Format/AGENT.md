# DevBase.Format Agent Guide

## Overview
DevBase.Format provides parsers for lyrics (LRC), subtitles (SRT), and environment files (ENV).

## Key Classes

| Class | Namespace | Purpose |
|-------|-----------|---------|
| `LrcParser` | `DevBase.Format.Formats.LrcFormat` | LRC lyrics parser |
| `SrtParser` | `DevBase.Format.Formats.SrtFormat` | SRT subtitle parser |
| `EnvParser` | `DevBase.Format.Formats.EnvFormat` | ENV file parser |
| `TimeStampedLyric` | `DevBase.Format.Structure` | Simple timestamped lyric |
| `RichTimeStampedLyric` | `DevBase.Format.Structure` | Word-level timestamped lyric |

## Quick Reference

### Parse LRC Lyrics
```csharp
using DevBase.Format.Formats.LrcFormat;
using DevBase.Format.Structure;

LrcParser parser = new LrcParser();
AList<TimeStampedLyric> lyrics = parser.Parse(lrcContent);

foreach (var lyric in lyrics.GetAsList())
{
    Console.WriteLine($"{lyric.StartTime}: {lyric.Text}");
}
```

### Parse SRT Subtitles
```csharp
using DevBase.Format.Formats.SrtFormat;

SrtParser parser = new SrtParser();
AList<RichTimeStampedLyric> subtitles = parser.Parse(srtContent);
```

### Parse from File
```csharp
using DevBase.Format;

var fileParser = new FileParser<LrcParser, AList<TimeStampedLyric>>();
var lyrics = fileParser.ParseFromDisk(fileInfo);
```

## Data Structures

### TimeStampedLyric
```csharp
public class TimeStampedLyric
{
    public TimeSpan StartTime { get; set; }
    public string Text { get; set; }
}
```

### RichTimeStampedLyric
```csharp
public class RichTimeStampedLyric
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Text { get; set; }
    public AList<RichTimeStampedWord> Words { get; set; }
}
```

### RichTimeStampedWord
```csharp
public class RichTimeStampedWord
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Word { get; set; }
}
```

## File Structure
```
DevBase.Format/
├── Formats/
│   ├── LrcFormat/
│   │   └── LrcParser.cs
│   ├── SrtFormat/
│   │   └── SrtParser.cs
│   ├── EnvFormat/
│   │   └── EnvParser.cs
│   └── KLyricsFormat/     # Karaoke lyrics
├── Structure/
│   ├── TimeStampedLyric.cs
│   ├── RichTimeStampedLyric.cs
│   └── RichTimeStampedWord.cs
└── FileParser.cs
```

## Important Notes

1. **Use `AList<T>`** for parsed results (not `List<T>`)
2. **TimeSpan for timestamps** (not long/int)
3. **RichTimeStampedLyric** has word-level timing
4. **TimeStampedLyric** has line-level timing only
5. **FileParser<TParser, TResult>** for file-based parsing
