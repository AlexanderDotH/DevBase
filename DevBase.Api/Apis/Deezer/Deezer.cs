using System.Net;
using System.Text;
using DevBase.Api.Apis.Deezer.Structure.Json;
using DevBase.Api.Apis.Deezer.Structure.Objects;
using DevBase.Api.Enums;
using DevBase.Api.Exceptions;
using DevBase.Api.Serializer;
using DevBase.Cryptography.Blowfish;
using DevBase.Enums;
using DevBase.Format.Formats.LrcFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.Net.Core;
using DevBase.Net.Data.Body;
using Newtonsoft.Json.Linq;

namespace DevBase.Api.Apis.Deezer;

public class Deezer : ApiClient
{
    private readonly string _authEndpoint;
    private readonly string _apiEndpoint;
    private readonly string _pipeEndpoint;
    private readonly string _websiteEndpoint;
    private readonly string _mediaEndpoint;

    private readonly CookieContainer _cookieContainer;

    public Deezer(string arlToken = "")
    {
        this._authEndpoint = "https://auth.deezer.com";
        this._apiEndpoint = "https://api.deezer.com";
        this._pipeEndpoint = "https://pipe.deezer.com";
        this._websiteEndpoint = "https://www.deezer.com";
        this._mediaEndpoint = "https://media.deezer.com";

        this._cookieContainer = new CookieContainer();
        
        if (arlToken != null && arlToken.Length != 0)
            this._cookieContainer.Add(new Cookie("arl", arlToken, "/", "deezer.com"));
    }

    public async Task<JsonDeezerJwtToken> GetJwtToken()
    {
        if (!IsArlTokenPresent())
            return Throw<object>(new DeezerException(EnumDeezerExceptionType.ArlToken));
        
        string url = $"{this._authEndpoint}/login/arl?i=c&jo=p&rto=n";
        
        Request request = new Request(url)
            .AsPost()
            .WithTimeout(TimeSpan.FromMinutes(1))
            .WithHeader("Accept", "*/*")
            .WithHeader("Accept-Encoding", "gzip, deflate, br")
            .WithJsonBody("{}");

        ApplyCookies(request, url);
        
        Response response = await request.SendAsync();
        UpdateCookies(response);

        return await response.ParseJsonAsync<JsonDeezerJwtToken>(false);
    }

    public async Task<JsonDeezerAuthTokenResponse> GetAccessToken(string appID = "457142")
    {
        string url = $"{this._apiEndpoint}/platform/generic/token/unlogged";
        
        Request request = new Request(url)
            .AsPost()
            .WithHeader("Accept", "*/*")
            .WithEncodedForm(("app_id", appID));
        
        ApplyCookies(request, url);
        
        Response response = await request.SendAsync();
        UpdateCookies(response);

        string content = await response.GetStringAsync();

        if (content.Contains("unable to get unlogged token for this app"))
            return Throw<object>(new DeezerException(EnumDeezerExceptionType.AppId));
        
        return await response.ParseJsonAsync<JsonDeezerAuthTokenResponse>(false);
    }
    
    public async Task<JsonDeezerAuthTokenResponse> GetAccessToken(string sessionID, string appID = "457142")
    {
        string url = $"{this._apiEndpoint}/platform/generic/token/create-from-session";

        Request request = new Request(url)
            .AsPost()
            .WithHeader("Accept", "*/*")
            .WithEncodedForm(("app_id", appID), ("sid", sessionID));
        
        ApplyCookies(request, url);
        
        Response response = await request.SendAsync();
        UpdateCookies(response);

        string content = await response.GetStringAsync();

        if (content.Contains("Internal Server Error"))
            return Throw<object>(new DeezerException(EnumDeezerExceptionType.AppSessionId));
        
        if (content.Contains("unable to get unlogged token for this app"))
            return Throw<object>(new DeezerException(EnumDeezerExceptionType.AppId));

        if (content.Contains("No session found"))
            return Throw<object>(new DeezerException(EnumDeezerExceptionType.SessionId));
        
        return await response.ParseJsonAsync<JsonDeezerAuthTokenResponse>(false);
    }

