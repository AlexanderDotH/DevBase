namespace DevBase.Api.Apis.Musixmatch.Structure.Types;

public sealed class RichsyncLine
{
    public TimeSpan TimeStart { get; set; }

    public TimeSpan TimeEnd { get; set; }

    public List<RichsyncCharacter> Characters { get; set; } = [];

    public string FullLine { get; set; } = string.Empty;
}
