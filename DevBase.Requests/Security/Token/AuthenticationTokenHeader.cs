namespace DevBase.Requests.Security.Token;

public record AuthenticationTokenHeader
{
    public string? Algorithm { get; set; }
    public string? Type { get; set; }
    public string? KeyId { get; set; }
    public List<KeyValuePair<string,object>>? Header { get; set; }
    public string? RawHeader { get; set; }
}