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
    /// <summary>
    /// Provides static utility methods for file operations.
    /// </summary>
    public static class AFile
    {
        /// <summary>
        /// Gets a list of files in a directory matching the specified filter.
        /// </summary>
        /// <param name="directory">The directory to search.</param>
        /// <param name="readContent">Whether to read the content of each file.</param>
        /// <param name="filter">The file filter pattern.</param>
        /// <returns>A list of AFileObject representing the files.</returns>
        /// <exception cref="DirectoryNotFoundException">Thrown if the directory does not exist.</exception>
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

        /// <summary>
        /// Reads a file and returns an AFileObject containing its data.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <returns>The AFileObject with file data.</returns>
        public static AFileObject ReadFileToObject(string filePath) => ReadFileToObject(new FileInfo(filePath));

        /// <summary>
        /// Reads a file and returns an AFileObject containing its data.
        /// </summary>
        /// <param name="file">The FileInfo of the file.</param>
        /// <returns>The AFileObject with file data.</returns>
        public static AFileObject ReadFileToObject(FileInfo file)
        {
            Memory<byte> binary = ReadFile(file, out Encoding encoding);
            return new AFileObject(file, binary, encoding);
        }
        
        /// <summary>
        /// Reads the content of a file into a memory buffer.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <returns>The file content as a memory buffer.</returns>
        public static Memory<byte> ReadFile(string filePath) => ReadFile(new FileInfo(filePath));

        /// <summary>
        /// Reads the content of a file into a memory buffer and detects its encoding.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="encoding">The detected encoding.</param>
        /// <returns>The file content as a memory buffer.</returns>
        public static Memory<byte> ReadFile(string filePath, out Encoding encoding)
        {
            return ReadFile(new FileInfo(filePath), out encoding);
        }

        /// <summary>
        /// Reads the content of a file into a memory buffer and detects its encoding.
        /// </summary>
        /// <param name="fileInfo">The FileInfo of the file.</param>
        /// <returns>The file content as a memory buffer.</returns>
        public static Memory<byte> ReadFile(FileInfo fileInfo) => ReadFile(fileInfo, out Encoding encoding);
        
        /// <summary>
        /// Reads the content of a file into a memory buffer and detects its encoding.
        /// </summary>
        /// <param name="fileInfo">The FileInfo of the file.</param>
        /// <param name="encoding">The detected encoding.</param>
        /// <returns>The file content as a memory buffer.</returns>
        /// <exception cref="IOException">Thrown if the file cannot be fully read.</exception>
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

        /// <summary>
        /// Checks if a file can be accessed with the specified access rights.
        /// </summary>
        /// <param name="fileInfo">The FileInfo of the file.</param>
        /// <param name="fileAccess">The requested file access.</param>
        /// <returns>True if the file can be accessed, false otherwise.</returns>
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
