using Newtonsoft.Json;

namespace DevBase.Api.Apis.BeautifulLyrics.Structure.Json;

public class JsonBeautifulLyricsLineLyricsResponseVocalGroup
{
    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("OppositeAligned")]
    public bool OppositeAligned { get; set; }

    [JsonProperty("Text")]
    public string Text { get; set; }

    [JsonProperty("StartTime")]
    public double StartTime { get; set; }

    [JsonProperty("EndTime")]
    public double EndTime { get; set; }
}