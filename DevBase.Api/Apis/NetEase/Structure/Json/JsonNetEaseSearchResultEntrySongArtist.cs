using Newtonsoft.Json;

namespace DevBase.Api.Apis.NetEase.Structure.Json;

public class JsonNetEaseSearchResultEntrySongArtist
{
    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("name")]
    public string name { get; set; }

    [JsonProperty("picUrl")]
    public object picUrl { get; set; }

    [JsonProperty("alias")]
    public List<object> alias { get; set; }

    [JsonProperty("albumSize")]
    public int albumSize { get; set; }

    [JsonProperty("picId")]
    public int picId { get; set; }

    [JsonProperty("fansGroup")]
    public object fansGroup { get; set; }

    [JsonProperty("img1v1Url")]
    public string img1v1Url { get; set; }

    [JsonProperty("img1v1")]
    public int img1v1 { get; set; }

    [JsonProperty("trans")]
    public object trans { get; set; }
}