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
    WrongParameter, 
    LyricsNotFound,
    CsrfParsing,
    UserData,
    UrlData
}