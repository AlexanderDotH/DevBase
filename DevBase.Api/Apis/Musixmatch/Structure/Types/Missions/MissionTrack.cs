using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Types;

public class MissionTrack
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("artistName")]
    public string? Artist { get; set; }

    [JsonProperty("commonTrackId")]
    public string? CommonTrackId { get; set; }

    [JsonProperty("language")]
    public string? Language { get; set; }

    [JsonProperty("hasLyrics")]
    public bool HasLyrics { get; set; }

    [JsonProperty("hasSync")]
    public bool HasSync { get; set; }

    [JsonIgnore]
    public bool HasSubtitles
    {
        get => this.HasSync;
        set => this.HasSync = value;
    }

    [JsonProperty("title")]
    public string? Title { get; set; }

    [JsonProperty("missionId")]
    public string? MissionId { get; set; }

    [JsonProperty("actionURI")]
    public string? ActionUri { get; set; }

    [JsonProperty("publishedStatusMacro")]
    public string? PublishedStatusMacro { get; set; }

    [JsonProperty("rank")]
    public int Rank { get; set; }

    [JsonProperty("status")]
    public string? Status { get; set; }

    [JsonProperty("trackLength")]
    public int TrackLength { get; set; }
}
