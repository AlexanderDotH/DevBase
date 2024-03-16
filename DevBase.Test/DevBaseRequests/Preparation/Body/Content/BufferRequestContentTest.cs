using System.Diagnostics;
using DevBase.Extensions.Stopwatch;
using DevBase.Requests.Preparation.Header.Body.Content;
using DevBase.Test.Test;

namespace DevBase.Test.DevBaseRequests.Preparation.Body.Content;

public class BufferRequestContentTest
{
    private byte[] Buffer { get; set; }
    private const int Count = 1_000_000;
    
    [SetUp]
    public void Setup()
    {
        this.Buffer = new byte[] { 0x4A, 0x4F, 0x45 };
    }

    [Test]
    public void IsValidTest()
    {
        BufferRequestContent bufferRequestContent = new BufferRequestContent();
        
        Stopwatch stopwatch = PenetrationTest.Run(() =>
        {
            bufferRequestContent.IsValid(this.Buffer);
        }, Count);
        
        Console.WriteLine($"Validated buffer {Convert.ToHexString(this.Buffer)} {Count}times");
        
        stopwatch.PrintTimeTable();
    }
}