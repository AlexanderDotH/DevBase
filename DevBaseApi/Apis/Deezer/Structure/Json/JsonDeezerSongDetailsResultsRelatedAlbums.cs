using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerSongDetailsResultsRelatedAlbums
{
    [JsonProperty("data")]
    public List<JsonDeezerSongDetailsResultsRelatedAlbumsData> data { get; set; }

    [JsonProperty("count")]
    public int count { get; set; }

    [JsonProperty("total")]
    public int total { get; set; }
}