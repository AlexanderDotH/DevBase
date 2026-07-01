using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Types;

public sealed class MusixmatchRichsyncFormattedLine
{
    [JsonProperty("ts")]
    public double TimeStart { get; set; }

    [JsonProperty("te")]
    public double TimeEnd { get; set; }

    [JsonProperty("l")]
    public List<SingleCharOffset> SingleCharOffsets { get; set; } = [];

    [JsonProperty("x")]
    public string FullLine { get; set; } = string.Empty;

    public sealed class SingleCharOffset
    {
        [JsonProperty("c")]
        public string Char { get; set; } = string.Empty;

        [JsonProperty("o")]
        public double Offset { get; set; }
    }
}
