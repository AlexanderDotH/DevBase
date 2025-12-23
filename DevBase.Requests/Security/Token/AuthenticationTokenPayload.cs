namespace DevBase.Requests.Security.Token;

public record AuthenticationTokenPayload
{
    public string? Issuer { get; set; }
    public string? Subject { get; set; }
    public string? Audience { get; set; }
    public DateTime ExpiresAt { get; set; }
    public int NotBefore { get; set; }
    public DateTime IssuedAt { get; set; }
    public string? Id { get; set; }
    public Dictionary<string, object>? Claims { get; set; }
    public string? RawPayload { get; set; }
}