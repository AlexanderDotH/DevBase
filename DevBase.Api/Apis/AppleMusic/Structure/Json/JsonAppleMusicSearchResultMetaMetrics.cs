using Newtonsoft.Json;

namespace DevBase.Api.Apis.AppleMusic.Structure.Json;

public class JsonAppleMusicSearchResultMetaMetrics
{
    [JsonProperty("dataSetId")]
    public string DataSetId { get; set; }
}