using Newtonsoft.Json;

namespace DevBase.Api.Apis.AppleMusic.Structure.Json;

public class JsonAppleMusicSearchResultResultsSong
{
    [JsonProperty("href")]
    public string Href { get; set; }

    [JsonProperty("next")]
    public string Next { get; set; }

    [JsonProperty("data")]
    public List<JsonAppleMusicSearchResultResultsSongData> Songs { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("groupId")]
    public string GroupId { get; set; }
}