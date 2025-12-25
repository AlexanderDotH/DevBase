namespace DevBase.Cryptography.BouncyCastle.Exception;

/// <summary>
/// Exception thrown when a key pair operation is attempted but no key pair is found.
/// </summary>
public class KeypairNotFoundException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeypairNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public KeypairNotFoundException(string message) : base(message){ }
}