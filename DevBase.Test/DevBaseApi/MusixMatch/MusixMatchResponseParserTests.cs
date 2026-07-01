namespace DevBase.Test.DevBaseApi.MusixMatch;

public sealed class MusixMatchResponseParserTests
{
    [Test]
    public void Parse_WithSuccessfulEnvelope_ExtractsStatusTimingHeaderAndBody()
    {
        object response = MusixMatchTestSupport.ParseApiResponse(
            MusixMatchTestSupport.ReadFixture("track-get.json"),
            strictErrorHandling: true);

        Assert.Multiple(() =>
        {
            Assert.That(MusixMatchTestSupport.GetInt(response, "StatusCode"), Is.EqualTo(200));
            Assert.That(MusixMatchTestSupport.GetMember(response, "Header")?.ToString(), Does.Contain("execute_time"));
            Assert.That(MusixMatchTestSupport.GetMember(response, "Body")?.ToString(), Does.Contain("Fixture Track"));
        });
    }

    [Test]
    public void Parse_WithFailedEnvelopeAndLenientMode_ReturnsResponseStatus()
    {
        object response = MusixMatchTestSupport.ParseApiResponse(
            MusixMatchTestSupport.ReadFixture("error-404.json"),
            strictErrorHandling: false);

        Assert.That(MusixMatchTestSupport.GetInt(response, "StatusCode"), Is.EqualTo(404));
    }

    [Test]
    public void Parse_WithFailedEnvelopeAndStrictMode_ThrowsMusixmatchRequestException()
    {
        Type exceptionType = MusixMatchTestSupport.ApiType("MusixMatchRequestException");

        var exception = Assert.Throws(
            exceptionType,
            () => MusixMatchTestSupport.ParseApiResponse(
                MusixMatchTestSupport.ReadFixture("error-404.json"),
                strictErrorHandling: true));

        Assert.That(exception, Is.Not.Null);
    }
}
