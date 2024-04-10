using System.Net;
using System.Text;
using DevBase.Api.Apis.Deezer.Structure.Json;
using DevBase.Api.Apis.Deezer.Structure.Objects;
using DevBase.Api.Enums;
using DevBase.Api.Exceptions;
using DevBase.Api.Objects.Token;
using DevBase.Api.Serializer;
using DevBase.Cryptography.Blowfish;
using DevBase.Enums;
using DevBase.Format.Formats.LrcFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.Web;
using DevBase.Web.RequestData;
using DevBase.Web.RequestData.Data;
using DevBase.Web.ResponseData;
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
        
        RequestData requestData = new RequestData($"{this._authEndpoint}/login/arl?i=c&jo=p&rto=n", EnumRequestMethod.POST);

        requestData.Timeout = TimeSpan.FromMinutes(1);

        requestData.Header.Add("Accept", "*/*");
        requestData.Header.Add("Accept-Encoding", "gzip, deflate, br");
        
        requestData.CookieContainer = this._cookieContainer;
        
        requestData.SetContentType(EnumContentType.APPLICATION_JSON);
        
        ResponseData responseData = new Request(requestData).GetResponse();
        return new JsonDeserializer<JsonDeezerJwtToken>().Deserialize(responseData.GetContentAsString());
    }

    public async Task<JsonDeezerAuthTokenResponse> GetAccessToken(string appID = "457142")
    {
        RequestData requestData = new RequestData($"{this._apiEndpoint}/platform/generic/token/unlogged", EnumRequestMethod.POST);
        requestData.Header.Add("Accept", "*/*");

        AList<FormKeypair> formData = new AList<FormKeypair>();
        formData.Add(new FormKeypair("app_id", appID));
        
        requestData.AddFormData(formData);
        
        requestData.CookieContainer = this._cookieContainer;
        
        ResponseData responseData = await new Request(requestData).GetResponseAsync();

        string response = responseData.GetContentAsString();

        if (response.Contains("unable to get unlogged token for this app"))
            return Throw<object>(new DeezerException(EnumDeezerExceptionType.AppId));
        
        return new JsonDeserializer<JsonDeezerAuthTokenResponse>().Deserialize(response);
    }
    
    public async Task<JsonDeezerAuthTokenResponse> GetAccessToken(string sessionID, string appID = "457142")
    {
        RequestData requestData = new RequestData($"{this._apiEndpoint}/platform/generic/token/create-from-session", EnumRequestMethod.POST);
        requestData.Header.Add("Accept", "*/*");

        AList<FormKeypair> formData = new AList<FormKeypair>();
        formData.Add(new FormKeypair("app_id", appID));
        formData.Add(new FormKeypair("sid", sessionID));
        
        requestData.AddFormData(formData);
        
        requestData.CookieContainer = this._cookieContainer;
        
        ResponseData responseData = await new Request(requestData).GetResponseAsync();

        string response = responseData.GetContentAsString();

        if (response.Contains("Internal Server Error"))
            return Throw<object>(new DeezerException(EnumDeezerExceptionType.AppSessionId));
        
        if (response.Contains("unable to get unlogged token for this app"))
            return Throw<object>(new DeezerException(EnumDeezerExceptionType.AppId));

        if (response.Contains("No session found"))
            return Throw<object>(new DeezerException(EnumDeezerExceptionType.SessionId));
        
        return new JsonDeserializer<JsonDeezerAuthTokenResponse>().Deserialize(response);
    }

    public async Task<string> GetArlTokenFromSession(string sessionID)
    {
        RequestData requestData = new RequestData(
            $"{this._websiteEndpoint}/ajax/gw-light.php?method=user.getArl&input=3&api_version=1.0&api_token=null", 
            EnumRequestMethod.GET);

        CookieContainer cookieContainer = new CookieContainer();
        cookieContainer.Add(new Cookie("sid", sessionID, "/", "deezer.com"));

        requestData.CookieContainer = cookieContainer;
        
        Request request = new Request(requestData);
        ResponseData responseData = await request.GetResponseAsync();

        string response = responseData.GetContentAsString();

        if (response.Contains("Require user auth"))
            return Throw<object>(new DeezerException(EnumDeezerExceptionType.SessionId));

        JsonDeezerArlTokenResponse token = new JsonDeserializer<JsonDeezerArlTokenResponse>().Deserialize(response);
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

        RequestData requestData = new RequestData(
            $"{this._websiteEndpoint}/ajax/gw-light.php?method=song.getLyrics&api_version=1.0&input=3&api_token={csrfToken}&cid={this.RandomCid}",
            EnumRequestMethod.POST);

        requestData.Timeout = TimeSpan.FromMinutes(1);
        
        JObject jsonTrackID = new JObject();
        jsonTrackID["sng_id"] = trackID;

        requestData.AddContent(jsonTrackID.ToString());
        requestData.SetContentType(EnumContentType.APPLICATION_JSON);

        requestData.CookieContainer = this._cookieContainer;
        
        Request request = new Request(requestData);
        ResponseData responseData = await request.GetResponseAsync();

        string response = responseData.GetContentAsString();

        if (response.Contains("Invalid CSRF token"))
            return Throw<object>(new DeezerException(EnumDeezerExceptionType.NoCsrfToken));
        
        return new JsonDeserializer<JsonDeezerRawLyricsResponse>().Deserialize(response);
    }
    
    public async Task<JsonDeezerLyricsResponse> GetLyricsGraph(string trackID)
    {
        JsonDeezerJwtToken jwtToken = await this.GetJwtToken();
        
        RequestData requestData = new RequestData($"{this._pipeEndpoint}/api", EnumRequestMethod.POST);

        JObject trackId = new JObject();
        trackId["trackId"] = trackID;

        JObject jObject = new JObject();
        jObject["query"] = "query SynchronizedTrackLyrics($trackId: String!) {  track(trackId: $trackId) {    ...SynchronizedTrackLyrics    __typename  }}fragment SynchronizedTrackLyrics on Track {  id  lyrics {    ...Lyrics    __typename  }  album {    cover {      small: urls(pictureRequest: {width: 100, height: 100})      medium: urls(pictureRequest: {width: 264, height: 264})      large: urls(pictureRequest: {width: 800, height: 800})      explicitStatus      __typename    }    __typename  }  __typename}fragment Lyrics on Lyrics {  id  copyright  text  writers  synchronizedLines {    ...LyricsSynchronizedLines    __typename  }  __typename}fragment LyricsSynchronizedLines on LyricsSynchronizedLine {  lrcTimestamp  line  lineTranslated  milliseconds  duration  __typename}";
        jObject["variables"] = trackId;

        requestData.CookieContainer = this._cookieContainer;
        
        requestData.AddContent(jObject.ToString());
        
        requestData.SetContentType(EnumContentType.APPLICATION_JSON);
            
        requestData.AddAuthMethod(new Auth(jwtToken.jwt, EnumAuthType.OAUTH2));

        Request request = new Request(requestData);
        ResponseData responseData = await request.GetResponseAsync();

        string response = responseData.GetContentAsString();

        if (response.Contains("JwtTokenExpiredError"))
            return Throw<object>(new DeezerException(EnumDeezerExceptionType.JwtExpired));
        
        return new JsonDeserializer<JsonDeezerLyricsResponse>().Deserialize(responseData.GetContentAsString());
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
        
        return new JsonDeserializer<JsonDeezerUserData>().Deserialize(rawUserData);
    }

    public async Task<string> GetUserDataRaw(int retries = 5)
    {
        RequestData requestData = new RequestData(
            $"{this._websiteEndpoint}/ajax/gw-light.php?method=deezer.getUserData&api_version=1.0&input=3&api_token=&cid={this.RandomCid}", 
            EnumRequestMethod.GET);
        
        requestData.CookieContainer = this._cookieContainer;
            
        Request request = new Request(requestData);
        ResponseData responseData = await request.GetResponseAsync();

        string content = Encoding.ASCII.GetString(responseData.Content);
            
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
        
        for (int i = 0; i < retries; i++)
        {
            RequestData requestData = new RequestData(
                $"{this._websiteEndpoint}/ajax/gw-light.php?method=deezer.pageTrack&api_version=1.0&input=3&api_token={csrfToken}", 
                EnumRequestMethod.POST);

            requestData.Timeout = TimeSpan.FromSeconds(10);
            
            JObject jObject = new JObject();
            jObject["sng_id"] = trackID;
            
            requestData.AddContent(jObject.ToString());
            requestData.SetContentType(EnumContentType.APPLICATION_JSON);
        
            requestData.CookieContainer = this._cookieContainer;
        
            Request request = new Request(requestData);
            ResponseData responseData = await request.GetResponseAsync();
            
            string content = responseData.GetContentAsString();

            if (content.Contains("Invalid CSRF token"))
                return Throw<object>(new DeezerException(EnumDeezerExceptionType.InvalidCsrfToken));

            if (content.Contains("Wrong parameters"))
                return Throw<object>(new DeezerException(EnumDeezerExceptionType.WrongParameter));
            
            if (!content.Contains("deprecated?method=deezer.pageTrack"))
                return new JsonDeserializer<JsonDeezerSongDetails>().Deserialize(content);
        }

        return null;
    }
    
    public async Task<JsonDeezerSongSource> GetSongUrls(string trackToken, string licenseToken)
    {
        RequestData requestData = new RequestData($"{this._mediaEndpoint}/v1/get_url", EnumRequestMethod.POST);
        
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
        
        requestData.AddContent(jObject.ToString());
        requestData.SetContentType(EnumContentType.APPLICATION_JSON);
        
        requestData.CookieContainer = this._cookieContainer;
        
        Request request = new Request(requestData);
        ResponseData responseData = await request.GetResponseAsync();
        
        return new JsonDeserializer<JsonDeezerSongSource>().Deserialize(responseData.GetContentAsString());
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

                    Request request = new Request(source.url);
                    ResponseData responseData = await request.GetResponseAsync();
                    
                    byte[] buffer = responseData.Content;
                    byte[] key = CalculateDecryptionKey(trackID);
                    
                    return DecryptSongData(buffer, key);
                }
            }
        }

        return null;
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
        RequestData requestData = new RequestData($"{this._apiEndpoint}/search?q={query}");
        
        Request request = new Request(requestData);
        ResponseData responseData = await request.GetResponseAsync();
        
        return new JsonDeserializer<JsonDeezerSearchResponse>().Deserialize(responseData.GetContentAsString());
    }
    
    public async Task<JsonDeezerSearchResponse> Search(string track = "", string artist = "", string album = "", bool strict = false)
    {
        return new JsonDeserializer<JsonDeezerSearchResponse>()
            .Deserialize(await SearchRaw(track, artist, album, strict));
    }

    private async Task<string> SearchRaw(string track = "", string artist = "", string album = "", bool strict = false)
    {
        string strictSearch = strict ? "?strict=on" : "";
        
        RequestData requestData = new RequestData(
            $"{this._apiEndpoint}/search?q=track:\"{track}\" artist:\"{artist}\" album:\"{album}\"{strictSearch}");
        
        Request request = new Request(requestData);
        ResponseData responseData = await request.GetResponseAsync();

        return responseData.GetContentAsString();
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
                return Throw<object>(new DeezerException(EnumDeezerExceptionType.MissingSongDetails));
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