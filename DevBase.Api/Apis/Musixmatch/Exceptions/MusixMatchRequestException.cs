namespace DevBase.Api.Apis.Musixmatch;

public sealed class MusixMatchRequestException : System.Exception
{
    private static readonly IReadOnlyDictionary<MusixMatchStatusCode, string> Messages =
        new Dictionary<MusixMatchStatusCode, string>
        {
            [MusixMatchStatusCode.Success] = "The request was successful.",
            [MusixMatchStatusCode.BadSyntax] = "The request had bad syntax or was inherently impossible to satisfy.",
            [MusixMatchStatusCode.AuthFailed] = "Authentication failed, probably because of an invalid or missing API key.",
            [MusixMatchStatusCode.UsageLimitReached] = "The usage limit has been reached.",
            [MusixMatchStatusCode.NotAuthorized] = "You are not authorized to perform this operation.",
            [MusixMatchStatusCode.ResourceNotFound] = "The requested resource was not found.",
            [MusixMatchStatusCode.MethodNotFound] = "The requested method was not found.",
            [MusixMatchStatusCode.ConflictDetected] = "The submitted data conflicts with the server data.",
            [MusixMatchStatusCode.ServerError] = "The Musixmatch server returned an error.",
            [MusixMatchStatusCode.ServerBusy] = "The Musixmatch server is busy."
        };

    public MusixMatchRequestException(int statusCode)
        : this((MusixMatchStatusCode)statusCode, statusCode)
    {
    }

    public MusixMatchRequestException(MusixMatchStatusCode statusCode)
        : this(statusCode, (int)statusCode)
    {
    }

    private MusixMatchRequestException(MusixMatchStatusCode statusCode, int rawStatusCode)
        : base(Messages.TryGetValue(statusCode, out string? message)
            ? message
            : $"Musixmatch request failed with status code {rawStatusCode}.")
    {
        this.StatusCode = statusCode;
        this.RawStatusCode = rawStatusCode;
    }

    public MusixMatchStatusCode StatusCode { get; }

    public int RawStatusCode { get; }
}
