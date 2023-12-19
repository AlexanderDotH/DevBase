using System.Net;
using System.Reflection.Metadata;
using System.Text;
using DevBase.Api.Apis.Deezer.Structure.Json;
using DevBase.Api.Apis.Deezer.Structure.Objects;
using DevBase.Api.Serializer;
using DevBase.Cryptography.Blowfish;
using DevBase.Enums;
using DevBase.Format;
using DevBase.Format.Formats.LrcFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.Web;
using DevBase.Web.RequestData;
using DevBase.Web.RequestData.Data;
using DevBase.Web.ResponseData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DevBase.Api.Apis.Deezer;

public class Deezer
{
    private readonly string _authEndpoint;
    private readonly string _apiEndpoint;
    private readonly string _pipeEndpoint;
    private readonly string _websiteEndpoint;
    private readonly string _mediaEndpoint;

    private CookieContainer _cookieContainer;

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
            throw new System.Exception("No arl token present");
        
        RequestData requestData = new RequestData(string.Format("{0}/login/arl?i=c&jo=p&rto=n", this._authEndpoint), EnumRequestMethod.POST);

        requestData.Header.Add("Accept", "*/*");
        requestData.Header.Add("Accept-Encoding", "gzip, deflate, br");
        
        requestData.CookieContainer = this._cookieContainer;
        
        requestData.SetContentType(EnumContentType.APPLICATION_JSON);
        
