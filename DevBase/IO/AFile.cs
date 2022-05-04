using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace DevBase.IO
{
    public class AFile
    {
        public static List<AFileObject> GetFiles(string directory, bool readContent = false, string filter = "*.*")
        {
            if (!System.IO.Directory.Exists(directory))
                throw new SystemException("Cannot get files from directory, because directory doesn't exist");

            List<AFileObject> fileHolders = new List<AFileObject>();

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
    }
}
