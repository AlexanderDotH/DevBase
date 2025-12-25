using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBase.IO
{
    /// <summary>
    /// Provides utility methods for directory operations.
    /// </summary>
    public class ADirectory
    {
        /// <summary>
        /// Gets a list of directory objects from a specified path.
        /// </summary>
        /// <param name="directory">The root directory path.</param>
        /// <param name="filter">The search filter string.</param>
        /// <returns>A list of directory objects.</returns>
        /// <exception cref="SystemException">Thrown if the directory does not exist.</exception>
        public static List<ADirectoryObject> GetDirectories(string directory, string filter = "*.*")
        {
            if (!System.IO.Directory.Exists(directory))
                throw new SystemException("Cannot get infos from directory, because the directory does not exist");

            List<ADirectoryObject> directoryObjects = new List<ADirectoryObject>();

            DirectoryInfo di = new DirectoryInfo(directory);

            try
            {
                foreach (DirectoryInfo d in di.EnumerateDirectories(filter, SearchOption.AllDirectories))
                {
                    directoryObjects.Add(new ADirectoryObject(d));
                }
            }
            catch (SystemException e) { }

            directoryObjects.AddRange(GetDirectories(directory, filter));

            return directoryObjects;
        }

    }
}
