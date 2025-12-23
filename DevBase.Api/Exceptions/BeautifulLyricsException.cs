using DevBase.Api.Enums;
using DevBase.Exception;

namespace DevBase.Api.Exceptions;

public class BeautifulLyricsException : System.Exception
{
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