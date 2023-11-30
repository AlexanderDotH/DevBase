using Newtonsoft.Json;

namespace DevBase.Format.Formats.RmmlFormat.Json;

public class RichSyncLine
{
    [JsonProperty("ts")]
    public double TimeStart;

    [JsonProperty("te")]
    public double TimeEnd;

    [JsonProperty("l")]
    public List<RichSyncChar> SingleCharOffsets;

    [JsonProperty("x")]
    public string FullLine;
}