        ResponseData responseData = new Request(requestData).GetResponse();
        return new JsonDeserializer<JsonDeezerJwtToken>().Deserialize(responseData.GetContentAsString());
    }

    public async Task<JsonDeezerAuthTokenResponse> GetAccessToken(string appID = "457142")
    {
        RequestData requestData = new RequestData(string.Format("{0}/platform/generic/token/unlogged", this._apiEndpoint), EnumRequestMethod.POST);
        requestData.Header.Add("Accept", "*/*");

        AList<FormKeypair> formData = new AList<FormKeypair>();
        formData.Add(new FormKeypair("app_id", appID));
        
        requestData.AddFormData(formData);
        
        requestData.CookieContainer = this._cookieContainer;
        
        ResponseData responseData = await new Request(requestData).GetResponseAsync();

        string response = responseData.GetContentAsString();

        if (response.Contains("unable to get unlogged token for this app"))
            throw new System.Exception("Invalid AppID");
        
        return new JsonDeserializer<JsonDeezerAuthTokenResponse>().Deserialize(response);
    }
    
    public async Task<JsonDeezerAuthTokenResponse> GetAccessToken(string sessionID, string appID = "457142")
    {
        RequestData requestData = new RequestData(string.Format("{0}/platform/generic/token/create-from-session", this._apiEndpoint), EnumRequestMethod.POST);
        requestData.Header.Add("Accept", "*/*");

        AList<FormKeypair> formData = new AList<FormKeypair>();
        formData.Add(new FormKeypair("app_id", appID));
        formData.Add(new FormKeypair("sid", sessionID));
        
        requestData.AddFormData(formData);
        
        requestData.CookieContainer = this._cookieContainer;
        
        ResponseData responseData = await new Request(requestData).GetResponseAsync();

        string response = responseData.GetContentAsString();

        if (response.Contains("Internal Server Error"))
            throw new System.Exception("AppID and SessionID mismatch");
        
        if (response.Contains("unable to get unlogged token for this app"))
            throw new System.Exception("Invalid AppID");

        if (response.Contains("No session found"))
            throw new System.Exception("Invalid SessionID");
        
        return new JsonDeserializer<JsonDeezerAuthTokenResponse>().Deserialize(response);
    }

    public async Task<string> GetArlTokenFromSession(string sessionID)
    {
        RequestData requestData = new RequestData(string.Format("{0}/ajax/gw-light.php?method=user.getArl&input=3&api_version=1.0&api_token=null", this._websiteEndpoint), 
            EnumRequestMethod.GET);

        CookieContainer cookieContainer = new CookieContainer();
        cookieContainer.Add(new Cookie("sid", sessionID, "/", "deezer.com"));

        requestData.CookieContainer = cookieContainer;
        
        Request request = new Request(requestData);
        ResponseData responseData = await request.GetResponseAsync();

        string response = responseData.GetContentAsString();

        if (response.Contains("Require user auth"))
            throw new System.Exception("Invalid SessionID");

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
        JsonDeezerUserData userData = await this.GetUserData();

        RequestData requestData = new RequestData(
            string.Format("{0}/ajax/gw-light.php?method=song.getLyrics&api_version=1.0&input=3&api_token={1}&cid={2}",
                this._websiteEndpoint, userData.results.checkForm, this.RandomCid),
            EnumRequestMethod.POST);

        JObject jsonTrackID = new JObject();
        jsonTrackID["sng_id"] = trackID;

        requestData.AddContent(jsonTrackID.ToString());
        requestData.SetContentType(EnumContentType.APPLICATION_JSON);

        requestData.CookieContainer = this._cookieContainer;
        
        Request request = new Request(requestData);
        ResponseData responseData = await request.GetResponseAsync();

        string response = responseData.GetContentAsString();

        if (response.Contains("Invalid CSRF token"))
            throw new System.Exception("Invalid CSRF token");
        
        return new JsonDeserializer<JsonDeezerRawLyricsResponse>().Deserialize(response);
    }
    
    public async Task<JsonDeezerLyricsResponse> GetLyricsGraph(string trackID)
    {
        JsonDeezerJwtToken jwtToken = await this.GetJwtToken();
        
        RequestData requestData = new RequestData(string.Format("{0}/api", this._pipeEndpoint), EnumRequestMethod.POST);

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
            throw new System.Exception("The Jwt token is expired");
        
        return new JsonDeserializer<JsonDeezerLyricsResponse>().Deserialize(responseData.GetContentAsString());
    }
    
    public async Task<JsonDeezerUserData> GetUserData(int retries = 5)
    {
        for (int i = 0; i < retries; i++)
        {
            RequestData requestData = new RequestData(
                string.Format("{0}/ajax/gw-light.php?method=deezer.getUserData&api_version=1.0&input=3&api_token=&cid={1}", this._websiteEndpoint, this.RandomCid), 
                EnumRequestMethod.GET);
        
            requestData.CookieContainer = this._cookieContainer;
            
            Request request = new Request(requestData);
            ResponseData responseData = await request.GetResponseAsync();

            string content = responseData.GetContentAsString();
            
            if (!content.Contains("deprecated?method=deezer.getUserData"))
                return new JsonDeserializer<JsonDeezerUserData>().Deserialize(content);
        }

        return null;
    }

    public async Task<DeezerTrack> GetSong(string trackID)
    {
        JsonDeezerUserData userData = await this.GetUserData();

        if (userData == null)
            throw new System.Exception("No CSRF provided");
        
        JsonDeezerSongDetails songDetails = await this.GetSongDetails(trackID, userData.results.checkForm);

        if (songDetails.results == null)
            throw new System.Exception("Cannot find song details");
        
        List<string> artists = new List<string>();
        songDetails.results.DATA.ARTISTS.ForEach(a => artists.Add(a.ART_NAME));
        
        int durationS = Convert.ToInt32(songDetails.results.DATA.DURATION);

        List<string> artworkUrls = new List<string>();
        artworkUrls.Add(string.Format("https://e-cdns-images.dzcdn.net/images/cover/{0}/56x56-000000-80-0-0.jpg", songDetails.results.DATA.ALB_PICTURE));
        artworkUrls.Add(string.Format("https://e-cdns-images.dzcdn.net/images/cover/{0}/250x250-000000-80-0-0.jpg", songDetails.results.DATA.ALB_PICTURE));
        artworkUrls.Add(string.Format("https://e-cdns-images.dzcdn.net/images/cover/{0}/500x500-000000-80-0-0.jpg", songDetails.results.DATA.ALB_PICTURE));
        artworkUrls.Add(string.Format("https://e-cdns-images.dzcdn.net/images/cover/{0}/1000x1000-000000-80-0-0.jpg", songDetails.results.DATA.ALB_PICTURE));

        DeezerTrack track = new DeezerTrack()
        {
            Title = songDetails.results.DATA.SNG_TITLE,
            Album = songDetails.results.DATA.ALB_TITLE,
            Duration = (int)TimeSpan.FromSeconds(durationS).TotalMilliseconds,
            Artists = artists.ToArray(),
            ArtworkUrls = artworkUrls.ToArray(),
            ServiceInternalId = songDetails.results.DATA.SNG_ID
        };

        return track;
    }
    
    public async Task<JsonDeezerSongDetails> GetSongDetails(string trackID, string apiToken, int retries = 5)
    {
        for (int i = 0; i < retries; i++)
        {
            RequestData requestData = new RequestData(string.Format("{0}/ajax/gw-light.php?method=deezer.pageTrack&api_version=1.0&input=3&api_token={1}", 
                this._websiteEndpoint, 
                apiToken), EnumRequestMethod.POST);

            JObject jObject = new JObject();
            jObject["sng_id"] = trackID;
            
            requestData.AddContent(jObject.ToString());
            requestData.SetContentType(EnumContentType.TEXT_PLAIN);
        
            requestData.CookieContainer = this._cookieContainer;
        
            Request request = new Request(requestData);
            ResponseData responseData = await request.GetResponseAsync();
            
            string content = responseData.GetContentAsString();

            if (content.Contains("Invalid CSRF token"))
                throw new System.Exception("Invalid CSRF token provided");
            
            if (!content.Contains("deprecated?method=deezer.pageTrack"))
                return new JsonDeserializer<JsonDeezerSongDetails>().Deserialize(content);
        }

        return null;
    }
    
    public async Task<JsonDeezerSongSource> GetSongUrls(string trackToken, string licenseToken)
    {
        RequestData requestData = new RequestData(string.Format("{0}/v1/get_url", 
            this._mediaEndpoint), EnumRequestMethod.POST);
        
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

    public async Task<byte[]> DownloadSong(string trackID)
    {
        JsonDeezerUserData userData = await this.GetUserData();

        if (userData == null)
            return null;
        
        JsonDeezerSongDetails songDetails = await this.GetSongDetails(trackID, userData.results.checkForm);

        if (songDetails == null)
            return null;
        
        JsonDeezerSongSource url = await this.GetSongUrls(songDetails.results.DATA.TRACK_TOKEN, userData.results.USER.OPTIONS.license_token);

        if (url == null)
            return null;

        if (url.data.Count == 0)
            return null;

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

    private byte[] CalculateDecryptionKey(string trackID)
    {
        string secret = "g4el58wc0zvf9na1";
        
        string hash = DevBase.Cryptography.MD5.MD5.ToMD5String(trackID);

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
        RequestData requestData = new RequestData(string.Format("{0}/search?q={1}", this._apiEndpoint, query));
        
        Request request = new Request(requestData);
        ResponseData responseData = await request.GetResponseAsync();
        
        return new JsonDeserializer<JsonDeezerSearchResponse>().Deserialize(responseData.GetContentAsString());
    }
    
    public async Task<JsonDeezerSearchResponse> Search(string track = "", string artist = "", string album = "", bool strict = false)
    {
        RequestData requestData = new RequestData(
            string.Format("{0}/search?q=track:\"{1}\" artist:\"{2}\" album:\"{3}\"{4}", this._apiEndpoint, track, artist, album, strict ? "?strict=on" : ""));
        
        Request request = new Request(requestData);
        ResponseData responseData = await request.GetResponseAsync();
        
        return new JsonDeserializer<JsonDeezerSearchResponse>().Deserialize(responseData.GetContentAsString());
    }

    private async Task<(string rawLyrics, AList<TimeStampedLyric> syncedElements)> GetLyricsGraphAndParse(string trackID)
    {
        string rawText = string.Empty;
        
        JsonDeezerLyricsResponse lyricsResponse = await GetLyricsGraph(trackID);

        if (lyricsResponse.data.track.lyrics == null)
            throw new System.Exception("Lyrics not found");

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

            lrcFile.AppendLine(string.Format("{0} {1}", synchronizedLine.lrcTimestamp, synchronizedLine.line));
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

        if (lyricsResponse.results == null)
            throw new System.Exception("Lyrics not found");

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
                
            lrcFile.AppendLine(string.Format("{0} {1}", lyricsLine.lrc_timestamp, lyricsLine.line));
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