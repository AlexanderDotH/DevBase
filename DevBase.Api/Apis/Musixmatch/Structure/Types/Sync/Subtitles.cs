using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Types;

public sealed class Subtitles
{
    public Subtitles()
    {
    }

    public Subtitles(string? serializedSubtitles)
    {
        if (string.IsNullOrWhiteSpace(serializedSubtitles)) {
            return;
        }

        List<MusixmatchSubtitleFormattedLine>? lines =
            JsonConvert.DeserializeObject<List<MusixmatchSubtitleFormattedLine>>(serializedSubtitles);

        this.LoadFromMusixmatchSubtitleLines(lines);
    }

    public Subtitles(List<MusixmatchSubtitleFormattedLine>? lines)
    {
        this.LoadFromMusixmatchSubtitleLines(lines);
    }

    public List<LyricsLine> Lines { get; set; } = [];

    public List<MusixmatchSubtitleFormattedLine> GetMusixmatchSubtitles()
    {
        List<MusixmatchSubtitleFormattedLine> lines = [];

        foreach (LyricsLine line in this.Lines)
        {
            lines.Add(MusixmatchSubtitleFormattedLine.FromLyricsLine(line));
        }

        return lines;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this.GetMusixmatchSubtitles());
    }

    private void LoadFromMusixmatchSubtitleLines(List<MusixmatchSubtitleFormattedLine>? lines)
    {
        this.Lines = [];

        if (lines is null) {
            return;
        }

        foreach (MusixmatchSubtitleFormattedLine line in lines)
        {
            this.Lines.Add(LyricsLine.FromMusixmatchSubtitle(line));
        }
    }
}
