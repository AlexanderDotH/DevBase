using DevBase.Net.Core;

namespace DevBase.Test.DevBaseApi.MusixMatch;

public sealed class MusixMatchRequestBuilderTests
{
    [Test]
    public async Task BuildUrlAsync_WithUnsignedPostContext_AddsStandardParametersAndEncodedBody()
    {
        Guid guid = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
        Dictionary<string, string?> query = new()
        {
            ["format"] = "xml",
            ["empty"] = "",
            ["persist"] = "yes"
        };
        Dictionary<string, string?> data = new()
        {
            ["credential_list"] = "[]",
            ["unused"] = ""
        };

        Request request = await MusixMatchTestSupport.BuildRequestAsync(
            "AccountPost",
            "AuthWeb",
            "test-token",
            guid,
            query,
            data);

        Uri uri = request.GetUri()!;
        IReadOnlyDictionary<string, string> parameters = MusixMatchTestSupport.QueryParameters(uri);
        string body = MusixMatchTestSupport.RequestBody(request);

        Assert.Multiple(() =>
        {
            Assert.That(request.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(uri.ToString(), Does.StartWith("https://account.musixmatch.com/ws/1.1/account.post?"));
            Assert.That(parameters["format"], Is.EqualTo("json"));
            Assert.That(parameters["usertoken"], Is.EqualTo("test-token"));
            Assert.That(parameters["guid"], Is.EqualTo(guid.ToString()));
            Assert.That(parameters["persist"], Is.EqualTo("yes"));
            Assert.That(parameters.ContainsKey("app_id"), Is.False);
            Assert.That(parameters.ContainsKey("empty"), Is.False);
            Assert.That(body, Is.EqualTo("credential_list=%5B%5D"));
        });
    }

    [Test]
    public async Task BuildUrlAsync_WithSignedDesktopContext_AddsDeterministicHmacSignature()
    {
        const string fallbackKey = "741941edc264ea6293cb9a6458103b4eda3ac8ed";
        Guid guid = Guid.Parse("11111111-2222-3333-4444-555555555555");
        Dictionary<string, string?> query = new()
        {
            ["track_id"] = "42",
            ["ignored"] = ""
        };

        Request request = await MusixMatchTestSupport.BuildRequestAsync(
            "TrackGet",
            "Desktop",
            "signed-token",
            guid,
            query,
            primeSignatureKey: true);

        Uri uri = request.GetUri()!;
        string absoluteUri = uri.AbsoluteUri;
        int signatureIndex = absoluteUri.IndexOf("&signature=", StringComparison.Ordinal);
        Assert.That(signatureIndex, Is.GreaterThan(0), "Signed URLs should append a signature parameter after regular query parameters.");

        string unsignedUrl = absoluteUri[..signatureIndex];
        IReadOnlyDictionary<string, string> parameters = MusixMatchTestSupport.QueryParameters(uri);

        Assert.Multiple(() =>
        {
            Assert.That(request.Method, Is.EqualTo(HttpMethod.Get));
            Assert.That(unsignedUrl, Does.StartWith("https://apic-desktop.musixmatch.com/ws/1.1/track.get?"));
            Assert.That(parameters["format"], Is.EqualTo("json"));
            Assert.That(parameters["app_id"], Is.EqualTo("web-desktop-app-v1.0"));
            Assert.That(parameters["usertoken"], Is.EqualTo("signed-token"));
            Assert.That(parameters["guid"], Is.EqualTo(guid.ToString()));
            Assert.That(parameters["track_id"], Is.EqualTo("42"));
            Assert.That(parameters.ContainsKey("ignored"), Is.False);
            Assert.That(parameters["signature_protocol"], Is.EqualTo("sha1"));
            Assert.That(parameters["signature"], Is.EqualTo(MusixMatchTestSupport.CreateSignature(unsignedUrl, fallbackKey)));
        });
    }

    [Test]
    public async Task BuildUrlAsync_WithChartArtistsEndpoint_UsesArtistsResource()
    {
        Request request = await MusixMatchTestSupport.BuildRequestAsync(
            "ChartArtistsGet",
            "Desktop",
            "test-token",
            Guid.Parse("99999999-8888-7777-6666-555555555555"),
            primeSignatureKey: true);

        Uri uri = request.GetUri()!;

        Assert.That(uri.AbsolutePath, Does.EndWith("/chart.artists.get"));
    }
}
