using Newtonsoft.Json;

namespace DevBase.Api.Apis.BeautifulLyrics.Structure.Json;

public class JsonBeautifulLyricsLineLyricsResponse
{
    [JsonProperty("StartTime")]
    public double StartTime { get; set; }

    [JsonProperty("EndTime")]
    public double EndTime { get; set; }

    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("VocalGroups")]
    public List<JsonBeautifulLyricsLineLyricsResponseVocalGroup> VocalGroups { get; set; }
}