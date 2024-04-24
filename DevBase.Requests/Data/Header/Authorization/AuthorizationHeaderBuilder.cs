using System.Diagnostics;
using System.Text;
using DevBase.Requests.Abstract;
using DevBase.Requests.Data.Header.Authorization.AuthorizationHeaders;

namespace DevBase.Requests.Data.Header.Authorization;

public class AuthorizationHeaderBuilder : HttpFieldBuilder<AuthorizationHeaderBuilder>
{
    private AuthorizationHeader? AuthorizationHeader { get; set; }

    public AuthorizationHeaderBuilder UseBasicAuthorization(string username, string password)
    {
        AuthorizationHeader = new BasicAuthorizationHeader(username, password);
        return this;
    }
    public AuthorizationHeaderBuilder UseBearerAuthorization(string token)
    {
        AuthorizationHeader = new BearerAuthorizationHeader(token);
        return this;
    }

    public ReadOnlySpan<char> Prefix => AuthorizationHeader!.Prefix;
    public ReadOnlySpan<char> Token => AuthorizationHeader!.Token;

    public ReadOnlySpan<char> AuthenticationKey => base.FieldEntry.Key;
    public ReadOnlySpan<char> AuthenticationValue => base.FieldEntry.Value;
    
    protected override Action BuildAction => () =>
    {
        if (this.AuthorizationHeader == null)
            return;
        
        if (this.AuthorizationHeader.Prefix.IsEmpty)
            return;
        
        if (this.AuthorizationHeader.Token.IsEmpty)
            return;

        int size = this.Prefix.Length + this.Token.Length + 1;
        
        StringBuilder entryBuilder = new StringBuilder(size);
        entryBuilder.Append(this.Prefix);
        entryBuilder.Append(' ');
        entryBuilder.Append(this.Token);
        
        this.FieldEntry = KeyValuePair.Create(
            "Authorization", 
            entryBuilder.ToString());
    };
}