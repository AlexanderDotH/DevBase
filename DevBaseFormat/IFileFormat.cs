using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Generic;

namespace DevBaseFormat
{
    public interface IFileFormat<T>
    {
        T FormatFromFile(string filePath);
        T FormatFromString(string lyricString);
    }
}
