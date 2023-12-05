using Newtonsoft.Json;

namespace DevBase.Api.Apis.NetEase.Structure.Json;

public class JsonNetEaseSearchResultEntrySong
{
    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("name")]
    public string name { get; set; }

    [JsonProperty("artists")]
    public List<JsonNetEaseSearchResultEntrySongArtist> artists { get; set; }

    [JsonProperty("album")]
    public JsonNetEaseSearchResultEntrySongAlbum album { get; set; }

    [JsonProperty("duration")]
    public int duration { get; set; }

    [JsonProperty("copyrightId")]
    public int copyrightId { get; set; }

    [JsonProperty("status")]
    public int status { get; set; }

    [JsonProperty("alias")]
    public List<object> alias { get; set; }

    [JsonProperty("rtype")]
    public int rtype { get; set; }

    [JsonProperty("ftype")]
    public int ftype { get; set; }

    [JsonProperty("mvid")]
    public int mvid { get; set; }

    [JsonProperty("fee")]
    public int fee { get; set; }

    [JsonProperty("rUrl")]
    public object rUrl { get; set; }

    [JsonProperty("mark")]
    public int mark { get; set; }
}