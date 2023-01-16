using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerSearchDataArtistResponse
{
    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("name")]
    public string name { get; set; }

    [JsonProperty("link")]
    public string link { get; set; }

    [JsonProperty("picture")]
    public string picture { get; set; }

    [JsonProperty("picture_small")]
    public string picture_small { get; set; }

    [JsonProperty("picture_medium")]
    public string picture_medium { get; set; }

    [JsonProperty("picture_big")]
    public string picture_big { get; set; }

    [JsonProperty("picture_xl")]
    public string picture_xl { get; set; }

    [JsonProperty("tracklist")]
    public string tracklist { get; set; }

    [JsonProperty("type")]
    public string type { get; set; }
}