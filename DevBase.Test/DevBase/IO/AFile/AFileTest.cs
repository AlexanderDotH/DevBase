using DevBase.IO;

namespace DevBase.Test.DevBase.IO.AFile;

public class AFileTest
{
    [Test]
    public void ReadFileTest()
    {
        AFileObject buffer = global::DevBase.IO.AFile.ReadFileToObject(
            Path.Combine(
                "..",
                "..",
                "..",
                "DevBaseFormatData",
                "LRC",
                "M\u00dcNCHEN.lrc"));
        
        Assert.That(buffer.Buffer.ToArray(), Is.Not.Empty);
    }
}
