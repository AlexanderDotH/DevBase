using DevBase.Api.Apis.Musixmatch.Internal;
using DevBase.Api.Apis.Musixmatch.Structure.Requests;
using DevBase.Net.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace DevBase.Api.Apis.Musixmatch;

public partial class MusixMatch
{
    public async Task<MusixMatchToken> LoginWithMusixMatchAccountAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            AuthenticationGet? authentication = await this.Login(email, password, cancellationToken);
            string? userToken = authentication?.Tokens?.WebDesktopAppV10;

            if (string.IsNullOrWhiteSpace(userToken)) {
                throw new InvalidOperationException("Musixmatch account login did not return a web desktop token.");
            }

            MusixMatchToken token = new MusixMatchToken(userToken, MusixMatchApiContext.Desktop);
            this.Token = token;
            this.Context = token.Context;

            return token;
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<MusixMatchToken>(exception);
        }
    }

    public MusixMatchToken LoginWithMusixMatchAccount(string email, string password)
    {
        return this.LoginWithMusixMatchAccountAsync(email, password).GetAwaiter().GetResult();
    }

    public async Task AuthenticateGoogleAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.CredentialPost,
                cancellationToken: cancellationToken);
            string body = CreateGoogleCredentialPayload(accessToken);
            Request request = new Request(url)
                .AsPost()
                .WithRawBody(body, Encoding.UTF8)
                .WithHeader("Content-Type", GetMusixMatchContentType(body));
            Response response = await request.SendAsync(cancellationToken);
            _ = await this.ParseMusixMatchApiResponseAsync(response, cancellationToken);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            this.Throw<object>(exception);
        }
    }

    public void AuthenticateGoogle(string accessToken)
    {
        this.AuthenticateGoogleAsync(accessToken).GetAwaiter().GetResult();
    }

    public async Task AuthenticateMusixMatchAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.CredentialPost,
                cancellationToken: cancellationToken);
            string body = CreateMusixMatchCredentialPayload(email, password);
            Request request = new Request(url)
                .AsPost()
                .WithRawBody(body, Encoding.UTF8)
                .WithHeader("Content-Type", GetMusixMatchContentType(body));
            Response response = await request.SendAsync(cancellationToken);
            _ = await this.ParseMusixMatchApiResponseAsync(response, cancellationToken);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            this.Throw<object>(exception);
        }
    }

    public void AuthenticateMusixMatch(string email, string password)
    {
        this.AuthenticateMusixMatchAsync(email, password).GetAwaiter().GetResult();
    }

    public async Task DeauthenticateAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.CredentialPost,
                cancellationToken: cancellationToken);
            string body = CreateEmptyCredentialPayload();
            Request request = new Request(url)
                .AsPost()
                .WithRawBody(body, Encoding.UTF8)
                .WithHeader("Content-Type", GetMusixMatchContentType(body));
            Response response = await request.SendAsync(cancellationToken);
            _ = await this.ParseMusixMatchApiResponseAsync(response, cancellationToken);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            this.Throw<object>(exception);
        }
    }

    public void Deauthenticate()
    {
        this.DeauthenticateAsync().GetAwaiter().GetResult();
    }

    public async Task<AuthenticationGet?> Login(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.AccountPost,
                context: MusixMatchApiContext.AuthWeb,
                cancellationToken: cancellationToken);
            string body = CreateMusixMatchCredentialPayload(email, password);
            Request request = new Request(url)
                .AsPost()
                .WithRawBody(body, Encoding.UTF8)
                .WithHeader("Content-Type", GetMusixMatchContentType(body));
            Response response = await request.SendAsync(cancellationToken);

            return await this.ParseMusixMatchApiBodyAsync<AuthenticationGet>(response, cancellationToken);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<AuthenticationGet>(exception);
        }
    }

    internal async Task<MusixMatchApiResponse> ParseMusixMatchApiResponseAsync(
        Response response,
        CancellationToken cancellationToken)
    {
        string content = await response.GetStringAsync(cancellationToken: cancellationToken);
        return MusixMatchResponseParser.Parse(content, this.StrictErrorHandling);
    }

    internal async Task<T?> ParseMusixMatchApiBodyAsync<T>(
        Response response,
        CancellationToken cancellationToken)
    {
        MusixMatchApiResponse apiResponse = await this.ParseMusixMatchApiResponseAsync(response, cancellationToken);

        if (apiResponse.StatusCode != (int)MusixMatchStatusCode.Success) {
            return this.Throw<T>(new MusixMatchRequestException(apiResponse.StatusCode));
        }

        return apiResponse.Cast<T>();
    }

    private static string CreateMusixMatchCredentialPayload(string email, string password)
    {
        return CreateCredentialPayload(new JObject
        {
            ["type"] = "mxm",
            ["email"] = email,
            ["password"] = password,
            ["action"] = "login"
        });
    }

    private static string CreateGoogleCredentialPayload(string accessToken)
    {
        return CreateCredentialPayload(new JObject
        {
            ["type"] = "g2",
            ["auth_token"] = accessToken
        });
    }

    private static string CreateEmptyCredentialPayload()
    {
        return new JObject
        {
            ["credential_list"] = new JArray()
        }.ToString(Formatting.None);
    }

    private static string CreateCredentialPayload(JObject credential)
    {
        return new JObject
        {
            ["credential_list"] = new JArray
            {
                new JObject
                {
                    ["credential"] = credential
                }
            }
        }.ToString(Formatting.None);
    }
}
