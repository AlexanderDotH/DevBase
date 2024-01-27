namespace DevBase.Requests.Exceptions;

public class HttpHeaderException : System.Exception
{
    public HttpHeaderException(string message) : base(message) { }
}