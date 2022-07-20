using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DevBase.IO
{
    public class ADirectoryObject
    {
        private readonly DirectoryInfo _directoryInfo;

        public ADirectoryObject(DirectoryInfo directoryInfo)
        {
            this._directoryInfo = directoryInfo;
        }

        public DirectoryInfo GetDirectoryInfo
        {
            get { return this._directoryInfo; }
        }
    }
}
