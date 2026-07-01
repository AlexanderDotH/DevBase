namespace DevBase.Api.Apis.Musixmatch.Structure.Types;

public sealed class RichsyncCharacter
{
    public TimeSpan TimeStart { get; set; }

    public TimeSpan TimeOffset { get; set; }

    public string Character { get; set; } = string.Empty;
}
