using Newtonsoft.Json;

namespace DevBase.Api.Apis.Replicate.Structure;

public class ReplicatePredictionResult
{
    [JsonProperty("completed_at")]
    public DateTime completed_at { get; set; }

    [JsonProperty("created_at")]
    public DateTime created_at { get; set; }

    [JsonProperty("error")]
    public object error { get; set; }

    [JsonProperty("id")]
    public string id { get; set; }

    [JsonProperty("input")]
    public ReplicatePredictionResultInput input { get; set; }

    [JsonProperty("logs")]
    public string logs { get; set; }

    [JsonProperty("metrics")]
    public ReplicatePredictionResultMetrics metrics { get; set; }

    [JsonProperty("output")]
    public ReplicatePredictionResultOutput output { get; set; }

    [JsonProperty("started_at")]
    public DateTime started_at { get; set; }

    [JsonProperty("status")]
    public string status { get; set; }

    [JsonProperty("urls")]
    public ReplicatePredictionResultUrls urls { get; set; }

    [JsonProperty("version")]
    public string version { get; set; }

    [JsonProperty("webhook_completed")]
    public object webhook_completed { get; set; }
}