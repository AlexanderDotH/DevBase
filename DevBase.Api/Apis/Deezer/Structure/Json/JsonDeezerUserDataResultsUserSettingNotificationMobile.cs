using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsUserSettingNotificationMobile
{
    [JsonProperty("share")]
    public bool share { get; set; }

    [JsonProperty("friend_follow")]
    public bool friend_follow { get; set; }

    [JsonProperty("playlist_comment")]
    public bool playlist_comment { get; set; }

    [JsonProperty("playlist_follow")]
    public bool playlist_follow { get; set; }

    [JsonProperty("artist_new_release")]
    public bool artist_new_release { get; set; }

    [JsonProperty("artist_status")]
    public bool artist_status { get; set; }

    [JsonProperty("new_message")]
    public bool new_message { get; set; }
}