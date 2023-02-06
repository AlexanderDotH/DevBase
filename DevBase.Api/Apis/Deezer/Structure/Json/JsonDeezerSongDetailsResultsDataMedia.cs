using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerSongDetailsResultsDataMedia
{
    [JsonProperty("TYPE")]
    public string TYPE { get; set; }

    [JsonProperty("HREF")]
    public string HREF { get; set; }
}