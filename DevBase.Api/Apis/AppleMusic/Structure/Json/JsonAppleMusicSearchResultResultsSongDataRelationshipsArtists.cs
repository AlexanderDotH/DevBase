using Newtonsoft.Json;

namespace DevBase.Api.Apis.AppleMusic.Structure.Json;

public class JsonAppleMusicSearchResultResultsSongDataRelationshipsArtists
{
    [JsonProperty("href")]
    public string Href { get; set; }

    [JsonProperty("data")]
    public List<JsonAppleMusicSearchResultResultsSongData> Data { get; set; }
}