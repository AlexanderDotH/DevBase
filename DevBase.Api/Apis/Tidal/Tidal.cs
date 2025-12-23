using System.Net;
using System.Text;
using DevBase.Api.Apis.Tidal.Structure.Json;
using DevBase.Api.Enums;
using DevBase.Api.Exceptions;
using DevBase.Api.Serializer;
using DevBase.Enums;
using DevBase.Generics;
using DevBase.Net.Core;
using DevBase.Net.Data.Body;
using DevBase.Net.Data.Parameters;

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
            string authToken = Convert.ToBase64String(Encoding.Default.GetBytes($"{this._clientId}:{this._clientSecret}"));

            Response response = await new Request($"{this._authEndpoint}/oauth2/device_authorization")
                .AsPost()
                .WithEncodedForm(
                    ("client_id", this._clientIdKeyPair.Value),
                    ("scope", this._scopeKeyPair.Value)
                )
                .UseBasicAuthentication(this._clientId, this._clientSecret) // Actually wait, manual header or helper? Helper encodes it. Original did manual base64. Helper is cleaner.
                // Original: requestData.AddAuthMethod(new Auth(authToken, EnumAuthType.BASIC)); where authToken was manually base64 encoded "client:secret".
                // .UseBasicAuthentication(user, pass) does exactly that.
                .SendAsync();

            return await response.ParseJsonAsync<JsonTidalAuthDevice>(false);
        }

        public async Task<JsonTidalAccountAccess> GetTokenFrom(string deviceCode)
        {
            try
            {
                Response response = await new Request($"{this._authEndpoint}/oauth2/token")
                    .AsPost()
                    .WithEncodedForm(
                        ("client_id", this._clientIdKeyPair.Value),
                        ("device_code", deviceCode),
                        ("grant_type", "urn:ietf:params:oauth:grant-type:device_code"),
                        ("scope", this._scopeKeyPair.Value)
                    )
                    .SendAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));

                string content = await response.GetStringAsync();
                if (content.Contains("authorization_pending"))
                    return Throw<object>(new TidalException(EnumTidalExceptionType.AuthorizationPending));

                return await response.ParseJsonAsync<JsonTidalAccountAccess>(false);
            }
            catch
            {
                return Throw<object>(new TidalException(EnumTidalExceptionType.ParsingError));
            }
        }

        public async Task<JsonTidalSession> Login(string accessToken)
        {
            try
            {
                Response response = await new Request($"{this._apiEndpoint}/sessions")
                    .AsGet()
                    .WithAcceptJson()
                    .WithBogusUserAgent()
                    .UseBearerAuthentication(accessToken)
                    .SendAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));

                return await response.ParseJsonAsync<JsonTidalSession>(false);
            }
            catch
            {
                return Throw<object>(new TidalException(EnumTidalExceptionType.ParsingError));
            }
        }

        public async Task<JsonTidalSearchResult> Search(string query, string countryCode = "AS", int limit = 10)
        {
            try
            {
                Response response = await new Request($"{this._apiEndpoint}/search/tracks")
                    .AsGet()
                    .WithParameters(
                        ("countryCode", countryCode),
                        ("query", query),
                        ("limit", limit.ToString())
                    )
                    .WithHeader("x-tidal-token", this._clientId)
                    .SendAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));

                return await response.ParseJsonAsync<JsonTidalSearchResult>(false);
            }
            catch
            {
                return Throw<object>(new TidalException(EnumTidalExceptionType.ParsingError));
            }
        }

        public async Task<JsonTidalAuthAccess> AuthTokenToAccess(string authToken)
        {
            try
            {
                Response response = await new Request($"{this._authEndpoint}/oauth2/token")
                    .AsPost()
                    .WithEncodedForm(
                        ("client_id", this._clientIdKeyPair.Value),
                        ("user_auth_token", authToken),
                        ("grant_type", "user_auth_token"),
                        ("scope", this._scopeKeyPair.Value)
                    )
                    .WithAccept("*/*")
                    .SendAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));
                
                return await response.ParseJsonAsync<JsonTidalAuthAccess>(false);
            }
            catch
            {
                return Throw<object>(new TidalException(EnumTidalExceptionType.ParsingError));
            }
        }

        public async Task<JsonTidalAccountRefreshAccess> RefreshToken(string refreshToken)
        {
            try
            {
                Response response = await new Request($"{this._authEndpoint}/oauth2/token")
                    .AsPost()
                    .WithEncodedForm(
                        ("client_id", this._clientIdKeyPair.Value),
                        ("refresh_token", refreshToken),
                        ("grant_type", "refresh_token"),
                        ("scope", this._scopeKeyPair.Value)
                    )
                    .UseBasicAuthentication(this._clientId, this._clientSecret)
                    .SendAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));

                string content = await response.GetStringAsync();
                if (content.Contains("authorization_pending"))
                    return Throw<object>(new TidalException(EnumTidalExceptionType.AuthorizationPending));

                return await response.ParseJsonAsync<JsonTidalAccountRefreshAccess>(false);
            }
            catch
            {
                return Throw<object>(new TidalException(EnumTidalExceptionType.ParsingError));
            }
        }

        public async Task<JsonTidalLyricsResult> GetLyrics(string accessToken, string trackId, string countryCode = "US")
        {
            try
            {
                Response response = await new Request($"{this._authEndpoint}/tracks/{trackId}/lyrics?countryCode={countryCode}")
                    .AsGet()
                    .UseBearerAuthentication(accessToken)
                    .SendAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));

                return await response.ParseJsonAsync<JsonTidalLyricsResult>(false);
            }
            catch
            {
                return Throw<object>(new TidalException(EnumTidalExceptionType.ParsingError));
            }
        }
        
        public async Task<JsonTidalDownloadResult> DownloadSong(string accessToken, string trackId, string soundQuality = "LOW")
        {
            try
            {
                Response response = await new Request($"{this._authEndpoint}/tracks/{trackId}/streamUrl?soundQuality={soundQuality}")
                    .AsGet()
                    .UseBearerAuthentication(accessToken)
                    .SendAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));

                return await response.ParseJsonAsync<JsonTidalDownloadResult>(false);
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

            Response response = await new Request(result.url)
                .AsGet()
                .SendAsync();

            if (response.StatusCode != HttpStatusCode.OK)
                return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));
            
            return await response.GetBytesAsync();
        }
    }
}
