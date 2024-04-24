namespace DevBase.Requests.Security.Token;

public record AuthenticationTokenPayload
{
    public DateTime ExpiresAt { get; set; }
    public DateTime IssuedAt { get; set; }
    public string? Issuer { get; set; }
    public List<KeyValuePair<string,string>> Payload { get; set; }

    public string? RawPayload { get; set; }
}