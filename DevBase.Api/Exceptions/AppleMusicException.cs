using DevBase.Api.Enums;
using DevBase.Exception;

namespace DevBase.Api.Exceptions;

public class AppleMusicException : System.Exception
{
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