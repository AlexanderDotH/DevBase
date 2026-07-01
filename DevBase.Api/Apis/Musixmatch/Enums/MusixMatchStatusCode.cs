namespace DevBase.Api.Apis.Musixmatch;

public enum MusixMatchStatusCode
{
    Undefined = 0,
    Success = 200,
    BadSyntax = 400,
    AuthFailed = 401,
    UsageLimitReached = 402,
    NotAuthorized = 403,
    ResourceNotFound = 404,
    MethodNotFound = 405,
    ConflictDetected = 409,
    ServerError = 500,
    ServerBusy = 503
}
