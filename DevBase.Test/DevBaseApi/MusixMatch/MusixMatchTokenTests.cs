using ApiMusixMatchToken = DevBase.Api.Apis.Musixmatch.MusixMatchToken;

namespace DevBase.Test.DevBaseApi.MusixMatch;

public sealed class MusixMatchTokenTests
{
    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Constructor_WithMissingToken_ThrowsArgumentException(string? token)
    {
        Assert.Throws(Is.InstanceOf<ArgumentException>(), () => new ApiMusixMatchToken(token!));
    }

    [Test]
    public void Tokens_WithSameTokenAndContext_AreEqual()
    {
        ApiMusixMatchToken first = new("token", Api.Apis.Musixmatch.MusixMatchApiContext.Web);
        ApiMusixMatchToken second = new("token", Api.Apis.Musixmatch.MusixMatchApiContext.Web);

        Assert.That(first, Is.EqualTo(second));
    }
}
