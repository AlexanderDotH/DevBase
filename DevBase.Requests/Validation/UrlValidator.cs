using System.Net;
using System.Text.RegularExpressions;

namespace DevBase.Requests.Validation;

public static partial class UrlValidator
{
    private const int MaxUrlLength = 2048;
    
    [GeneratedRegex(@"^[a-zA-Z0-9\-._~:/?#\[\]@!$&'()*+,;=%]+$")]
    private static partial Regex AllowedUrlCharactersRegex();

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

        if (!AllowedUrlCharactersRegex().IsMatch(url))
            return ValidationResult.Fail("URL contains invalid characters");

        return ValidationResult.Success();
    }

    public static bool IsValidProtocol(string scheme)
    {
        return scheme.Equals("http", StringComparison.OrdinalIgnoreCase) ||
               scheme.Equals("https", StringComparison.OrdinalIgnoreCase);
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
