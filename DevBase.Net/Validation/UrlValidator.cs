using System.Net;
using DevBase.Net.Constants;

namespace DevBase.Net.Validation;

public static class UrlValidator
{
    private const int MaxUrlLength = 2048;

    public static ValidationResult Validate(Uri? uri, bool validateProtocol = true, bool validateHost = true, bool validateLength = true)
    {
        if (uri == null)
            return ValidationResult.Fail("URL is required");

        string url = uri.ToString();

        if (validateLength && url.Length > MaxUrlLength)
            return ValidationResult.Fail($"URL exceeds maximum length of {MaxUrlLength} characters");

        if (validateProtocol && !IsValidProtocol(uri.Scheme))
            return ValidationResult.Fail($"Invalid protocol: {uri.Scheme}. Only http and https are supported.");

        if (validateHost && !IsValidHost(uri.Host))
            return ValidationResult.Fail($"Invalid host: {uri.Host}");

        if (!IsValidUrlCharacters(url))
            return ValidationResult.Fail("URL contains invalid characters");

        return ValidationResult.Success();
    }

    public static bool IsValidProtocol(string scheme)
    {
        ReadOnlySpan<char> schemeSpan = scheme.AsSpan();
        return schemeSpan.Equals(ProtocolConstants.Http.Span, StringComparison.OrdinalIgnoreCase) ||
               schemeSpan.Equals(ProtocolConstants.Https.Span, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsValidUrlCharacters(string url)
    {
        foreach (char c in url)
        {
            if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') ||
                  c == '-' || c == '.' || c == '_' || c == '~' || c == ':' || c == '/' ||
                  c == '?' || c == '#' || c == '[' || c == ']' || c == '@' || c == '!' ||
                  c == '$' || c == '&' || c == '\'' || c == '(' || c == ')' || c == '*' ||
                  c == '+' || c == ',' || c == ';' || c == '=' || c == '%'))
                return false;
        }
        return true;
    }

    public static bool IsValidHost(string host)
    {
        if (string.IsNullOrWhiteSpace(host))
            return false;

        if (IPAddress.TryParse(host, out _))
            return true;

        if (Uri.CheckHostName(host) == UriHostNameType.Dns)
            return true;

        return false;
    }

    public static ValidationResult ValidateQueryParameters(IEnumerable<KeyValuePair<string, string>> parameters)
    {
        foreach (KeyValuePair<string, string> param in parameters)
        {
            if (string.IsNullOrWhiteSpace(param.Key))
                return ValidationResult.Fail("Query parameter key cannot be empty");

            if (param.Key.Length > 256)
                return ValidationResult.Fail($"Query parameter key '{param.Key}' exceeds maximum length of 256 characters");

            if (param.Value?.Length > 2048)
                return ValidationResult.Fail($"Query parameter value for '{param.Key}' exceeds maximum length of 2048 characters");
        }

        return ValidationResult.Success();
    }
}
