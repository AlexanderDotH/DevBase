using System.Runtime.InteropServices;
using DevBase.IO;
using DevBase.Requests.Abstract;
using DevBase.Requests.Objects;
using DevBase.Requests.Preparation.Header.Body;

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
        
        Assert.IsNotEmpty(buffer.Buffer.ToArray());
    }
}