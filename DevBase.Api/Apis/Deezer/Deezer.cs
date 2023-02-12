using System.Net;
using System.Text;
using DevBase.Api.Apis.Deezer.Structure.Json;
using DevBase.Api.Serializer;
using DevBase.Cryptography.Blowfish;
using DevBase.Enums;
using DevBase.Web;
using DevBase.Web.RequestData;
using DevBase.Web.RequestData.Data;
using DevBase.Web.ResponseData;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer;

public class Deezer
{
    private readonly string _authEndpoint;
    private readonly string _apiEndpoint;
    private readonly string _pipeEndpoint;
    private readonly string _websiteEndpoint;
    private readonly string _mediaEndpoint;

    private CookieContainer _cookieContainer;

    public Deezer()
    {
        this._authEndpoint = "https://auth.deezer.com";
        this._apiEndpoint = "https://api.deezer.com";
        this._pipeEndpoint = "https://pipe.deezer.com";
        this._websiteEndpoint = "https://www.deezer.com";
        this._mediaEndpoint = "https://media.deezer.com";

        this._cookieContainer = new CookieContainer();
    }

    public async Task<JsonDeezerJwtToken> GetJwtToken()
    {
        RequestData requestData = new RequestData(string.Format("{0}/login/arl?i=c&jo=p&rto=n", this._authEndpoint), EnumRequestMethod.POST);

        requestData.Header.Add("Accept", "*/*");
        requestData.Header.Add("Accept-Encoding", "gzip, deflate, br");
        requestData.Header.Add("Cookie", "dzr_uniq_id=dzr_uniq_id_frbc23e096510b536bbd344475e7c61c87c05d94; _abck=CF1B0048C5324B20771F59D3F9E18B4B~0~YAAQn9AXAqVAFVuFAQAAvQGvpwnwTyfZG6KLfAr0OC19zlmJ8TudSo1kTpXETra15CK/sV78AR0pSs/rHqgwaMMvEa687zsiEzqQlM9/EVObBlT33f9UzKEg7JuVtE2KFL487gT5akMFdBwY+igP9ys7Ly+x2uC4gBL2w4nCjwOOXDerebdv+8SlvDvtDYSGZ1O+gMVUpKAz7V96JSb937MlYaxdigyL7+63WPfehArj/pNH0sb9GOePB97g6Czxua5BFi0okJNJOEY97lRe3QNmxAgZzb/CqBh2oRNmpT4v0MNvqYe4vclHS/rVW4Bbp/cMvj7t4rF+4YPVurq8tfPltK12A1/6BBBSpRUuU+WMDGcUxsSoRP8aZyW4oZ6ex0U1nTrm7SKfsamxdvWViag0LwChcHknBA==~-1~-1~-1; arl=447cae8221e4ba86cf8b479dae0d93fc3e70addfb4aed8a74cef16a799bfef493f22594d7118f34bbf65be490f32c0e77053bc323558d9b74e450b293174004539cfe43f6efb584dbdd59d038b6d0c803cc8db2fbe911290bdd0f0f2a80fbf89; comeback=1; ab.storage.deviceId.5ba97124-1b79-4acc-86b7-9547bc58cb18=%7B%22g%22%3A%22535ae124-06cc-5d7c-4f55-a40b08e12a2f%22%2C%22c%22%3A1662576184591%2C%22l%22%3A1673556074398%7D; ab.storage.userId.5ba97124-1b79-4acc-86b7-9547bc58cb18=%7B%22g%22%3A%225111209242%22%2C%22c%22%3A1662576184595%2C%22l%22%3A1673556074399%7D; ab.storage.sessionId.5ba97124-1b79-4acc-86b7-9547bc58cb18=%7B%22g%22%3A%229f53fd13-ba8b-90e5-a22a-f7ae771c54f9%22%2C%22e%22%3A1673559265157%2C%22c%22%3A1673556074398%2C%22l%22%3A1673557465157%7D; ak_bmsc=35D9F773CDA29199DE589EC86AC9E1DA~000000000000000000000000000000~YAAQn9AXAqZAFVuFAQAAvQGvpxLj2QJohYhosRko30MNWlpT7BpIbyT4DoojphXyyYKIcCkxZQmhicfwvwO4I40ZAan9iw51oZkC/F2cWmZ796nL/L0FIvKLkYvQvE4wsuKzCwwTYeSrMSZY5v40JHL+rrQsAF+GNPUTVj4aINqLrl3DBz84jja18Y6U0W1X/k4x1t1jo7WFOn7rVz8AMLAIS1GNw1csHoMrSniwbcj6mcKnSQk/yAp3fB9yuMcP0ne7SUonswF8AqfG58BiuP84tDYqHS8pQLqr0lDntvmQzjub2+Ge0DGSFzPN6lTAidM2+X5tf7726yQJTIdbeD/T4WjxksdGREeFHbYSZgvHL+8n59kb4VNkJPsu+4aiIg6GflzCNKWDhQ==; bm_sz=57CA358B5373057BF707E0AF3E125849~YAAQn9AXAqdAFVuFAQAAvQGvpxIqG0C29+2L+tsuEFeWjd+QBSOeinEqnN4n9A5ebZyQyIVeb5w0AdcP9jx3ch2KF1nNGuVEG9rZNopVgjjmyI7tPJAaVeiMSR1a+yJkl9gJh+fIz14ruNzDgefsVWCf9pCdQmbzF1Evzm/IsoJxZS/avzFWlz6AwPgjmbc05XBNgcSkL9m3arkh/CiMgCxN1y6J8V9tQMj2gZRwaaeK2VS0iLX4e1AS8GdL/2m2QzgdmVefnRfPiQLYi0rOxiIXDbnGuQdBe02VOjx9rQD2ipA=~3621957~3359812; bm_sv=831A64C0499197289F12B77E5602E689~YAAQiNAXAoglcCGFAQAAOAjjpxJSp+p8+ytP2d7xKottbNoKdCXCGp9mRrAeQSSTDd+nH/OWAG09Ox3NbXXQO05PpVpeBYqMe/rN7QEJxD8buiXJAvBViwAu88GvPAmJacJAJM1jaVYZbftV9SNZNZOqHGEsKgC4s7H6+AX8y6dY+hqAKnCa+v67ANO1ItrFoHZ2DOcyuP7xkohkPWZv0J2ZIW5sxV8N2eM/UDlgvG9YmOU48wN421ZQW1Ifal8lXA==~1; sid=frf0a902aabfd798fda01ac31052590d5033170d; bm_mi=777177B1A96261681750DF8015BBCB0A~YAAQiNAXAkvhbCGFAQAAyk3MpxKhX6Bq+BSjJ+xJncyX/CJbtG9Xx41ep36zPZhLlhTnWP9A61ZSl/Kn2pFQ0+7V5ngfXtVt2EBN+AcQnRjPbAhPo+EeEEccHRbIl3PRrU7bEH/w6wMNE8Ry1gK7IDAImPCaB6VMgQgSrPoAm2BWE93QN2uh2rc4OSzBQOIKstNmMoZl4tCiYh6kcSRKcc4Ybjd5NcBfpjEuYc11Je7oDb9e3PnCTK7pjoPbZFqLS+fFREF5iPBelxV65Zpz2a8sayxFVAm0VDiQv0noA7eN662eDCs7+A6iZaLCqWha94XiTkMh2gYv2GWNYxyBpDA3IfFH~1; consentStatistics=1; consentMarketing=1");

        requestData.CookieContainer = this._cookieContainer;
        
        requestData.SetContentType(EnumContentType.APPLICATION_JSON);
        
        ResponseData responseData = await new Request(requestData).GetResponseAsync();
        return JsonConvert.DeserializeObject<JsonDeezerJwtToken>(responseData.GetContentAsString());
    }

