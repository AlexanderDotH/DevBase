namespace DevBase.Format
{
    public interface IFileFormat<T>
    {
        T FormatFromFile(string filePath);
        T FormatFromString(string lyricString);
    }
}
