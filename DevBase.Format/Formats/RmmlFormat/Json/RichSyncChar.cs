using Newtonsoft.Json;

namespace DevBase.Format.Formats.RmmlFormat.Json;

public class RichSyncChar
{
    [JsonProperty("c")]
    public string Char;

    [JsonProperty("o")]
    public double Offset;
}