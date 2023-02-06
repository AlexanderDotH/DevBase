using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerDataResponse
{
    [JsonProperty("track")]
    public JsonDeezerLyricsTrackResponse track { get; set; }
}