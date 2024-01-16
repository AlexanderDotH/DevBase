namespace DevBase.Requests.Preparation.Header.UserAgent.Bogus.Generator;

public interface IBogusUserAgentGenerator
{
    public ReadOnlySpan<char> UserAgentPart { get; }
}