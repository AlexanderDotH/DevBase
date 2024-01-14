namespace DevBase.Requests.Preparation.Header.Authorization;

public abstract class AuthorizationHeader
{
    public abstract ReadOnlySpan<char> Prefix { get; }

    public abstract ReadOnlySpan<char> Token { get; }
}