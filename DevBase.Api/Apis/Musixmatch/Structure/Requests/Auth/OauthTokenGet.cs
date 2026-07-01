using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public sealed class OauthTokenGet : MusixMatchApiResponse
{
    [JsonProperty("oauthtoken")]
    public Oauthtoken? Token { get; set; }

    public sealed class OauthRefreshtokenReply
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonProperty("token_type")]
        public string TokenType { get; set; } = string.Empty;

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; } = string.Empty;

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;

        [JsonProperty("refresh_date")]
        public DateTime? RefreshDate { get; set; }

        [JsonProperty("encrypted_refresh_token")]
        public string EncryptedRefreshToken { get; set; } = string.Empty;
    }

    public sealed class PlaylistsMap
    {
        [JsonProperty("lyrics_music_id")]
        public string LyricsMusicId { get; set; } = string.Empty;
    }

    public sealed class Oauthtoken
    {
        [JsonProperty("oauth_refreshtoken_reply")]
        public OauthRefreshtokenReply? OauthRefreshtokenReply { get; set; }

        [JsonProperty("last_updated")]
        public DateTime? LastUpdated { get; set; }

        [JsonProperty("callback")]
        public string Callback { get; set; } = string.Empty;

        [JsonProperty("service")]
        public string Service { get; set; } = string.Empty;

        [JsonProperty("service_user_prefix")]
        public string ServiceUserPrefix { get; set; } = string.Empty;

        [JsonProperty("playlists_map")]
        public PlaylistsMap? PlaylistsMap { get; set; }
    }
}