    public async Task<string> GetArlTokenFromSession(string sessionID)
    {
        string url = $"{this._websiteEndpoint}/ajax/gw-light.php?method=user.getArl&input=3&api_version=1.0&api_token=null";

        Request request = new Request(url)
            .AsGet()
            .WithCookie($"sid={sessionID}");
        
        Response response = await request.SendAsync();
        string content = await response.GetStringAsync();

        if (content.Contains("Require user auth"))
            return Throw<object>(new DeezerException(EnumDeezerExceptionType.SessionId));

        JsonDeezerArlTokenResponse token = await response.ParseJsonAsync<JsonDeezerArlTokenResponse>(false);
        return token.results;
    }
    
    public async Task<(string RawLyrics, AList<TimeStampedLyric> TimeStampedLyrics)> GetLyrics(string trackID)
    {
        return IsArlTokenPresent() ? 
            await GetLyricsGraphAndParse(trackID) : 
            await GetLyricsAjaxAndParse(trackID);
    }

    public async Task<JsonDeezerRawLyricsResponse> GetLyricsAjax(string trackID)
    {
        string csrfToken = await GetCsrfToken();

        if (string.IsNullOrEmpty(csrfToken))
            return Throw<object>(new DeezerException(EnumDeezerExceptionType.InvalidCsrfToken));

        string url = $"{this._websiteEndpoint}/ajax/gw-light.php?method=song.getLyrics&api_version=1.0&input=3&api_token={csrfToken}&cid={this.RandomCid}";

        JObject jsonTrackID = new JObject();
        jsonTrackID["sng_id"] = trackID;

        Request request = new Request(url)
            .AsPost()
            .WithTimeout(TimeSpan.FromMinutes(1))
            .WithJsonBody(jsonTrackID.ToString());
        
        ApplyCookies(request, url);
        
        Response response = await request.SendAsync();
        UpdateCookies(response);

        string content = await response.GetStringAsync();

        if (content.Contains("Invalid CSRF token"))
            return Throw<object>(new DeezerException(EnumDeezerExceptionType.NoCsrfToken));
        
        return await response.ParseJsonAsync<JsonDeezerRawLyricsResponse>(false);
    }
    
    public async Task<JsonDeezerLyricsResponse> GetLyricsGraph(string trackID)
    {
        JsonDeezerJwtToken jwtToken = await this.GetJwtToken();
        string url = $"{this._pipeEndpoint}/api";
        
        JObject trackId = new JObject();
        trackId["trackId"] = trackID;

        JObject jObject = new JObject();
        jObject["query"] = "query SynchronizedTrackLyrics($trackId: String!) {  track(trackId: $trackId) {    ...SynchronizedTrackLyrics    __typename  }}fragment SynchronizedTrackLyrics on Track {  id  lyrics {    ...Lyrics    __typename  }  album {    cover {      small: urls(pictureRequest: {width: 100, height: 100})      medium: urls(pictureRequest: {width: 264, height: 264})      large: urls(pictureRequest: {width: 800, height: 800})      explicitStatus      __typename    }    __typename  }  __typename}fragment Lyrics on Lyrics {  id  copyright  text  writers  synchronizedLines {    ...LyricsSynchronizedLines    __typename  }  __typename}fragment LyricsSynchronizedLines on LyricsSynchronizedLine {  lrcTimestamp  line  lineTranslated  milliseconds  duration  __typename}";
        jObject["variables"] = trackId;

        Request request = new Request(url)
            .AsPost()
            .WithJsonBody(jObject.ToString())
            .UseBearerAuthentication(jwtToken.jwt);
        
        ApplyCookies(request, url);
        
        Response response = await request.SendAsync();
        UpdateCookies(response);

        string content = await response.GetStringAsync();

        if (content.Contains("JwtTokenExpiredError"))
            return Throw<object>(new DeezerException(EnumDeezerExceptionType.JwtExpired));
        
        return await response.ParseJsonAsync<JsonDeezerLyricsResponse>(false);
    }

