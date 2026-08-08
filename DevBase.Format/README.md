# DevBase.Format

`DevBase.Format` parses lyric and configuration text formats for .NET 9.0. Use `FileParser<P, T>` to parse either an in-memory string or a file on disk.

## Installation

```bash
dotnet add package DevBase.Format
```

## Supported formats

| Format | Parser | Result |
| --- | --- | --- |
| LRC | `LrcParser` | `AList<TimeStampedLyric>` |
| ELRC | `ElrcParser` | `AList<RichTimeStampedLyric>` |
| RLRC | `RlrcParser` | `AList<RawLyric>` |
| SRT | `SrtParser` | `AList<RichTimeStampedLyric>` |
| KLyrics | `KLyricsParser` | `AList<RichTimeStampedLyric>` |
| MML JSON | `MmlParser` | `AList<TimeStampedLyric>` |
| RMML JSON | `RmmlParser` | `AList<RichTimeStampedLyric>` |
| Apple XML | `AppleXmlParser` | `AList<RawLyric>` |
| Apple LRC XML | `AppleLrcXmlParser` | `AList<TimeStampedLyric>` |
| Apple Rich XML | `AppleRichXmlParser` | `AList<RichTimeStampedLyric>` |
| ENV | `EnvParser` | `ATupleList<string, string>` |

`LrcParser`, `ElrcParser`, and `RlrcParser` are reversible and also implement `Revert` and `TryRevert`.

## Parser examples

Every example uses the same `FileParser<P, T>` API for both a source string and a disk file. `ParseFromDisk` reads the file; callers do not need to read it themselves first.

### LRC

Leading and trailing whitespace around each source line is trimmed. Metadata-style lines are ignored rather than returned as metadata objects.

```csharp
using DevBase.Format;
using DevBase.Format.Formats.LrcFormat;
using DevBase.Format.Structure;
using DevBase.Generics;

string lrcContent = @"
    [00:12.00]First line
    [00:15.50]Second line
    [00:20.00]Third line
    ";

var parser = new FileParser<LrcParser, AList<TimeStampedLyric>>();
AList<TimeStampedLyric> lyrics = parser.ParseFromString(lrcContent);
AList<TimeStampedLyric> lyricsFromFile = parser.ParseFromDisk("song.lrc");
```

### ELRC

ELRC stores a line's start/end time and individual word timings.

```csharp
using DevBase.Format;
using DevBase.Format.Formats.ElrcFormat;
using DevBase.Format.Structure;
using DevBase.Generics;

string elrcContent = @"
    [00:00:12.000] - [00:00:13.000] First line
    [
        [00:00:12.000] - [00:00:12.500] First
        [00:00:12.500] - [00:00:13.000] line
    ]
    ";

var parser = new FileParser<ElrcParser, AList<RichTimeStampedLyric>>();
AList<RichTimeStampedLyric> lyrics = parser.ParseFromString(elrcContent);
AList<RichTimeStampedLyric> lyricsFromFile = parser.ParseFromDisk("song.elrc");
```

### RLRC

RLRC is a list of raw lyric lines.

```csharp
using DevBase.Format;
using DevBase.Format.Formats.RlrcFormat;
using DevBase.Format.Structure;
using DevBase.Generics;

string rlrcContent = "First line\nSecond line";

var parser = new FileParser<RlrcParser, AList<RawLyric>>();
AList<RawLyric> lyrics = parser.ParseFromString(rlrcContent);
AList<RawLyric> lyricsFromFile = parser.ParseFromDisk("song.rlrc");
```

### SRT

`SrtParser` produces rich lyrics, including both a start and an end time.

```csharp
using DevBase.Format;
using DevBase.Format.Formats.SrtFormat;
using DevBase.Format.Structure;
using DevBase.Generics;

string srtContent = "1\n00:00:12,000 --> 00:00:15,000\nFirst subtitle\n\n";

var parser = new FileParser<SrtParser, AList<RichTimeStampedLyric>>();
AList<RichTimeStampedLyric> subtitles = parser.ParseFromString(srtContent);
AList<RichTimeStampedLyric> subtitlesFromFile = parser.ParseFromDisk("movie.srt");
```

### KLyrics

```csharp
using DevBase.Format;
using DevBase.Format.Formats.KLyricsFormat;
using DevBase.Format.Structure;
using DevBase.Generics;

string kLyricsContent = "[12000,1000](0,500)First(0,500)line";

var parser = new FileParser<KLyricsParser, AList<RichTimeStampedLyric>>();
AList<RichTimeStampedLyric> lyrics = parser.ParseFromString(kLyricsContent);
AList<RichTimeStampedLyric> lyricsFromFile = parser.ParseFromDisk("song.klyrics");
```

