using System.Text;
using DevBase.Generics;
using DevBase.Typography;
using DevBase.Typography.Encoded;

namespace DevBase.IntegrationTests.Core;

[Category("Integration")]
[Category("Library")]
public sealed class CoreCollectionsTypographyIntegrationTests
{
    [Test]
    public void AStringLinesCanFlowThroughAListAndBase64Encoding()
    {
        AString source = new("alpha\nbeta\ngamma");
        AList<string> lines = source.AsList();
        AList<AList<string>> chunks = lines.Slice(2);
        string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(source.ToString()));
        Base64EncodedAString base64 = new(encoded);

        Assert.Multiple(() =>
        {
            Assert.That(lines.Length, Is.EqualTo(3));
            Assert.That(lines.Get(1), Is.EqualTo("beta"));
            Assert.That(chunks.Length, Is.EqualTo(2));
            Assert.That(chunks.Get(0).Get(1), Is.EqualTo("beta"));
            Assert.That(base64.GetDecoded().ToString(), Is.EqualTo(source.ToString()));
            Assert.That(base64.UrlEncoded().UrlDecoded().GetDecoded().ToString(), Is.EqualTo(source.ToString()));
        });
    }
}
