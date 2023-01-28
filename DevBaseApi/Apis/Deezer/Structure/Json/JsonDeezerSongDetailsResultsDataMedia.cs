using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerSongDetailsResultsDataMedia
{
    [JsonProperty("TYPE")]
    public string TYPE { get; set; }

    [JsonProperty("HREF")]
    public string HREF { get; set; }
}