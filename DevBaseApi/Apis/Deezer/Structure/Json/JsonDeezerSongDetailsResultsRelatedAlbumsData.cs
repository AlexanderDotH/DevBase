using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerSongDetailsResultsRelatedAlbumsData
{
    [JsonProperty("ART_NAME")]
    public string ART_NAME { get; set; }

    [JsonProperty("ART_ID")]
    public string ART_ID { get; set; }

    [JsonProperty("ALB_PICTURE")]
    public string ALB_PICTURE { get; set; }

    [JsonProperty("ALB_ID")]
    public string ALB_ID { get; set; }

    [JsonProperty("ALB_TITLE")]
    public string ALB_TITLE { get; set; }

    [JsonProperty("DURATION")]
    public string DURATION { get; set; }

    [JsonProperty("DIGITAL_RELEASE_DATE")]
    public string DIGITAL_RELEASE_DATE { get; set; }

    [JsonProperty("RIGHTS")]
    public JsonDeezerSongDetailsResultsRelatedAlbumsDataRights RIGHTS { get; set; }

    [JsonProperty("LYRICS_ID")]
    public int LYRICS_ID { get; set; }

    [JsonProperty("__TYPE__")]
    public string __TYPE__ { get; set; }
}