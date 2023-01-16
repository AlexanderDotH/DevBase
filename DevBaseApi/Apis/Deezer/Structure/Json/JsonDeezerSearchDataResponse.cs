using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerSearchDataResponse
{
    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("readable")]
    public bool readable { get; set; }

    [JsonProperty("title")]
    public string title { get; set; }

    [JsonProperty("title_short")]
    public string title_short { get; set; }

    [JsonProperty("title_version")]
    public string title_version { get; set; }

    [JsonProperty("link")]
    public string link { get; set; }

    [JsonProperty("duration")]
    public int duration { get; set; }

    [JsonProperty("rank")]
    public int rank { get; set; }

    [JsonProperty("explicit_lyrics")]
    public bool explicit_lyrics { get; set; }

    [JsonProperty("explicit_content_lyrics")]
    public int explicit_content_lyrics { get; set; }

    [JsonProperty("explicit_content_cover")]
    public int explicit_content_cover { get; set; }

    [JsonProperty("preview")]
    public string preview { get; set; }

    [JsonProperty("md5_image")]
    public string md5_image { get; set; }

    [JsonProperty("artist")]
    public JsonDeezerSearchDataArtistResponse artist { get; set; }

    [JsonProperty("album")]
    public JsonDeezerSearchDataAlbumResponse album { get; set; }

    [JsonProperty("type")]
    public string type { get; set; }
}