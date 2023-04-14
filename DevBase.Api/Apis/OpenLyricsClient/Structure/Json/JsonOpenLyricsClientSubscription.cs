using Newtonsoft.Json;

namespace DevBase.Api.Apis.OpenLyricsClient.Structure.Json;

public class JsonOpenLyricsClientSubscription
{
    [JsonProperty("userID")]
    public string UserID { get; set; }
    
    [JsonProperty("userSecret")]
    public string UserSecret { get; set; }
}