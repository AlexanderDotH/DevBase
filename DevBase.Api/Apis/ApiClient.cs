using System.Runtime.CompilerServices;

namespace DevBase.Api.Apis;

public class ApiClient
{
    public bool StrictErrorHandling { get; set; }
    
    protected dynamic Throw<T>(
        System.Exception exception,
        [CallerMemberName] string callerMember = "", 
        [CallerFilePath] string callerFilePath = "", 
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (StrictErrorHandling)
            throw exception;
        
        return ToType<T>();
    }
    
    private dynamic ToType<T>()
    {
        T type = (T)Activator.CreateInstance(typeof(T));
        
        if (type?.GetType() == typeof(bool))
            return false;

        if (type?.GetType() == typeof(object))
            return null;

        if (type?.GetType() == typeof(string))
            return string.Empty;
        
        return null;
    }
}