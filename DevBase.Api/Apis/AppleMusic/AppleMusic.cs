using System.Net;
using System.Text.RegularExpressions;
using DevBase.Api.Apis.AppleMusic.Structure.Json;
using DevBase.Api.Apis.AppleMusic.Structure.Objects;
using DevBase.Api.Objects;
using DevBase.Api.Objects.Token;
using DevBase.Api.Serializer;
using DevBase.Enums;
using DevBase.Generics;
using DevBase.Web;
using DevBase.Web.RequestData;
using DevBase.Web.RequestData.Data;
using DevBase.Web.ResponseData;
using HtmlAgilityPack;

namespace DevBase.Api.Apis.AppleMusic;

public class AppleMusic
{
    private readonly string _baseUrl;
    private readonly AuthenticationToken _apiToken;
    private GenericAuthenticationToken _userMediaToken;

    private static readonly string _baseWebsite;
    private static readonly string _webAuthUrl;

    static AppleMusic()
    {
        _baseWebsite = "https://music.apple.com";
        _webAuthUrl = "https://buy.music.apple.com";
    }
    
    public AppleMusic(string apiToken)
    {
        this._baseUrl = "https://amp-api.music.apple.com";
        this._apiToken = AuthenticationToken.FromString(apiToken);
    }

    public AppleMusic WithMediaUserToken(GenericAuthenticationToken userMediaToken)
    {
        this._userMediaToken = userMediaToken;
        return this;
    }
    
    public async Task WithMediaUserTokenFromCookie(string myacinfoCookie)
    {
        string url = $"{_webAuthUrl}/account/web/auth";

        RequestData requestData = new RequestData(url, EnumRequestMethod.POST);
        requestData.ContentTypeHolder.Set(EnumContentType.APPLICATION_JSON);
        requestData.Header.Add("Origin", "https://music.apple.com");

        requestData.Accept = "*/*";
        
        requestData.CookieContainer.Add(new Cookie("myacinfo", myacinfoCookie, "/", "apple.com"));
            
        Request request = new Request(requestData);
        ResponseData responseData = await request.GetResponseAsync();

        WebHeaderCollection headers = responseData.Response.Headers;
        this._userMediaToken = GetMediaUserToken(headers);
    }

    private GenericAuthenticationToken GetMediaUserToken(WebHeaderCollection headerCollection)
    {
        string? header = headerCollection.Get("Set-Cookie");
        
        string[] splitted = header.Split(",");

        string parsedToken = string.Empty;
        string parsedExpiresIn = string.Empty;
        
        for (var i = 0; i < splitted.Length; i++)
        {
            string element = splitted[i].Trim();

            if (element.Contains("media-user-token"))
            {
                string[] cookie = element.Split(";");

                for (int j = 0; j < cookie.Length; j++)
                {
                    string elementInCookie = cookie[j];

                    if (elementInCookie.Contains("media-user-token"))
                        parsedToken = elementInCookie.Split("=")[1];

                    if (elementInCookie.Contains("Max-Age"))
                    {
                        parsedExpiresIn = elementInCookie.Split("=")[1];
                        break;
                    }
                }
            }
        }

        int convertedExpiresIn;
        int.TryParse(parsedExpiresIn, out convertedExpiresIn);
        
        DateTimeOffset expires = DateTimeOffset.Now.AddSeconds(convertedExpiresIn);

        GenericAuthenticationToken authenticationToken = new GenericAuthenticationToken()
        {
            Token = parsedToken,
            ExpiresAt = expires.DateTime
        };

        return authenticationToken;
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

    public async Task<List<AppleMusicTrack>> Search(string searchTerm, int limit = 10)
    {
        JsonAppleMusicSearchResult searchResult = await RawSearch(searchTerm, limit);

        List<AppleMusicTrack> appleMusicTracks = new List<AppleMusicTrack>();
        
        if (searchResult == null || 
            searchResult.SearchResults == null ||
            searchResult.SearchResults.SongResult == null || 
            searchResult.SearchResults.SongResult.Songs == null)
        {
            return null;
        }

        for (int i = 0; i < searchResult.SearchResults.SongResult.Songs.Count; i++)
        {
            JsonAppleMusicSearchResultResultsSongData songData = searchResult
                    .SearchResults
                    .SongResult
                    .Songs[i];
            
            appleMusicTracks.Add(AppleMusicTrack.FromResponse(songData));
        }

        return appleMusicTracks;
    }
    
    // The apple search api is really dynamic. I just can't mirror it in my code
    public async Task<JsonAppleMusicSearchResult> RawSearch(string searchTerm, int limit = 10)
    {
        string url =
            $"{this._baseUrl}/v1/catalog/de/search?fields[artists]=url,name,artwork&include[songs]=artists&limit={limit}&types=songs&with=lyricHighlights,lyrics,serverBubbles&term={searchTerm}";

        RequestData requestData = new RequestData(url);
        requestData.Header.Add("Origin", "https://music.apple.com");
        
        requestData.AddAuthMethod(new Auth(this._apiToken.RawToken, EnumAuthType.OAUTH2));

        Request request = new Request(requestData);
        ResponseData responseData = await request.GetResponseAsync();

        string response = responseData.GetContentAsString();

        return new JsonDeserializer<JsonAppleMusicSearchResult>().Deserialize(response);
    }
    
    public async Task<JsonAppleMusicLyricsResponse> GetLyrics(string trackId)
    {
        if (string.IsNullOrEmpty(this._userMediaToken.Token))
            throw new System.Exception("User-Media-Token is not set");
        
        string url = $"{this._baseUrl}/v1/catalog/de/songs/{trackId}/syllable-lyrics";

        RequestData requestData = new RequestData(url);
        requestData.Header.Add("Origin", "https://music.apple.com");
        requestData.Header.Add("Media-User-Token", this._userMediaToken.Token);
        
        requestData.AddAuthMethod(new Auth(this._apiToken.RawToken, EnumAuthType.OAUTH2));

        Request request = new Request(requestData);
        ResponseData responseData = await request.GetResponseAsync();

        string response = responseData.GetContentAsString();

        return new JsonDeserializer<JsonAppleMusicLyricsResponse>().Deserialize(response);
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

    public AuthenticationToken ApiToken => _apiToken;
}