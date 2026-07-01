using DevBase.Api.Apis.Musixmatch.Internal;
using DevBase.Net.Core;
using Newtonsoft.Json.Linq;

namespace DevBase.Api.Apis.Musixmatch;

public partial class MusixMatch : ApiClient
{
    private static readonly MusixMatchRequestBuilder SharedRequestBuilder = new(MusixMatchSignatureService.Shared);

    private readonly Guid _guid = Guid.NewGuid();
    private readonly MusixMatchRequestBuilder _requestBuilder;

    public MusixMatch()
        : this(IssueTokenCoreAsync(MusixMatchApiContext.Desktop, true, CancellationToken.None).GetAwaiter().GetResult())
    {
    }

    public MusixMatch(string userToken, MusixMatchApiContext context = MusixMatchApiContext.Desktop)
        : this(new MusixMatchToken(userToken, context))
    {
    }

    public MusixMatch(MusixMatchToken token)
    {
        ArgumentNullException.ThrowIfNull(token);

        this.Token = token;
        this.Context = token.Context;
        this._requestBuilder = SharedRequestBuilder;
    }

    public MusixMatchToken Token { get; private set; }

    public MusixMatchApiContext Context { get; private set; }

    public async Task<MusixMatchToken> IssueTokenAsync(
        MusixMatchApiContext context = MusixMatchApiContext.Desktop,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await IssueTokenCoreAsync(context, this.StrictErrorHandling, cancellationToken);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<MusixMatchToken>(exception);
        }
    }

    public MusixMatchToken IssueToken(MusixMatchApiContext context = MusixMatchApiContext.Desktop)
    {
        return this.IssueTokenAsync(context).GetAwaiter().GetResult();
    }

    public string GetAuthUrl()
    {
        MusixMatchApiContextDefinition context = MusixMatchApiContextDefinition.For(this.Context);
        string appId = Uri.EscapeDataString(context.AppId ?? string.Empty);
        string userToken = Uri.EscapeDataString(this.Token.Token);

        return $"https://oauth.musixmatch.com/credential/signin?app_id={appId}&usertoken={userToken}&origin=none";
    }

    internal async Task<string> BuildMusixMatchUrlAsync(
        MusixMatchEndpoint endpoint,
        IDictionary<string, string?>? query = null,
        MusixMatchApiContext? context = null,
        CancellationToken cancellationToken = default)
    {
        MusixMatchApiContext requestContext = context ?? this.Context;
        MusixMatchApiContextDefinition contextDefinition = MusixMatchApiContextDefinition.For(requestContext);
        string userToken = requestContext == MusixMatchApiContext.AuthWeb && endpoint == MusixMatchEndpoint.AccountPost
            ? string.Empty
            : this.Token.Token;

        return await this._requestBuilder.BuildUrlAsync(
            endpoint,
            contextDefinition,
            userToken,
            this._guid,
            query,
            cancellationToken);
    }

    internal async Task<string> BuildMusixMatchUrlAsync(
        MusixmatchEndpoint endpoint,
        IDictionary<string, string?>? query = null,
        MusixMatchApiContext? context = null,
        CancellationToken cancellationToken = default)
    {
        return await this.BuildMusixMatchUrlAsync(ToEndpoint(endpoint), query, context, cancellationToken);
    }

    internal async Task<string> BuildMusixMatchUrlAsync(
        Enums.MusixmatchEndpoint endpoint,
        IDictionary<string, string?>? query = null,
        MusixMatchApiContext? context = null,
        CancellationToken cancellationToken = default)
    {
        return await this.BuildMusixMatchUrlAsync(ToEndpoint(endpoint), query, context, cancellationToken);
    }

    internal static string BuildMusixMatchBody(IDictionary<string, string?>? data)
    {
        return MusixMatchRequestBuilder.BuildBody(data);
    }

    internal static string GetMusixMatchContentType(string body)
    {
        return MusixMatchRequestBuilder.GetContentType(body);
    }

    internal async Task<MusixMatchApiResponse> ParseMusixMatchResponseAsync(
        Response response,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(response);

        string content = await response.GetStringAsync(cancellationToken: cancellationToken);
        return this.ParseMusixMatchResponse(content);
    }

    internal MusixMatchApiResponse ParseMusixMatchResponse(string content)
    {
        ArgumentNullException.ThrowIfNull(content);

        return MusixMatchResponseParser.Parse(content, this.StrictErrorHandling);
    }

    internal T? ParseMusixMatchBody<T>(MusixMatchApiResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        if (response.StatusCode != (int)MusixMatchStatusCode.Success) {
            return this.Throw<T>(new MusixMatchRequestException(response.StatusCode));
        }

        return response.Cast<T>();
    }

    internal static List<TOut> SelectList<TIn, TOut>(IEnumerable<TIn>? source, Func<TIn, TOut> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        return source?.Select(selector).ToList() ?? [];
    }

    private static T RequireResponseData<T>(
        T? data,
        string resourceName,
        string? resourceId,
        Func<T, bool> hasData)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(hasData);

        if (data is null || !hasData(data)) {
            throw new MusixMatchEmptyResponseDataException(resourceName, resourceId);
        }

        return data;
    }

    private static async Task<MusixMatchToken> IssueTokenCoreAsync(
        MusixMatchApiContext context,
        bool strictErrorHandling,
        CancellationToken cancellationToken)
    {
        MusixMatchApiContextDefinition contextDefinition = MusixMatchApiContextDefinition.For(context);
        string url = await SharedRequestBuilder.BuildUrlAsync(
            MusixMatchEndpoint.TokenGet,
            contextDefinition,
            string.Empty,
            Guid.NewGuid(),
            null,
            cancellationToken);
        Request request = new Request(url);

        Response response = await request.SendAsync(cancellationToken);
        string content = await response.GetStringAsync(cancellationToken: cancellationToken);
        MusixMatchApiResponse apiResponse = MusixMatchResponseParser.Parse(content, strictErrorHandling);
        string? token = JObject.Parse(apiResponse.Body).SelectToken("user_token")?.Value<string>();

        if (string.IsNullOrWhiteSpace(token)) {
            throw new InvalidOperationException("Musixmatch token response did not contain a user_token.");
        }

        return new MusixMatchToken(token, context);
    }

    private static MusixMatchEndpoint ToEndpoint(Enums.MusixmatchEndpoint endpoint)
    {
        return (MusixMatchEndpoint)(int)endpoint;
    }

    private static MusixMatchEndpoint ToEndpoint(MusixmatchEndpoint endpoint)
    {
        return (MusixMatchEndpoint)(int)endpoint;
    }
}
