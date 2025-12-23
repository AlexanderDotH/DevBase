using System.Net;
using System.Text;
using DevBase.Api.Apis.Tidal.Structure.Json;
using DevBase.Api.Enums;
using DevBase.Api.Exceptions;
using DevBase.Api.Serializer;
using DevBase.Enums;
using DevBase.Generics;
using DevBase.Web;
using DevBase.Web.RequestData;
using DevBase.Web.RequestData.Data;
using DevBase.Web.ResponseData;

namespace DevBase.Api.Apis.Tidal
{
    public class Tidal : ApiClient
    {
        private readonly string _authEndpoint;
        private readonly string _apiEndpoint;

        private readonly string _clientId;
        private readonly string _clientSecret;

        private readonly FormKeypair _clientIdKeyPair;
        private readonly FormKeypair _scopeKeyPair;

        public Tidal()
        {
            this._authEndpoint = "https://auth.tidal.com/v1";
            this._apiEndpoint = "https://api.tidal.com/v1";

            this._clientId = "zU4XHVVkc2tDPo4t";
            this._clientSecret = "VJKhDFqJPqvsPVNBV6ukXTJmwlvbttP7wlMlrc72se4=";

            this._clientIdKeyPair = new FormKeypair("client_id", this._clientId);
            this._scopeKeyPair = new FormKeypair("scope", "r_usr+w_usr+w_sub");
        }

        public async Task<JsonTidalAuthDevice> RegisterDevice()
        {
            AList<FormKeypair> formData = new AList<FormKeypair>();
            formData.Add(this._clientIdKeyPair);
            formData.Add(this._scopeKeyPair);

            RequestData requestData = new RequestData(new Uri($"{this._authEndpoint}/oauth2/device_authorization"),
                EnumRequestMethod.POST,
                EnumContentType.APPLICATION_FORM_URLENCODED);
            
            requestData.AddFormData(formData);

            string authToken = Convert.ToBase64String(Encoding.Default.GetBytes($"{this._clientId}:{this._clientSecret}"));
            requestData.AddAuthMethod(new Auth(authToken, EnumAuthType.BASIC));

            Request request = new Request(requestData);

            ResponseData response = await request.GetResponseAsync();

            return new JsonDeserializer<JsonTidalAuthDevice>().Deserialize(response.GetContentAsString());
        }

        public async Task<JsonTidalAccountAccess> GetTokenFrom(string deviceCode)
        {
            AList<FormKeypair> formData = new AList<FormKeypair>();
            formData.Add(this._clientIdKeyPair);
            formData.Add(new FormKeypair("device_code", deviceCode));
            formData.Add(new FormKeypair("grant_type", "urn:ietf:params:oauth:grant-type:device_code"));
            formData.Add(this._scopeKeyPair);

            RequestData requestData = new RequestData(new Uri($"{this._authEndpoint}/oauth2/token"),
                EnumRequestMethod.POST,
                EnumContentType.APPLICATION_FORM_URLENCODED);
            
            requestData.AddFormData(formData);

            try
            {
                Request request = new Request(requestData);
                ResponseData response = await request.GetResponseAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));

                if (response.GetContentAsString().Contains("authorization_pending"))
                    return Throw<object>(new TidalException(EnumTidalExceptionType.AuthorizationPending));

