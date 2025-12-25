using DevBase.Api.Enums;
using DevBase.Exception;

namespace DevBase.Api.Exceptions;

/// <summary>
/// Exception thrown for Deezer API related errors.
/// </summary>
public class DeezerException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeezerException"/> class.
    /// </summary>
    /// <param name="type">The type of error.</param>
    public DeezerException(EnumDeezerExceptionType type) : base(GetMessage(type)) { }

    private static string GetMessage(EnumDeezerExceptionType type)
    {
        switch (type)
        {
            case EnumDeezerExceptionType.ArlToken:
                return "No arl token present";
            case EnumDeezerExceptionType.AppId:
                return "Invalid AppID";
            case EnumDeezerExceptionType.AppSessionId:
                return "AppID and SessionID mismatch";
            case EnumDeezerExceptionType.SessionId:
                return "Invalid SessionID";
            case EnumDeezerExceptionType.NoCsrfToken:
                return "No CSRF provided";
            case EnumDeezerExceptionType.InvalidCsrfToken:
                return "Invalid CSRF token";
            case EnumDeezerExceptionType.JwtExpired:
                return "The Jwt token is expired";
            case EnumDeezerExceptionType.MissingSongDetails:
                return "Cannot find song details";
            case EnumDeezerExceptionType.WrongParameter:
                return "Wrong parameters provided";
            case EnumDeezerExceptionType.LyricsNotFound:
                return "Lyrics not found";
            case EnumDeezerExceptionType.CsrfParsing:
                return "Failed to parse csrf token";
            case EnumDeezerExceptionType.UserData:
                return "Failed to receive user data";
            case EnumDeezerExceptionType.UrlData:
                return "Failed to receive urls";
            case EnumDeezerExceptionType.FailedToReceiveSongDetails:
                return "Failed to receive song details";
        }

        throw new ErrorStatementException();
    }
}