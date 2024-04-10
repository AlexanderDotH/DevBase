using System.Diagnostics;
using System.Text;
using DevBase.Extensions.Stopwatch;
using DevBase.Requests.Preparation.Header.Body.Content;
using DevBase.Test.Test;

namespace DevBase.Test.DevBaseRequests.Preparation.Body.Content;

public class StringRequestContentTest
{
    private byte[] Value { get; set; }
    private const int Count = 1_000_000;
    
    [SetUp]
    public void Setup()
    {
        this.Value = Encoding.ASCII.GetBytes("Joe");
    }

    [Test]
    public void IsValidTest()
    {
        StringRequestContent stringRequestContent = new StringRequestContent(Encoding.ASCII);
        
        Stopwatch stopwatch = PenetrationTest.Run(() =>
        {
            Assert.IsTrue(stringRequestContent.IsValid(this.Value));
        }, Count);
        
        Console.WriteLine($"Validated buffer {Convert.ToHexString(this.Value)} {Count}times");
        
        stopwatch.PrintTimeTable();
    }
}