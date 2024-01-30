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
        AFileObject buffer = global::DevBase.IO.AFile.ReadFileToObject("..\\..\\..\\DevBaseFormatData\\LRC\\MÜNCHEN.lrc");
        Assert.IsNotEmpty(buffer.Buffer.ToArray());
    }
}