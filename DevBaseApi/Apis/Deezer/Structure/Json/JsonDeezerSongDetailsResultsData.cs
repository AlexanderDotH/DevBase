using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerSongDetailsResultsData
{
     [JsonProperty("SNG_ID")]
        public string SNG_ID { get; set; }

        [JsonProperty("PRODUCT_TRACK_ID")]
        public string PRODUCT_TRACK_ID { get; set; }

        [JsonProperty("UPLOAD_ID")]
        public int UPLOAD_ID { get; set; }

        [JsonProperty("SNG_TITLE")]
        public string SNG_TITLE { get; set; }

        [JsonProperty("ART_ID")]
        public string ART_ID { get; set; }

        [JsonProperty("PROVIDER_ID")]
        public string PROVIDER_ID { get; set; }

        [JsonProperty("ART_NAME")]
        public string ART_NAME { get; set; }

        [JsonProperty("ARTIST_IS_DUMMY")]
        public bool ARTIST_IS_DUMMY { get; set; }

        [JsonProperty("ARTISTS")]
        public List<JsonDeezerSongDetailsResultsDataArtist> ARTISTS { get; set; }

        [JsonProperty("ALB_ID")]
        public string ALB_ID { get; set; }

        [JsonProperty("ALB_TITLE")]
        public string ALB_TITLE { get; set; }

        [JsonProperty("TYPE")]
        public int TYPE { get; set; }

        [JsonProperty("VIDEO")]
        public bool VIDEO { get; set; }

        [JsonProperty("DURATION")]
        public string DURATION { get; set; }

        [JsonProperty("ALB_PICTURE")]
        public string ALB_PICTURE { get; set; }

        [JsonProperty("ART_PICTURE")]
        public string ART_PICTURE { get; set; }

        [JsonProperty("RANK_SNG")]
        public string RANK_SNG { get; set; }

        [JsonProperty("FILESIZE_AAC_64")]
        public string FILESIZE_AAC_64 { get; set; }

        [JsonProperty("FILESIZE_MP3_64")]
        public string FILESIZE_MP3_64 { get; set; }

        [JsonProperty("FILESIZE_MP3_128")]
        public string FILESIZE_MP3_128 { get; set; }

        [JsonProperty("FILESIZE_MP3_256")]
        public string FILESIZE_MP3_256 { get; set; }

        [JsonProperty("FILESIZE_MP3_320")]
        public string FILESIZE_MP3_320 { get; set; }

        [JsonProperty("FILESIZE_MP4_RA1")]
        public string FILESIZE_MP4_RA1 { get; set; }

        [JsonProperty("FILESIZE_MP4_RA2")]
        public string FILESIZE_MP4_RA2 { get; set; }

        [JsonProperty("FILESIZE_MP4_RA3")]
        public string FILESIZE_MP4_RA3 { get; set; }

        [JsonProperty("FILESIZE_MHM1_RA1")]
        public string FILESIZE_MHM1_RA1 { get; set; }

        [JsonProperty("FILESIZE_MHM1_RA2")]
        public string FILESIZE_MHM1_RA2 { get; set; }

        [JsonProperty("FILESIZE_MHM1_RA3")]
        public string FILESIZE_MHM1_RA3 { get; set; }

        [JsonProperty("FILESIZE_FLAC")]
        public string FILESIZE_FLAC { get; set; }

        [JsonProperty("FILESIZE")]
        public string FILESIZE { get; set; }

        [JsonProperty("GAIN")]
        public string GAIN { get; set; }

        [JsonProperty("MEDIA_VERSION")]
        public string MEDIA_VERSION { get; set; }

        [JsonProperty("DISK_NUMBER")]
        public string DISK_NUMBER { get; set; }

        [JsonProperty("TRACK_NUMBER")]
        public string TRACK_NUMBER { get; set; }

        [JsonProperty("TRACK_TOKEN")]
        public string TRACK_TOKEN { get; set; }

        [JsonProperty("TRACK_TOKEN_EXPIRE")]
        public int TRACK_TOKEN_EXPIRE { get; set; }

        [JsonProperty("VERSION")]
        public string VERSION { get; set; }

        [JsonProperty("MEDIA")]
        public List<JsonDeezerSongDetailsResultsDataMedia> MEDIA { get; set; }

        [JsonProperty("EXPLICIT_LYRICS")]
        public string EXPLICIT_LYRICS { get; set; }

        [JsonProperty("RIGHTS")]
        public JsonDeezerSongDetailsResultsDataRights RIGHTS { get; set; }

        [JsonProperty("ISRC")]
        public string ISRC { get; set; }

        [JsonProperty("HIERARCHICAL_TITLE")]
        public string HIERARCHICAL_TITLE { get; set; }

        [JsonProperty("SNG_CONTRIBUTORS")]
        public JsonDeezerSongDetailsResultsDataContributors SNG_CONTRIBUTORS { get; set; }

        [JsonProperty("LYRICS_ID")]
        public int LYRICS_ID { get; set; }

        [JsonProperty("EXPLICIT_TRACK_CONTENT")]
        public JsonDeezerSongDetailsResultsDataExplicitContent EXPLICIT_TRACK_CONTENT { get; set; }

        [JsonProperty("COPYRIGHT")]
        public string COPYRIGHT { get; set; }

        [JsonProperty("PHYSICAL_RELEASE_DATE")]
        public string PHYSICAL_RELEASE_DATE { get; set; }

        [JsonProperty("S_MOD")]
        public int S_MOD { get; set; }

        [JsonProperty("S_PREMIUM")]
        public int S_PREMIUM { get; set; }

        [JsonProperty("DATE_START_PREMIUM")]
        public string DATE_START_PREMIUM { get; set; }

        [JsonProperty("DATE_START")]
        public string DATE_START { get; set; }

        [JsonProperty("STATUS")]
        public int STATUS { get; set; }

        [JsonProperty("USER_ID")]
        public int USER_ID { get; set; }

        [JsonProperty("URL_REWRITING")]
        public string URL_REWRITING { get; set; }

        [JsonProperty("SNG_STATUS")]
        public string SNG_STATUS { get; set; }

        [JsonProperty("AVAILABLE_COUNTRIES")]
        public JsonDeezerSongDetailsResultsDataAvailableCountries AVAILABLE_COUNTRIES { get; set; }

        [JsonProperty("UPDATE_DATE")]
        public string UPDATE_DATE { get; set; }

        [JsonProperty("__TYPE__")]
        public string __TYPE__ { get; set; }
}