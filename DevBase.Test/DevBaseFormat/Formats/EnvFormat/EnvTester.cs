using DevBase.Format.Formats.EnvFormat;
using DevBase.Generics;

namespace DevBase.Test.DevBaseFormat.Formats.EnvFormat;

/// <summary>
/// Tests for ENV format parser.
/// </summary>
public class EnvTester : FormatTest
{
    [Test]
    public void TestParseIndentedContentTrimsLineWhitespace()
    {
        ATupleList<string, string> variables = new EnvParser().Parse("    API_KEY=\"value\"    ");

        Assert.That(variables.Length, Is.EqualTo(1));
        Assert.That(variables.Get(0).Item1, Is.EqualTo("API_KEY"));
        Assert.That(variables.Get(0).Item2, Is.EqualTo("value"));
    }
}
