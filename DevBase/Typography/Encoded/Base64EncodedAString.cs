using System.Text;
using System.Text.RegularExpressions;
using DevBase.Exception;
using DevBase.Extensions;

namespace DevBase.Typography.Encoded;

public class Base64EncodedAString : EncodedAString
{
    private static Regex ENCODED_REGEX_BASE64;
    private static Regex DECODED_REGEX_BASE64;

    static Base64EncodedAString()
    {
        ENCODED_REGEX_BASE64 = new Regex(@"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.Multiline);
        DECODED_REGEX_BASE64 = new Regex(@"^[a-zA-Z0-9\+/\-_]*={0,3}$", RegexOptions.Multiline);
    }
    
    public Base64EncodedAString(string value) : base(value)
    {
        base._value = Normalize(value);

        if (!IsEncoded())
            throw new EncodingException("The given string is not a base64 encoded string");
    }

    private string Normalize(string value)
    {
        if (value.Length % 4 != 0)
        {
            int diff = (4 - value.Length % 4);
            value += "=".Repeat(diff);
        }

        return value;
    }

    public Base64EncodedAString UrlDecoded()
    {
        string decoded = base._value
            .Replace('-', '+')
            .Replace('_', '/');

        return new Base64EncodedAString(decoded);
    }
    
    public Base64EncodedAString UrlEncoded()
    {
        string decoded = base._value
            .Replace('+', '-')
            .Replace('/', '_');

        return new Base64EncodedAString(decoded);
    }
    
    public override AString GetDecoded()
    {
        byte[] decoded = Convert.FromBase64String(this._value);
        return new AString(Encoding.UTF8.GetString(decoded));
    }

    public byte[] GetDecodedBuffer() => Convert.FromBase64String(this._value);
    
    public string Value
    {
        get => base._value;
    }

    public override bool IsEncoded()
    {
        return this._value.Length % 4 == 0 && 
               (ENCODED_REGEX_BASE64.IsMatch(base._value) || DECODED_REGEX_BASE64.IsMatch(base._value));
    }
}