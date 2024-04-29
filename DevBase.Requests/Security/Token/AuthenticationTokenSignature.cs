namespace DevBase.Requests.Security.Token;

public record AuthenticationTokenSignature
{
    public string? Signature { get; set; }
    public bool Verified { get; set; }
}