using Newtonsoft.Json;

namespace DevBase.Api.Apis.AppleMusic.Structure.Json;

public class JsonAppleMusicSearchResultResultsSongDataRelationshipsAlbums
{
    [JsonProperty("href")]
    public string Href { get; set; }

    [JsonProperty("next")]
    public string Next { get; set; }

    [JsonProperty("data")]
    public List<JsonAppleMusicSearchResultResultsSongData> Data { get; set; }
}