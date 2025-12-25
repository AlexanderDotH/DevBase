using DevBase.Api.Enums;
using DevBase.Exception;

namespace DevBase.Api.Exceptions;

/// <summary>
/// Exception thrown for NetEase API related errors.
/// </summary>
public class NetEaseException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NetEaseException"/> class.
    /// </summary>
    /// <param name="type">The type of error.</param>
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