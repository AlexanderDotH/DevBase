using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBase.Utilities
{
    class EncodingUtils
    {
        public static Encoding GetEncoding(byte[] byteArray)
        {
            Encoding returnEncoding = null;

            using (StreamReader reader =
                   new StreamReader(new MemoryStream(byteArray), detectEncodingFromByteOrderMarks: true))
            {
                returnEncoding = reader.CurrentEncoding;
            }

            return returnEncoding;
        }
    }
}
