namespace DevBase.Requests.Data.Header.Authorization.AuthorizationHeaders;

public class BearerAuthorizationHeader : AuthorizationHeader
{
    private readonly string _bearerToken;
    
    public BearerAuthorizationHeader(string bearerToken)
    {
        this._bearerToken = bearerToken;
    }
    
    public override ReadOnlySpan<char> Prefix => "Bearer";

    public override ReadOnlySpan<char> Token => this._bearerToken;
}