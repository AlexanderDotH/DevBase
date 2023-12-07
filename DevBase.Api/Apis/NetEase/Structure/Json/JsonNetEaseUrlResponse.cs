using Newtonsoft.Json;

namespace DevBase.Api.Apis.NetEase.Structure.Json;

public class JsonNetEaseUrlResponse
{
    [JsonProperty("data")]
    public List<JsonNetEaseUrlResponseData> data { get; set; }

    [JsonProperty("code")]
    public int code { get; set; }
}