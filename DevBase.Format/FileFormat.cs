using System.Runtime.CompilerServices;
using DevBase.Format.Exceptions;
using DevBase.IO;

namespace DevBase.Format;

public abstract class FileFormat<F, T>
{
    public bool StrictErrorHandling { get; set; } = true;
    
    public abstract T Parse(F from);
    
    public abstract bool TryParse(F from, out T parsed);
    
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