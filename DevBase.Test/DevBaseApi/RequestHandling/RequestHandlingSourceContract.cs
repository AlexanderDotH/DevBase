using System.Text.RegularExpressions;
using NUnit.Framework;

namespace DevBase.Test.DevBaseApi.RequestHandling;

internal static class RequestHandlingSourceContract
{
    public static readonly string[] SharedRequestLayerTokens =
    [
        "ApiClientOptions",
        "ApiHttpResponse",
        "ApiResponsePolicy",
        "DevBase.Api.Requests",
        "DevBaseNetRequestExecutor",
        "ExecuteRequestAsync",
        "IApiRequestExecutor",
        "RequestExecutor",
        "CreateRequest",
        "SendMusixmatchRequestAsync",
        "SendMusixMatchRequestAsync",
        "SendMusixMatchBodyAsync",
        "BuildMusixMatchRequestAsync",
        "BuildMusixMatchApiRequestAsync",
        "BuildMusixMatchApiRawRequestAsync",
        "SendJsonAsync",
        "SendStringAsync",
        "SendBytesAsync"
    ];

    private static readonly string[] ProviderClientRelativePaths =
    [
        "AppleMusic/AppleMusic.cs",
        "BeautifulLyrics/BeautifulLyrics.cs",
        "Deezer/Deezer.cs",
        "Musixmatch/MusixMatch.cs",
        "NetEase/NetEase.cs",
        "Tidal/Tidal.cs"
    ];

    public static string RepositoryRoot { get; } = FindRepositoryRoot();

    public static string ApiClientSourcePath => Path.Combine(
        RepositoryRoot,
        "DevBase.Api",
        "Apis",
        "ApiClient.cs");

    public static IEnumerable<TestCaseData> ProviderClientSources()
    {
        foreach (string relativePath in ProviderClientRelativePaths)
        {
            string path = Path.Combine(RepositoryRoot, "DevBase.Api", "Apis", relativePath);
            string name = Regex.Replace(Path.GetFileNameWithoutExtension(path), "[^A-Za-z0-9_]+", "_");
            yield return new TestCaseData(path).SetName($"{name}_uses_per_method_requests");
        }
    }

    public static string ReadSource(string path)
    {
        Assert.That(File.Exists(path), Is.True, $"Expected source file to exist: {path}");
        return File.ReadAllText(path);
    }

    public static string ReadProviderOwnedRequestSource(string path)
    {
        string source = ReadSource(path);

        if (!path.EndsWith(Path.Combine("Musixmatch", "MusixMatch.cs"), StringComparison.Ordinal)) {
            return source;
        }

        string builderPath = Path.Combine(
            RepositoryRoot,
            "DevBase.Api",
            "Apis",
            "Musixmatch",
            "Internal",
            "MusixMatchRequestBuilder.cs");

        return source + Environment.NewLine + ReadSource(builderPath);
    }

    public static IReadOnlyList<string> FindSharedRequestLayerTokens(string source)
    {
        return SharedRequestLayerTokens
            .Where(token => source.Contains(token, StringComparison.Ordinal))
            .ToArray();
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(TestContext.CurrentContext.TestDirectory);

        while (directory != null)
        {
            bool hasApiProject = Directory.Exists(Path.Combine(directory.FullName, "DevBase.Api"));
            bool hasTestProject = Directory.Exists(Path.Combine(directory.FullName, "DevBase.Test"));

            if (hasApiProject && hasTestProject) {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new AssertionException("Could not locate DevBase repository root from the test output directory.");
    }
}
