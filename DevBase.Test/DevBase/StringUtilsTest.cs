using System.Diagnostics;
using DevBase.Extensions.Stopwatch;
using DevBase.Test.Test;
using DevBase.Utilities;
using Dumpify;

namespace DevBase.Test.DevBase;

/// <summary>
/// Tests for StringUtils methods.
/// </summary>
public class StringUtilsTest
{
    private int _count;

    /// <summary>
    /// Setup test environment.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._count = 1_000_000;
    }
    
    /// <summary>
    /// Tests the Separate method for joining string arrays.
    /// Includes a performance test (PenetrationTest).
    /// </summary>
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
    
    /// <summary>
    /// Tests the DeSeparate method for splitting strings.
    /// </summary>
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
