using DevBase.Requests.Abstract;
using DevBase.Requests.Data.Header.Authorization.AuthorizationHeaders;

namespace DevBase.Requests.Data.Header.Authorization;

public class AuthorizationHeaderBuilder : HttpFieldBuilder<AuthorizationHeaderBuilder>
{
    private AuthorizationHeader AuthorizationHeader { get; set; }

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

    public ReadOnlySpan<char> Prefix => AuthorizationHeader.Prefix;
    public ReadOnlySpan<char> Token => AuthorizationHeader.Token;


    protected override Action BuildAction => () =>
    {
        this.FieldEntry = new KeyValuePair<string, string>(
            AuthorizationHeader.Prefix.ToString(), 
            AuthorizationHeader.Token.ToString());
    };
}