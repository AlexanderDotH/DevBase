using System.Text;

namespace DevBase.Requests.Data.Header.Authentication.Headers;

public class BasicAuthenticationHeader : AuthenticationHeader
{
    private readonly string _username;
    private readonly string _password;
    
    public BasicAuthenticationHeader(string username, string password)
    {
        this._username = username;
        this._password = password;
    }
    
    public override ReadOnlySpan<char> Prefix => "Basic";

    public override ReadOnlySpan<char> Token
    {
        get
        {
            Encoding encoding = Encoding.UTF8;

            ReadOnlySpan<char> username = this._username; 
            ReadOnlySpan<char> password = this._password;

            StringBuilder combined = new StringBuilder();
            combined.Append(username);
            combined.Append(':');
            combined.Append(password);

            byte[] encodingBytes = encoding.GetBytes(combined.ToString());
        
            ReadOnlySpan<char> encoded = Convert.ToBase64String(encodingBytes);

            return encoded;
        }
    }
}