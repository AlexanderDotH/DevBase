using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerSongSourceDataMediaCipher
{
    [JsonProperty("type")]
    public string type { get; set; }
}