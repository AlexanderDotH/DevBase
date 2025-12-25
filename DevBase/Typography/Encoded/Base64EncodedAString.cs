using System.Text;
using System.Text.RegularExpressions;
using DevBase.Exception;
using DevBase.Extensions;

namespace DevBase.Typography.Encoded;

/// <summary>
/// Represents a Base64 encoded string.
/// </summary>
public class Base64EncodedAString : EncodedAString
{
    private static Regex ENCODED_REGEX_BASE64;
    private static Regex DECODED_REGEX_BASE64;

    static Base64EncodedAString()
    {
        ENCODED_REGEX_BASE64 = new Regex(@"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.Multiline);
        DECODED_REGEX_BASE64 = new Regex(@"^[a-zA-Z0-9\+/\-_]*={0,3}$", RegexOptions.Multiline);
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Base64EncodedAString"/> class.
    /// Validates and pads the input value.
    /// </summary>
    /// <param name="value">The base64 encoded string.</param>
    /// <exception cref="EncodingException">Thrown if the string is not a valid base64 string.</exception>
    public Base64EncodedAString(string value) : base(value)
    {
        if (base._value.Length % 4 != 0)
        {
            int diff = (4 - base._value.Length % 4);
            base._value += "=".Repeat(diff);
        }
        
        if (!IsEncoded())
            throw new EncodingException("The given string is not a base64 encoded string");
    }

    /// <summary>
    /// Decodes the URL-safe Base64 string to standard Base64.
    /// </summary>
    /// <returns>A new Base64EncodedAString instance.</returns>
    public Base64EncodedAString UrlDecoded()
    {
        string decoded = base._value
            .Replace('-', '+')
            .Replace('_', '/');

        return new Base64EncodedAString(decoded);
    }
    
    /// <summary>
    /// Encodes the Base64 string to URL-safe Base64.
    /// </summary>
    /// <returns>A new Base64EncodedAString instance.</returns>
    public Base64EncodedAString UrlEncoded()
    {
        string decoded = base._value
            .Replace('+', '-')
            .Replace('/', '_');

        return new Base64EncodedAString(decoded);
    }
    
    /// <summary>
    /// Decodes the Base64 string to plain text using UTF-8 encoding.
    /// </summary>
    /// <returns>An AString containing the decoded value.</returns>
    public override AString GetDecoded()
    {
        byte[] decoded = Convert.FromBase64String(base._value);
        return new AString(Encoding.UTF8.GetString(decoded));
    }

    /// <summary>
    /// Decodes the Base64 string to a byte array.
    /// </summary>
    /// <returns>The decoded byte array.</returns>
    public byte[] GetDecodedBuffer() => Convert.FromBase64String(base._value);
    
    /// <summary>
    /// Gets the raw string value.
    /// </summary>
    public string Value
    {
        get => base._value;
    }

    /// <summary>
    /// Checks if the string is a valid Base64 encoded string.
    /// </summary>
    /// <returns>True if encoded correctly, otherwise false.</returns>
    public override bool IsEncoded()
    {
        return base._value.Length % 4 == 0 && 
               (ENCODED_REGEX_BASE64.IsMatch(base._value) || DECODED_REGEX_BASE64.IsMatch(base._value));
    }
}