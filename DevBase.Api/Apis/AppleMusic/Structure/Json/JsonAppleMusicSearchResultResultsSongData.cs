using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Cms;

namespace DevBase.Api.Apis.AppleMusic.Structure.Json;

public class JsonAppleMusicSearchResultResultsSongData
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("href")]
    public string Href { get; set; }

    [JsonProperty("attributes")]
    public JsonAppleMusicSearchResultResultsSongDataAttributes Attributes { get; set; }

    [JsonProperty("relationships")]
    public JsonAppleMusicSearchResultResultsSongDataRelationships Relationships { get; set; }
}