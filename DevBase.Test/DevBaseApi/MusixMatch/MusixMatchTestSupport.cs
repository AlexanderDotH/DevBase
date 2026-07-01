using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using DevBase.Net.Core;
using Newtonsoft.Json;

namespace DevBase.Test.DevBaseApi.MusixMatch;

internal static class MusixMatchTestSupport
{
    private const string ApiRootNamespace = "DevBase.Api.Apis.Musixmatch";

    public static string RepositoryRoot { get; } = FindRepositoryRoot();

    public static Assembly ApiAssembly => typeof(Api.Apis.Musixmatch.MusixMatch).Assembly;

    public static Type ApiType(string name)
    {
        string fullName = name.StartsWith("DevBase.", StringComparison.Ordinal)
            ? name
            : $"{ApiRootNamespace}.{name}";

        return ApiAssembly.GetType(fullName)
            ?? throw new AssertionException($"Expected API type to exist: {fullName}");
    }

    public static string ReadProductionSource(params string[] relativeParts)
    {
        string path = Path.Combine(
            [RepositoryRoot, "DevBase.Api", "Apis", "Musixmatch", .. relativeParts]);

        Assert.That(File.Exists(path), Is.True, $"Expected production source to exist: {path}");
        return File.ReadAllText(path);
    }

    public static string ReadAllMusixMatchSources()
    {
        string directory = Path.Combine(RepositoryRoot, "DevBase.Api", "Apis", "Musixmatch");
        return string.Join(
            Environment.NewLine,
            Directory.EnumerateFiles(directory, "MusixMatch*.cs", SearchOption.TopDirectoryOnly)
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(File.ReadAllText));
    }

    public static string ReadFixture(string fileName)
    {
        string path = Path.Combine(RepositoryRoot, "DevBase.Test", "DevBaseApi", "MusixMatch", "Fixtures", fileName);
        Assert.That(File.Exists(path), Is.True, $"Expected fixture to exist: {path}");
        return File.ReadAllText(path);
    }

    public static object DeserializeFixtureBody(string fixtureName, string dtoTypeName)
    {
        object response = ParseApiResponse(ReadFixture(fixtureName), false);
        string body = (string)(GetMember(response, "Body") ?? throw new AssertionException("Parsed response body was null."));
        Type dtoType = ApiType(dtoTypeName);
        return JsonConvert.DeserializeObject(body, dtoType)
            ?? throw new AssertionException($"Fixture {fixtureName} did not deserialize into {dtoType.FullName}.");
    }

    public static object ParseApiResponse(string json, bool strictErrorHandling)
    {
        Type parserType = ApiType("Internal.MusixMatchResponseParser");
        MethodInfo parse = parserType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static)
            ?? throw new AssertionException("MusixMatchResponseParser.Parse was not found.");

