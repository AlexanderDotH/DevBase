using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public class TokenGet : MusixMatchApiResponse
{
    [JsonProperty("user_token")]
    public string? UserToken { get; set; }

    [JsonProperty("app_config")]
    public TokenAppConfig? ApplicationConfig { get; set; }

    [JsonProperty("location")]
    public TokenLocation? LocationData { get; set; }
}

public class TokenMissions
{
    [JsonProperty("enabled")]
    public bool Enabled { get; set; }

    [JsonProperty("community")]
    public List<string>? Community { get; set; }
}

public class TokenTracking
{
    [JsonProperty("context")]
    public string? Context { get; set; }

    [JsonProperty("delay")]
    public int Delay { get; set; }

    [JsonProperty("track_cache_ttl")]
    public int TrackCacheTtl { get; set; }
}

public class TokenTrackingList
{
    [JsonProperty("tracking")]
    public TokenTracking? Tracking { get; set; }
}

public class TokenOauthWebSignin
{
    [JsonProperty("enabled")]
    public bool Enabled { get; set; }

    [JsonProperty("user_prefix")]
    public string? UserPrefix { get; set; }
}

public class TokenService
{
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("display_name")]
    public string? DisplayName { get; set; }

    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("oauth_api")]
    public bool OauthApi { get; set; }

    [JsonProperty("oauth_web_signin")]
    public TokenOauthWebSignin? OauthWebSignin { get; set; }

    [JsonProperty("streaming")]
    public bool Streaming { get; set; }

    [JsonProperty("playlist")]
    public bool Playlist { get; set; }

    [JsonProperty("locker")]
    public bool Locker { get; set; }

    [JsonProperty("deeplink")]
    public bool Deeplink { get; set; }
}

public class TokenServiceList
{
    [JsonProperty("service")]
    public TokenService? Service { get; set; }
}

public class TokenPiggyback
{
    [JsonProperty("server_weight")]
    public int ServerWeight { get; set; }
}

public class TokenEventMap
{
    [JsonProperty("regex")]
    public string? Regex { get; set; }

    [JsonProperty("enabled")]
    public bool Enabled { get; set; }

    [JsonProperty("piggyback")]
    public TokenPiggyback? Piggyback { get; set; }
}

public class TokenAppConfig
{
    [JsonProperty("trial")]
    public bool Trial { get; set; }

    [JsonProperty("mobilePopOverMaximum")]
    public int MobilePopOverMaximum { get; set; }

    [JsonProperty("mobilePopOverMinTimes")]
    public int MobilePopOverMinTimes { get; set; }

    [JsonProperty("mobilePopOverMaxTimes")]
    public int MobilePopOverMaxTimes { get; set; }

    [JsonProperty("isRoviCopyEnabled")]
    public bool IsRoviCopyEnabled { get; set; }

    [JsonProperty("isGettyCopyEnabled")]
    public bool IsGettyCopyEnabled { get; set; }

    [JsonProperty("searchMaxResults")]
    public int SearchMaxResults { get; set; }

    [JsonProperty("fbShareUrlSpotify")]
    public bool FbShareUrlSpotify { get; set; }

    [JsonProperty("twShareUrlSpotify")]
    public bool TwShareUrlSpotify { get; set; }

    [JsonProperty("fbPostTimeline")]
    public bool FbPostTimeline { get; set; }

    [JsonProperty("fbOpenGraph")]
    public bool FbOpenGraph { get; set; }

    [JsonProperty("subtitlesMaxDeviation")]
    public int SubtitlesMaxDeviation { get; set; }

    [JsonProperty("localeDefaultLang")]
    public string? LocaleDefaultLang { get; set; }

    [JsonProperty("missionEnable")]
    public bool MissionEnable { get; set; }

    [JsonProperty("content_team")]
    public bool ContentTeam { get; set; }

    [JsonProperty("missions")]
    public TokenMissions? Missions { get; set; }

    [JsonProperty("appstore_products")]
    public List<object>? AppstoreProducts { get; set; }

    [JsonProperty("tracking_list")]
    public List<TokenTrackingList>? TrackingList { get; set; }

    [JsonProperty("spotifyCountries")]
    public List<string>? SpotifyCountries { get; set; }

    [JsonProperty("service_list")]
    public List<TokenServiceList>? ServiceList { get; set; }

    [JsonProperty("show_amazon_music")]
    public bool ShowAmazonMusic { get; set; }

    [JsonProperty("isSentryEnabled")]
    public bool IsSentryEnabled { get; set; }

    [JsonProperty("languages")]
    public List<string>? Languages { get; set; }

    [JsonProperty("last_updated")]
    public DateTime LastUpdated { get; set; }

    [JsonProperty("cluster")]
    public string? Cluster { get; set; }

    [JsonProperty("event_map")]
    public List<TokenEventMap>? EventMap { get; set; }
}

public class TokenLocation
{
    [JsonProperty("GEOIP_CITY_COUNTRY_CODE")]
    public string? GeoipCityCountryCode { get; set; }

    [JsonProperty("GEOIP_CITY_COUNTRY_CODE3")]
    public string? GeoipCityCountryCode3 { get; set; }

    [JsonProperty("GEOIP_CITY_COUNTRY_NAME")]
    public string? GeoipCityCountryName { get; set; }

    [JsonProperty("GEOIP_CITY")]
    public string? GeoipCity { get; set; }

    [JsonProperty("GEOIP_CITY_CONTINENT_CODE")]
    public string? GeoipCityContinentCode { get; set; }

    [JsonProperty("GEOIP_LATITUDE")]
    public double GeoipLatitude { get; set; }

    [JsonProperty("GEOIP_LONGITUDE")]
    public double GeoipLongitude { get; set; }

    [JsonProperty("GEOIP_AS_ORG")]
    public string? GeoipAsOrg { get; set; }

    [JsonProperty("GEOIP_ORG")]
    public string? GeoipOrg { get; set; }

    [JsonProperty("GEOIP_ISP")]
    public string? GeoipIsp { get; set; }

    [JsonProperty("GEOIP_NET_NAME")]
    public string? GeoipNetName { get; set; }

    [JsonProperty("BADIP_TAGS")]
    public List<object>? BadipTags { get; set; }
}
