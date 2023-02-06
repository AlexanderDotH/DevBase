using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsUserTryAndBuy
{
    [JsonProperty("AVAILABLE")]
    public bool AVAILABLE { get; set; }

    [JsonProperty("ACTIVE")]
    public string ACTIVE { get; set; }

    [JsonProperty("DATE_START")]
    public string DATE_START { get; set; }

    [JsonProperty("DATE_END")]
    public string DATE_END { get; set; }

    [JsonProperty("PLATEFORM")]
    public string PLATEFORM { get; set; }

    [JsonProperty("DAYS_LEFT_MOB")]
    public int DAYS_LEFT_MOB { get; set; }
}