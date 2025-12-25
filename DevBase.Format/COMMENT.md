# DevBase.Format Project Documentation

This document contains all class, method, and field signatures with their corresponding comments for the DevBase.Format project.

## Table of Contents

- [Exceptions](#exceptions)
  - [ParsingException](#parsingexception)
- [Core](#core)
  - [FileFormat&lt;F, T&gt;](#fileformatf-t)
  - [FileParser&lt;P, T&gt;](#fileparserp-t)
- [Extensions](#extensions)
  - [LyricsExtensions](#lyricsextensions)
- [Structure](#structure)
  - [RawLyric](#rawlyric)
  - [RegexHolder](#regexholder)
  - [RichTimeStampedLyric](#richtimestampedlyric)
  - [RichTimeStampedWord](#richtimestampedword)
  - [TimeStampedLyric](#timestampedlyric)
- [Formats](#formats)
  - [Format Parsers Overview](#format-parsers-overview)

## Exceptions

### ParsingException

```csharp
/// <summary>
/// Exception thrown when a parsing error occurs.
/// </summary>
public class ParsingException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParsingException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ParsingException(string message)
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ParsingException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public ParsingException(string message, System.Exception innerException)
}
```

## Core

### FileFormat&lt;F, T&gt;

```csharp
/// <summary>
/// Base class for defining file formats and their parsing logic.
/// </summary>
/// <typeparam name="F">The type of the input format (e.g., string, byte[]).</typeparam>
/// <typeparam name="T">The type of the parsed result.</typeparam>
public abstract class FileFormat<F, T>
{
    /// <summary>
    /// Gets or sets a value indicating whether strict error handling is enabled.
    /// If true, exceptions are thrown on errors; otherwise, default values are returned.
    /// </summary>
    public bool StrictErrorHandling { get; set; }
    
    /// <summary>
    /// Parses the input into the target type.
    /// </summary>
    /// <param name="from">The input data to parse.</param>
    /// <returns>The parsed object of type <typeparamref name="T"/>.</returns>
    public abstract T Parse(F from)
    
    /// <summary>
    /// Attempts to parse the input into the target type.
    /// </summary>
    /// <param name="from">The input data to parse.</param>
    /// <param name="parsed">The parsed object, or default if parsing fails.</param>
    /// <returns>True if parsing was successful; otherwise, false.</returns>
    public abstract bool TryParse(F from, out T parsed)
    
    /// <summary>
    /// Handles errors during parsing. Throws an exception if strict error handling is enabled.
    /// </summary>
    /// <typeparam name="TX">The return type (usually nullable or default).</typeparam>
    /// <param name="message">The error message.</param>
    /// <param name="callerMember">The calling member name.</param>
    /// <param name="callerFilePath">The source file path.</param>
    /// <param name="callerLineNumber">The source line number.</param>
    /// <returns>The default value of <typeparamref name="TX"/> if strict error handling is disabled.</returns>
    protected dynamic Error<TX>(string message, [CallerMemberName] string callerMember = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
    
    /// <summary>
    /// Handles exceptions during parsing. Rethrows wrapped in a ParsingException if strict error handling is enabled.
    /// </summary>
    /// <typeparam name="TX">The return type.</typeparam>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="callerMember">The calling member name.</param>
    /// <param name="callerFilePath">The source file path.</param>
    /// <param name="callerLineNumber">The source line number.</param>
    /// <returns>The default value of <typeparamref name="TX"/> if strict error handling is disabled.</returns>
    protected dynamic Error<TX>(System.Exception exception, [CallerMemberName] string callerMember = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
}
```

### FileParser&lt;P, T&gt;

```csharp
/// <summary>
/// Provides high-level parsing functionality using a specific file format.
/// </summary>
/// <typeparam name="P">The specific file format implementation.</typeparam>
/// <typeparam name="T">The result type of the parsing.</typeparam>
public class FileParser<P, T> where P : FileFormat<string, T>
{
    /// <summary>
    /// Parses content from a string.
    /// </summary>
    /// <param name="content">The string content to parse.</param>
    /// <returns>The parsed object.</returns>
    public T ParseFromString(string content)
    
    /// <summary>
    /// Attempts to parse content from a string.
    /// </summary>
    /// <param name="content">The string content to parse.</param>
    /// <param name="parsed">The parsed object, or default on failure.</param>
    /// <returns>True if parsing was successful; otherwise, false.</returns>
    public bool TryParseFromString(string content, out T parsed)
    
    /// <summary>
    /// Parses content from a file on disk.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <returns>The parsed object.</returns>
    public T ParseFromDisk(string filePath)
    
    /// <summary>
    /// Attempts to parse content from a file on disk.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <param name="parsed">The parsed object, or default on failure.</param>
    /// <returns>True if parsing was successful; otherwise, false.</returns>
    public bool TryParseFromDisk(string filePath, out T parsed)
    
    /// <summary>
    /// Parses content from a file on disk using a FileInfo object.
    /// </summary>
    /// <param name="fileInfo">The FileInfo object representing the file.</param>
    /// <returns>The parsed object.</returns>
    public T ParseFromDisk(FileInfo fileInfo)
}
```

## Extensions

### LyricsExtensions

```csharp
/// <summary>
/// Provides extension methods for converting between different lyric structures and text formats.
/// </summary>
public static class LyricsExtensions
{
    /// <summary>
    /// Converts a list of raw lyrics to a plain text string.
    /// </summary>
    /// <param name="rawElements">The list of raw lyrics.</param>
    /// <returns>A string containing the lyrics.</returns>
    public static string ToPlainText(this AList<RawLyric> rawElements)
    
    /// <summary>
    /// Converts a list of time-stamped lyrics to a plain text string.
    /// </summary>
    /// <param name="elements">The list of time-stamped lyrics.</param>
    /// <returns>A string containing the lyrics.</returns>
    public static string ToPlainText(this AList<TimeStampedLyric> elements)
    
    /// <summary>
    /// Converts a list of rich time-stamped lyrics to a plain text string.
    /// </summary>
    /// <param name="richElements">The list of rich time-stamped lyrics.</param>
    /// <returns>A string containing the lyrics.</returns>
    public static string ToPlainText(this AList<RichTimeStampedLyric> richElements)
    
    /// <summary>
    /// Converts a list of time-stamped lyrics to raw lyrics (removing timestamps).
    /// </summary>
    /// <param name="timeStampedLyrics">The list of time-stamped lyrics.</param>
    /// <returns>A list of raw lyrics.</returns>
    public static AList<RawLyric> ToRawLyrics(this AList<TimeStampedLyric> timeStampedLyrics)
    
    /// <summary>
    /// Converts a list of rich time-stamped lyrics to raw lyrics (removing timestamps and extra data).
    /// </summary>
    /// <param name="richTimeStampedLyrics">The list of rich time-stamped lyrics.</param>
    /// <returns>A list of raw lyrics.</returns>
    public static AList<RawLyric> ToRawLyrics(this AList<RichTimeStampedLyric> richTimeStampedLyrics)
    
    /// <summary>
    /// Converts a list of rich time-stamped lyrics to standard time-stamped lyrics (simplifying the structure).
    /// </summary>
    /// <param name="richElements">The list of rich time-stamped lyrics.</param>
    /// <returns>A list of time-stamped lyrics.</returns>
    public static AList<TimeStampedLyric> ToTimeStampedLyrics(this AList<RichTimeStampedLyric> richElements)
}
```

## Structure

### RawLyric

```csharp
/// <summary>
/// Represents a basic lyric line without timestamps.
/// </summary>
public class RawLyric
{
    /// <summary>
    /// Gets or sets the text of the lyric line.
    /// </summary>
    public string Text { get; set; }
}
```

### RegexHolder

```csharp
/// <summary>
/// Holds compiled Regular Expressions for various lyric formats.
/// </summary>
public class RegexHolder
{
    /// <summary>Regex pattern for standard LRC format.</summary>
    public const string REGEX_LRC = "((\\[)([0-9]*)([:])([0-9]*)([:]|[.])(\\d+\\.\\d+|\\d+)(\\]))((\\s|.).*$)";
    /// <summary>Regex pattern for garbage/metadata lines.</summary>
    public const string REGEX_GARBAGE = "\\D(\\?{0,2}).([:]).([\\w /]*)";
    /// <summary>Regex pattern for environment variables/metadata.</summary>
    public const string REGEX_ENV = "(\\w*)\\=\"(\\w*)";
    /// <summary>Regex pattern for SRT timestamps.</summary>
    public const string REGEX_SRT_TIMESTAMPS = "([0-9:,]*)(\\W(-->)\\W)([0-9:,]*)";
    /// <summary>Regex pattern for Enhanced LRC (ELRC) format data.</summary>
    public const string REGEX_ELRC_DATA = "(\\[)([0-9]*)([:])([0-9]*)([:])(\\d+\\.\\d+|\\d+)(\\])(\\s-\\s)(\\[)([0-9]*)([:])([0-9]*)([:])(\\d+\\.\\d+|\\d+)(\\])\\s(.*$)";
    /// <summary>Regex pattern for KLyrics word format.</summary>
    public const string REGEX_KLYRICS_WORD = "(\\()([0-9]*)(\\,)([0-9]*)(\\))([^\\(\\)\\[\\]\\n]*)";
    /// <summary>Regex pattern for KLyrics timestamp format.</summary>
    public const string REGEX_KLYRICS_TIMESTAMPS = "(\\[)([0-9]*)(\\,)([0-9]*)(\\])";

    /// <summary>Compiled Regex for standard LRC format.</summary>
    public static Regex RegexLrc { get; }
    /// <summary>Compiled Regex for garbage/metadata lines.</summary>
    public static Regex RegexGarbage { get; }
    /// <summary>Compiled Regex for environment variables/metadata.</summary>
    public static Regex RegexEnv { get; }
    /// <summary>Compiled Regex for SRT timestamps.</summary>
    public static Regex RegexSrtTimeStamps { get; }
    /// <summary>Compiled Regex for Enhanced LRC (ELRC) format data.</summary>
    public static Regex RegexElrc { get; }
    /// <summary>Compiled Regex for KLyrics word format.</summary>
    public static Regex RegexKlyricsWord { get; }
    /// <summary>Compiled Regex for KLyrics timestamp format.</summary>
    public static Regex RegexKlyricsTimeStamps { get; }
}
```

### RichTimeStampedLyric

```csharp
/// <summary>
/// Represents a lyric line with start/end times and individual word timestamps.
/// </summary>
public class RichTimeStampedLyric
{
    /// <summary>
    /// Gets or sets the full text of the lyric line.
    /// </summary>
    public string Text { get; set; }
    
    /// <summary>
    /// Gets or sets the start time of the lyric line.
    /// </summary>
    public TimeSpan StartTime { get; set; }
    
    /// <summary>
    /// Gets or sets the end time of the lyric line.
    /// </summary>
    public TimeSpan EndTime { get; set; }
    
    /// <summary>
    /// Gets the start timestamp in total milliseconds.
    /// </summary>
    public long StartTimestamp { get; }
    
    /// <summary>
    /// Gets the end timestamp in total milliseconds.
    /// </summary>
    public long EndTimestamp { get; }
    
    /// <summary>
    /// Gets or sets the list of words with their own timestamps within this line.
    /// </summary>
    public AList<RichTimeStampedWord> Words { get; set; }
}
```

### RichTimeStampedWord

```csharp
/// <summary>
/// Represents a single word in a lyric with start and end times.
/// </summary>
public class RichTimeStampedWord
{
    /// <summary>
    /// Gets or sets the word text.
    /// </summary>
    public string Word { get; set; }
    
    /// <summary>
    /// Gets or sets the start time of the word.
    /// </summary>
    public TimeSpan StartTime { get; set; }
    
    /// <summary>
    /// Gets or sets the end time of the word.
    /// </summary>
    public TimeSpan EndTime { get; set; }
    
    /// <summary>
    /// Gets the start timestamp in total milliseconds.
    /// </summary>
    public long StartTimestamp { get; }
    
    /// <summary>
    /// Gets the end timestamp in total milliseconds.
    /// </summary>
    public long EndTimestamp { get; }
}
```

### TimeStampedLyric

```csharp
/// <summary>
/// Represents a lyric line with a start time.
/// </summary>
public class TimeStampedLyric
{
    /// <summary>
    /// Gets or sets the text of the lyric line.
    /// </summary>
    public string Text { get; set; }
    
    /// <summary>
    /// Gets or sets the start time of the lyric line.
    /// </summary>
    public TimeSpan StartTime { get; set; }

    /// <summary>
    /// Gets the start timestamp in total milliseconds.
    /// </summary>
    public long StartTimestamp { get; }
}
```

## Formats

### Format Parsers Overview

The DevBase.Format project includes various format parsers:

- **LrcParser** - Parses standard LRC format into `AList<TimeStampedLyric>`
- **ElrcParser** - Parses enhanced LRC format into `AList<RichTimeStampedLyric>`
- **KLyricsParser** - Parses KLyrics format into `AList<RichTimeStampedLyric>`
- **SrtParser** - Parses SRT subtitle format into `AList<RichTimeStampedLyric>`
- **AppleLrcXmlParser** - Parses Apple's Line-timed TTML XML into `AList<TimeStampedLyric>`
- **AppleRichXmlParser** - Parses Apple's Word-timed TTML XML into `AList<RichTimeStampedLyric>`
- **AppleXmlParser** - Parses Apple's non-timed TTML XML into `AList<RawLyric>`
- **MmlParser** - Parses Musixmatch JSON format into `AList<TimeStampedLyric>`
- **RmmlParser** - Parses Rich Musixmatch JSON format into `AList<RichTimeStampedLyric>`
- **EnvParser** - Parses KEY=VALUE style content
- **RlrcParser** - Parses raw lines as lyrics

Each parser extends the `FileFormat<string, T>` base class and implements the `Parse` and `TryParse` methods for their specific format types.
