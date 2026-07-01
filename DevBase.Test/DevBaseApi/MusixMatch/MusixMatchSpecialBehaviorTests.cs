using System.Reflection;
using DevBase.Net.Core;

namespace DevBase.Test.DevBaseApi.MusixMatch;

public sealed class MusixMatchSpecialBehaviorTests
{
    public static IEnumerable<TestCaseData> EmptySingleResourceResponseCases()
    {
        yield return EmptyResourceCase(
            "RequireTrackLyricsResponse",
            "Structure.Requests.TrackLyricsGet",
            "Lyrics",
            "Structure.Types.Lyrics",
            "lyrics",
            202);

        yield return EmptyResourceCase(
            "RequireArtistResponse",
            "Structure.Requests.ArtistGet",
            "Artist",
            "Structure.Types.Artist",
            "artist",
            902);

        yield return EmptyResourceCase(
            "RequireAlbumResponse",
            "Structure.Requests.AlbumGet",
            "Album",
            "Structure.Types.Album",
            "album",
            702);

        yield return EmptyResourceCase(
            "RequireSubtitleResponse",
            "Structure.Requests.TrackSubtitleGet",
            "Subtitle",
            "Structure.Types.SubtitleRawResponse",
            "subtitle",
            202);

        yield return EmptyResourceCase(
            "RequireRichsyncResponse",
            "Structure.Requests.TrackRichsyncGet",
            "Richsync",
            "Structure.Types.RichsyncRawResponse",
            "richsync",
            202);

        yield return EmptyResourceCase(
            "RequireUserInfoResponse",
            "Structure.Requests.UserGet",
            "UserData",
            "Structure.Requests.UserGet+UserAccount",
            "user",
            null);

        yield return EmptyResourceCase(
            "RequireOauthTokenResponse",
            "Structure.Requests.OauthTokenGet",
            "Token",
            "Structure.Requests.OauthTokenGet+Oauthtoken",
            "oauth token",
            null);

        yield return EmptyResourceCase(
            "RequireUserScoreResponse",
            "Structure.Requests.CrowdScoreGet",
            "User",
            "Structure.Types.User",
            "user score",
            null);
    }

    [Test]
    public async Task ClientUrl_WithAuthWebAccountPost_OmitsUserTokenForAnonymousAccountRequest()
    {
        string url = await MusixMatchTestSupport.BuildClientUrlAsync(
            "client-token",
            "Desktop",
            "AccountPost",
            requestContextName: "AuthWeb");

        IReadOnlyDictionary<string, string> parameters = MusixMatchTestSupport.QueryParameters(new Uri(url));

        Assert.Multiple(() =>
        {
            Assert.That(url, Does.StartWith("https://account.musixmatch.com/ws/1.1/account.post?"));
            Assert.That(parameters.ContainsKey("usertoken"), Is.False);
            Assert.That(parameters.ContainsKey("app_id"), Is.False);
            Assert.That(parameters["format"], Is.EqualTo("json"));
        });
    }

    [Test]
    public async Task ClientUrl_WithAuthWebCredentialPost_KeepsUserTokenForCredentialRequest()
    {
        string url = await MusixMatchTestSupport.BuildClientUrlAsync(
            "client-token",
            "Desktop",
            "CredentialPost",
            requestContextName: "AuthWeb");

        IReadOnlyDictionary<string, string> parameters = MusixMatchTestSupport.QueryParameters(new Uri(url));

        Assert.Multiple(() =>
        {
            Assert.That(url, Does.StartWith("https://account.musixmatch.com/ws/1.1/credential.post?"));
            Assert.That(parameters["usertoken"], Is.EqualTo("client-token"));
            Assert.That(parameters.ContainsKey("app_id"), Is.False);
        });
    }

    [Test]
    public async Task BuildUrlAsync_QueryCannotOverrideStandardIdentityParameters()
    {
        Guid guid = Guid.Parse("11111111-2222-3333-4444-555555555555");
        Dictionary<string, string?> query = new()
        {
            ["format"] = "xml",
            ["app_id"] = "wrong-app",
            ["usertoken"] = "wrong-token",
            ["guid"] = "wrong-guid",
            ["track_id"] = "42"
        };

        Request request = await MusixMatchTestSupport.BuildRequestAsync(
            "TrackGet",
            "Desktop",
            "correct-token",
            guid,
            query,
            primeSignatureKey: true);

        IReadOnlyDictionary<string, string> parameters = MusixMatchTestSupport.QueryParameters(request.GetUri()!);

        Assert.Multiple(() =>
        {
            Assert.That(parameters["format"], Is.EqualTo("json"));
            Assert.That(parameters["app_id"], Is.EqualTo("web-desktop-app-v1.0"));
            Assert.That(parameters["usertoken"], Is.EqualTo("correct-token"));
            Assert.That(parameters["guid"], Is.EqualTo(guid.ToString()));
            Assert.That(parameters["track_id"], Is.EqualTo("42"));
        });
    }

    [Test]
    public void BuildBody_WithJsonPayload_UsesJsonContentType()
    {
        string body = "{\"credential_list\":[]}";

        string contentType = MusixMatchTestSupport.GetContentType(body);

        Assert.That(contentType, Is.EqualTo("application/json"));
    }

