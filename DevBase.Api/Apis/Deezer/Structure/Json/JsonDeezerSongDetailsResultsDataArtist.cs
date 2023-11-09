using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerSongDetailsResultsDataArtist
{
    [JsonProperty("ART_ID")]
    public string ART_ID { get; set; }

    [JsonProperty("ROLE_ID")]
    public string ROLE_ID { get; set; }

    [JsonProperty("ARTISTS_SONGS_ORDER")]
    public string ARTISTS_SONGS_ORDER { get; set; }

    [JsonProperty("ART_NAME")]
    public string ART_NAME { get; set; }

    [JsonProperty("ARTIST_IS_DUMMY")]
    public bool ARTIST_IS_DUMMY { get; set; }

    [JsonProperty("ART_PICTURE")]
    public string ART_PICTURE { get; set; }

    [JsonProperty("RANK")]
    public string RANK { get; set; }

    [JsonProperty("LOCALES")]
    public List<object> LOCALES { get; set; }

    [JsonProperty("__TYPE__")]
    public string __TYPE__ { get; set; }
}