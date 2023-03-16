using Newtonsoft.Json;

namespace DevBase.Api.Apis.OpenLyricsClient.Structure.Json;

public class JsonOpenLyricsClientAiPredictionResult
{
    [JsonProperty("id")]
    public string ID { get; set; }
    
    [JsonProperty("srt")]
    public string SRT { get; set; }
}