using DevBase.Api.Apis;

namespace DevBase.Test.DevBaseApi.RequestHandling;

public sealed class ApiClientContractTests
{
    [Test]
    public void NewApiClient_UsesLenientErrorHandlingByDefault()
    {
        ErrorHandlingProbeApiClient client = new();

        Assert.Multiple(() =>
        {
            Assert.That(client.StrictErrorHandling, Is.False);
            Assert.That(client.InvokeThrowString(new InvalidOperationException("lenient")), Is.Empty);
            Assert.That(client.InvokeThrowObject(new InvalidOperationException("lenient")), Is.Null);

            (string rawLyrics, bool isRichSync) tuple = client.InvokeThrowTuple(new InvalidOperationException("lenient"));
            Assert.That(tuple.rawLyrics, Is.Empty);
            Assert.That(tuple.isRichSync, Is.False);
        });
    }

    [Test]
    public void StrictErrorHandling_ThrowsTheOriginalException()
    {
        InvalidOperationException expected = new("strict");
        ErrorHandlingProbeApiClient client = new()
        {
            StrictErrorHandling = true
        };

        InvalidOperationException? actual = Assert.Throws<InvalidOperationException>(
            () => client.InvokeThrowObject(expected));

        Assert.That(actual, Is.SameAs(expected));
    }

    [Test]
    public void ApiClientSource_RemainsOnlyAnErrorHandlingBase()
    {
        string source = RequestHandlingSourceContract.ReadSource(RequestHandlingSourceContract.ApiClientSourcePath);
        IReadOnlyList<string> forbiddenTokens = RequestHandlingSourceContract.FindSharedRequestLayerTokens(source);

        Assert.That(
            forbiddenTokens,
            Is.Empty,
            "ApiClient should stay limited to strict/lenient error defaults. Request construction, sending, response checks, and parsing belong in each API method.");
    }

    private sealed class ErrorHandlingProbeApiClient : ApiClient
    {
        public string InvokeThrowString(System.Exception exception)
        {
            return Throw<string>(exception);
        }

        public object? InvokeThrowObject(System.Exception exception)
        {
            return Throw<object>(exception);
        }

        public (string RawLyrics, bool IsRichSync) InvokeThrowTuple(System.Exception exception)
        {
            return ThrowTuple(exception);
        }
    }
}
