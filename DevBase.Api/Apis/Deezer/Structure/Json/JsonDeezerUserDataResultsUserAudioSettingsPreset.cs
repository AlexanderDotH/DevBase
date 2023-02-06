using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsUserAudioSettingsPreset
{
    [JsonProperty("mobile_download")]
    public string mobile_download { get; set; }

    [JsonProperty("mobile_streaming")]
    public string mobile_streaming { get; set; }

    [JsonProperty("wifi_download")]
    public string wifi_download { get; set; }

    [JsonProperty("wifi_streaming")]
    public string wifi_streaming { get; set; }

    [JsonProperty("id")]
    public string id { get; set; }

    [JsonProperty("title")]
    public string title { get; set; }

    [JsonProperty("description")]
    public string description { get; set; }
}