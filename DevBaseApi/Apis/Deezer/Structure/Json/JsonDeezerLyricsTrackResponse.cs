using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerLyricsTrackResponse
{
    [JsonProperty("id")]
    public string id { get; set; }

    [JsonProperty("lyrics")]
    public JsonDeezerLyricsTrackResponseLyricsResponse lyrics { get; set; }

    [JsonProperty("album")]
    public JsonDeezerLyricsTrackResponseAlbumResponse album { get; set; }

    [JsonProperty("__typename")]
    public string __typename { get; set; }
}