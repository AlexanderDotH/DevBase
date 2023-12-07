using Newtonsoft.Json;

namespace DevBase.Api.Apis.NetEase.Structure.Json;

public class JsonNetEaseDetailResponsePrivilege
{
    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("fee")]
    public int fee { get; set; }

    [JsonProperty("payed")]
    public int payed { get; set; }

    [JsonProperty("st")]
    public int st { get; set; }

    [JsonProperty("pl")]
    public int pl { get; set; }

    [JsonProperty("dl")]
    public int dl { get; set; }

    [JsonProperty("sp")]
    public int sp { get; set; }

    [JsonProperty("cp")]
    public int cp { get; set; }

    [JsonProperty("subp")]
    public int subp { get; set; }

    [JsonProperty("cs")]
    public bool cs { get; set; }

    [JsonProperty("maxbr")]
    public int maxbr { get; set; }

    [JsonProperty("fl")]
    public int fl { get; set; }

    [JsonProperty("toast")]
    public bool toast { get; set; }

    [JsonProperty("flag")]
    public int flag { get; set; }

    [JsonProperty("preSell")]
    public bool preSell { get; set; }

    [JsonProperty("playMaxbr")]
    public int playMaxbr { get; set; }

    [JsonProperty("downloadMaxbr")]
    public int downloadMaxbr { get; set; }

    [JsonProperty("maxBrLevel")]
    public string maxBrLevel { get; set; }

    [JsonProperty("playMaxBrLevel")]
    public string playMaxBrLevel { get; set; }

    [JsonProperty("downloadMaxBrLevel")]
    public string downloadMaxBrLevel { get; set; }

    [JsonProperty("plLevel")]
    public string plLevel { get; set; }

    [JsonProperty("dlLevel")]
    public string dlLevel { get; set; }

    [JsonProperty("flLevel")]
    public string flLevel { get; set; }

    [JsonProperty("rscl")]
    public object rscl { get; set; }

    [JsonProperty("freeTrialPrivilege")]
    public JsonNetEaseDetailResponsePrivilegeFreeTrialPrivilege freeTrialPrivilege { get; set; }

    [JsonProperty("chargeInfoList")]
    public List<JsonNetEaseDetailResponsePrivilegeChargeInfoList> chargeInfoList { get; set; }
}