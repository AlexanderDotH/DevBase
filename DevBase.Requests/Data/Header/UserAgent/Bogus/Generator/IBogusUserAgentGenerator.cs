namespace DevBase.Requests.Data.Header.UserAgent.Bogus.Generator;

public interface IBogusUserAgentGenerator
{
    public ReadOnlySpan<char> UserAgentPart { get; }
}