### MML JSON

`MmlParser` currently keeps only entries whose `text` matches the library's metadata-style pattern. Ordinary lyric text such as `First line` is filtered out; the example below reflects the parser's current behavior.

```csharp
using DevBase.Format;
using DevBase.Format.Formats.MmlFormat;
using DevBase.Format.Structure;
using DevBase.Generics;

string mmlContent = """
    [{"text":"[ar:Artist]","time":{"total":1250,"minutes":0,"seconds":1,"hundredths":25}}]
    """;

var parser = new FileParser<MmlParser, AList<TimeStampedLyric>>();
AList<TimeStampedLyric> lyrics = parser.ParseFromString(mmlContent);
AList<TimeStampedLyric> lyricsFromFile = parser.ParseFromDisk("song.mml");
```

### RMML JSON

```csharp
using DevBase.Format;
using DevBase.Format.Formats.RmmlFormat;
using DevBase.Format.Structure;
using DevBase.Generics;

string rmmlContent = """
    [{"ts":12.0,"te":13.0,"l":[{"c":"First","o":0},{"c":" line","o":1}],"x":"First line"}]
    """;

var parser = new FileParser<RmmlParser, AList<RichTimeStampedLyric>>();
AList<RichTimeStampedLyric> lyrics = parser.ParseFromString(rmmlContent);
AList<RichTimeStampedLyric> lyricsFromFile = parser.ParseFromDisk("song.rmml");
```

### Apple XML

Apple XML uses TTML with `itunes:timing="None"` and produces raw lyric lines.

```csharp
using DevBase.Format;
using DevBase.Format.Formats.AppleXmlFormat;
using DevBase.Format.Structure;
using DevBase.Generics;

string appleXmlContent = """
    <tt xmlns="http://www.w3.org/ns/ttml" xmlns:itunes="http://music.apple.com/lyric-ttml-internal" itunes:timing="None">
      <body><div><p>First line</p></div></body>
    </tt>
    """;

var parser = new FileParser<AppleXmlParser, AList<RawLyric>>();
AList<RawLyric> lyrics = parser.ParseFromString(appleXmlContent);
AList<RawLyric> lyricsFromFile = parser.ParseFromDisk("song-raw.xml");
```

### Apple LRC XML

Apple LRC XML uses TTML with `itunes:timing="Line"` and produces timestamped lines.

```csharp
using DevBase.Format;
using DevBase.Format.Formats.AppleLrcXmlFormat;
using DevBase.Format.Structure;
using DevBase.Generics;

string appleLrcXmlContent = """
    <tt xmlns="http://www.w3.org/ns/ttml" xmlns:itunes="http://music.apple.com/lyric-ttml-internal" itunes:timing="Line">
      <body><div><p begin="00:00:12.000" end="00:00:13.000">First line</p></div></body>
    </tt>
    """;

var parser = new FileParser<AppleLrcXmlParser, AList<TimeStampedLyric>>();
AList<TimeStampedLyric> lyrics = parser.ParseFromString(appleLrcXmlContent);
AList<TimeStampedLyric> lyricsFromFile = parser.ParseFromDisk("song-line.xml");
```

### Apple Rich XML

Apple Rich XML uses TTML with `itunes:timing="Word"` and produces word-timed lyrics.

```csharp
using DevBase.Format;
using DevBase.Format.Formats.AppleRichXmlFormat;
using DevBase.Format.Structure;
using DevBase.Generics;

string appleRichXmlContent = """
    <tt xmlns="http://www.w3.org/ns/ttml" xmlns:itunes="http://music.apple.com/lyric-ttml-internal" itunes:timing="Word">
      <body><div><p begin="00:00:12.000" end="00:00:13.000"><span begin="00:00:12.000" end="00:00:12.500">First</span><span begin="00:00:12.500" end="00:00:13.000">line</span></p></div></body>
    </tt>
    """;

var parser = new FileParser<AppleRichXmlParser, AList<RichTimeStampedLyric>>();
AList<RichTimeStampedLyric> lyrics = parser.ParseFromString(appleRichXmlContent);
AList<RichTimeStampedLyric> lyricsFromFile = parser.ParseFromDisk("song-word.xml");
```

### ENV

