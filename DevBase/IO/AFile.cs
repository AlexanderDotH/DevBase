using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Generic;
using DevBase.Utilities;
using Microsoft.Win32.SafeHandles;

namespace DevBase.IO
{
    public class AFile
    {
        public static GenericList<AFileObject> GetFiles(string directory, bool readContent = false, string filter = "*.txt")
        {
            if (!System.IO.Directory.Exists(directory))
                throw new SystemException("Cannot get files from directory, because directory doesn't exist");

            GenericList<AFileObject> fileHolders = new GenericList<AFileObject>();

            DirectoryInfo di = new DirectoryInfo(directory);

            foreach (FileInfo f in di.EnumerateFiles(filter, SearchOption.AllDirectories))
            {
                AFileObject fileHolder;

                if (readContent)
                {
                    fileHolder = new AFileObject(f, File.ReadAllBytes(f.FullName));
                }
                else
                {
                    fileHolder = new AFileObject(f);
                }

                fileHolders.Add(fileHolder);
            }

            return fileHolders;
        }

        public static AFileObject ReadFile(FileInfo file)
        {
            byte[] binary = File.ReadAllBytes(file.FullName);
            return new AFileObject(file, binary);
        }

        public static AFileObject ReadFile(string filePath)
        {
            return ReadFile(new FileInfo(filePath));
        }

        public static bool CanFileBeAccessed(FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
                return false;

            try
            {
                fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException e)
            {
                return false;
            }

            return true;
        }
    }
}
