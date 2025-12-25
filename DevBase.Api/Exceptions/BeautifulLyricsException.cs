using DevBase.Api.Enums;
using DevBase.Exception;

namespace DevBase.Api.Exceptions;

/// <summary>
/// Exception thrown for Beautiful Lyrics API related errors.
/// </summary>
public class BeautifulLyricsException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BeautifulLyricsException"/> class.
    /// </summary>
    /// <param name="type">The type of error.</param>
    public BeautifulLyricsException(EnumBeautifulLyricsExceptionType type) : base(GetMessage(type)) { }

    private static string GetMessage(EnumBeautifulLyricsExceptionType type)
    {
        switch (type)
        {
            case EnumBeautifulLyricsExceptionType.LyricsNotFound:
                return "Failed to receive lyrics";
            case EnumBeautifulLyricsExceptionType.LyricsParsed:
                return "Failed to parse lyrics";
        }

        throw new ErrorStatementException();
    }
}