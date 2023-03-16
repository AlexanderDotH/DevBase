using Newtonsoft.Json;

namespace DevBase.Api.Apis.OpenLyricsClient.Structure.Json;

public class JsonOpenLyricsClientAiPredictionResponse
{
    [JsonProperty("id")]
    public string ID { get; set; }
}