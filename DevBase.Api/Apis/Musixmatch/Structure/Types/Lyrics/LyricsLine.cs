namespace DevBase.Api.Apis.Musixmatch.Structure.Types;

public sealed class LyricsLine
{
    public TimeSpan LyricsTime { get; set; }

    public string Text { get; set; } = string.Empty;

    public MusixmatchSubtitleFormattedLine GetMusixmatchSubtitle()
    {
        return MusixmatchSubtitleFormattedLine.FromLyricsLine(this);
    }

    public static LyricsLine FromMusixmatchSubtitle(MusixmatchSubtitleFormattedLine line)
    {
        ArgumentNullException.ThrowIfNull(line);

        return new LyricsLine
        {
            LyricsTime = TimeSpan.FromSeconds(line.Time.Total),
            Text = line.Text
        };
    }
}
