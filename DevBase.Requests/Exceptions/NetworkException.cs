namespace DevBase.Requests.Exceptions;

public class NetworkException : System.Exception
{
    public string? RemoteAddress { get; }
    public int AttemptNumber { get; }

    public NetworkException(string message, string? remoteAddress = null, int attemptNumber = 0)
        : base(message)
    {
        RemoteAddress = remoteAddress;
        AttemptNumber = attemptNumber;
    }

    public NetworkException(string message, System.Exception innerException, string? remoteAddress = null, int attemptNumber = 0)
        : base(message, innerException)
    {
        RemoteAddress = remoteAddress;
        AttemptNumber = attemptNumber;
    }
}
