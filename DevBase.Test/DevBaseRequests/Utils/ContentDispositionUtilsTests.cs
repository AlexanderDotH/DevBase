using System.Buffers;
using System.Diagnostics;
using System.Text;
using DevBase.Extensions.Stopwatch;
using DevBase.Requests.Builder;
using DevBase.Requests.Objects;
using DevBase.Test.Test;

namespace DevBase.Test.DevBaseRequests.Utils;

public class ContentDispositionUtilsTests
{
    [Test]
    public void FromFileTest()
    {
        int count = 1_000_000;

        MimeFileObject mime = MimeFileObject.FromBuffer(Encoding.UTF8.GetBytes("JoeMama"));
        
        Stopwatch penetrationTest = PenetrationTest.Run(() =>
        {
           ReadOnlySpan<byte> lastValue = ContentDispositionUtils.FromFile("fileUpload", mime);
        }, count);
        
        Console.WriteLine($"Created {count}times the following text \n\"{Encoding.ASCII.GetString(ContentDispositionUtils.FromFile("fileUpload", mime))}\"\n");
        
        penetrationTest.PrintTimeTable();
    }
    
    [Test]
    public void FromValueTest()
    {
        int count = 1_000_000;
        
        Stopwatch penetrationTest = PenetrationTest.Run(() =>
        {
            ReadOnlySpan<byte> lastValue = ContentDispositionUtils.FromValue("fieldName", "fieldValue");
        }, count);
        
        Console.WriteLine($"Created {count}times the following text \n\"{Encoding.ASCII.GetString(ContentDispositionUtils.FromValue("fieldName", "fieldValue"))}\"\n");
        
        penetrationTest.PrintTimeTable();
    }
}