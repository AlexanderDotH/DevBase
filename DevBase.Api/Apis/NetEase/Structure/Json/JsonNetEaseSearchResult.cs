using Newtonsoft.Json;

namespace DevBase.Api.Apis.NetEase.Structure.Json;

public class JsonNetEaseSearchResult
{
    [JsonProperty("result")]
    public JsonNetEaseSearchResultEntry result { get; set; }

    [JsonProperty("code")]
    public int code { get; set; }
}