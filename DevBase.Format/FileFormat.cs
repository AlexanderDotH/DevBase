using System.Runtime.CompilerServices;
using DevBase.Format.Exceptions;
using DevBase.IO;

namespace DevBase.Format;

public abstract class FileFormat<F, T>
{
    public bool StrictErrorHandling { get; set; } = true;
    
    public abstract T Parse(F from);

    // TODO: Rename this moron
    protected dynamic HandleException(
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

        return null;
    }
    
    protected dynamic HandleException(
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

        return null;
    }
}