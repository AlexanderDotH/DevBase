using Newtonsoft.Json;

namespace DevBase.Api.Apis.Replicate.Structure;

public class ReplicatePredictionResultMetrics
{
    [JsonProperty("predict_time")]
    public double predict_time { get; set; }
}