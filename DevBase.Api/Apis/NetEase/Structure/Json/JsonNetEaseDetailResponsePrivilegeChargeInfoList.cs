using Newtonsoft.Json;

namespace DevBase.Api.Apis.NetEase.Structure.Json;

public class JsonNetEaseDetailResponsePrivilegeChargeInfoList
{
    [JsonProperty("rate")]
    public int rate { get; set; }

    [JsonProperty("chargeUrl")]
    public object chargeUrl { get; set; }

    [JsonProperty("chargeMessage")]
    public object chargeMessage { get; set; }

    [JsonProperty("chargeType")]
    public int chargeType { get; set; }
}