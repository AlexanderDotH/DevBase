namespace DevBase.Format.Structure;

public class RichLyrics
{
    public long Start { get; set; }
    public long End { get; set; }
    public string FullLine { get; set; }
    public List<RichLyricsElement> Elements { get; set; }
}