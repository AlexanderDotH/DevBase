namespace DevBase.Format
{
    public class FileFormatParser<T>
    {

        private IFileFormat<T> _fileFormatParser;

        public FileFormatParser(IFileFormat<T> fileFormatParser)
        {
            this._fileFormatParser = fileFormatParser;
        }

        public T FormatFromFile(string filePath)
        {
            return this._fileFormatParser.FormatFromFile(filePath);
        }

        public T FormatFromString(string content)
        {
            return this._fileFormatParser.FormatFromString(content);
        }

    }
}
