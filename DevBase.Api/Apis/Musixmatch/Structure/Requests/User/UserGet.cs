using DevBase.Api.Apis.Musixmatch.Structure.Types;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public sealed class UserGet : MusixMatchApiResponse
{
    [JsonProperty("user")]
    public UserAccount? UserData { get; set; }

    [JsonProperty("device")]
    public Device? DeviceData { get; set; }

    public sealed class UserLastLocation
    {
        [JsonProperty("GEOIP_CITY_COUNTRY_CODE")]
        public string GeoIpCityCountryCode { get; set; } = string.Empty;

        [JsonProperty("GEOIP_CITY_COUNTRY_CODE3")]
        public string GeoIpCityCountryCode3 { get; set; } = string.Empty;

        [JsonProperty("GEOIP_CITY_COUNTRY_NAME")]
        public string GeoIpCityCountryName { get; set; } = string.Empty;

        [JsonProperty("GEOIP_CITY")]
        public string GeoIpCity { get; set; } = string.Empty;

        [JsonProperty("GEOIP_CITY_CONTINENT_CODE")]
        public string GeoIpCityContinentCode { get; set; } = string.Empty;

        [JsonProperty("GEOIP_LATITUDE")]
        public double GeoIpLatitude { get; set; }

        [JsonProperty("GEOIP_LONGITUDE")]
        public double GeoIpLongitude { get; set; }

        [JsonProperty("GEOIP_AS_ORG")]
        public string GeoIpAsOrg { get; set; } = string.Empty;

        [JsonProperty("GEOIP_ORG")]
        public string GeoIpOrg { get; set; } = string.Empty;

        [JsonProperty("GEOIP_ISP")]
        public string GeoIpIsp { get; set; } = string.Empty;

        [JsonProperty("GEOIP_NET_NAME")]
        public string GeoIpNetName { get; set; } = string.Empty;

        [JsonProperty("BADIP_TAGS")]
        public List<object> BadIpTags { get; set; } = [];
    }

    public sealed class Offers
    {
    }

    public sealed class ReferralCode
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("invitation")]
        public string Invitation { get; set; } = string.Empty;

        [JsonProperty("url")]
        public string Url { get; set; } = string.Empty;
    }

    public sealed class ReferralCodeList
    {
        [JsonProperty("referral_code")]
        public ReferralCode? ReferralCode { get; set; }
    }

    public sealed class Profile
    {
        [JsonProperty("last_name")]
        public string LastName { get; set; } = string.Empty;

        [JsonProperty("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [JsonProperty("middle_name")]
        public string MiddleName { get; set; } = string.Empty;

        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("user_id")]
        public string UserId { get; set; } = string.Empty;

        [JsonProperty("user_picture")]
        public string UserPicture { get; set; } = string.Empty;

        [JsonProperty("user_nickname")]
        public string UserNickname { get; set; } = string.Empty;

        [JsonProperty("user_type")]
        public string UserType { get; set; } = string.Empty;

        [JsonProperty("user_last_location")]
        public UserLastLocation? UserLastLocation { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("birthday")]
        public string Birthday { get; set; } = string.Empty;

        [JsonProperty("gender")]
        public string Gender { get; set; } = string.Empty;

        [JsonProperty("age_range")]
        public string AgeRange { get; set; } = string.Empty;

        [JsonProperty("user_creation_date")]
        public DateTime? UserCreationDate { get; set; }

        [JsonProperty("has_profile_photo")]
        public bool HasProfilePhoto { get; set; }
    }

    public sealed class Authorizations
    {
        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("private")]
        public bool Private { get; set; }

        [JsonProperty("safe")]
        public string Safe { get; set; } = string.Empty;

        [JsonProperty("user_id")]
        public string UserId { get; set; } = string.Empty;

        [JsonProperty("last_updated")]
        public DateTime? LastUpdated { get; set; }

        [JsonProperty("namespace")]
        public string Namespace { get; set; } = string.Empty;

        [JsonProperty("userdata_id")]
        public string UserdataId { get; set; } = string.Empty;

        [JsonProperty("mission_subscriber")]
        public bool MissionSubscriber { get; set; }
    }

    public sealed class Features
    {
        [JsonProperty("no_ads")]
        public int NoAds { get; set; }

        [JsonProperty("lyrics_offline")]
        public int LyricsOffline { get; set; }

        [JsonProperty("clean_metadata")]
        public int CleanMetadata { get; set; }

        [JsonProperty("party_mode")]
        public int PartyMode { get; set; }
    }

    public sealed class ActiveProduct
    {
        [JsonProperty("product_id")]
        public string ProductId { get; set; } = string.Empty;

        [JsonProperty("product_type")]
        public string ProductType { get; set; } = string.Empty;

        [JsonProperty("creation_date")]
        public DateTime? CreationDate { get; set; }

        [JsonProperty("start_date")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("end_date")]
        public DateTime? EndDate { get; set; }

        [JsonProperty("credits")]
        public int Credits { get; set; }

        [JsonProperty("features")]
        public Features? Features { get; set; }
    }

    public sealed class UserAccount
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; } = string.Empty;

        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("user_last_location")]
        public UserLastLocation? UserLastLocation { get; set; }

        [JsonProperty("user_creation_date")]
        public DateTime? UserCreationDate { get; set; }

        [JsonProperty("offers")]
        public Offers? Offers { get; set; }

        [JsonProperty("referral_code_list")]
        public List<ReferralCodeList> ReferralCodeList { get; set; } = [];

        [JsonProperty("profile")]
        public Profile? Profile { get; set; }

        [JsonProperty("authorizations")]
        public Authorizations? Authorizations { get; set; }

        [JsonProperty("payment_widget_link")]
        public string PaymentWidgetLink { get; set; } = string.Empty;

        [JsonProperty("active_products")]
        public List<ActiveProduct> ActiveProducts { get; set; } = [];
    }

    public sealed class Device
    {
        [JsonProperty("guid")]
        public string Guid { get; set; } = string.Empty;

        [JsonProperty("build_number")]
        public string BuildNumber { get; set; } = string.Empty;
    }
}
