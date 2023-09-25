using Newtonsoft.Json;

namespace DevBase.Api.Apis.Tidal.Structure.Json;

public class JsonTidalLyricsResult
{
    [JsonProperty("trackId")]
    public int trackId { get; set; }

    [JsonProperty("lyricsProvider")]
    public string lyricsProvider { get; set; }

    [JsonProperty("providerCommontrackId")]
    public string providerCommontrackId { get; set; }

    [JsonProperty("providerLyricsId")]
    public string providerLyricsId { get; set; }

    [JsonProperty("lyrics")]
    public string lyrics { get; set; }

    [JsonProperty("subtitles")]
    public string subtitles { get; set; }

    [JsonProperty("isRightToLeft")]
    public bool isRightToLeft { get; set; }
}