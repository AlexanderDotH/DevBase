# DevBase.Format

A versatile file format parser library for .NET supporting various text-based formats including lyrics, subtitles, and configuration files.

## Features

- **LRC Parser** - Standard lyrics format with timestamps
- **ELRC Parser** - Enhanced LRC with word-level timing
- **SRT Parser** - SubRip subtitle format
- **ENV Parser** - Environment variable files
- **Apple Lyrics XML** - Apple Music lyrics formats
- **MML/RMML** - Musixmatch lyrics formats
- **Extensible Architecture** - Easy to add new format parsers

## Installation

```xml
<PackageReference Include="DevBase.Format" Version="x.x.x" />
```

Or via NuGet CLI:

```bash
dotnet add package DevBase.Format
```

## Supported Formats

| Format | Extension | Description |
|--------|-----------|-------------|
| LRC | `.lrc` | Standard lyrics with line timestamps |
| ELRC | `.elrc` | Enhanced LRC with word timing |
| RLRC | `.rlrc` | Rich LRC format |
| SRT | `.srt` | SubRip subtitle format |
| ENV | `.env` | Environment configuration |
| Apple XML | `.xml` | Apple Music lyrics XML |
| Apple Rich XML | `.xml` | Apple Music rich sync lyrics |
| MML | `.json` | Musixmatch lyrics format |
| RMML | `.json` | Rich Musixmatch format |
| KLyrics | - | Korean lyrics format |

## Usage Examples

### LRC Parsing

```csharp
using DevBase.Format.Formats.LrcFormat;

string lrcContent = @"
[00:12.00]Line one
[00:17.20]Line two
[00:21.10]Line three
";

LrcParser parser = new LrcParser();
var lyrics = parser.Parse(lrcContent);

foreach (var line in lyrics.Lines)
{
    Console.WriteLine($"[{line.Timestamp}] {line.Text}");
}
```

### SRT Parsing

```csharp
using DevBase.Format.Formats.SrtFormat;

string srtContent = @"
1
00:00:12,000 --> 00:00:17,200
First subtitle line

2
00:00:17,200 --> 00:00:21,100
Second subtitle line
";

SrtParser parser = new SrtParser();
var subtitles = parser.Parse(srtContent);
```

### ENV File Parsing

```csharp
using DevBase.Format.Formats.EnvFormat;

string envContent = @"
DATABASE_URL=postgres://localhost:5432/db
API_KEY=secret123
DEBUG=true
";

EnvParser parser = new EnvParser();
var variables = parser.Parse(envContent);

string dbUrl = variables["DATABASE_URL"];
```

### Generic File Parsing

```csharp
using DevBase.Format;

// Auto-detect format by extension
FileParser fileParser = new FileParser();
var result = fileParser.Parse("lyrics.lrc");

// Or specify format explicitly
var lrcResult = fileParser.Parse<LrcParser>("content");
```

### Converting Between Formats

```csharp
// Parse LRC
LrcParser lrcParser = new LrcParser();
var lyrics = lrcParser.Parse(lrcContent);

// Convert to SRT
SrtParser srtParser = new SrtParser();
string srtOutput = srtParser.ToString(lyrics);
```

## Architecture

```
DevBase.Format/
├── FileFormat.cs           # Base format class
├── FileParser.cs           # Generic file parser
├── RevertableFileFormat.cs # Format with undo support
├── Formats/
│   ├── LrcFormat/          # LRC parser
│   ├── ElrcFormat/         # Enhanced LRC parser
│   ├── RlrcFormat/         # Rich LRC parser
│   ├── SrtFormat/          # SRT subtitle parser
│   ├── EnvFormat/          # Environment file parser
│   ├── AppleXmlFormat/     # Apple lyrics XML
│   ├── AppleLrcXmlFormat/  # Apple LRC XML
│   ├── AppleRichXmlFormat/ # Apple rich sync XML
│   ├── MmlFormat/          # Musixmatch format
│   ├── RmmlFormat/         # Rich Musixmatch format
│   └── KLyricsFormat/      # Korean lyrics format
├── Structure/              # Common data structures
└── Utilities/              # Helper utilities
```

## License

MIT License - see LICENSE file for details.
