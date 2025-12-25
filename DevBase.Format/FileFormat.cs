using System.Runtime.CompilerServices;
using DevBase.Format.Exceptions;
using DevBase.IO;

namespace DevBase.Format;

/// <summary>
/// Base class for defining file formats and their parsing logic.
/// </summary>
/// <typeparam name="F">The type of the input format (e.g., string, byte[]).</typeparam>
/// <typeparam name="T">The type of the parsed result.</typeparam>
public abstract class FileFormat<F, T>
{
    /// <summary>
    /// Gets or sets a value indicating whether strict error handling is enabled.
    /// If true, exceptions are thrown on errors; otherwise, default values are returned.
    /// </summary>
    public bool StrictErrorHandling { get; set; } = true;
    
    /// <summary>
    /// Parses the input into the target type.
    /// </summary>
    /// <param name="from">The input data to parse.</param>
    /// <returns>The parsed object of type <typeparamref name="T"/>.</returns>
    public abstract T Parse(F from);
    
    /// <summary>
    /// Attempts to parse the input into the target type.
    /// </summary>
    /// <param name="from">The input data to parse.</param>
    /// <param name="parsed">The parsed object, or default if parsing fails.</param>
    /// <returns>True if parsing was successful; otherwise, false.</returns>
    public abstract bool TryParse(F from, out T parsed);
    
    /// <summary>
    /// Handles errors during parsing. Throws an exception if strict error handling is enabled.
    /// </summary>
    /// <typeparam name="TX">The return type (usually nullable or default).</typeparam>
    /// <param name="message">The error message.</param>
    /// <param name="callerMember">The calling member name.</param>
    /// <param name="callerFilePath">The source file path.</param>
    /// <param name="callerLineNumber">The source line number.</param>
    /// <returns>The default value of <typeparamref name="TX"/> if strict error handling is disabled.</returns>
    protected dynamic Error<TX>(
        string message, 
        [CallerMemberName] string callerMember = "", 
        [CallerFilePath] string callerFilePath = "", 
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (StrictErrorHandling)
        {
            string exceptionMessage = $"[{callerMember}] Error in file {callerFilePath} at line {callerLineNumber}\n[Message] {message}";
            throw new ParsingException(exceptionMessage);
        }

        return TypeReturn<TX>();
    }
    
    /// <summary>
    /// Handles exceptions during parsing. Rethrows wrapped in a ParsingException if strict error handling is enabled.
    /// </summary>
    /// <typeparam name="TX">The return type.</typeparam>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="callerMember">The calling member name.</param>
    /// <param name="callerFilePath">The source file path.</param>
    /// <param name="callerLineNumber">The source line number.</param>
    /// <returns>The default value of <typeparamref name="TX"/> if strict error handling is disabled.</returns>
    protected dynamic Error<TX>(
        System.Exception exception, 
        [CallerMemberName] string callerMember = "", 
        [CallerFilePath] string callerFilePath = "", 
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (StrictErrorHandling)
        {
            string message = $"[{callerMember}] Error in file {callerFilePath} at line {callerLineNumber}";
            throw new ParsingException(message, exception);
        }

        return TypeReturn<TX>();
    }

    private dynamic TypeReturn<TX>()
    {
        TX type = (TX)Activator.CreateInstance(typeof(TX));
        
        if (type?.GetType() == typeof(bool))
        {
            return false;
        }

        if (type?.GetType() == typeof(object))
        {
            return null;
        }
        
        return null;
    }
}