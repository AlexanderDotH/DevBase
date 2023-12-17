namespace DevBase.Api.Objects.Token;

public class AuthenticationTokenHeader
{
    public string Algorithm { get; set; }
    public string Type { get; set; }
    public string KeyId { get; set; }
    public string RawHeader { get; set; }
}