using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerLyricsTrackResponseLyricsResponse
{
    [JsonProperty("id")]
    public string id { get; set; }

    [JsonProperty("copyright")]
    public string copyright { get; set; }

    [JsonProperty("text")]
    public string text { get; set; }

    [JsonProperty("writers")]
    public string writers { get; set; }

    [JsonProperty("synchronizedLines")]
    public List<JsonDeezerLyricsTrackResponseLyricsSynchronizedLineResponse> synchronizedLines { get; set; }

    [JsonProperty("__typename")]
    public string __typename { get; set; }
}