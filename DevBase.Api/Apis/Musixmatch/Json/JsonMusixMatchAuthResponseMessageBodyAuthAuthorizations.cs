using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Json;

public class JsonMusixMatchAuthResponseMessageBodyAuthAuthorizations
{
    [JsonProperty("active")]
    public bool active { get; set; }

    [JsonProperty("private")]
    public bool @private { get; set; }

    [JsonProperty("safe")]
    public string safe { get; set; }

    [JsonProperty("user_id")]
    public string user_id { get; set; }

    [JsonProperty("last_updated")]
    public DateTime last_updated { get; set; }

    [JsonProperty("namespace")]
    public string @namespace { get; set; }

    [JsonProperty("userdata_id")]
    public string userdata_id { get; set; }

    [JsonProperty("referral_list")]
    public List<JsonMusixMatchAuthResponseMessageBodyAuthAuthorizationsReferralList> referral_list { get; set; }
}