using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerRawLyricsResponseResults
{
    [JsonProperty("LYRICS_ID")]
    public string LYRICS_ID { get; set; }

    [JsonProperty("LYRICS_SYNC_JSON")]
    public List<JsonDeezerRawLyricsResponseResultsSync> LYRICS_SYNC_JSON { get; set; }

    [JsonProperty("LYRICS_TEXT")]
    public string LYRICS_TEXT { get; set; }

    [JsonProperty("LYRICS_COPYRIGHTS")]
    public string LYRICS_COPYRIGHTS { get; set; }

    [JsonProperty("LYRICS_WRITERS")]
    public string LYRICS_WRITERS { get; set; }
}