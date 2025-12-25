namespace DevBase.Typography.Encoded;

/// <summary>
/// Abstract base class for encoded strings.
/// </summary>
public abstract class EncodedAString : AString
{
    /// <summary>
    /// Gets the decoded AString.
    /// </summary>
    /// <returns>The decoded AString.</returns>
    public abstract AString GetDecoded();

    /// <summary>
    /// Checks if the string is properly encoded.
    /// </summary>
    /// <returns>True if encoded, false otherwise.</returns>
    public abstract bool IsEncoded();
    
    /// <summary>
    /// Initializes a new instance of the <see cref="EncodedAString"/> class.
    /// </summary>
    /// <param name="value">The encoded string value.</param>
    protected EncodedAString(string value) : base(value) { }
}