namespace DevBase.Api.Objects.Token;

public  class GenericAuthenticationToken
{
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
}