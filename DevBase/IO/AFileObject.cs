using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Generics;
using DevBase.Utilities;

namespace DevBase.IO
{
    public class AFileObject
    {
        private readonly FileInfo _fileInfo;
        private readonly byte[] _binaryData;

        public AFileObject(FileInfo fileInfo, byte[] binaryData)
        {
            this._fileInfo = fileInfo;
            this._binaryData = binaryData;
        }

        public AFileObject(FileInfo fi)
        {
            this._fileInfo = fi;
        }

        public AList<string> ToList()
        {
            if (this._binaryData == null)
                return null;

            AList<string> genericList = new AList<string>();

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

        public string ToStringData()
        {
            if (this._binaryData == null)
                return string.Empty;

            return EncodingUtils.GetEncoding(this._binaryData).GetString(this._binaryData);
        }

        public FileInfo FileInfo
        {
            get { return this._fileInfo; }
        }

        public byte[] BinaryData
        {
            get { return this._binaryData; }
        }
    }
}
