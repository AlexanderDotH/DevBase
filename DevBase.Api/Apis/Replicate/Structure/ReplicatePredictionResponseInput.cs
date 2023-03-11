using Newtonsoft.Json;

namespace DevBase.Api.Apis.Replicate.Structure;

public class ReplicatePredictionResponseInput
{
    [JsonProperty("audio")]
    public string audio { get; set; }

    [JsonProperty("model")]
    public string model { get; set; }

    [JsonProperty("transcription")]
    public string transcription { get; set; }
}