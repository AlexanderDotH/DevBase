using Newtonsoft.Json;

namespace DevBase.Api.Apis.AppleMusic.Structure.Json;

public class JsonAppleMusicSearchResultResultsSongDataRelationships
{
    [JsonProperty("artists")]
    public JsonAppleMusicSearchResultResultsSongDataRelationshipsArtists Artists { get; set; }

    [JsonProperty("albums")]
    public JsonAppleMusicSearchResultResultsSongDataRelationshipsAlbums Albums { get; set; }
}