    [Test]
    public void BuildBody_WithFormPayload_EncodesValuesAndUsesFormContentType()
    {
        Dictionary<string, string?> data = new()
        {
            ["translation"] = "nie aufgeben",
            ["empty"] = "",
            ["language"] = "de"
        };

        string body = MusixMatchTestSupport.BuildBody(data);
        string contentType = MusixMatchTestSupport.GetContentType(body);

        Assert.Multiple(() =>
        {
            Assert.That(body, Is.EqualTo("translation=nie%20aufgeben&language=de"));
            Assert.That(contentType, Is.EqualTo("application/x-www-form-urlencoded"));
        });
    }

    [Test]
    public void Parse_WithNestedEnvelopeFallback_ExtractsHeaderAndBody()
    {
        const string json = """
            {
              "outer": {
                "message": {
                  "header": {
                    "status_code": 200,
                    "execute_time": 0.42
                  },
                  "body": {
                    "track": {
                      "track_name": "Nested Track"
                    }
                  }
                }
              }
            }
            """;

        object response = MusixMatchTestSupport.ParseApiResponse(json, strictErrorHandling: true);

        Assert.Multiple(() =>
        {
            Assert.That(MusixMatchTestSupport.GetInt(response, "StatusCode"), Is.EqualTo(200));
            Assert.That(MusixMatchTestSupport.GetMember(response, "Header")?.ToString(), Does.Contain("execute_time"));
            Assert.That(MusixMatchTestSupport.GetMember(response, "Body")?.ToString(), Does.Contain("Nested Track"));
        });
    }

    [Test]
    public void Parse_WithMissingEnvelope_ReturnsUndefinedStatusAndEmptyBodyInLenientMode()
    {
        object response = MusixMatchTestSupport.ParseApiResponse("{}", strictErrorHandling: false);

        Assert.Multiple(() =>
        {
            Assert.That(MusixMatchTestSupport.GetInt(response, "StatusCode"), Is.EqualTo(0));
            Assert.That(MusixMatchTestSupport.GetMember(response, "Header")?.ToString(), Is.EqualTo("{}"));
            Assert.That(MusixMatchTestSupport.GetMember(response, "Body")?.ToString(), Is.EqualTo("{}"));
        });
    }

    [TestCase(false)]
    [TestCase(true)]
    public void GetTrackByIdResponse_WithMissingOrEmptyTrackData_Throws(bool includeEmptyTrackObject)
    {
        object response = Activator.CreateInstance(MusixMatchTestSupport.ApiType("Structure.Requests.TrackGet"))!;

        if (includeEmptyTrackObject) {
            object track = Activator.CreateInstance(MusixMatchTestSupport.ApiType("Structure.Types.Track"))!;
            response.GetType().GetProperty("Track")!.SetValue(response, track);
        }

        MethodInfo requireTrack = MusixMatchTestSupport.ApiType("MusixMatch")
            .GetMethod("RequireTrackByIdResponse", BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new AssertionException("MusixMatch.RequireTrackByIdResponse was not found.");
        Type exceptionType = MusixMatchTestSupport.ApiType("MusixMatchEmptyTrackDataException");

        var exception = Assert.Throws(exceptionType, () =>
            InvokeRequireTrack(requireTrack, response, 202))!;

        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Does.Contain("track id 202"));
            Assert.That(MusixMatchTestSupport.GetInt(exception, "TrackId"), Is.EqualTo(202));
        });
    }

    [TestCaseSource(nameof(EmptySingleResourceResponseCases))]
    public void SingleResourceResponse_WithMissingOrEmptyData_Throws(
        string methodName,
        string responseTypeName,
        string responsePropertyName,
        string resourceTypeName,
        string resourceName,
        int? resourceId)
    {
        Type exceptionType = MusixMatchTestSupport.ApiType("MusixMatchEmptyResponseDataException");

        foreach (bool includeEmptyResourceObject in new[] { false, true })
        {
            object response = Activator.CreateInstance(MusixMatchTestSupport.ApiType(responseTypeName))!;

            if (includeEmptyResourceObject) {
                object resource = Activator.CreateInstance(MusixMatchTestSupport.ApiType(resourceTypeName))!;
                response.GetType().GetProperty(responsePropertyName)!.SetValue(response, resource);
            }

            MethodInfo requireResponse = MusixMatchTestSupport.ApiType("MusixMatch")
                .GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic)
                ?? throw new AssertionException($"MusixMatch.{methodName} was not found.");
            object?[] arguments = resourceId.HasValue
                ? [response, resourceId.Value]
                : [response];

            var exception = Assert.Throws(exceptionType, () => InvokeRequireMethod(requireResponse, arguments))!;

            Assert.Multiple(() =>
            {
                Assert.That(exception.Message, Does.Contain(resourceName));
                Assert.That(MusixMatchTestSupport.GetString(exception, "ResourceName"), Is.EqualTo(resourceName));
            });
        }
    }

    private static TestCaseData EmptyResourceCase(
        string methodName,
        string responseTypeName,
        string responsePropertyName,
        string resourceTypeName,
        string resourceName,
        int? resourceId)
    {
        return new TestCaseData(
                methodName,
                responseTypeName,
                responsePropertyName,
                resourceTypeName,
                resourceName,
                resourceId)
            .SetName($"{methodName}_throws_for_empty_{resourceName.Replace(' ', '_')}");
    }

    private static void InvokeRequireTrack(MethodInfo requireTrack, object response, int trackId)
    {
        InvokeRequireMethod(requireTrack, [response, trackId]);
    }

    private static void InvokeRequireMethod(MethodInfo requireMethod, object?[] arguments)
    {
        try
        {
            requireMethod.Invoke(null, arguments);
        }
        catch (TargetInvocationException exception) when (exception.InnerException != null)
        {
            throw exception.InnerException;
        }
    }
}
