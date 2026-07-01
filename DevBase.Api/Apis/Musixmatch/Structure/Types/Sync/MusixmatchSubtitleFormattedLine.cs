using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Types;

public sealed class MusixmatchSubtitleFormattedLine
{
    [JsonProperty("text")]
    public string Text { get; set; } = string.Empty;

    [JsonProperty("time")]
    public LineTime Time { get; set; } = new();

    public static MusixmatchSubtitleFormattedLine FromLyricsLine(LyricsLine line)
    {
        ArgumentNullException.ThrowIfNull(line);

        return new MusixmatchSubtitleFormattedLine
        {
            Text = line.Text,
            Time = LineTime.FromTimeSpan(line.LyricsTime)
        };
    }

    public sealed class LineTime
    {
        [JsonProperty("total")]
        public double Total { get; set; }

        [JsonProperty("minutes")]
        public int Minutes { get; set; }

        [JsonProperty("seconds")]
        public int Seconds { get; set; }

        [JsonProperty("hundredths")]
        public int Hundredths { get; set; }

        public static LineTime FromTimeSpan(TimeSpan time)
        {
            return new LineTime
            {
                Total = time.TotalSeconds,
                Minutes = time.Minutes,
                Seconds = time.Seconds,
                Hundredths = time.Milliseconds / 10
            };
        }
    }
}
