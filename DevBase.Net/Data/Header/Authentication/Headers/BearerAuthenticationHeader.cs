namespace DevBase.Net.Data.Header.Authentication.Headers;

public class BearerAuthenticationHeader : AuthenticationHeader
{
    private readonly string _bearerToken;
    
    public BearerAuthenticationHeader(string bearerToken)
    {
        this._bearerToken = bearerToken;
    }
    
    public override ReadOnlySpan<char> Prefix => "Bearer";

    public override ReadOnlySpan<char> Token => this._bearerToken;
}