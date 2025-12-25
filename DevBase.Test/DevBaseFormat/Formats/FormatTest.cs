namespace DevBase.Test.DevBaseFormat.Formats;

/// <summary>
/// Base class for format tests providing helper methods for file access.
/// </summary>
public class FormatTest
{
    /// <summary>
    /// Gets a FileInfo object for a test file located in the DevBaseFormatData directory.
    /// </summary>
    /// <param name="folder">The subfolder name in DevBaseFormatData.</param>
    /// <param name="name">The file name.</param>
    /// <returns>FileInfo object pointing to the test file.</returns>
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