        try
        {
            return parse.Invoke(null, [json, strictErrorHandling])
                ?? throw new AssertionException("MusixMatchResponseParser.Parse returned null.");
        }
        catch (TargetInvocationException exception) when (exception.InnerException != null)
        {
            throw exception.InnerException;
        }
    }

    public static async Task<string> BuildClientUrlAsync(
        string token,
        string clientContextName,
        string endpointName,
        IDictionary<string, string?>? query = null,
        string? requestContextName = null)
    {
        Type clientType = ApiType("MusixMatch");
        Type contextType = ApiType("MusixMatchApiContext");
        object clientContext = Enum.Parse(contextType, clientContextName);
        object endpoint = Enum.Parse(ApiType("MusixMatchEndpoint"), endpointName);
        object client = Activator.CreateInstance(clientType, [token, clientContext])
            ?? throw new AssertionException("Could not create MusixMatch client.");
        object? requestContext = requestContextName == null
            ? null
            : Enum.Parse(contextType, requestContextName);
        MethodInfo buildUrlAsync = clientType
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(method =>
            {
                ParameterInfo[] parameters = method.GetParameters();
                return method.Name == "BuildMusixMatchUrlAsync"
                    && parameters.Length == 4
                    && parameters[0].ParameterType.Name == "MusixMatchEndpoint";
            });

        object taskObject = buildUrlAsync.Invoke(
            client,
            [endpoint, query, requestContext, CancellationToken.None])
            ?? throw new AssertionException("BuildMusixMatchUrlAsync returned null.");

        await (Task)taskObject;

        return (string)taskObject.GetType().GetProperty("Result")!.GetValue(taskObject)!;
    }

    public static string BuildBody(IDictionary<string, string?>? parameters)
    {
        MethodInfo buildBody = ApiType("MusixMatch")
            .GetMethod("BuildMusixMatchBody", BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new AssertionException("MusixMatch.BuildMusixMatchBody was not found.");

        return (string)buildBody.Invoke(null, [parameters])!;
    }

    public static string GetContentType(string body)
    {
        MethodInfo getContentType = ApiType("MusixMatch")
            .GetMethod("GetMusixMatchContentType", BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new AssertionException("MusixMatch.GetMusixMatchContentType was not found.");

        return (string)getContentType.Invoke(null, [body])!;
    }

    public static async Task<Request> BuildRequestAsync(
        string endpointName,
        string contextName,
        string userToken,
        Guid guid,
        IDictionary<string, string?>? query = null,
        IDictionary<string, string?>? data = null,
        bool primeSignatureKey = false)
    {
        Type signatureServiceType = ApiType("Internal.MusixMatchSignatureService");
        object signatureService = signatureServiceType
            .GetProperty("Shared", BindingFlags.Public | BindingFlags.Static)!
            .GetValue(null)!;

        if (primeSignatureKey)
        {
            FieldInfo signatureKey = signatureServiceType.GetField("_signatureKey", BindingFlags.NonPublic | BindingFlags.Instance)
                ?? throw new AssertionException("MusixMatchSignatureService._signatureKey was not found.");
            signatureKey.SetValue(signatureService, "741941edc264ea6293cb9a6458103b4eda3ac8ed");
        }

        Type builderType = ApiType("Internal.MusixMatchRequestBuilder");
        object builder = Activator.CreateInstance(
            builderType,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            args: [signatureService],
            culture: CultureInfo.InvariantCulture)
            ?? throw new AssertionException("Could not create MusixMatchRequestBuilder.");

        object endpoint = Enum.Parse(ApiType("MusixMatchEndpoint"), endpointName);
        object context = Enum.Parse(ApiType("MusixMatchApiContext"), contextName);
        object contextDefinition = ApiType("Internal.MusixMatchApiContextDefinition")
            .GetMethod("For", BindingFlags.Public | BindingFlags.Static)!
            .Invoke(null, [context])!;

        MethodInfo buildUrlAsync = builderType.GetMethod("BuildUrlAsync", BindingFlags.Public | BindingFlags.Instance)
            ?? throw new AssertionException("MusixMatchRequestBuilder.BuildUrlAsync was not found.");

        object taskObject = buildUrlAsync.Invoke(
            builder,
            [endpoint, contextDefinition, userToken, guid, query, CancellationToken.None])
            ?? throw new AssertionException("BuildUrlAsync returned null.");

        await (Task)taskObject;

        string url = (string)taskObject.GetType().GetProperty("Result")!.GetValue(taskObject)!;
        Request request = new Request(url);

        if (data == null) {
            return request;
        }

        MethodInfo buildBody = builderType.GetMethod("BuildBody", BindingFlags.Public | BindingFlags.Static)
            ?? throw new AssertionException("MusixMatchRequestBuilder.BuildBody was not found.");
        MethodInfo getContentType = builderType.GetMethod("GetContentType", BindingFlags.Public | BindingFlags.Static)
            ?? throw new AssertionException("MusixMatchRequestBuilder.GetContentType was not found.");
        string body = (string)buildBody.Invoke(null, [data])!;

        request.AsPost();

        if (body.Length == 0) {
            return request;
        }

        return request
            .WithRawBody(body, Encoding.UTF8)
            .WithHeader("Content-Type", (string)getContentType.Invoke(null, [body])!);
    }

    public static IReadOnlyDictionary<string, string> QueryParameters(Uri uri)
    {
        return uri.Query
            .TrimStart('?')
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(part => part.Split('=', 2))
            .ToDictionary(
                part => Uri.UnescapeDataString(part[0]),
                part => part.Length == 2 ? Uri.UnescapeDataString(part[1]) : string.Empty,
                StringComparer.Ordinal);
    }

    public static object? GetMember(object? instance, string name)
    {
        if (instance == null) {
            return null;
        }

        Type type = instance.GetType();
        PropertyInfo? property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);

        if (property != null) {
            return property.GetValue(instance);
        }

        FieldInfo? field = type.GetField(name, BindingFlags.Public | BindingFlags.Instance);
        return field?.GetValue(instance);
    }

    public static object? GetFirstExistingMember(object instance, params string[] names)
    {
        foreach (string name in names)
        {
            object? value = GetMember(instance, name);

            if (value != null) {
                return value;
            }
        }

        return null;
    }

    public static IReadOnlyList<object> GetList(object instance, string name)
    {
        object? value = GetMember(instance, name);
        Assert.That(value, Is.Not.Null, $"Expected {instance.GetType().Name}.{name} to be populated.");

        return ((IEnumerable)value!)
            .Cast<object>()
            .ToArray();
    }

    public static string? GetString(object instance, string name)
    {
        return GetMember(instance, name) as string;
    }

    public static int GetInt(object instance, string name)
    {
        object? value = GetMember(instance, name);
        Assert.That(value, Is.Not.Null, $"Expected {instance.GetType().Name}.{name} to be populated.");
        return Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }

    public static string RequestBody(Request request)
    {
        return Encoding.UTF8.GetString(request.Body.ToArray());
    }

    public static string CreateSignature(string unsignedUrl, string key)
    {
        string input = unsignedUrl + DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        byte[] keyBytes = Encoding.ASCII.GetBytes(key);
        byte[] inputBytes = Encoding.ASCII.GetBytes(input);

        using HMACSHA1 hmac = new(keyBytes);
        return Convert.ToBase64String(hmac.ComputeHash(inputBytes));
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
