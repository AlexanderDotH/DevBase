using System.Net;
using System.Reflection;
using System.Text;
using DevBase.Api.Apis.Tidal.Structure.Json;
using DevBase.Api.Serializer;
using DevBase.Enums;
using DevBase.Generics;
using DevBase.Web;
using DevBase.Web.RequestData;
using DevBase.Web.RequestData.Data;
using DevBase.Web.ResponseData;

namespace DevBase.Api.Apis.Tidal
{
    public class Tidal
    {
        private readonly string _authEndpoint;
        private readonly string _apiEndpoint;

        private readonly string _clientID;
        private readonly string _clientSecret;

        public Tidal()
        {
            this._authEndpoint = "https://auth.tidal.com/v1";
            this._apiEndpoint = "https://api.tidal.com/v1";

            this._clientID = "zU4XHVVkc2tDPo4t";
            this._clientSecret = "VJKhDFqJPqvsPVNBV6ukXTJmwlvbttP7wlMlrc72se4=";
        }

        public async Task<JsonTidalAuthDevice> RegisterDevice()
        {
            AList<FormKeypair> formData = new AList<FormKeypair>();
            formData.Add(new FormKeypair("client_id", this._clientID));
            formData.Add(new FormKeypair("scope", "r_usr+w_usr+w_sub"));

            RequestData requestData = new RequestData(new Uri(string.Format("{0}/oauth2/device_authorization", this._authEndpoint)),
                EnumRequestMethod.POST,
                EnumContentType.APPLICATION_FORM_URLENCODED);
            
            requestData.AddFormData(formData);

            string authToken = Convert.ToBase64String(Encoding.Default.GetBytes(this._clientID + ":" + this._clientSecret));
            requestData.AddAuthMethod(new Auth(authToken, EnumAuthType.BASIC));

            Request request = new Request(requestData);

            ResponseData response = await request.GetResponseAsync();

            return new JsonDeserializer<JsonTidalAuthDevice>().Deserialize(response.GetContentAsString());
        }

        public async Task<JsonTidalAccountAccess> GetTokenFrom(string deviceCode)
        {
            AList<FormKeypair> formData = new AList<FormKeypair>();
            formData.Add(new FormKeypair("client_id", this._clientID));
            formData.Add(new FormKeypair("device_code", deviceCode));
            formData.Add(new FormKeypair("grant_type", "urn:ietf:params:oauth:grant-type:device_code"));
            formData.Add(new FormKeypair("scope", "r_usr+w_usr+w_sub"));

            RequestData requestData = new RequestData(new Uri(string.Format("{0}/oauth2/token", this._authEndpoint)),
                EnumRequestMethod.POST,
                EnumContentType.APPLICATION_FORM_URLENCODED);
            
            requestData.AddFormData(formData);

            try
            {
                Request request = new Request(requestData);
                ResponseData response = await request.GetResponseAsync();

                if (response.StatusCode == HttpStatusCode.NoContent)
                    return null;

                if (response.StatusCode != HttpStatusCode.OK)
                    return null;

                if (response.GetContentAsString().Contains("authorization_pending"))
                    return null;

                JsonTidalAccountAccess accountAccess =
                    new JsonDeserializer<JsonTidalAccountAccess>().Deserialize(response.GetContentAsString());

                if (accountAccess == null)
                    return null;

                return accountAccess;
            }
            catch (System.Exception e) { }

            return null;
        }

        public async Task<JsonTidalSession> Login(string accessToken)
        {
            RequestData requestData = new RequestData(new Uri(string.Format("{0}/sessions", this._apiEndpoint)), EnumRequestMethod.GET, 
                EnumContentType.APPLICATION_JSON, RequestData.GetRandomUseragent());
            
            requestData.Header.Add("Authorization", "Bearer " + accessToken);

            try
            {
                Request request = new Request(requestData);
                ResponseData response = await request.GetResponseAsync();

                if (response.StatusCode == HttpStatusCode.NoContent)
                    return null;

                if (response.StatusCode != HttpStatusCode.OK)
                    return null;

                return new JsonDeserializer<JsonTidalSession>().Deserialize(response.GetContentAsString());

            }
            catch (System.Exception e) { }

            return null;
        }

        public async Task<JsonTidalSearchResult> Search(string query, string countryCode = "AS", int limit = 10)
        {
            RequestData requestData = new RequestData(
                string.Format("{0}/search/tracks?countryCode={1}&query={2}&limit={3}", this._apiEndpoint, countryCode, query, limit));
            
            requestData.Header.Add("x-tidal-token", this._clientID);

            try
            {
                Request request = new Request(requestData);
                ResponseData response = await request.GetResponseAsync();

                if (response.StatusCode == HttpStatusCode.NoContent)
                    return null;
                 
                if (response.StatusCode != HttpStatusCode.OK)
                    return null;

                return new JsonDeserializer<JsonTidalSearchResult>().Deserialize(response.GetContentAsString());

            }
            catch (System.Exception e) { }

            return null;
        }

