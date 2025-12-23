using System.Diagnostics;
using DevBase.Extensions.Stopwatch;
using DevBase.Test.Test;
using DevBase.Utilities;
using Dumpify;

namespace DevBase.Test.DevBase;

public class StringUtilsTest
{
    private int _count;

    [SetUp]
    public void Setup()
    {
        this._count = 1_000_000;
    }
    
    [Test]
    public void SeparateTest()
    {
        string[] elements = new string[] { "Apple", "Banana", "Sausage" };

        string separated = string.Empty;
        
        Stopwatch stopwatch = PenetrationTest.Run(() =>
        {
            separated = StringUtils.Separate(elements, ", ");
        }, _count);

        separated.DumpConsole();
        
        Assert.That(separated, Is.EqualTo("Apple, Banana, Sausage"));

        stopwatch.PrintTimeTable();
    }
    
    [Test]
    public void DeSeparateTest()
    {
        string separated = "Apple, Banana, Sausage";
        
        string[] elements = new string[] { "Apple", "Banana", "Sausage" };

        string[] deseperated = StringUtils.DeSeparate(separated, ", ");

        deseperated.DumpConsole();
        Assert.That(deseperated, Is.EqualTo(elements));
    }
}
