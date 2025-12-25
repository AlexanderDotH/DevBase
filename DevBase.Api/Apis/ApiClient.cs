using System.Runtime.CompilerServices;

namespace DevBase.Api.Apis;

/// <summary>
/// Base class for API clients, providing common error handling and type conversion utilities.
/// </summary>
public class ApiClient
{
    /// <summary>
    /// Gets or sets a value indicating whether to throw exceptions on errors or return default values.
    /// </summary>
    public bool StrictErrorHandling { get; set; }
    
    /// <summary>
    /// Throws an exception if strict error handling is enabled, otherwise returns a default value for type T.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="exception">The exception to throw.</param>
    /// <param name="callerMember">The calling member name.</param>
    /// <param name="callerFilePath">The calling file path.</param>
    /// <param name="callerLineNumber">The calling line number.</param>
    /// <returns>The default value of T if exception is not thrown.</returns>
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
    
    /// <summary>
    /// Throws an exception if strict error handling is enabled, otherwise returns a default tuple (empty string, false).
    /// </summary>
    /// <param name="exception">The exception to throw.</param>
    /// <param name="callerMember">The calling member name.</param>
    /// <param name="callerFilePath">The calling file path.</param>
    /// <param name="callerLineNumber">The calling line number.</param>
    /// <returns>A tuple (string.Empty, false) if exception is not thrown.</returns>
    protected (string, bool) ThrowTuple(
        System.Exception exception,
        [CallerMemberName] string callerMember = "", 
        [CallerFilePath] string callerFilePath = "", 
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (StrictErrorHandling)
            throw exception;
        
        return (string.Empty, false);
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