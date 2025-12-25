namespace DevBase.Format;

/// <summary>
/// Base class for file formats that support both parsing and reverting (serializing) operations.
/// </summary>
/// <typeparam name="F">The type of the input/output format.</typeparam>
/// <typeparam name="T">The type of the parsed object.</typeparam>
public abstract class RevertableFileFormat<F, T> : FileFormat<F, T>
{
    /// <summary>
    /// Reverts the object back to its original format representation.
    /// </summary>
    /// <param name="to">The object to revert.</param>
    /// <returns>The format representation of the object.</returns>
    public abstract F Revert(T to);

    /// <summary>
    /// Attempts to revert the object back to its original format representation.
    /// </summary>
    /// <param name="to">The object to revert.</param>
    /// <param name="from">The format representation, or default on failure.</param>
    /// <returns>True if reverting was successful; otherwise, false.</returns>
    public abstract bool TryRevert(T to, out F from);

}