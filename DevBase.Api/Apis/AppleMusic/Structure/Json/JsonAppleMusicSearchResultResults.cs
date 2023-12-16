using Newtonsoft.Json;

namespace DevBase.Api.Apis.AppleMusic.Structure.Json;

public class JsonAppleMusicSearchResultResults
{
    [JsonProperty("song")]
    public JsonAppleMusicSearchResultResultsSong SongResult { get; set; }

    [JsonProperty("order")]
    public List<string> Order { get; set; }

    [JsonProperty("rawOrder")]
    public List<string> RawOrder { get; set; }
}