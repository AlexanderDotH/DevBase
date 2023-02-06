using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerSearchDataAlbumResponse
{
    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("title")]
    public string title { get; set; }

    [JsonProperty("cover")]
    public string cover { get; set; }

    [JsonProperty("cover_small")]
    public string cover_small { get; set; }

    [JsonProperty("cover_medium")]
    public string cover_medium { get; set; }

    [JsonProperty("cover_big")]
    public string cover_big { get; set; }

    [JsonProperty("cover_xl")]
    public string cover_xl { get; set; }

    [JsonProperty("md5_image")]
    public string md5_image { get; set; }

    [JsonProperty("tracklist")]
    public string tracklist { get; set; }

    [JsonProperty("type")]
    public string type { get; set; }
}