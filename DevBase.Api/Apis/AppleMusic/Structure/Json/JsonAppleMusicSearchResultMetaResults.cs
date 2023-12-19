using Newtonsoft.Json;

namespace DevBase.Api.Apis.AppleMusic.Structure.Json;

public class JsonAppleMusicSearchResultMetaResults
{
    [JsonProperty("order")]
    public List<string> Order { get; set; }
    
    [JsonProperty("rawOrder")]
    public List<string> RawOrder { get; set; }
}