    public async Task<JsonDeezerLyricsResponse> GetLyrics(string trackID)
    {
        JsonDeezerJwtToken jwtToken = await this.GetJwtToken();
        
        RequestData requestData = new RequestData(string.Format("{0}/api", this._pipeEndpoint), EnumRequestMethod.POST);

        string data = @"{
            ""query"": ""query SynchronizedTrackLyrics($trackId: String!) {  track(trackId: $trackId) {    ...SynchronizedTrackLyrics    __typename  }}fragment SynchronizedTrackLyrics on Track {  id  lyrics {    ...Lyrics    __typename  }  album {    cover {      small: urls(pictureRequest: {width: 100, height: 100})      medium: urls(pictureRequest: {width: 264, height: 264})      large: urls(pictureRequest: {width: 800, height: 800})      explicitStatus      __typename    }    __typename  }  __typename}fragment Lyrics on Lyrics {  id  copyright  text  writers  synchronizedLines {    ...LyricsSynchronizedLines    __typename  }  __typename}fragment LyricsSynchronizedLines on LyricsSynchronizedLine {  lrcTimestamp  line  lineTranslated  milliseconds  duration  __typename}"",
            ""variables"": {
                ""trackId"": ""1762856547""
            }
        }".Replace("1762856547", trackID);
        
