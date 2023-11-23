using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Json;

public class JsonMusixMatchAuthResponseMessageBodyCredentialAccount
{
    [JsonProperty("email")]
    public string email { get; set; }

    [JsonProperty("email_redirect_url")]
    public string email_redirect_url { get; set; }

    [JsonProperty("extra_params")]
    public JsonMusixMatchAuthResponseMessageBodyCredentialAccountExtraParams extra_params { get; set; }

    [JsonProperty("first_name")]
    public string first_name { get; set; }

    [JsonProperty("last_name")]
    public string last_name { get; set; }

    [JsonProperty("id")]
    public string id { get; set; }

    [JsonProperty("registration_country")]
    public string registration_country { get; set; }

    [JsonProperty("user_type")]
    public string user_type { get; set; }

    [JsonProperty("user_id")]
    public string user_id { get; set; }

    [JsonProperty("user_picture")]
    public string user_picture { get; set; }

    [JsonProperty("user_nickname")]
    public string user_nickname { get; set; }

    [JsonProperty("name")]
    public string name { get; set; }

    [JsonProperty("verified")]
    public bool verified { get; set; }

    [JsonProperty("user_last_location")]
    public JsonMusixMatchAuthResponseMessageBodyCredentialAccountUserLastLocation user_last_location { get; set; }

    [JsonProperty("user_creation_date")]
    public DateTime user_creation_date { get; set; }

    [JsonProperty("last_updated")]
    public DateTime last_updated { get; set; }

    [JsonProperty("namespace")]
    public string @namespace { get; set; }

    [JsonProperty("userdata_id")]
    public string userdata_id { get; set; }

    [JsonProperty("has_profile_photo")]
    public bool has_profile_photo { get; set; }

    [JsonProperty("country")]
    public string country { get; set; }
}