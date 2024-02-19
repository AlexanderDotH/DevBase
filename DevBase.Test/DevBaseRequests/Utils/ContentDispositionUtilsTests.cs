using System.Buffers;
using System.Diagnostics;
using System.Text;
using DevBase.Extensions.Stopwatch;
using DevBase.Requests.Objects;
using DevBase.Requests.Utils;
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
            Memory<byte> lastValue = ContentDispositionUtils.FromFile("fileUpload", mime);
        }, count);
        
        Console.WriteLine($"Created {count}times the following text \n\"{Encoding.ASCII.GetString(ContentDispositionUtils.FromFile("fileUpload", mime).ToArray())}\"\n");
        
        penetrationTest.PrintTimeTable();
    }
    
    [Test]
    public void FromValueTest()
    {
        int count = 1_000_000;
        
        Stopwatch penetrationTest = PenetrationTest.Run(() =>
        {
            Memory<byte> lastValue = ContentDispositionUtils.FromValue("fieldName", "fieldValue");
        }, count);
        
        Console.WriteLine($"Created {count}times the following text \n\"{Encoding.ASCII.GetString(ContentDispositionUtils.FromValue("fieldName", "fieldValue").ToArray())}\"\n");
        
        penetrationTest.PrintTimeTable();
    }
}