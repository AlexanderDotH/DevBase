namespace DevBase.Test.DevBaseApi.RequestHandling;

public sealed class PerMethodRequestSourceTests
{
    [TestCaseSource(typeof(RequestHandlingSourceContract), nameof(RequestHandlingSourceContract.ProviderClientSources))]
    public void ProviderClient_DoesNotUseTheSharedRequestLayer(string sourcePath)
    {
        string source = RequestHandlingSourceContract.ReadSource(sourcePath);
        IReadOnlyList<string> forbiddenTokens = RequestHandlingSourceContract.FindSharedRequestLayerTokens(source);

        Assert.That(
            forbiddenTokens,
            Is.Empty,
            $"{Path.GetFileName(sourcePath)} should own request construction, SendAsync calls, status/text checks, and parsing inside its API methods.");
    }

    [TestCaseSource(typeof(RequestHandlingSourceContract), nameof(RequestHandlingSourceContract.ProviderClientSources))]
    public void ProviderClient_CreatesAndSendsDevBaseNetRequestsDirectly(string sourcePath)
    {
        string source = RequestHandlingSourceContract.ReadProviderOwnedRequestSource(sourcePath);

        Assert.Multiple(() =>
        {
            Assert.That(
                source,
                Does.Contain("new Request("),
                $"{Path.GetFileName(sourcePath)} should construct DevBase.Net Request instances inside provider-owned code.");

            Assert.That(
                source,
                Does.Contain(".SendAsync("),
                $"{Path.GetFileName(sourcePath)} should send requests directly from provider-owned code.");
        });
    }
}
