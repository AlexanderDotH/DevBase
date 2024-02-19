using System.Buffers;
using System.Diagnostics;
using DevBase.Extensions.Stopwatch;
using DevBase.Requests.Builder;
using DevBase.Test.Test;

namespace DevBase.Test.DevBaseRequests.Utils;

public class ContentDispositionUtilsTests
{
    [Test]
    public void FromFileTest()
    {
        int count = 1_000_000;

        Stopwatch penetrationTest = PenetrationTest.Run(() =>
        {
            ReadOnlySpan<char> lastValue = ContentDispositionUtils.FromFile("fileUpload", "rickAstley.mp3");
        }, count);
        
        Console.WriteLine($"Created {count}times the following text \n\"{ContentDispositionUtils.FromFile("fileUpload", "rickAstley.mp3")}\"\n");
        
        penetrationTest.PrintTimeTable();
    }
    
    [Test]
    public void FromValueTest()
    {
        int count = 1_000_000;
        
        Stopwatch penetrationTest = PenetrationTest.Run(() =>
        {
            ReadOnlySpan<char> lastValue = ContentDispositionUtils.FromValue("fieldName", "fieldValue");
        }, count);
        
        Console.WriteLine($"Created {count}times the following text \n\"{ContentDispositionUtils.FromValue("fieldName", "fieldValue")}\"\n");
        
        penetrationTest.PrintTimeTable();
    }
}