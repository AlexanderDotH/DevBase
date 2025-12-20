using System.Text;
using System.Text.RegularExpressions;

namespace DevBase.Requests.Validation;

public static partial class HeaderValidator
{
    [GeneratedRegex(@"^[A-Za-z0-9+/=]+$")]
    private static partial Regex Base64Regex();

    [GeneratedRegex(@"^[A-Za-z0-9\-_]+\.[A-Za-z0-9\-_]+\.[A-Za-z0-9\-_]*$")]
    private static partial Regex JwtRegex();

    [GeneratedRegex(@"^[a-zA-Z0-9!#$%&'*+.^_`|~-]+/[a-zA-Z0-9!#$%&'*+.^_`|~-]+")]
    private static partial Regex MimeTypeRegex();

    [GeneratedRegex(@"^[^=;\s]+=[^;\s]*(?:;\s*[^=;\s]+=[^;\s]*)*$")]
    private static partial Regex CookieRegex();

    public static ValidationResult ValidateBasicAuth(string headerValue)
    {
        if (string.IsNullOrWhiteSpace(headerValue))
            return ValidationResult.Fail("Basic authentication header is empty");

        var parts = headerValue.Split(' ', 2);
        if (parts.Length != 2 || !parts[0].Equals("Basic", StringComparison.OrdinalIgnoreCase))
            return ValidationResult.Fail("Invalid Basic authentication format");

        var base64Part = parts[1];
        if (!Base64Regex().IsMatch(base64Part))
            return ValidationResult.Fail("Invalid Base64 encoding in Basic authentication");

        try
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(base64Part));
            if (!decoded.Contains(':'))
                return ValidationResult.Fail("Basic authentication must contain username:password");
        }
        catch
        {
            return ValidationResult.Fail("Failed to decode Basic authentication credentials");
        }

        return ValidationResult.Success();
    }

    public static ValidationResult ValidateBearerAuth(string headerValue)
    {
        if (string.IsNullOrWhiteSpace(headerValue))
            return ValidationResult.Fail("Bearer authentication header is empty");

        var parts = headerValue.Split(' ', 2);
        if (parts.Length != 2 || !parts[0].Equals("Bearer", StringComparison.OrdinalIgnoreCase))
            return ValidationResult.Fail("Invalid Bearer authentication format");

        var token = parts[1];
        if (string.IsNullOrWhiteSpace(token))
            return ValidationResult.Fail("Bearer token is empty");

        if (JwtRegex().IsMatch(token))
            return ValidationResult.Success();

        if (token.Length < 10)
            return ValidationResult.Fail("Bearer token is too short");

        return ValidationResult.Success();
    }

    public static ValidationResult ValidateContentType(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            return ValidationResult.Fail("Content-Type is empty");

        if (!MimeTypeRegex().IsMatch(contentType))
            return ValidationResult.Fail($"Invalid Content-Type format: {contentType}");

        return ValidationResult.Success();
    }

    public static ValidationResult ValidateContentLength(string? contentLength, long actualLength)
    {
        if (string.IsNullOrWhiteSpace(contentLength))
            return ValidationResult.Success();

        if (!long.TryParse(contentLength, out var declaredLength))
            return ValidationResult.Fail("Content-Length must be a valid number");

        if (declaredLength != actualLength)
            return ValidationResult.Fail($"Content-Length mismatch: declared {declaredLength}, actual {actualLength}");

        return ValidationResult.Success();
    }

    public static ValidationResult ValidateHost(string? host, Uri? requestUri)
    {
        if (string.IsNullOrWhiteSpace(host))
            return ValidationResult.Fail("Host header is empty");

        if (requestUri != null)
        {
            var expectedHost = requestUri.Port == 80 || requestUri.Port == 443
                ? requestUri.Host
                : $"{requestUri.Host}:{requestUri.Port}";

            if (!host.Equals(expectedHost, StringComparison.OrdinalIgnoreCase) &&
                !host.Equals(requestUri.Host, StringComparison.OrdinalIgnoreCase))
            {
                return ValidationResult.Fail($"Host header '{host}' does not match request URI host '{expectedHost}'");
            }
        }

        return ValidationResult.Success();
    }

    public static ValidationResult ValidateUserAgent(string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
            return ValidationResult.Fail("User-Agent is empty");

        return ValidationResult.Success();
    }

    public static ValidationResult ValidateAccept(string? accept)
    {
        if (string.IsNullOrWhiteSpace(accept))
            return ValidationResult.Fail("Accept header is empty");

        if (accept == "*/*")
            return ValidationResult.Success();

        var parts = accept.Split(',');
        foreach (var part in parts)
        {
            var mimeType = part.Split(';')[0].Trim();
            if (mimeType != "*/*" && !MimeTypeRegex().IsMatch(mimeType))
                return ValidationResult.Fail($"Invalid Accept MIME type: {mimeType}");
        }

        return ValidationResult.Success();
    }

    public static ValidationResult ValidateCookie(string? cookie)
    {
        if (string.IsNullOrWhiteSpace(cookie))
            return ValidationResult.Fail("Cookie header is empty");

        if (!CookieRegex().IsMatch(cookie))
            return ValidationResult.Fail("Invalid cookie format");

        return ValidationResult.Success();
    }

    public static ValidationResult ValidateAcceptEncoding(string? acceptEncoding)
    {
        if (string.IsNullOrWhiteSpace(acceptEncoding))
            return ValidationResult.Fail("Accept-Encoding header is empty");

        var validEncodings = new[] { "gzip", "deflate", "br", "identity", "*" };
        var parts = acceptEncoding.Split(',');
        
        foreach (var part in parts)
        {
            var encoding = part.Split(';')[0].Trim().ToLowerInvariant();
            if (!validEncodings.Contains(encoding))
                return ValidationResult.Fail($"Invalid Accept-Encoding value: {encoding}");
        }

        return ValidationResult.Success();
    }
}
