namespace DevBase.Exception;

/// <summary>
/// Exception thrown when an encoding error occurs.
/// </summary>
public class EncodingException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EncodingException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public EncodingException(string message) : base(message) {}
}