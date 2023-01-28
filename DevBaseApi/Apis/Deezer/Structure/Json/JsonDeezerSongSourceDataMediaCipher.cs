using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerSongSourceDataMediaCipher
{
    [JsonProperty("type")]
    public string type { get; set; }
}