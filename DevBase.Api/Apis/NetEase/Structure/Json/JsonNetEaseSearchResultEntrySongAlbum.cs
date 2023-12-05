using Newtonsoft.Json;

namespace DevBase.Api.Apis.NetEase.Structure.Json;

public class JsonNetEaseSearchResultEntrySongAlbum
{
    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("name")]
    public string name { get; set; }

    [JsonProperty("artist")]
    public JsonNetEaseSearchResultEntrySongArtist artist { get; set; }

    [JsonProperty("publishTime")]
    public object publishTime { get; set; }

    [JsonProperty("size")]
    public int size { get; set; }

    [JsonProperty("copyrightId")]
    public int copyrightId { get; set; }

    [JsonProperty("status")]
    public int status { get; set; }

    [JsonProperty("picId")]
    public object picId { get; set; }

    [JsonProperty("mark")]
    public int mark { get; set; }

    [JsonProperty("transNames")]
    public List<string> transNames { get; set; }
}