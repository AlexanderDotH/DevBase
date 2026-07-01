using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public class AuthenticationGet : MusixMatchApiResponse
{
    [JsonProperty("tokens")]
    public AuthenticationTokens? Tokens { get; set; }

    [JsonProperty("credential")]
    public AuthenticationCredential? Credential { get; set; }

    [JsonProperty("auth")]
    public AuthenticationAuth? Auth { get; set; }

    [JsonIgnore]
    public AuthenticationTokens? tokens
    {
        get => this.Tokens;
        set => this.Tokens = value;
    }

    [JsonIgnore]
    public AuthenticationCredential? credential
    {
        get => this.Credential;
        set => this.Credential = value;
    }

    [JsonIgnore]
    public AuthenticationAuth? auth
    {
        get => this.Auth;
        set => this.Auth = value;
    }
}

public class AuthenticationAccount
{
    [JsonProperty("email")]
    public string? Email { get; set; }

    [JsonProperty("email_redirect_url")]
    public string? EmailRedirectUrl { get; set; }

    [JsonProperty("extra_params")]
    public AuthenticationExtraParams? ExtraParams { get; set; }

    [JsonProperty("first_name")]
    public string? FirstName { get; set; }

    [JsonProperty("last_name")]
    public string? LastName { get; set; }

    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("registration_country")]
    public string? RegistrationCountry { get; set; }

    [JsonProperty("user_type")]
    public string? UserType { get; set; }

    [JsonProperty("user_id")]
    public string? UserId { get; set; }

    [JsonProperty("user_picture")]
    public string? UserPicture { get; set; }

    [JsonProperty("user_nickname")]
    public string? UserNickname { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("verified")]
    public bool Verified { get; set; }

    [JsonProperty("user_last_location")]
    public AuthenticationUserLastLocation? UserLastLocation { get; set; }

    [JsonProperty("user_creation_date")]
    public DateTime UserCreationDate { get; set; }

    [JsonProperty("last_updated")]
    public DateTime LastUpdated { get; set; }

    [JsonProperty("namespace")]
    public string? Namespace { get; set; }

    [JsonProperty("userdata_id")]
    public string? UserdataId { get; set; }

    [JsonProperty("has_profile_photo")]
    public bool HasProfilePhoto { get; set; }

    [JsonProperty("country")]
    public string? Country { get; set; }
}

public class AuthenticationAuth
{
    [JsonProperty("user_id")]
    public string? UserId { get; set; }

    [JsonProperty("authorizations")]
    public AuthenticationAuthorizations? Authorizations { get; set; }
}

public class AuthenticationAuthorizations
{
    [JsonProperty("active")]
    public bool Active { get; set; }

    [JsonProperty("private")]
    public bool Private { get; set; }

    [JsonProperty("safe")]
    public string? Safe { get; set; }

    [JsonProperty("user_id")]
    public string? UserId { get; set; }

    [JsonProperty("last_updated")]
    public DateTime LastUpdated { get; set; }

    [JsonProperty("namespace")]
    public string? Namespace { get; set; }

    [JsonProperty("userdata_id")]
    public string? UserdataId { get; set; }

    [JsonProperty("referral_list")]
    public List<AuthenticationReferralList>? ReferralList { get; set; }
}

public class AuthenticationCredential
{
    [JsonProperty("action")]
    public string? Action { get; set; }

    [JsonProperty("email")]
    public string? Email { get; set; }

    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("account")]
    public AuthenticationAccount? Account { get; set; }
}

public class AuthenticationExtraParams
{
    [JsonProperty("mode")]
    public string? Mode { get; set; }

    [JsonProperty("redirect_url")]
    public string? RedirectUrl { get; set; }
}

public class AuthenticationReferralList
{
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("active")]
    public bool Active { get; set; }

    [JsonProperty("code")]
    public string? Code { get; set; }

    [JsonProperty("invitation")]
    public string? Invitation { get; set; }
}

public class AuthenticationTokens
{
    [JsonProperty("musixmatch-artists-v2.0")]
    public string? MusixmatchArtistsV20 { get; set; }

    [JsonProperty("mxm-backoffice-v1.0")]
    public string? MxmBackofficeV10 { get; set; }

    [JsonProperty("musixmatch-podcasts-v2.0")]
    public string? MusixmatchPodcastsV20 { get; set; }

    [JsonProperty("musixmatch-podcasts-v2.0-pp")]
    public string? MusixmatchPodcastsV20Pp { get; set; }

    [JsonProperty("mxm-pro-web-v1.0")]
    public string? MxmProWebV10 { get; set; }

    [JsonProperty("mxm-com-v1.0")]
    public string? MxmComV10 { get; set; }

    [JsonProperty("musixmatch-publishers-v2.0")]
    public string? MusixmatchPublishersV20 { get; set; }

    [JsonProperty("web-desktop-app-v1.0")]
    public string? WebDesktopAppV10 { get; set; }

    [JsonProperty("community-app-v1.0")]
    public string? CommunityAppV10 { get; set; }

    [JsonProperty("mxm-proxy-manager-v1.0")]
    public string? MxmProxyManagerV10 { get; set; }

    [JsonProperty("mxm-studio-v1.0")]
    public string? MxmStudioV10 { get; set; }

    [JsonProperty("mxm-account-v1.0")]
    public string? MxmAccountV10 { get; set; }

    [JsonIgnore]
    public string? webdesktopappv10
    {
        get => this.WebDesktopAppV10;
        set => this.WebDesktopAppV10 = value;
    }
}

public class AuthenticationUserLastLocation
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
