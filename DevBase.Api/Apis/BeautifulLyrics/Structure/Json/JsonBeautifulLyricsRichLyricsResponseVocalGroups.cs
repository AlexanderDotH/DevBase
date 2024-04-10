using Newtonsoft.Json;

namespace DevBase.Api.Apis.BeautifulLyrics.Structure.Json;

public class JsonBeautifulLyricsRichLyricsResponseVocalGroups
{
    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("OppositeAligned")]
    public bool OppositeAligned { get; set; }

    [JsonProperty("StartTime")]
    public double StartTime { get; set; }

    [JsonProperty("EndTime")]
    public double EndTime { get; set; }

    [JsonProperty("Lead")]
    public List<JsonBeautifulLyricsRichLyricsResponseVocalGroupsLead> Lead { get; set; }

    [JsonProperty("Background")]
    public List<JsonBeautifulLyricsRichLyricsResponseVocalGroupsBackground> Background { get; set; }
}