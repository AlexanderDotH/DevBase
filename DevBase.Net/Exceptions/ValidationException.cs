namespace DevBase.Net.Exceptions;

public class ValidationException : System.Exception
{
    public string PropertyName { get; }
    public object? InvalidValue { get; }

    public ValidationException(string propertyName, string message, object? invalidValue = null)
        : base(message)
    {
        PropertyName = propertyName;
        InvalidValue = invalidValue;
    }

    public ValidationException(string propertyName, string message, System.Exception innerException, object? invalidValue = null)
        : base(message, innerException)
    {
        PropertyName = propertyName;
        InvalidValue = invalidValue;
    }
}
