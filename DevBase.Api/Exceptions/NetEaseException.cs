using DevBase.Api.Enums;
using DevBase.Exception;

namespace DevBase.Api.Exceptions;

public class NetEaseException : System.Exception
{
    public NetEaseException(EnumNetEaseExceptionType type) : base(GetMessage(type)) { }

    private static string GetMessage(EnumNetEaseExceptionType type)
    {
        switch (type)
        {
            case EnumNetEaseExceptionType.EmptySearchResults:
                return "Search results are empty";
            case EnumNetEaseExceptionType.EmptyUrls:
                return "Failed to receive track urls";
            case EnumNetEaseExceptionType.EmptyLyrics:
                return "Failed to receive lyrics";
            case EnumNetEaseExceptionType.DownloadTrack:
                return "Failed to download track";
        }

        throw new ErrorStatementException();
    }
}