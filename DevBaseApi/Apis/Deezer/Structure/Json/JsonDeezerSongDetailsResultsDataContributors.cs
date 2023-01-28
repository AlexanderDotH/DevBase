using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerSongDetailsResultsDataContributors
{
    [JsonProperty("main_artist")]
    public List<string> main_artist { get; set; }

    [JsonProperty("author")]
    public List<string> author { get; set; }

    [JsonProperty("composer")]
    public List<string> composer { get; set; }

    [JsonProperty("music publisher")]
    public List<string> musicpublisher { get; set; }
}