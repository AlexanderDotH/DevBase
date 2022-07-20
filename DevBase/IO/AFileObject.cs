using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Generic;
using DevBase.Utilities;

namespace DevBase.IO
{
    public class AFileObject
    {
        private FileInfo _fileInfo = null;
        private byte[] _binaryData = new byte[0];

        public AFileObject(FileInfo fi, byte[] bd)
        {
            this._fileInfo = fi;
            this._binaryData = bd;
        }

        public AFileObject(FileInfo fi)
        {
            this._fileInfo = fi;
        }

        public FileInfo FileInfo
        {
            get { return this._fileInfo; }
        }

        public byte[] BinaryData
        {
            get { return this._binaryData; }
        }

        public string ToStringData()
        {
            if (this._binaryData == null)
                return string.Empty;

            return EncodingUtils.GetEncoding(this._binaryData).GetString(this._binaryData);
        }

        public GenericList<string> ToList()
        {
            if (this._binaryData == null)
                return null;

            GenericList<string> genericList = new GenericList<string>();

            using (StringReader reader = new StringReader(ToStringData()))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    genericList.Add(line);
                }
            }

            return genericList;
        }
    }
}
