namespace DevBase.Requests.Exceptions;

public class RequestTimeoutException : System.Exception
{
    public TimeSpan Timeout { get; }
    public Uri? RequestUri { get; }
    public int AttemptNumber { get; }

    public RequestTimeoutException(TimeSpan timeout, Uri? requestUri = null, int attemptNumber = 0)
        : base($"Request timed out after {timeout.TotalSeconds:F1}s")
    {
        Timeout = timeout;
        RequestUri = requestUri;
        AttemptNumber = attemptNumber;
    }

    public RequestTimeoutException(string message, TimeSpan timeout, Uri? requestUri = null, int attemptNumber = 0)
        : base(message)
    {
        Timeout = timeout;
        RequestUri = requestUri;
        AttemptNumber = attemptNumber;
    }
}