    public async Task<string> GetCsrfToken()
    {
        string rawUserData = await GetUserDataRaw();

        if (string.IsNullOrEmpty(rawUserData))
            return string.Empty;

        JObject parsed = JObject.Parse(rawUserData);

        if (!parsed.ContainsKey("results"))
            return Throw<string>(new DeezerException(EnumDeezerExceptionType.CsrfParsing));

        JToken jToken = parsed.SelectToken("$.results.checkForm");

        if (jToken == null)
            return Throw<string>(new DeezerException(EnumDeezerExceptionType.CsrfParsing));

        return jToken.Value<string>();
    }
    
    public async Task<JsonDeezerUserData> GetUserData(int retries = 5)
    {
        string rawUserData = await GetUserDataRaw(retries);

        if (string.IsNullOrEmpty(rawUserData))
            return null;
        
        return await new JsonDeserializer<JsonDeezerUserData>().DeserializeAsync(rawUserData);
    }

    public async Task<string> GetUserDataRaw(int retries = 5)
    {
        string url = $"{this._websiteEndpoint}/ajax/gw-light.php?method=deezer.getUserData&api_version=1.0&input=3&api_token=&cid={this.RandomCid}";

        Request request = new Request(url)
            .AsGet()
            .WithTimeout(TimeSpan.FromSeconds(10));
        
        ApplyCookies(request, url);
        
        Response response = await request.SendAsync();
        UpdateCookies(response);

        string content = await response.GetStringAsync(Encoding.ASCII);
            
        if (!content.Contains("deprecated?method=deezer.getUserData"))
            return content;

        return string.Empty;
    }

    public async Task<DeezerTrack> GetSong(string trackID)
    {
        string csrfToken = await GetCsrfToken();

        if (string.IsNullOrEmpty(csrfToken))
            return Throw<object>(new DeezerException(EnumDeezerExceptionType.NoCsrfToken));
        
        JsonDeezerSongDetails songDetails = await this.GetSongDetails(trackID, csrfToken);

        if (songDetails == null || 
            songDetails.results == null || 
            songDetails.results.DATA == null ||
            songDetails.results.ISRC == null || 
            songDetails.results.RELATED_ALBUMS == null)
            return Throw<object>(new DeezerException(EnumDeezerExceptionType.MissingSongDetails));
        
        int durationS = Convert.ToInt32(songDetails.results.DATA.DURATION);
        
        DeezerTrack track = new DeezerTrack()
        {
            Title = songDetails.results.DATA.SNG_TITLE,
            Album = songDetails.results.DATA.ALB_TITLE,
            Duration = (int)TimeSpan.FromSeconds(durationS).TotalMilliseconds,
            Artists = GetArtists(songDetails.results.DATA.ARTISTS),
            ArtworkUrls = GetArtworks(songDetails.results.DATA.ALB_PICTURE),
            ServiceInternalId = songDetails.results.DATA.SNG_ID
        };

        return track;
    }
    
