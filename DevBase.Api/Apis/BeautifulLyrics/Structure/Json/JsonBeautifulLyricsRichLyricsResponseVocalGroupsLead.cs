using Newtonsoft.Json;

namespace DevBase.Api.Apis.BeautifulLyrics.Structure.Json;

public class JsonBeautifulLyricsRichLyricsResponseVocalGroupsLead
{
    [JsonProperty("Text")]
    public string Text { get; set; }

    [JsonProperty("IsPartOfWord")]
    public bool IsPartOfWord { get; set; }

    [JsonProperty("StartTime")]
    public double StartTime { get; set; }

    [JsonProperty("EndTime")]
    public double EndTime { get; set; }
}