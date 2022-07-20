using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Generic;

namespace DevBaseFormat
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
