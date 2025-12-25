using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DevBase.IO
{
    /// <summary>
    /// Represents a directory object wrapper around DirectoryInfo.
    /// </summary>
    public class ADirectoryObject
    {
        private readonly DirectoryInfo _directoryInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ADirectoryObject"/> class.
        /// </summary>
        /// <param name="directoryInfo">The DirectoryInfo object.</param>
        public ADirectoryObject(DirectoryInfo directoryInfo)
        {
            this._directoryInfo = directoryInfo;
        }

        /// <summary>
        /// Gets the underlying DirectoryInfo.
        /// </summary>
        public DirectoryInfo GetDirectoryInfo
        {
            get { return this._directoryInfo; }
        }
    }
}
