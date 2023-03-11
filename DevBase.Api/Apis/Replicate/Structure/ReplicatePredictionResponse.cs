using Newtonsoft.Json;

namespace DevBase.Api.Apis.Replicate.Structure;

public class ReplicatePredictionResponse
{
    [JsonProperty("completed_at")]
    public object completed_at { get; set; }

    [JsonProperty("created_at")]
    public DateTime created_at { get; set; }

    [JsonProperty("error")]
    public object error { get; set; }

    [JsonProperty("id")]
    public string id { get; set; }

    [JsonProperty("input")]
    public ReplicatePredictionResponseInput input { get; set; }

    [JsonProperty("logs")]
    public object logs { get; set; }

    [JsonProperty("metrics")]
    public ReplicatePredictionResponseMetrics metrics { get; set; }

    [JsonProperty("output")]
    public object output { get; set; }

    [JsonProperty("started_at")]
    public object started_at { get; set; }

    [JsonProperty("status")]
    public string status { get; set; }

    [JsonProperty("urls")]
    public ReplicatePredictionResponseUrls urls { get; set; }

    [JsonProperty("version")]
    public string version { get; set; }

    [JsonProperty("webhook_completed")]
    public object webhook_completed { get; set; }
}