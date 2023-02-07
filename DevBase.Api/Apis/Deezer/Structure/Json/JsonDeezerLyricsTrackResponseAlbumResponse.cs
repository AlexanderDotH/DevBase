using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerLyricsTrackResponseAlbumResponse
{
    [JsonProperty("cover")]
    public JsonDeezerLyricsTrackResponseAlbumCoverResponse cover { get; set; }

    [JsonProperty("__typename")]
    public string __typename { get; set; }
}