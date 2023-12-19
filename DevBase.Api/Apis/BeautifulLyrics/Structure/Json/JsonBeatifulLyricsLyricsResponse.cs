using Newtonsoft.Json;

namespace DevBase.Api.Apis.BeautifulLyrics.Structure.Json;

public class JsonBeatifulLyricsLyricsResponse
{
    [JsonProperty("Source")]
    public string Source { get; set; }

    [JsonProperty("ReleaseId")]
    public int ReleaseId { get; set; }

    [JsonProperty("Content")]
    public string Content { get; set; }

    [JsonProperty("IsSynced")]
    public bool IsSynced { get; set; }
}