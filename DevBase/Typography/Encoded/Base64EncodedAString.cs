using System.Text;
using System.Text.RegularExpressions;
using DevBase.Extensions;

namespace DevBase.Typography.Encoded;

public class Base64EncodedAString : EncodedAString
{
    private readonly Regex _regexBase64;
    
    public Base64EncodedAString(string value) : base(value)
    {
        if (value.Length % 4 != 0)
        {
            int diff = (4 - value.Length % 4);
            base._value += "=".Repeat(diff);
        }
        
        this._regexBase64 = new Regex(@"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.Multiline);

        if (!IsEncoded())
            throw new System.Exception("The given string is not a base64 encoded string");
    }

    public override AString GetDecoded()
    {
        byte[] decoded = Convert.FromBase64String(this._value);
        return new AString(Encoding.UTF8.GetString(decoded));
    }

    public override bool IsEncoded()
    {
        return this._value.Length % 4 == 0 && this._regexBase64.IsMatch(this._value);
    }
}