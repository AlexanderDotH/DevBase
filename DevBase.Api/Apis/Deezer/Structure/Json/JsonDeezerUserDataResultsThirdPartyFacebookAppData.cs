using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsThirdPartyFacebookAppData
{
    [JsonProperty("id")]
    public long id { get; set; }

    [JsonProperty("namespace")]
    public string @namespace { get; set; }

    [JsonProperty("scope")]
    public string scope { get; set; }

    [JsonProperty("version")]
    public string version { get; set; }

    [JsonProperty("channel")]
    public string channel { get; set; }

    [JsonProperty("client_id")]
    public string client_id { get; set; }

    [JsonProperty("client_key")]
    public string client_key { get; set; }

    [JsonProperty("name")]
    public string name { get; set; }

    [JsonProperty("redirect_uri")]
    public string redirect_uri { get; set; }

    [JsonProperty("access_type")]
    public string access_type { get; set; }

    [JsonProperty("cookie_policy")]
    public string cookie_policy { get; set; }

    [JsonProperty("request_visible_actions")]
    public string request_visible_actions { get; set; }
}