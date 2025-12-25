using DevBase.Api.Enums;
using DevBase.Exception;

namespace DevBase.Api.Exceptions;

/// <summary>
/// Exception thrown for Apple Music API related errors.
/// </summary>
public class AppleMusicException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppleMusicException"/> class.
    /// </summary>
    /// <param name="type">The type of error.</param>
    public AppleMusicException(EnumAppleMusicExceptionType type) : base(GetMessage(type)) { }

    private static string GetMessage(EnumAppleMusicExceptionType type)
    {
        switch (type)
        {
            case EnumAppleMusicExceptionType.UnprovidedUserMediaToken:
                return "User-Media-Token is not set";
            case EnumAppleMusicExceptionType.AccessTokenUnavailable:
                return "Failed to receive access token";
            case EnumAppleMusicExceptionType.SearchResultsEmpty:
                return "Search results are empty";
        }

        throw new ErrorStatementException();
    }
}