using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Generics;
using DevBase.Utilities;
using Microsoft.Win32.SafeHandles;

namespace DevBase.IO
{
    public static class AFile
    {
        public static AList<AFileObject> GetFiles(string directory, bool readContent = false, string filter = "*.txt")
        {
            if (!System.IO.Directory.Exists(directory))
                throw new DirectoryNotFoundException("Cannot get files from directory, because the directory does not exist");

            AList<AFileObject> fileHolders = new AList<AFileObject>();

            DirectoryInfo di = new DirectoryInfo(directory);

            foreach (FileInfo f in di.EnumerateFiles(filter, SearchOption.AllDirectories))
            {
                AFileObject fileHolder;

                if (readContent)
                {
                    fileHolder = ReadFileToObject(f);
                }
                else
                {
                    fileHolder = new AFileObject(f);
                }

                fileHolders.Add(fileHolder);
            }

            return fileHolders;
        }

        public static AFileObject ReadFileToObject(string filePath) => ReadFileToObject(new FileInfo(filePath));

        public static AFileObject ReadFileToObject(FileInfo file)
        {
            Memory<byte> binary = ReadFile(file, out Encoding encoding);
            return new AFileObject(file, binary, encoding);
        }
        
        public static Memory<byte> ReadFile(string filePath) => ReadFile(new FileInfo(filePath));

        public static Memory<byte> ReadFile(string filePath, out Encoding encoding)
        {
            return ReadFile(new FileInfo(filePath), out encoding);
        }

        public static Memory<byte> ReadFile(FileInfo fileInfo) => ReadFile(fileInfo, out Encoding encoding);
        
        public static Memory<byte> ReadFile(FileInfo fileInfo, out Encoding encoding)
        {
            using FileStream fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read);
            using StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8, true);
            using BufferedStream bufferedStream = new BufferedStream(fileStream);

            byte[] allocatedBuffer = new byte[fileStream.Length];
            Span<byte> buffer = allocatedBuffer;

            int read = bufferedStream.Read(buffer);

            if (read == 0 || read != fileStream.Length)
                throw new IOException("Buffer is not full or empty");

            encoding = streamReader.CurrentEncoding;
            
            return allocatedBuffer;
        }

        public static bool CanFileBeAccessed(FileInfo fileInfo, FileAccess fileAccess = FileAccess.Read)
        {
            if (!fileInfo.Exists)
                return false;

            try
            {
                fileInfo.Open(FileMode.Open, fileAccess, FileShare.None);
            }
            catch (IOException e)
            {
                return false;
            }

            return true;
        }
    }
}
