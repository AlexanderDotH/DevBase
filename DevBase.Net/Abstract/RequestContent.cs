namespace DevBase.Net.Abstract;

/// <summary>
/// Abstract base class for validating content of a request.
/// </summary>
public abstract class RequestContent
{
    /// <summary>
    /// Validates whether the provided content is valid according to the implementation rules.
    /// </summary>
    /// <param name="content">The content to validate.</param>
    /// <returns>True if valid, false otherwise.</returns>
    public abstract bool IsValid(ReadOnlySpan<byte> content);
}