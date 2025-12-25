namespace DevBase.Extensions.Exceptions;

/// <summary>
/// Exception thrown when a stopwatch operation is invalid, such as accessing results while it is still running.
/// </summary>
public class StopwatchException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StopwatchException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public StopwatchException(string message) : base(message) { }
}