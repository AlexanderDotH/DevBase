using Newtonsoft.Json;

namespace DevBase.Api.Apis.NetEase.Structure.Json;

public class JsonNetEaseUrlResponseDataFreeTrialPrivilege
{
    [JsonProperty("resConsumable")]
    public bool resConsumable { get; set; }

    [JsonProperty("userConsumable")]
    public bool userConsumable { get; set; }

    [JsonProperty("listenType")]
    public object listenType { get; set; }

    [JsonProperty("cannotListenReason")]
    public object cannotListenReason { get; set; }

    [JsonProperty("playReason")]
    public object playReason { get; set; }
}