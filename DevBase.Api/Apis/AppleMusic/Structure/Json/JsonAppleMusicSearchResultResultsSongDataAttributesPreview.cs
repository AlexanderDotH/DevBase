using Newtonsoft.Json;

namespace DevBase.Api.Apis.AppleMusic.Structure.Json;

public class JsonAppleMusicSearchResultResultsSongDataAttributesPreview
{
    [JsonProperty("url")]
    public string Url { get; set; }
}