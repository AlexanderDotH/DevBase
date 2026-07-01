using System.Security.Cryptography;
using System.Text;

namespace DevBase.Api.Apis.Musixmatch.Internal;

internal sealed class MusixMatchRequestBuilder
{
    private static readonly StringComparer ParameterComparer = StringComparer.Ordinal;

    private readonly MusixMatchSignatureService _signatureService;

    public MusixMatchRequestBuilder(MusixMatchSignatureService signatureService)
    {
        this._signatureService = signatureService;
    }

    public async Task<string> BuildUrlAsync(
        MusixMatchEndpoint endpoint,
        MusixMatchApiContextDefinition context,
        string userToken,
        Guid guid,
        IDictionary<string, string?>? query,
        CancellationToken cancellationToken)
    {
        MusixMatchEndpointDefinition endpointDefinition = MusixMatchEndpointDefinition.For(endpoint);
        return await this.BuildUrlCoreAsync(endpointDefinition, context, userToken, guid, query, cancellationToken);
    }

    private async Task<string> BuildUrlCoreAsync(
        MusixMatchEndpointDefinition endpoint,
        MusixMatchApiContextDefinition context,
        string userToken,
        Guid guid,
        IDictionary<string, string?>? query,
        CancellationToken cancellationToken)
    {
        string queryString = BuildQuery(context, userToken, guid, query);
        string url = $"{context.ApiUrl}{endpoint.Resource}{queryString}";

        if (!context.RequiresSignature) {
            return url;
        }

        string key = await this._signatureService.GetSignatureKeyAsync(cancellationToken);
        string signature = CreateSignature(url, key);
        string separator = url.Contains('?') ? "&" : "?";

        return $"{url}{separator}signature={Uri.EscapeDataString(signature)}&signature_protocol=sha1";
    }

    private static string BuildQuery(
        MusixMatchApiContextDefinition context,
        string userToken,
        Guid guid,
        IDictionary<string, string?>? query)
    {
        List<KeyValuePair<string, string>> parameters = [];
        AddParameters(parameters, query);
        RemoveParameter(parameters, "format");
        RemoveParameter(parameters, "app_id");
        RemoveParameter(parameters, "usertoken");
        RemoveParameter(parameters, "guid");
        AddParameter(parameters, "format", "json");
        AddParameter(parameters, "app_id", context.AppId);
        AddParameter(parameters, "usertoken", userToken);
        AddParameter(parameters, "guid", guid.ToString());

        string serialized = BuildParameters(parameters);
        return serialized.Length == 0 ? string.Empty : $"?{serialized}";
    }

    public static string BuildBody(IDictionary<string, string?>? parameters)
    {
        return BuildParameters(parameters);
    }

    public static string GetContentType(string body)
    {
        string trimmed = body.TrimStart();

        return trimmed.StartsWith('{') || trimmed.StartsWith('[')
            ? "application/json"
            : "application/x-www-form-urlencoded";
    }

    private static string BuildParameters(IDictionary<string, string?>? parameters)
    {
        List<KeyValuePair<string, string>> normalized = [];
        AddParameters(normalized, parameters);
        return BuildParameters(normalized);
    }

    private static string BuildParameters(IReadOnlyList<KeyValuePair<string, string>> parameters)
    {
        if (parameters.Count == 0) {
            return string.Empty;
        }

        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < parameters.Count; i++)
        {
            KeyValuePair<string, string> parameter = parameters[i];

            if (i > 0) {
                builder.Append('&');
            }

            builder.Append(Uri.EscapeDataString(parameter.Key));
            builder.Append('=');
            builder.Append(Uri.EscapeDataString(parameter.Value));
        }

        return builder.ToString();
    }

    private static void AddParameters(
        List<KeyValuePair<string, string>> destination,
        IEnumerable<KeyValuePair<string, string?>>? parameters)
    {
        if (parameters == null) {
            return;
        }

        foreach (KeyValuePair<string, string?> parameter in parameters)
        {
            AddParameter(destination, parameter.Key, parameter.Value);
        }
    }

    private static void AddParameter(List<KeyValuePair<string, string>> parameters, string key, string? value)
    {
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrEmpty(value)) {
            return;
        }

        parameters.Add(KeyValuePair.Create(key, value));
    }

    private static void RemoveParameter(List<KeyValuePair<string, string>> parameters, string key)
    {
        parameters.RemoveAll(parameter => ParameterComparer.Equals(parameter.Key, key));
    }

    private static string CreateSignature(string url, string key)
    {
        string input = url + DateTime.Now.ToString("yyyyMMdd");
        byte[] keyBytes = Encoding.ASCII.GetBytes(key);
        byte[] inputBytes = Encoding.ASCII.GetBytes(input);

        using HMACSHA1 hmac = new HMACSHA1(keyBytes);
        return Convert.ToBase64String(hmac.ComputeHash(inputBytes));
    }

}
