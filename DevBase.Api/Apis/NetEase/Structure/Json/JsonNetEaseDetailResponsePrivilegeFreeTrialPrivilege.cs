using Newtonsoft.Json;

namespace DevBase.Api.Apis.NetEase.Structure.Json;

public class JsonNetEaseDetailResponsePrivilegeFreeTrialPrivilege
{
    [JsonProperty("resConsumable")]
    public bool resConsumable { get; set; }

    [JsonProperty("userConsumable")]
    public bool userConsumable { get; set; }

    [JsonProperty("listenType")]
    public object listenType { get; set; }
}