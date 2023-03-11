using Newtonsoft.Json;

namespace DevBase.Api.Apis.Replicate.Structure;

public class ReplicatePredictionResultOutput
{
    [JsonProperty("segments")]
    public List<ReplicatePredictionResultOutputSegment> segments { get; set; }

    [JsonProperty("translation")]
    public object translation { get; set; }

    [JsonProperty("transcription")]
    public string transcription { get; set; }

    [JsonProperty("detected_language")]
    public string detected_language { get; set; }
}