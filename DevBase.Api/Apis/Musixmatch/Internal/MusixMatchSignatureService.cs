using System.Text.RegularExpressions;
using DevBase.Net.Core;

namespace DevBase.Api.Apis.Musixmatch.Internal;

internal sealed class MusixMatchSignatureService
{
    internal const string FallbackSignatureKey = "741941edc264ea6293cb9a6458103b4eda3ac8ed";

    private static readonly Regex CommonScriptRegex = new(
        "(?<url>(?:https:)?//[^\"']*common-[^\"']+\\.js|/[^\"']*common-[^\"']+\\.js)",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex SignatureSecretRegex = new(
        "signatureSecret\\s*:\\s*[\"'](?<key>[a-fA-F0-9]{40})[\"']",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private readonly SemaphoreSlim _keyLock = new(1, 1);
    private string? _signatureKey;

    public static MusixMatchSignatureService Shared { get; } = new();

    public async ValueTask<string> GetSignatureKeyAsync(CancellationToken cancellationToken)
    {
        if (this._signatureKey != null) {
            return this._signatureKey;
        }

        await this._keyLock.WaitAsync(cancellationToken);

        try
        {
            if (this._signatureKey != null) {
                return this._signatureKey;
            }

            this._signatureKey = await TryFetchSignatureKeyAsync(cancellationToken) ?? FallbackSignatureKey;
            return this._signatureKey;
        }
        finally
        {
            this._keyLock.Release();
        }
    }

    private static async Task<string?> TryFetchSignatureKeyAsync(CancellationToken cancellationToken)
    {
        try
        {
            Request communityRequest = new Request("https://www.musixmatch.com/community");
            Response communityResponse = await communityRequest.SendAsync(cancellationToken);
            string communityPage = await communityResponse.GetStringAsync(cancellationToken: cancellationToken);
            Match commonScriptMatch = CommonScriptRegex.Match(communityPage);

            if (!commonScriptMatch.Success) {
                return null;
            }

            string commonScriptUrl = NormalizeScriptUrl(commonScriptMatch.Groups["url"].Value);
            Request scriptRequest = new Request(commonScriptUrl);
            Response scriptResponse = await scriptRequest.SendAsync(cancellationToken);
            string scriptContent = await scriptResponse.GetStringAsync(cancellationToken: cancellationToken);
            Match secretMatch = SignatureSecretRegex.Match(scriptContent);

            return secretMatch.Success ? secretMatch.Groups["key"].Value : null;
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return null;
        }
    }

    private static string NormalizeScriptUrl(string scriptUrl)
    {
        if (scriptUrl.StartsWith("//", StringComparison.Ordinal)) {
            return $"https:{scriptUrl}";
        }

        if (scriptUrl.StartsWith("/", StringComparison.Ordinal)) {
            return $"https://www.musixmatch.com{scriptUrl}";
        }

        return scriptUrl;
    }
}
