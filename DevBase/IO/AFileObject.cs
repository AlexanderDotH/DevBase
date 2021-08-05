using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
