using System.Net;
using System.Text;
using DevBase.Api.Apis.Tidal.Structure.Json;
using DevBase.Api.Helper;
using DevBase.Enums;
using DevBase.Generic;
using DevBase.Web;
using DevBase.Web.RequestData;
using DevBase.Web.RequestData.Data;
using DevBase.Web.ResponseData;

namespace DevBase.Api.Apis.Tidal
{
    public class TidalClient
    {
        private readonly string _authEndpoint;
        private readonly string _apiEndpoint;

        private readonly string _clientID;
        private readonly string _clientSecret;

        public TidalClient()
        {
            this._authEndpoint = "https://auth.tidal.com/v1";
            this._apiEndpoint = "https://api.tidal.com/v1";

            this._clientID = "zU4XHVVkc2tDPo4t";
            this._clientSecret = "VJKhDFqJPqvsPVNBV6ukXTJmwlvbttP7wlMlrc72se4=";
        }

        public async Task<JsonTidalAuthDevice> RegisterDevice()
        {
            GenericList<FormKeypair> formData = new GenericList<FormKeypair>();
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

            return new JsonHelper<JsonTidalAuthDevice>().Deserialize(response.GetContentAsString());
        }

        public async Task<JsonTidalAccountAccess> GetTokenFrom(JsonTidalAuthDevice authDevice)
        {
            GenericList<FormKeypair> formData = new GenericList<FormKeypair>();
            formData.Add(new FormKeypair("client_id", this._clientID));
            formData.Add(new FormKeypair("device_code", authDevice.DeviceCode));
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
                    new JsonHelper<JsonTidalAccountAccess>().Deserialize(response.GetContentAsString());

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

                return new JsonHelper<JsonTidalSession>().Deserialize(response.GetContentAsString());

            }
            catch (System.Exception e) { }

            return null;
        }

        public async Task<JsonTidalSearchResult> Search(JsonTidalSession session, string query)
        {
            RequestData requestData = new RequestData(string.Format("{0}/search/tracks?countryCode={1}&query={2}", this._apiEndpoint, session.CountryCode, query));
            requestData.Header.Add("x-tidal-token", this._clientID);

            try
            {
                Request request = new Request(requestData);
                ResponseData response = await request.GetResponseAsync();

                if (response.StatusCode == HttpStatusCode.NoContent)
                    return null;
                 
                if (response.StatusCode != HttpStatusCode.OK)
                    return null;

                return new JsonHelper<JsonTidalSearchResult>().Deserialize(response.GetContentAsString());

            }
            catch (System.Exception e) { }

            return null;
        }

        public async Task<JsonTidalAccountRefreshAccess> RefreshToken(string refreshToken)
        {
            GenericList<FormKeypair> formData = new GenericList<FormKeypair>();
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
                    new JsonHelper<JsonTidalAccountRefreshAccess>().Deserialize(response.GetContentAsString());

                if (accountAccess == null)
                    return null;
            }
            catch (System.Exception e)
            { }

            return accountAccess;
        }
    }
}
