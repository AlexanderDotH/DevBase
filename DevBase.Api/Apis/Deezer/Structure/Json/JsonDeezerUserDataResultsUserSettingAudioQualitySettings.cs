using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsUserSettingAudioQualitySettings
{
    [JsonProperty("preset")]
    public string preset { get; set; }

    [JsonProperty("download_on_mobile_network")]
    public bool download_on_mobile_network { get; set; }

    [JsonProperty("connected_device_streaming_preset")]
    public bool connected_device_streaming_preset { get; set; }
}