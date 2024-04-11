namespace DevBase.Test.DevBaseFormat.Formats;

public class FormatTest
{
    public FileInfo GetTestFile(string folder, string name)
    {
        return new FileInfo(
            $"..{Path.DirectorySeparatorChar}.." +
            $"{Path.DirectorySeparatorChar}.." +
            $"{Path.DirectorySeparatorChar}DevBaseFormatData" +
            $"{Path.DirectorySeparatorChar}{folder}" +
            $"{Path.DirectorySeparatorChar}{name}");
    }
}