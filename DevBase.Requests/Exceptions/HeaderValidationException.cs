namespace DevBase.Requests.Exceptions;

public class HeaderValidationException : System.Exception
{
    public string HeaderName { get; }
    public string ValidationError { get; }

    public HeaderValidationException(string headerName, string validationError)
        : base($"Header '{headerName}' validation failed: {validationError}")
    {
        HeaderName = headerName;
        ValidationError = validationError;
    }
}
