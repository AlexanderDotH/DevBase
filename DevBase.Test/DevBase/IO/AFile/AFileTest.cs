using System.Runtime.InteropServices;
using DevBase.IO;
using DevBase.Net.Abstract;
using DevBase.Net.Objects;

namespace DevBase.Test.DevBase.IO.AFile;

public class AFileTest
{
    [Test]
    public void ReadFileTest()
    {
        AFileObject buffer = global::DevBase.IO.AFile.ReadFileToObject(
            $"..{Path.DirectorySeparatorChar}" +
            $"..{Path.DirectorySeparatorChar}" +
            $"..{Path.DirectorySeparatorChar}" +
            $"DevBaseFormatData{Path.DirectorySeparatorChar}" +
            $"LRC{Path.DirectorySeparatorChar}" +
            $"MÜNCHEN.lrc");
        
        Assert.That(buffer.Buffer.ToArray(), Is.Not.Empty);
    }
}