    public async Task<JsonDeezerSongDetails> GetSongDetails(string trackID, string csrfToken, int retries = 5)
    {
        if (trackID == "0")
            return Throw<object>(new DeezerException(EnumDeezerExceptionType.WrongParameter));
        
        string url = $"{this._websiteEndpoint}/ajax/gw-light.php?method=deezer.pageTrack&api_version=1.0&input=3&api_token={csrfToken}";

        for (int i = 0; i < retries; i++)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["sng_id"] = trackID;
            
                Request request = new Request(url)
                    .AsPost()
                    .WithTimeout(TimeSpan.FromSeconds(10))
                    .WithJsonBody(jObject.ToString());
                
                ApplyCookies(request, url);
        
                Response response = await request.SendAsync();
                UpdateCookies(response);
            
                string content = await response.GetStringAsync();

                if (content.Contains("Invalid CSRF token"))
                    return Throw<object>(new DeezerException(EnumDeezerExceptionType.InvalidCsrfToken));

                if (content.Contains("Wrong parameters"))
                    return Throw<object>(new DeezerException(EnumDeezerExceptionType.WrongParameter));
            
                if (!content.Contains("deprecated?method=deezer.pageTrack"))
                    return await response.ParseJsonAsync<JsonDeezerSongDetails>(false);
            }
            catch (System.Exception e)
            {
                return Throw<object>(new DeezerException(EnumDeezerExceptionType.FailedToReceiveSongDetails));
            }
        }

        return null;
    }
    
    public async Task<JsonDeezerSongSource> GetSongUrls(string trackToken, string licenseToken)
    {
        string url = $"{this._mediaEndpoint}/v1/get_url";
        
        JObject jObject = new JObject
        {
            {"license_token", licenseToken},
            {"media", 
                new JArray
                {
                    new JObject
                    {
                        {"type", "FULL"},
                        {"formats", new JArray
                        {
                            new JObject
                            {
                                {"cipher","BF_CBC_STRIPE"},
                                {"format","MP3_128"}
                            },
                            new JObject
                            {
                                {"cipher","BF_CBC_STRIPE"},
                                {"format","MP3_64"}
                            },
                            new JObject
                            {
                                {"cipher","BF_CBC_STRIPE"},
                                {"format","MP3_MISC"}
                            }
                        }}
                    }
                }
            },
            {"track_tokens", new JArray
            {
                trackToken
            }}
        };
        
        Request request = new Request(url)
            .AsPost()
            .WithJsonBody(jObject.ToString());
        
        ApplyCookies(request, url);
        
        Response response = await request.SendAsync();
        UpdateCookies(response);
        
        return await response.ParseJsonAsync<JsonDeezerSongSource>(false);
    }

    #pragma warning disable S1751
    public async Task<byte[]> DownloadSong(string trackID)
    {
        JsonDeezerUserData userData = await this.GetUserData();

        if (userData == null)
            return Throw<byte>(new DeezerException(EnumDeezerExceptionType.UserData));
        
        JsonDeezerSongDetails songDetails = await this.GetSongDetails(trackID, userData.results.checkForm);

        if (songDetails == null)
            return Throw<byte>(new DeezerException(EnumDeezerExceptionType.MissingSongDetails));
        
        JsonDeezerSongSource? url = await this.GetSongUrls(songDetails.results.DATA.TRACK_TOKEN, userData.results.USER.OPTIONS.license_token);

        if (url == null)
            return Throw<byte>(new DeezerException(EnumDeezerExceptionType.UrlData));

        if (url.data.Count == 0)
            return Throw<byte>(new DeezerException(EnumDeezerExceptionType.UrlData));

        for (int i = 0; i < url.data.Count; i++)
        {
            JsonDeezerSongSourceData data = url.data[i];

            for (int j = 0; j < data.media.Count; j++)
            {
                JsonDeezerSongSourceDataMedia media = data.media[j];

                for (int k = 0; k < media.sources.Count; k++)
                {
                    JsonDeezerSongSourceDataMediaSource source = media.sources[i];

                    Response response = await new Request(source.url).SendAsync();
                    
                    byte[] buffer = await response.GetBytesAsync();
                    byte[] key = CalculateDecryptionKey(trackID);
                    
                    return DecryptSongData(buffer, key);
                }
            }
        }

        return Throw<byte>(new DeezerException(EnumDeezerExceptionType.UrlData));
    }
    #pragma warning restore S1751

    private byte[] CalculateDecryptionKey(string trackID)
    {
        string secret = "g4el58wc0zvf9na1";
        
        string hash = Cryptography.MD5.MD5.ToMD5String(trackID);

        StringBuilder key = new StringBuilder();
            
        for (int i = 0; i < 16; i++)
        {
            key.Append(Convert.ToChar(hash[i] ^ hash[i + 16] ^ secret[i]));
        }

        return Encoding.ASCII.GetBytes(key.ToString());
    }
    
    private byte[] DecryptSongData(byte[] data, byte[] key)
    {
        int chunkSize = 2048;
        AList<byte> decryptedContent = new AList<byte>();

        Blowfish blowfish = new Blowfish(key);
        byte[] iv = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 };

        byte[][] chunks = data.Chunk(chunkSize).ToArray();

        for (int i = 0; i < chunks.Length; i++)
        {
            byte[] currentChunk = chunks[i];

            if (i % 3 == 0 && currentChunk.Length == chunkSize)
            {
                currentChunk = currentChunk.CopyAndPadIfNotAlreadyPadded();
                blowfish.Decrypt(currentChunk, iv);
            }
            
            decryptedContent.AddRange(currentChunk);
        }

        return decryptedContent.GetAsArray();
    }
    
    public async Task<JsonDeezerSearchResponse> Search(string query)
    {
        Response response = await new Request($"{this._apiEndpoint}/search?q={query}")
            .SendAsync();
        
        return await response.ParseJsonAsync<JsonDeezerSearchResponse>(false);
    }
    
    public async Task<JsonDeezerSearchResponse> Search(string track = "", string artist = "", string album = "", bool strict = false)
    {
        return await new JsonDeserializer<JsonDeezerSearchResponse>()
            .DeserializeAsync(await SearchRaw(track, artist, album, strict));
    }

    private async Task<string> SearchRaw(string track = "", string artist = "", string album = "", bool strict = false)
    {
        string strictSearch = strict ? "?strict=on" : "";
        
        Response response = await new Request($"{this._apiEndpoint}/search?q=track:\"{track}\" artist:\"{artist}\" album:\"{album}\"{strictSearch}")
            .SendAsync();

        return await response.GetStringAsync();
    }
    
    public async Task<List<DeezerTrack>> SearchSongData(
        string track = "", 
        string artist = "", 
        string album = "",
        bool strict = false, 
        int limit = 10)
    {
        string searchResults = await SearchRaw(track, artist, album, strict);

        JObject resultData = JObject.Parse(searchResults);

        if (!resultData.ContainsKey("data"))
            return new List<DeezerTrack>();

        IEnumerable<JToken> dataArray = resultData.SelectTokens("$.data[*].id").Take(limit);

        if (!dataArray.Any())
            return new List<DeezerTrack>();
        
        List<DeezerTrack> deezerTracks = new List<DeezerTrack>();

        foreach (JToken token in dataArray)
        {
            string trackId = token.Value<string>();

            try
            {
                if (trackId != "0")
                {
                    deezerTracks.Add(await GetSong(trackId));
                }
            }
            catch
            {
                Throw<object>(new DeezerException(EnumDeezerExceptionType.MissingSongDetails));
            }
        }

        return deezerTracks;
    }
    
    private async Task<(string rawLyrics, AList<TimeStampedLyric> syncedElements)> GetLyricsGraphAndParse(string trackID)
    {
        string rawText = string.Empty;
        
        JsonDeezerLyricsResponse lyricsResponse = await GetLyricsGraph(trackID);

        if (lyricsResponse.data.track.lyrics == null)
            return Throw<object>(new DeezerException(EnumDeezerExceptionType.LyricsNotFound));

        if (lyricsResponse.data.track.lyrics.text != String.Empty)
            rawText = lyricsResponse.data.track.lyrics.text;

        if (lyricsResponse.data.track.lyrics.synchronizedLines == null ||
            lyricsResponse.data.track.lyrics.synchronizedLines.Count == 0)
            return (rawText, new AList<TimeStampedLyric>());
        
        StringBuilder lrcFile = new StringBuilder();
            
        for (var i = 0; i < lyricsResponse.data.track.lyrics.synchronizedLines.Count; i++)
        {
            JsonDeezerLyricsTrackResponseLyricsSynchronizedLineResponse synchronizedLine =
                lyricsResponse.data.track.lyrics.synchronizedLines[i];
            
            lrcFile.AppendLine($"{synchronizedLine.lrcTimestamp} {synchronizedLine.line}");
        }

        AList<TimeStampedLyric> syncedLyrics = new AList<TimeStampedLyric>();

        AList<TimeStampedLyric> parsed = new LrcParser().Parse(lrcFile.ToString());
        
        syncedLyrics.AddRange(parsed);

        return (rawText, syncedLyrics);
    }
    
    private async Task<(string RawLyrics, AList<TimeStampedLyric> TimeStampedLyrics)> GetLyricsAjaxAndParse(string trackID)
    {
        string rawText = string.Empty;
        
        JsonDeezerRawLyricsResponse lyricsResponse = await GetLyricsAjax(trackID);

        if (lyricsResponse == null || lyricsResponse.results == null)
            return Throw<object>(new DeezerException(EnumDeezerExceptionType.LyricsNotFound));

        if (lyricsResponse.results.LYRICS_TEXT != string.Empty)
            rawText = lyricsResponse.results.LYRICS_TEXT;
        
        if (lyricsResponse.results.LYRICS_SYNC_JSON == null || lyricsResponse.results.LYRICS_SYNC_JSON.Count == 0)
            return (rawText, new AList<TimeStampedLyric>());
        
        StringBuilder lrcFile = new StringBuilder();
        
        for (var i = 0; i < lyricsResponse.results.LYRICS_SYNC_JSON.Count; i++)
        {
            JsonDeezerRawLyricsResponseResultsSync lyricsLine = lyricsResponse.results.LYRICS_SYNC_JSON[i];
                
            if (lyricsLine.line == string.Empty || lyricsLine.line.Length == 0)
                continue;
                
            lrcFile.AppendLine($"{lyricsLine.lrc_timestamp} {lyricsLine.line}");
        }

        AList<TimeStampedLyric> syncedLyrics = new LrcParser().Parse(lrcFile.ToString());

        return (rawText, syncedLyrics);
    }

    private bool IsArlTokenValid()
    {
        JsonDeezerUserData userData = this.GetUserData().GetAwaiter().GetResult();
        return userData.results.USER.USER_ID != 0;
    }
    
    private int RandomCid => new Random().Next(100000000, 999999999);
    
    private string[] GetArtworks(string coverId)
    {
        List<string> artworkUrls = new List<string>();
        
        artworkUrls.Add($"https://e-cdns-images.dzcdn.net/images/cover/{coverId}/56x56-000000-80-0-0.jpg");
        artworkUrls.Add($"https://e-cdns-images.dzcdn.net/images/cover/{coverId}/250x250-000000-80-0-0.jpg");
        artworkUrls.Add($"https://e-cdns-images.dzcdn.net/images/cover/{coverId}/500x500-000000-80-0-0.jpg");
        artworkUrls.Add($"https://e-cdns-images.dzcdn.net/images/cover/{coverId}/1000x1000-000000-80-0-0.jpg");

        return artworkUrls.ToArray();
    }
    
    private string[] GetArtists(List<JsonDeezerSongDetailsResultsDataArtist> artists)
    {
        List<string> convertedArtists = new List<string>();

        for (int i = 0; i < artists.Count; i++)
            convertedArtists.Add(artists[i].ART_NAME);

        return convertedArtists.ToArray();
    }
    
    private void ApplyCookies(Request request, string url)
    {
        string cookieHeader = this._cookieContainer.GetCookieHeader(new Uri(url));
        if (!string.IsNullOrEmpty(cookieHeader))
        {
            request.WithHeader("Cookie", cookieHeader);
        }
    }

    private void UpdateCookies(Response response)
    {
        CookieCollection cookies = response.GetCookies();
        if (cookies.Count > 0 && response.RequestUri != null)
        {
            foreach (Cookie cookie in cookies)
            {
                if (string.IsNullOrEmpty(cookie.Domain))
                    cookie.Domain = response.RequestUri.Host;
                
                this._cookieContainer.Add(cookie);
            }
        }
    }

    private bool IsArlTokenPresent()
    {
        CookieCollection cookies = this._cookieContainer.GetAllCookies();
        
        for (var i = 0; i < cookies.Count; i++)
        {
            Cookie cookie = cookies[i];
            if (cookie.Name.Equals("arl") && IsArlTokenValid())
                return true;
        }

        return false;
    }
}