`EnvParser` returns an `ATupleList<string, string>`. Its current grammar expects quoted word values.

```csharp
using DevBase.Format;
using DevBase.Format.Formats.EnvFormat;
using DevBase.Generics;

string envContent = @"
    DB_HOST=""localhost""
    API_KEY=""secret123""
    ";

var parser = new FileParser<EnvParser, ATupleList<string, string>>();
ATupleList<string, string> config = parser.ParseFromString(envContent);
ATupleList<string, string> configFromFile = parser.ParseFromDisk(".env");

string dbHost = config.Get(0).Item2;
string apiKey = config.Get(1).Item2;
```

## Safe parsing

`FileParser<P, T>` creates a fresh parser with strict error handling enabled. To receive `false` for validation failures routed through `FileFormat.Error` instead of a `ParsingException`, use the concrete parser and set `StrictErrorHandling` to `false`.

```csharp
using DevBase.Format.Formats.LrcFormat;
using DevBase.Format.Structure;
using DevBase.Generics;

const string lrcContent = "[00:12.00]First line";
var parser = new LrcParser { StrictErrorHandling = false };

if (parser.TryParse(lrcContent, out AList<TimeStampedLyric> lyrics))
    Console.WriteLine($"Parsed {lyrics.Length} lines");
else
    Console.WriteLine("Failed to parse LRC lyrics");
```

## Reverting parsed formats

Use the concrete reversible parser to serialize parsed data. SRT does not currently support reversion.

```csharp
using DevBase.Format.Formats.LrcFormat;
using DevBase.Format.Structure;
using DevBase.Generics;

const string lrcContent = "[00:12.00]First line";
var parser = new LrcParser();
AList<TimeStampedLyric> lyrics = parser.Parse(lrcContent);
string lrcContentAgain = parser.Revert(lyrics);

if (parser.TryRevert(lyrics, out string reverted))
    Console.WriteLine(reverted);
```

## Implement a custom parser

```csharp
using DevBase.Format;
using DevBase.Generics;

var parser = new FileParser<PlainTextParser, AList<string>>();
AList<string> result = parser.ParseFromString("line1\nline2\nline3");

public class PlainTextParser : FileFormat<string, AList<string>>
{
    public override AList<string> Parse(string input)
    {
        AList<string> lines = new AList<string>();

        foreach (string line in input.Split('\n'))
        {
            string trimmed = line.Trim();

            if (!string.IsNullOrEmpty(trimmed))
                lines.Add(trimmed);
        }

        return lines;
    }

    public override bool TryParse(string input, out AList<string> parsed)
    {
        parsed = Parse(input);
        return !parsed.IsEmpty();
    }
}
```

## Data structures and extensions

| Class | Purpose |
| --- | --- |
| `TimeStampedLyric` | Text plus a start time |
| `RichTimeStampedLyric` | Text plus start/end times and timed words |
| `RichTimeStampedWord` | A word plus start/end times |
| `RawLyric` | A raw lyric line |

```csharp
using DevBase.Format.Extensions;
using DevBase.Format.Structure;
using DevBase.Generics;

AList<TimeStampedLyric> timedLyrics = new AList<TimeStampedLyric>();
string plainText = timedLyrics.ToPlainText();
AList<RawLyric> rawLyrics = timedLyrics.ToRawLyrics();

AList<RichTimeStampedLyric> richLyrics = new AList<RichTimeStampedLyric>();
AList<TimeStampedLyric> simplifiedLyrics = richLyrics.ToTimeStampedLyrics();
```

## Timestamp utility

```csharp
using DevBase.Format.Utilities;

if (TimeUtils.TryParseTimeStamp("00:12.50", out TimeSpan time))
    Console.WriteLine(time);

TimeSpan anotherTime = TimeUtils.ParseTimeStamp("00:12.50");
```

## Integration with DevBase.Api

When the `DevBase.Api` package or project is also referenced, `Deezer.GetLyrics` returns both raw lyric text and parsed timestamped lyrics.

```csharp
using DevBase.Api.Apis.Deezer;
using DevBase.Format.Structure;
using DevBase.Generics;

var deezer = new Deezer();
(string rawLyrics, AList<TimeStampedLyric> lyrics) = await deezer.GetLyrics("123456");

foreach (TimeStampedLyric line in lyrics)
    Console.WriteLine($"[{line.StartTime}] {line.Text}");
```

## Target framework

- .NET 9.0

## License

MIT License — see the repository license.

## Repository

https://github.com/AlexanderDotH/DevBase
