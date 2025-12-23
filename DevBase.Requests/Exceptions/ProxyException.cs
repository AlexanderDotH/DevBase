using DevBase.Requests.Proxy;

namespace DevBase.Requests.Exceptions;

public class ProxyException : System.Exception
{
    public ProxyInfo? Proxy { get; }
    public int AttemptNumber { get; }

    public ProxyException(string message, ProxyInfo? proxy = null, int attemptNumber = 0)
        : base(message)
    {
        Proxy = proxy;
        AttemptNumber = attemptNumber;
    }

    public ProxyException(string message, System.Exception innerException, ProxyInfo? proxy = null, int attemptNumber = 0)
        : base(message, innerException)
    {
        Proxy = proxy;
        AttemptNumber = attemptNumber;
    }
}
