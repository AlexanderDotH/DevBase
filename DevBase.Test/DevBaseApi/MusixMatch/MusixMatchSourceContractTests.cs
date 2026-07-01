using System.Text.RegularExpressions;

namespace DevBase.Test.DevBaseApi.MusixMatch;

public sealed class MusixMatchSourceContractTests
{
    private static readonly string[] UnifiedSendHelpers =
    [
        "SendMusixmatchRequestAsync",
        "SendMusixMatchRequestAsync",
        "SendMusixMatchBodyAsync",
        "BuildMusixMatchRequestAsync",
        "BuildMusixMatchApiRequestAsync",
        "BuildMusixMatchApiRawRequestAsync"
    ];

    public static IEnumerable<TestCaseData> EndpointMethodCases()
    {
        foreach (string path in FeatureSourcePaths())
        {
            string source = File.ReadAllText(path);

            foreach (SourceMethod method in ExtractPublicAsyncMethods(source).Where(IsEndpointMethod))
            {
                string name = $"{Path.GetFileNameWithoutExtension(path)}_{method.Name}_owns_request";
                yield return new TestCaseData(path, method.Name, method.Body).SetName(SanitizeTestName(name));
            }
        }
    }

    [Test]
    public void MusixMatchCore_DoesNotExposeUnifiedSendOrRequestBuildHelpers()
    {
        string source = MusixMatchTestSupport.ReadProductionSource("MusixMatch.cs");
        IReadOnlyList<string> forbiddenHelpers = FindUnifiedSendHelperCalls(source);

        Assert.That(
            forbiddenHelpers,
            Is.Empty,
            "MusixMatch core should expose URL/body and response-parse helpers, not unified send or request-build helpers.");
    }

    [Test]
    public void MusixMatchCore_UrlAndParseHelpersDoNotCreateOrSendRequests()
    {
        string source = MusixMatchTestSupport.ReadProductionSource("MusixMatch.cs");
        string helperSource = SourceBetween(
            source,
            "internal async Task<string> BuildMusixMatchUrlAsync",
            "internal static List<TOut> SelectList");

        Assert.Multiple(() =>
        {
            Assert.That(helperSource, Does.Contain("BuildMusixMatchUrlAsync"));
            Assert.That(helperSource, Does.Contain("ParseMusixMatchResponseAsync"));
            Assert.That(helperSource, Does.Contain("ParseMusixMatchBody"));
            Assert.That(helperSource, Does.Not.Contain("new Request("));
            Assert.That(helperSource, Does.Not.Contain(".SendAsync("));
        });
    }

    [TestCaseSource(nameof(EndpointMethodCases))]
    public void MusixMatchEndpointMethod_DoesNotCallUnifiedSendHelpers(
        string sourcePath,
        string methodName,
        string methodBody)
    {
        IReadOnlyList<string> forbiddenHelpers = FindUnifiedSendHelperCalls(methodBody);

        Assert.That(
            forbiddenHelpers,
            Is.Empty,
            $"{Path.GetFileName(sourcePath)}::{methodName} should build a Request, call SendAsync locally, then parse the response.");
    }

    [TestCaseSource(nameof(EndpointMethodCases))]
    public void MusixMatchEndpointMethod_BuildsRequestAndCallsSendAsyncLocally(
        string sourcePath,
        string methodName,
        string methodBody)
    {
        Assert.Multiple(() =>
        {
            Assert.That(
                methodBody,
                Does.Match(@"\bRequest\s+\w+\s*=\s*new\s+Request\s*\("),
                $"{Path.GetFileName(sourcePath)}::{methodName} should instantiate a DevBase.Net.Core.Request in the endpoint method.");

            Assert.That(
                methodBody,
                Does.Contain(".SendAsync("),
                $"{Path.GetFileName(sourcePath)}::{methodName} should send its request locally.");
        });
    }

    private static IEnumerable<string> FeatureSourcePaths()
    {
        string directory = Path.Combine(
            MusixMatchTestSupport.RepositoryRoot,
            "DevBase.Api",
            "Apis",
            "Musixmatch");

        IEnumerable<string> topLevelFeatureSources = Directory
            .EnumerateFiles(directory, "MusixMatch.*.cs", SearchOption.TopDirectoryOnly)
            .Where(path => !path.EndsWith("MusixMatch.Missions.cs", StringComparison.Ordinal))
            .OrderBy(path => path, StringComparer.Ordinal);

        string missionClient = Path.Combine(directory, "Missions", "MusixMatchMissionClient.cs");
        return topLevelFeatureSources.Append(missionClient);
    }

    private static IEnumerable<SourceMethod> ExtractPublicAsyncMethods(string source)
    {
        int searchIndex = 0;

        while (true)
        {
            int signatureStart = source.IndexOf("public async Task", searchIndex, StringComparison.Ordinal);

            if (signatureStart < 0) {
                yield break;
            }

            int bodyStart = source.IndexOf('{', signatureStart);

            if (bodyStart < 0) {
                yield break;
            }

            int bodyEnd = FindClosingBrace(source, bodyStart);

            if (bodyEnd < 0) {
                yield break;
            }

            string signature = source[signatureStart..bodyStart];
            Match name = Regex.Match(
                signature,
                @"\b(?<name>[A-Za-z_][A-Za-z0-9_]*)\s*\([^)]*\)\s*$",
                RegexOptions.Singleline);

            if (name.Success) {
                yield return new SourceMethod(name.Groups["name"].Value, source[bodyStart..(bodyEnd + 1)]);
            }

            searchIndex = bodyEnd + 1;
        }
    }

    private static bool IsEndpointMethod(SourceMethod method)
    {
        return method.Body.Contains("MusixMatchEndpoint.", StringComparison.Ordinal)
            || method.Body.Contains("MusixmatchEndpoint.", StringComparison.Ordinal)
            || method.Body.Contains("Enums.MusixmatchEndpoint.", StringComparison.Ordinal)
            || method.Body.Contains("BuildMusixMatchUrlAsync(", StringComparison.Ordinal)
            || method.Body.Contains("BuildGraphQlPayload(", StringComparison.Ordinal)
            || UnifiedSendHelpers.Any(helper => method.Body.Contains(helper, StringComparison.Ordinal));
    }

    private static IReadOnlyList<string> FindUnifiedSendHelperCalls(string source)
    {
        return UnifiedSendHelpers
            .Where(helper => Regex.IsMatch(source, $@"\b{Regex.Escape(helper)}\s*(?:<|\()", RegexOptions.CultureInvariant))
            .ToArray();
    }

    private static int FindClosingBrace(string source, int openingBraceIndex)
    {
        int depth = 0;

        for (int index = openingBraceIndex; index < source.Length; index++)
        {
            char current = source[index];

            if (current == '{') {
                depth++;
            }

            if (current != '}') {
                continue;
            }

            depth--;

            if (depth == 0) {
                return index;
            }
        }

        return -1;
    }

    private static string SourceBetween(string source, string startMarker, string endMarker)
    {
        int start = source.IndexOf(startMarker, StringComparison.Ordinal);
        Assert.That(start, Is.GreaterThanOrEqualTo(0), $"Expected source marker: {startMarker}");

        int end = source.IndexOf(endMarker, start, StringComparison.Ordinal);
        Assert.That(end, Is.GreaterThan(start), $"Expected source marker after {startMarker}: {endMarker}");

        return source[start..end];
    }

    private static string SanitizeTestName(string name)
    {
        return Regex.Replace(name, "[^A-Za-z0-9_]+", "_");
    }

    private sealed record SourceMethod(string Name, string Body);
}
