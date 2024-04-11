namespace DevBase.Api.Enums;

public enum EnumDeezerExceptionType
{
    ArlToken, 
    AppId, 
    AppSessionId, 
    SessionId, 
    NoCsrfToken, 
    InvalidCsrfToken, 
    JwtExpired, 
    MissingSongDetails,
    FailedToReceiveSongDetails,
    WrongParameter, 
    LyricsNotFound,
    CsrfParsing,
    UserData,
    UrlData
}