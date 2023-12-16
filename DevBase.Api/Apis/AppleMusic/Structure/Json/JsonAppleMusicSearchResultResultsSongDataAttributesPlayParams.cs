using Newtonsoft.Json;

namespace DevBase.Api.Apis.AppleMusic.Structure.Json;

public class JsonAppleMusicSearchResultResultsSongDataAttributesPlayParams
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("kind")]
    public string Kind { get; set; }
}