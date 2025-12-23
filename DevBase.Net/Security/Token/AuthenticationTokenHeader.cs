namespace DevBase.Net.Security.Token;

public record AuthenticationTokenHeader
{
    public string? Algorithm { get; set; }
    public string? Type { get; set; }
    public Dictionary<string, object>? Header { get; set; }
    public string? RawHeader { get; set; }
}