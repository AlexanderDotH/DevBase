using DevBase.Typography.Encoded;

namespace DevBase.Extensions;

/// <summary>
/// Provides extension methods for Base64 encoding.
/// </summary>
public static class Base64EncodedAStringExtension
{
    /// <summary>
    /// Converts a string to a Base64EncodedAString.
    /// </summary>
    /// <param name="content">The string content to encode.</param>
    /// <returns>A new instance of Base64EncodedAString.</returns>
    public static Base64EncodedAString ToBase64(this string content)
    {
        return new Base64EncodedAString(content);
    }
}