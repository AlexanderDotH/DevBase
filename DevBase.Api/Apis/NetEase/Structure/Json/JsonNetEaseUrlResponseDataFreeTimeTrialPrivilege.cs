using Newtonsoft.Json;

namespace DevBase.Api.Apis.NetEase.Structure.Json;

public class JsonNetEaseUrlResponseDataFreeTimeTrialPrivilege
{
    [JsonProperty("resConsumable")]
    public bool resConsumable { get; set; }

    [JsonProperty("userConsumable")]
    public bool userConsumable { get; set; }

    [JsonProperty("type")]
    public int type { get; set; }

    [JsonProperty("remainTime")]
    public int remainTime { get; set; }
}