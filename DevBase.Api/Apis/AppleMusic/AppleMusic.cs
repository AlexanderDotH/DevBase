using System.Text.RegularExpressions;
using DevBase.Api.Apis.AppleMusic.Structure.Json;
using DevBase.Api.Objects;
using DevBase.Api.Serializer;
using DevBase.Enums;
using DevBase.Web;
using DevBase.Web.RequestData;
using DevBase.Web.RequestData.Data;
using DevBase.Web.ResponseData;
using HtmlAgilityPack;

namespace DevBase.Api.Apis.AppleMusic;

public class AppleMusic
{
    private readonly string _baseUrl;
    private readonly string _apiToken;

    private static readonly string _baseWebsite;

    static AppleMusic()
    {
        _baseWebsite = "https://music.apple.com";
    }
    
    public AppleMusic(string apiToken)
    {
        this._baseUrl = "https://amp-api.music.apple.com";
        this._apiToken = apiToken;
    }

    public static async Task<AppleMusic> WithAccessToken()
    {
        string url = $"{_baseWebsite}/us/browse";

        HtmlWeb htmlWeb = new HtmlWeb();
        HtmlDocument htmlDocument = await htmlWeb.LoadFromWebAsync(url);

        HtmlDocument assetDocument = await GetAssetContent(htmlDocument);

        if (assetDocument == null)
            return null;

        string accessToken = GetAccessToken(assetDocument);
        
        return new AppleMusic(accessToken);
    }
    
    // The apple search api is really dynamic. I just can't mirror it in my code
    public async Task<JsonAppleMusicSearchResult> Search(string searchTerm, int limit = 10)
    {
        string url =
            $"{this._baseUrl}/v1/catalog/de/search?fields[artists]=url,name,artwork&include[songs]=artists&limit={limit}&types=songs&with=lyricHighlights,lyrics,serverBubbles&term={searchTerm}";

        RequestData requestData = new RequestData(url);
        requestData.Header.Add("Origin", "https://music.apple.com");
        
        requestData.AddAuthMethod(new Auth(this._apiToken, EnumAuthType.OAUTH2));

        Request request = new Request(requestData);
        ResponseData responseData = await request.GetResponseAsync();

        string response = responseData.GetContentAsString();

        return new JsonDeserializer<JsonAppleMusicSearchResult>().Deserialize(response);
    }

    private static async Task<HtmlDocument> GetAssetContent(HtmlDocument htmlDocument)
    {
        if (htmlDocument == null)
            return null;

        string htmlParsedHead = htmlDocument.ParsedText;

        Regex assetRegex = new Regex("(/assets/index-[a-zA-Z0-9]*.js)");

        if (!assetRegex.IsMatch(htmlParsedHead))
            return null;

        Match assetMatch = assetRegex.Match(htmlParsedHead);

        string assetPath = assetMatch.Groups[1].Value;

        string url = $"{_baseWebsite}{assetPath}";

        return await new HtmlWeb().LoadFromWebAsync(url);
    }
    
    private static string GetAccessToken(HtmlDocument assetDocument)
    {
        string rawDocument = assetDocument.ParsedText;
        
        Regex accessTokenRegex = new Regex("(\\D)(eyJh[^\"]*)(\\D)");

        if (!accessTokenRegex.IsMatch(rawDocument))
            return null;

        Match accessTokenMatch = accessTokenRegex.Match(rawDocument);

        return accessTokenMatch.Groups[2].Value;
    }

    public string ApiToken => _apiToken;
}