        public async Task<JsonTidalAuthAccess> AuthTokenToAccess(string authToken)
        {
            AList<FormKeypair> formData = new AList<FormKeypair>();
            formData.Add(new FormKeypair("client_id", this._clientID));
            formData.Add(new FormKeypair("user_auth_token", authToken));
            formData.Add(new FormKeypair("grant_type", "user_auth_token"));
            formData.Add(new FormKeypair("scope", "r_usr+w_usr+w_sub"));
            
            RequestData requestData = new RequestData(new Uri(string.Format("{0}/oauth2/token", this._authEndpoint)),
                EnumRequestMethod.POST, EnumContentType.APPLICATION_FORM_URLENCODED);
            
            requestData.AddFormData(formData);

            requestData.Accept = "*/*";

            JsonTidalAuthAccess accountAccess = null;

            try
            {
                ResponseData response = await new Request(requestData).GetResponseAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return null;

                accountAccess =
                    new JsonDeserializer<JsonTidalAuthAccess>().Deserialize(response.GetContentAsString());

                if (accountAccess == null)
                    return null;
            }
            catch (System.Exception e) { }

            return accountAccess;
        }

        public async Task<JsonTidalAccountRefreshAccess> RefreshToken(string refreshToken)
        {
            AList<FormKeypair> formData = new AList<FormKeypair>();
            formData.Add(new FormKeypair("client_id", this._clientID));
            formData.Add(new FormKeypair("refresh_token", refreshToken));
            formData.Add(new FormKeypair("grant_type", "refresh_token"));
            formData.Add(new FormKeypair("scope", "r_usr+w_usr+w_sub"));

            RequestData requestData = new RequestData(new Uri(string.Format("{0}/oauth2/token", this._authEndpoint)),
                EnumRequestMethod.POST, EnumContentType.APPLICATION_FORM_URLENCODED);
            
            requestData.AddFormData(formData);

            string authToken = Convert.ToBase64String(Encoding.Default.GetBytes(this._clientID + ":" + this._clientSecret));
            requestData.AddAuthMethod(new Auth(authToken, EnumAuthType.BASIC));

            JsonTidalAccountRefreshAccess accountAccess = null;

            try
            {
                ResponseData response = await new Request(requestData).GetResponseAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return null;

                if (response.GetContentAsString().Contains("authorization_pending"))
                    return null;

                accountAccess =
                    new JsonDeserializer<JsonTidalAccountRefreshAccess>().Deserialize(response.GetContentAsString());

                if (accountAccess == null)
                    return null;
            }
            catch (System.Exception e)
            { }

            return accountAccess;
        }

        public async Task<JsonTidalLyricsResult> GetLyrics(string accessToken, string trackID, string countryCode = "US")
        {
            RequestData requestData = new RequestData(string.Format("{0}/tracks/{1}/lyrics?countryCode={2}",
                this._apiEndpoint, trackID, countryCode));
            
            requestData.AddAuthMethod(new Auth(accessToken, EnumAuthType.OAUTH2));

            try
            {
                ResponseData response = await new Request(requestData).GetResponseAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return null;

                JsonTidalLyricsResult lyrics =
                    new JsonDeserializer<JsonTidalLyricsResult>().Deserialize(response.GetContentAsString());

                return lyrics;
            }
            catch (System.Exception e) { }

            return null;
        }
        
        public async Task<JsonTidalDownloadResult> DownloadSong(string accessToken, string trackID, string soundQuality = "LOW")
        {
            RequestData requestData = new RequestData(string.Format("{0}/tracks/{1}/streamUrl?soundQuality={2}",
                this._apiEndpoint, trackID, soundQuality));
            
            requestData.AddAuthMethod(new Auth(accessToken, EnumAuthType.OAUTH2));

            try
            {
                ResponseData response = await new Request(requestData).GetResponseAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return null;

                JsonTidalDownloadResult downloadResult =
                    new JsonDeserializer<JsonTidalDownloadResult>().Deserialize(response.GetContentAsString());

                return downloadResult;
            }
            catch (System.Exception e) { }

            return null;
        }
        
        public async Task<byte[]> DownloadSongData(string accessToken, string trackID, string soundQuality = "LOW")
        {
            JsonTidalDownloadResult result = await this.DownloadSong(accessToken, trackID, soundQuality);

            if (result.url == null || result.url.Length == 0)
                return new byte[0];

            Request request = new Request(result.url);
            ResponseData response = await request.GetResponseAsync(false);

            if (response.StatusCode != HttpStatusCode.OK)
                return new byte[0];
            
            return response.Content;
        }
    }
}
