using DevBase.Api.Apis.OpenLyricsClient.Structure.Enum;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.OpenLyricsClient.Structure.Json;

public class JsonOpenLyricsClientSubscriptionModel
{
    [JsonProperty("model")]
    public EnumSubscriptions Model { get; set; }
}