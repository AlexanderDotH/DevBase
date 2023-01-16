using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerDataResponse
{
    [JsonProperty("track")]
    public JsonDeezerLyricsTrackResponse track { get; set; }
}