namespace DevBase.Requests.Data.Header.Authentication;

public abstract class AuthenticationHeader
{
    public abstract ReadOnlySpan<char> Prefix { get; }

    public abstract ReadOnlySpan<char> Token { get; }
}