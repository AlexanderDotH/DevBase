namespace DevBase.Requests.Security.Token;

public record AuthenticationTokenSignature
{
    public byte[]? Signature { get; set; }
    public bool Verified { get; set; }
}