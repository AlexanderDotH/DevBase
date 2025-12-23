using System.Diagnostics;
using System.Text;
using DevBase.Extensions.Stopwatch;
using DevBase.Requests.Objects;
using DevBase.Requests.Utils;
using DevBase.Test.Test;
using Org.BouncyCastle.Crmf;

namespace DevBase.Test.DevBaseRequests.Utils;

public class ContentDispositionUtilsTests
{
    [Test]
    public void FromFileTest()
    {
        int count = 1_000_000;

        MimeFileObject mime = MimeFileObject.FromBuffer(Encoding.UTF8.GetBytes("JoeMama"));

        Memory<byte> last;
        
        Stopwatch penetrationTest = PenetrationTest.RunWithLast<Memory<byte>>(() =>
        {
            return ContentDispositionUtils.FromFile("fileUpload", mime);
        }, out last, count);
        
        Console.WriteLine($"Created {count}times the following text \n\"{Encoding.ASCII.GetString(ContentDispositionUtils.FromFile("fileUpload", mime).ToArray())}\"\n");
        
        penetrationTest.PrintTimeTable();
        
        Assert.That(last.Length, Is.GreaterThan(0));
    }
    
    [Test]
    public void FromValueTest()
    {
        int count = 1_000_000;
        
        Memory<byte> last;
        
        Stopwatch penetrationTest = PenetrationTest.RunWithLast<Memory<byte>>(() =>
        {
            return ContentDispositionUtils.FromValue("fieldName", "fieldValue");
        }, out last, count);
        
        Console.WriteLine($"Created {count}times the following text \n\"{Encoding.ASCII.GetString(ContentDispositionUtils.FromValue("fieldName", "fieldValue").ToArray())}\"\n");
        
        penetrationTest.PrintTimeTable();
        Assert.That(last.Length, Is.GreaterThan(0));
    }
}
