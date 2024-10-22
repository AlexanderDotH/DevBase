using System.Text;
using DevBase.Requests.Abstract;
using DevBase.Requests.Data.Header.Authentication.Headers;

namespace DevBase.Requests.Data.Header.Authentication;

public class AuthenticationHeaderBuilder : HttpFieldBuilder<AuthenticationHeaderBuilder>
{
    private AuthenticationHeader? AuthenticationHeader { get; set; }

    public AuthenticationHeaderBuilder UseBasicAuthentication(string username, string password)
    {
        AuthenticationHeader = new BasicAuthenticationHeader(username, password);
        return this;
    }
    
    public AuthenticationHeaderBuilder UseBearerAuthentication(string token)
    {
        AuthenticationHeader = new BearerAuthenticationHeader(token);
        return this;
    }

    public ReadOnlySpan<char> Prefix => AuthenticationHeader!.Prefix;
    public ReadOnlySpan<char> Token => AuthenticationHeader!.Token;

    public ReadOnlySpan<char> AuthenticationKey => base.FieldEntry.Key;
    public ReadOnlySpan<char> AuthenticationValue => base.FieldEntry.Value;
    
    protected override Action BuildAction => () =>
    {
        if (this.AuthenticationHeader == null)
            return;
        
        if (this.AuthenticationHeader.Prefix.IsEmpty)
            return;
        
        if (this.AuthenticationHeader.Token.IsEmpty)
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