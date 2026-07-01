namespace DevBase.Api.Apis.Musixmatch;

public sealed class MusixMatchToken : IEquatable<MusixMatchToken>
{
    public MusixMatchToken(string token, MusixMatchApiContext context = MusixMatchApiContext.Desktop)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        this.Token = token;
        this.Context = context;
    }

    public string Token { get; }

    public string UserToken => this.Token;

    public MusixMatchApiContext Context { get; }

    public override string ToString()
    {
        return this.Token;
    }

    public bool Equals(MusixMatchToken? other)
    {
        return other != null
            && string.Equals(this.Token, other.Token, StringComparison.Ordinal)
            && this.Context == other.Context;
    }

    public override bool Equals(object? obj)
    {
        return obj is MusixMatchToken other && this.Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StringComparer.Ordinal.GetHashCode(this.Token), this.Context);
    }
}
