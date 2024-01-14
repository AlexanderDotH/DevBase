using System.Buffers;
using System.Text;

namespace DevBase.Requests.Preparation.Header.Authorization.AuthorizationHeaders;

public class BasicAuthorizationHeader : AuthorizationHeader
{
    private string _username;
    private string _password;
    
    public BasicAuthorizationHeader(string username, string password)
    {
        this._username = username;
        this._password = password;
    }
    
    public override ReadOnlySpan<char> Prefix => "Basic";

    public override ReadOnlySpan<char> Token
    {
        get
        {
            Encoding encoding = Encoding.Default;

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