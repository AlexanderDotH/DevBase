namespace DevBase.Api.Objects.Token;

public class AuthenticationTokenPayload
{
    public DateTime ExpiresAt { get; set; }
    public DateTime IssuedAt { get; set; }
    public string Issuer { get; set; }
}