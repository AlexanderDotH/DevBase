using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerSongDetailsResults
{
    [JsonProperty("DATA")]
    public JsonDeezerSongDetailsResultsData DATA { get; set; }

    [JsonProperty("ISRC")]
    public JsonDeezerSongDetailsResultsISRC ISRC { get; set; }

    [JsonProperty("RELATED_ALBUMS")]
    public JsonDeezerSongDetailsResultsRelatedAlbums RELATED_ALBUMS { get; set; }
}