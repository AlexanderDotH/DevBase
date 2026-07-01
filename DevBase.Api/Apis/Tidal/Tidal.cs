using System.Net;
using DevBase.Api.Apis.Tidal.Structure.Json;
using DevBase.Api.Enums;
using DevBase.Api.Exceptions;
using DevBase.Net.Core;
using DevBase.Net.Data.Parameters;
using Newtonsoft.Json;

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

        public async Task<JsonTidalAuthDevice> RegisterDevice(CancellationToken cancellationToken = default)
        {
            Request request = new Request($"{this._authEndpoint}/oauth2/device_authorization")
                .AsPost()
                .WithEncodedForm(
                    ("client_id", this._clientIdKeyPair.Value),
                    ("scope", this._scopeKeyPair.Value)
                )
                .UseBasicAuthentication(this._clientId, this._clientSecret);

            Response response = await request.SendAsync(cancellationToken);
            return await response.ParseJsonAsync<JsonTidalAuthDevice>(false, cancellationToken);
        }

        public async Task<JsonTidalAccountAccess> GetTokenFrom(string deviceCode, CancellationToken cancellationToken = default)
        {
            try
            {
                Request request = new Request($"{this._authEndpoint}/oauth2/token")
                    .AsPost()
                    .WithEncodedForm(
                        ("client_id", this._clientIdKeyPair.Value),
                        ("device_code", deviceCode),
                        ("grant_type", "urn:ietf:params:oauth:grant-type:device_code"),
                        ("scope", this._scopeKeyPair.Value)
                    );

                Response response = await request.SendAsync(cancellationToken);

                if (response.StatusCode != HttpStatusCode.OK) {
                    return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));
                }

                string content = await response.GetStringAsync(cancellationToken: cancellationToken);
                if (content.Contains("authorization_pending")) {
                    return Throw<object>(new TidalException(EnumTidalExceptionType.AuthorizationPending));
                }

                return JsonConvert.DeserializeObject<JsonTidalAccountAccess>(content);
            }
            catch (TidalException exception)
            {
                return Throw<object>(exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                return Throw<object>(new TidalException(EnumTidalExceptionType.ParsingError));
            }
        }

        public async Task<JsonTidalSession> Login(string accessToken, CancellationToken cancellationToken = default)
        {
            try
            {
                Request request = new Request($"{this._apiEndpoint}/sessions")
                    .AsGet()
                    .WithAcceptJson()
                    .WithBogusUserAgent()
                    .UseBearerAuthentication(accessToken);

                Response response = await request.SendAsync(cancellationToken);

                if (response.StatusCode != HttpStatusCode.OK) {
                    return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));
                }

                return await response.ParseJsonAsync<JsonTidalSession>(false, cancellationToken);
            }
            catch (TidalException exception)
            {
                return Throw<object>(exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                return Throw<object>(new TidalException(EnumTidalExceptionType.ParsingError));
            }
        }

        public async Task<JsonTidalSearchResult> Search(
            string query,
            string countryCode = "AS",
            int limit = 10,
            CancellationToken cancellationToken = default)
        {
            try
            {
                Request request = new Request($"{this._apiEndpoint}/search/tracks")
                    .AsGet()
                    .WithParameters(
                        ("countryCode", countryCode),
                        ("query", query),
                        ("limit", limit.ToString())
                    )
                    .WithHeader("x-tidal-token", this._clientId);

                Response response = await request.SendAsync(cancellationToken);

                if (response.StatusCode != HttpStatusCode.OK) {
                    return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));
                }

                return await response.ParseJsonAsync<JsonTidalSearchResult>(false, cancellationToken);
            }
            catch (TidalException exception)
            {
                return Throw<object>(exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                return Throw<object>(new TidalException(EnumTidalExceptionType.ParsingError));
            }
        }

        public async Task<JsonTidalAuthAccess> AuthTokenToAccess(
            string authToken,
            CancellationToken cancellationToken = default)
        {
            try
            {
                Request request = new Request($"{this._authEndpoint}/oauth2/token")
                    .AsPost()
                    .WithEncodedForm(
                        ("client_id", this._clientIdKeyPair.Value),
                        ("user_auth_token", authToken),
                        ("grant_type", "user_auth_token"),
                        ("scope", this._scopeKeyPair.Value)
                    )
                    .WithAccept("*/*");

                Response response = await request.SendAsync(cancellationToken);

                if (response.StatusCode != HttpStatusCode.OK) {
                    return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));
                }

                return await response.ParseJsonAsync<JsonTidalAuthAccess>(false, cancellationToken);
            }
            catch (TidalException exception)
            {
                return Throw<object>(exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                return Throw<object>(new TidalException(EnumTidalExceptionType.ParsingError));
            }
        }

        public async Task<JsonTidalAccountRefreshAccess> RefreshToken(
            string refreshToken,
            CancellationToken cancellationToken = default)
        {
            try
            {
                Request request = new Request($"{this._authEndpoint}/oauth2/token")
                    .AsPost()
                    .WithEncodedForm(
                        ("client_id", this._clientIdKeyPair.Value),
                        ("refresh_token", refreshToken),
                        ("grant_type", "refresh_token"),
                        ("scope", this._scopeKeyPair.Value)
                    )
                    .UseBasicAuthentication(this._clientId, this._clientSecret);

                Response response = await request.SendAsync(cancellationToken);

                if (response.StatusCode != HttpStatusCode.OK) {
                    return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));
                }

                string content = await response.GetStringAsync(cancellationToken: cancellationToken);
                if (content.Contains("authorization_pending")) {
                    return Throw<object>(new TidalException(EnumTidalExceptionType.AuthorizationPending));
                }

                return JsonConvert.DeserializeObject<JsonTidalAccountRefreshAccess>(content);
            }
            catch (TidalException exception)
            {
                return Throw<object>(exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                return Throw<object>(new TidalException(EnumTidalExceptionType.ParsingError));
            }
        }

        public async Task<JsonTidalLyricsResult> GetLyrics(
            string accessToken,
            string trackId,
            string countryCode = "US",
            CancellationToken cancellationToken = default)
        {
            try
            {
                Request request = new Request($"{this._authEndpoint}/tracks/{trackId}/lyrics?countryCode={countryCode}")
                    .AsGet()
                    .UseBearerAuthentication(accessToken);

                Response response = await request.SendAsync(cancellationToken);

                if (response.StatusCode != HttpStatusCode.OK) {
                    return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));
                }

                return await response.ParseJsonAsync<JsonTidalLyricsResult>(false, cancellationToken);
            }
            catch (TidalException exception)
            {
                return Throw<object>(exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                return Throw<object>(new TidalException(EnumTidalExceptionType.ParsingError));
            }
        }
        
        public async Task<JsonTidalDownloadResult> DownloadSong(
            string accessToken,
            string trackId,
            string soundQuality = "LOW",
            CancellationToken cancellationToken = default)
        {
            try
            {
                Request request = new Request($"{this._authEndpoint}/tracks/{trackId}/streamUrl?soundQuality={soundQuality}")
                    .AsGet()
                    .UseBearerAuthentication(accessToken);

                Response response = await request.SendAsync(cancellationToken);

                if (response.StatusCode != HttpStatusCode.OK) {
                    return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));
                }

                return await response.ParseJsonAsync<JsonTidalDownloadResult>(false, cancellationToken);
            }
            catch (TidalException exception)
            {
                return Throw<object>(exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                return Throw<object>(new TidalException(EnumTidalExceptionType.ParsingError));
            }
        }
        
        public async Task<byte[]> DownloadSongData(
            string accessToken,
            string trackId,
            string soundQuality = "LOW",
            CancellationToken cancellationToken = default)
        {
            JsonTidalDownloadResult result = await this.DownloadSong(accessToken, trackId, soundQuality, cancellationToken);

            if (result?.url == null || result.url.Length == 0) {
                return ThrowBytes(new TidalException(EnumTidalExceptionType.MissingDownloadUrl));
            }

            try
            {
                Request request = new Request(result.url).AsGet();
                Response response = await request.SendAsync(cancellationToken);

                if (response.StatusCode != HttpStatusCode.OK) {
                    return Throw<object>(new TidalException(EnumTidalExceptionType.NotOk));
                }

                return await response.GetBytesAsync(cancellationToken);
            }
            catch (TidalException exception)
            {
                return Throw<object>(exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
        }
    }
}
