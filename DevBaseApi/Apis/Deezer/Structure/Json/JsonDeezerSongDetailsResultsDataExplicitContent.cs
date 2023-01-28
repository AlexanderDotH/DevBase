using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerSongDetailsResultsDataExplicitContent
{
    [JsonProperty("EXPLICIT_LYRICS_STATUS")]
    public int EXPLICIT_LYRICS_STATUS { get; set; }

    [JsonProperty("EXPLICIT_COVER_STATUS")]
    public int EXPLICIT_COVER_STATUS { get; set; }
}