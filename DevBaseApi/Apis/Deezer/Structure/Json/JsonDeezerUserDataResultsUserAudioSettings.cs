using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsUserAudioSettings
{
    [JsonProperty("default_preset")]
    public string default_preset { get; set; }

    [JsonProperty("default_download_on_mobile_network")]
    public bool default_download_on_mobile_network { get; set; }

    [JsonProperty("presets")]
    public List<JsonDeezerUserDataResultsUserAudioSettingsPreset> presets { get; set; }

    [JsonProperty("connected_device_streaming_preset")]
    public string connected_device_streaming_preset { get; set; }
}