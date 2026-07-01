using System.Runtime.CompilerServices;

namespace DevBase.Api.Apis;

/// <summary>
/// Base class for API clients, providing common error handling and type conversion utilities.
/// Request construction and sending stay in each API method.
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
        if (StrictErrorHandling) {
            throw exception;
        }

        return ToType(typeof(T));
    }

    protected string ThrowString(System.Exception exception)
    {
        return this.Throw<string>(exception);
    }

    protected byte[] ThrowBytes(System.Exception exception)
    {
        return this.Throw<byte[]>(exception);
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
        if (StrictErrorHandling) {
            throw exception;
        }

        return (string.Empty, false);
    }

    private static dynamic? ToType(Type type)
    {
        if (Nullable.GetUnderlyingType(type) != null) {
            return null;
        }

        if (type == typeof(string)) {
            return string.Empty;
        }

        if (type.IsArray) {
            return Array.CreateInstance(type.GetElementType()!, 0);
        }

        if (type.IsValueType) {
            return Activator.CreateInstance(type);
        }

        return null;
    }
}
