namespace DevBase.Exception;

/// <summary>
/// Exception thrown when an exception state is not present.
/// </summary>
public class ErrorStatementException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorStatementException"/> class.
    /// </summary>
    public ErrorStatementException() : base("Exception state not present") { }
}