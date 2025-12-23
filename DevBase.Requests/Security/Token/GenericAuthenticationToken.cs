namespace DevBase.Requests.Security.Token;

public record GenericAuthenticationToken
{
    public string? Token { get; set; }
    public DateTime ExpiresAt { get; set; }
}