                return new JsonDeserializer<JsonTidalAccountAccess>().Deserialize(response.GetContentAsString());
            }
            catch
            {
                return Throw<object>(new TidalException(EnumTidalExceptionType.ParsingError));
            }
        }

        public async Task<JsonTidalSession> Login(string accessToken)
        {
            RequestData requestData = new RequestData(
                new Uri($"{this._apiEndpoint}/sessions"), 
                EnumRequestMethod.GET, 
                EnumContentType.APPLICATION_JSON, 
                RequestData.GetRandomUseragent());
            
            requestData.Header.Add("Authorization", "Bearer " + accessToken);

            try
            {
                Request request = new Request(requestData);
                ResponseData response = await request.GetResponseAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));

                return new JsonDeserializer<JsonTidalSession>().Deserialize(response.GetContentAsString());
            }
            catch
            {
                return Throw<object>(new TidalException(EnumTidalExceptionType.ParsingError));
            }
        }

        public async Task<JsonTidalSearchResult> Search(string query, string countryCode = "AS", int limit = 10)
        {
            RequestData requestData = new RequestData(
                $"{this._apiEndpoint}/search/tracks?countryCode={countryCode}&query={query}&limit={limit}");
            
            requestData.Header.Add("x-tidal-token", this._clientId);

            try
            {
                Request request = new Request(requestData);
                ResponseData response = await request.GetResponseAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));

                return new JsonDeserializer<JsonTidalSearchResult>().Deserialize(response.GetContentAsString());
            }
            catch
            {
                return Throw<object>(new TidalException(EnumTidalExceptionType.ParsingError));
            }
        }

        public async Task<JsonTidalAuthAccess> AuthTokenToAccess(string authToken)
        {
            AList<FormKeypair> formData = new AList<FormKeypair>();
            formData.Add(this._clientIdKeyPair);
            formData.Add(new FormKeypair("user_auth_token", authToken));
            formData.Add(new FormKeypair("grant_type", "user_auth_token"));
            formData.Add(this._scopeKeyPair);
            
            RequestData requestData = new RequestData(
                new Uri($"{this._authEndpoint}/oauth2/token"),
                EnumRequestMethod.POST, 
                EnumContentType.APPLICATION_FORM_URLENCODED);
            
            requestData.AddFormData(formData);

            requestData.Accept = "*/*";

            try
            {
                ResponseData response = await new Request(requestData).GetResponseAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));
                
                return new JsonDeserializer<JsonTidalAuthAccess>().Deserialize(response.GetContentAsString());
            }
            catch
            {
                return Throw<object>(new TidalException(EnumTidalExceptionType.ParsingError));
            }
        }

        public async Task<JsonTidalAccountRefreshAccess> RefreshToken(string refreshToken)
        {
            AList<FormKeypair> formData = new AList<FormKeypair>();
            formData.Add(this._clientIdKeyPair);
            formData.Add(new FormKeypair("refresh_token", refreshToken));
            formData.Add(new FormKeypair("grant_type", "refresh_token"));
            formData.Add(this._scopeKeyPair);

            RequestData requestData = new RequestData(
                new Uri($"{this._authEndpoint}/oauth2/token"),
                EnumRequestMethod.POST, 
                EnumContentType.APPLICATION_FORM_URLENCODED);
            
            requestData.AddFormData(formData);

            string authToken = Convert.ToBase64String(Encoding.Default.GetBytes(this._clientId + ":" + this._clientSecret));
            requestData.AddAuthMethod(new Auth(authToken, EnumAuthType.BASIC));

            try
            {
                ResponseData response = await new Request(requestData).GetResponseAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));

                if (response.GetContentAsString().Contains("authorization_pending"))
                    return Throw<object>(new TidalException(EnumTidalExceptionType.AuthorizationPending));

                return new JsonDeserializer<JsonTidalAccountRefreshAccess>().Deserialize(response.GetContentAsString());
            }
            catch
            {
                return Throw<object>(new TidalException(EnumTidalExceptionType.ParsingError));
            }
        }

        public async Task<JsonTidalLyricsResult> GetLyrics(string accessToken, string trackId, string countryCode = "US")
        {
            RequestData requestData = new RequestData($"{this._authEndpoint}/tracks/{trackId}/lyrics?countryCode={countryCode}");
            
            requestData.AddAuthMethod(new Auth(accessToken, EnumAuthType.OAUTH2));

            try
            {
                ResponseData response = await new Request(requestData).GetResponseAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));

                return new JsonDeserializer<JsonTidalLyricsResult>().Deserialize(response.GetContentAsString());
            }
            catch
            {
                return Throw<object>(new TidalException(EnumTidalExceptionType.ParsingError));
            }
        }
        
        public async Task<JsonTidalDownloadResult> DownloadSong(string accessToken, string trackId, string soundQuality = "LOW")
        {
            RequestData requestData = new RequestData($"{this._authEndpoint}/tracks/{trackId}/streamUrl?soundQuality={soundQuality}");
            
            requestData.AddAuthMethod(new Auth(accessToken, EnumAuthType.OAUTH2));

            try
            {
                ResponseData response = await new Request(requestData).GetResponseAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));

                return new JsonDeserializer<JsonTidalDownloadResult>().Deserialize(response.GetContentAsString());
            }
            catch
            {
                return Throw<object>(new TidalException(EnumTidalExceptionType.ParsingError));
            }
        }
        
        public async Task<byte[]> DownloadSongData(string accessToken, string trackId, string soundQuality = "LOW")
        {
            JsonTidalDownloadResult result = await this.DownloadSong(accessToken, trackId, soundQuality);

            if (result?.url == null || result.url.Length == 0)
                return new byte[0];

            Request request = new Request(result.url);
            ResponseData response = await request.GetResponseAsync(false);

            if (response.StatusCode != HttpStatusCode.OK)
                return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));
            
            return response.Content;
        }
    }
}
