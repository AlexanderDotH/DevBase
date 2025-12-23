using System.Net;
using System.Text.RegularExpressions;
using DevBase.Net.Core;
using DevBase.Net.Security.Token;
using DevBase.Api.Apis.AppleMusic.Structure.Json;
using DevBase.Api.Apis.AppleMusic.Structure.Objects;
using DevBase.Api.Enums;
using DevBase.Api.Exceptions;
using DevBase.Api.Serializer;
using DevBase.Enums;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.AppleMusic;

public class AppleMusic : ApiClient
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

        Response response = await new Request(url)
            .AsPost()
            .WithHeader("Origin", this._baseUrl)
            .WithAccept("*/*")
            .WithCookie($"myacinfo={myacinfoCookie}")
            .SendAsync();

        this._userMediaToken = GetMediaUserToken(response.Headers);
    }

    private GenericAuthenticationToken GetMediaUserToken(System.Net.Http.Headers.HttpResponseHeaders headerCollection)
    {
        if (!headerCollection.TryGetValues("Set-Cookie", out IEnumerable<string> values))
            return new GenericAuthenticationToken();
        
        string parsedToken = string.Empty;
        string parsedExpiresIn = string.Empty;

        foreach (string element in values)
        {
            if (element.Contains("media-user-token"))
            {
                string[] cookieParts = element.Split(";");

                foreach (string part in cookieParts)
                {
                    string trimmed = part.Trim();
                    
                    if (trimmed.StartsWith("media-user-token="))
                        parsedToken = trimmed.Split("=")[1];

                    if (trimmed.StartsWith("Max-Age="))
                    {
                        parsedExpiresIn = trimmed.Split("=")[1];
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

        Response response = await new Request(url).SendAsync();
        string content = await response.GetStringAsync();

        HtmlDocument htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(content);

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
            return Throw<object>(new AppleMusicException(EnumAppleMusicExceptionType.SearchResultsEmpty));
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

        Response response = await new Request(url)
            .AsGet()
            .WithHeader("Origin", this._baseUrl)
            .UseBearerAuthentication(this._apiToken.RawToken)
            .SendAsync();

        return await response.ParseJsonAsync<JsonAppleMusicSearchResult>(false);
    }
    
    public async Task<JsonAppleMusicLyricsResponse> GetLyrics(string trackId)
    {
        if (string.IsNullOrEmpty(this._userMediaToken?.Token))
            throw new AppleMusicException(EnumAppleMusicExceptionType.UnprovidedUserMediaToken);
        
        string url = $"{this._baseUrl}/v1/catalog/de/songs/{trackId}/syllable-lyrics";

        Response response = await new Request(url)
            .AsGet()
            .WithHeader("Origin", this._baseUrl)
            .WithHeader("Media-User-Token", this._userMediaToken.Token)
            .UseBearerAuthentication(this._apiToken.RawToken)
            .SendAsync();

        return await response.ParseJsonAsync<JsonAppleMusicLyricsResponse>(false);
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

        Response response = await new Request(url).SendAsync();
        string content = await response.GetStringAsync();
        
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(content);
        
        return doc;
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