        requestData.CookieContainer = this._cookieContainer;
        
        requestData.AddContent(data);
        
        requestData.SetContentType(EnumContentType.APPLICATION_JSON);
            
        requestData.AddAuthMethod(new Auth(jwtToken.jwt, EnumAuthType.OAUTH2));

        Request request = new Request(requestData);
        ResponseData responseData = await request.GetResponseAsync();
        return new JsonDeserializer<JsonDeezerLyricsResponse>().Deserialize(responseData.GetContentAsString());
    }
    
    public async Task<JsonDeezerUserData> GetUserData(int retries = 5)
    {
        for (int i = 0; i < retries; i++)
        {
            RequestData requestData = new RequestData(string.Format("{0}/ajax/gw-light.php?method=deezer.getUserData&api_version=1.0&input=3&api_token=&cid={1}", 
                this._websiteEndpoint, 
                new Random().Next(100000000, 999999999)), EnumRequestMethod.GET);
        
            requestData.CookieContainer = this._cookieContainer;
            
            Request request = new Request(requestData);
            ResponseData responseData = await request.GetResponseAsync();

            string content = responseData.GetContentAsString();
            
            if (!content.Contains("deprecated?method=deezer.getUserData"))
                return new JsonDeserializer<JsonDeezerUserData>().Deserialize(content);
        }

        return null;
    }
    
    public async Task<JsonDeezerSongDetails> GetSongDetails(string trackID, string apiToken, int retries = 5)
    {
        for (int i = 0; i < retries; i++)
        {
            RequestData requestData = new RequestData(string.Format("{0}/ajax/gw-light.php?method=deezer.pageTrack&api_version=1.0&input=3&api_token={1}", 
                this._websiteEndpoint, 
                apiToken), EnumRequestMethod.POST);

            string data = "{\r\n    \"sng_id\" : \"1424522622\"\r\n}".Replace("1424522622", trackID);
        
            requestData.AddContent(data);
            requestData.SetContentType(EnumContentType.TEXT_PLAIN);
        
            requestData.CookieContainer = this._cookieContainer;
        
            Request request = new Request(requestData);
            ResponseData responseData = await request.GetResponseAsync();
            
            string content = responseData.GetContentAsString();
            
            if (!content.Contains("deprecated?method=deezer.pageTrack"))
                return new JsonDeserializer<JsonDeezerSongDetails>().Deserialize(content);
        }

        return null;
    }
    
    public async Task<JsonDeezerSongSource> GetSongUrls(string trackToken, string licenseToken)
    {
        RequestData requestData = new RequestData(string.Format("{0}/v1/get_url", 
            this._mediaEndpoint), EnumRequestMethod.POST);

        string data = "{\r\n  \"license_token\": \"(licenseToken)\",\r\n  \"media\": [\r\n    {\r\n      \"type\": \"FULL\",\r\n      \"formats\": [\r\n        {\r\n          \"cipher\": \"BF_CBC_STRIPE\",\r\n          \"format\": \"MP3_128\"\r\n        },\r\n        {\r\n          \"cipher\": \"BF_CBC_STRIPE\",\r\n          \"format\": \"MP3_64\"\r\n        },\r\n        {\r\n          \"cipher\": \"BF_CBC_STRIPE\",\r\n          \"format\": \"MP3_MISC\"\r\n        }\r\n      ]\r\n    }\r\n  ],\r\n  \"track_tokens\": [\r\n    \"(trackToken)\"\r\n  ]\r\n}"
            .Replace("(licenseToken)", licenseToken)
            .Replace("(trackToken)", trackToken);
        
        requestData.AddContent(data);
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
        List<byte> decryptedContent = new List<byte>();

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

        return decryptedContent.ToArray();
    }
    
    public async Task<JsonDeezerSearchResponse> Search(string query)
    {
        RequestData requestData = new RequestData(string.Format("{0}/search?q={1}", this._apiEndpoint, query));
        Request request = new Request(requestData);
        ResponseData responseData = await request.GetResponseAsync();
        return JsonConvert.DeserializeObject<JsonDeezerSearchResponse>(responseData.GetContentAsString());
    }

}