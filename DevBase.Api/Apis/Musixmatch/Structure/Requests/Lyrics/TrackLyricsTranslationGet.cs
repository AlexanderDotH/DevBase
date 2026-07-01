using DevBase.Api.Apis.Musixmatch.Structure;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public sealed class TrackLyricsTranslationGet : MusixmatchApiResponse
{
    [JsonProperty("lyrics")]
    public TranslationLyrics Lyrics { get; set; } = new();

    public class TranslatedLyrics
    {
        [JsonProperty("selected_language")]
        public string SelectedLanguage { get; set; } = string.Empty;

        [JsonProperty("lyrics_body")]
        public string LyricsBody { get; set; } = string.Empty;

        [JsonProperty("script_tracking_url")]
        public string ScriptTrackingUrl { get; set; } = string.Empty;
    }

    public class TranslationLyrics
    {
        [JsonProperty("lyrics_id")]
        public int LyricsId { get; set; }

        [JsonProperty("lyrics_body")]
        public string LyricsBody { get; set; } = string.Empty;

        [JsonProperty("script_tracking_url")]
        public string ScriptTrackingUrl { get; set; } = string.Empty;

        [JsonProperty("translated_lyrics")]
        public TranslatedLyrics TranslatedLyrics { get; set; } = new();
    }
}
