namespace DevBase.Requests.Preparation.Header.Authorization.AuthorizationHeaders;

public class BearerAuthorizationHeader : AuthorizationHeader
{
    private string _bearerToken;
    
    public BearerAuthorizationHeader(string bearerToken)
    {
        this._bearerToken = bearerToken;
    }
    
    public override ReadOnlySpan<char> Prefix => "Bearer";

    public override ReadOnlySpan<char> Token => this._bearerToken;
}