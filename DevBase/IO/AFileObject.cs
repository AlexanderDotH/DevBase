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
    /// <summary>
    /// Represents a file object including its info, content buffer, and encoding.
    /// </summary>
    public class AFileObject
    {
        /// <summary>
        /// Gets or sets the file info.
        /// </summary>
        public FileInfo FileInfo { get; protected set; }
        
        /// <summary>
        /// Gets or sets the memory buffer of the file content.
        /// </summary>
        public Memory<byte> Buffer { get; protected set; }
        
        /// <summary>
        /// Gets or sets the encoding of the file content.
        /// </summary>
        public Encoding Encoding { get; protected set; }

        // WARNING: For internal purposes you need to know what you are doing
        protected AFileObject() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="AFileObject"/> class.
        /// </summary>
        /// <param name="fileInfo">The file info.</param>
        /// <param name="readFile">Whether to read the file content immediately.</param>
        public AFileObject(FileInfo fileInfo, bool readFile = false)
        {
            FileInfo = fileInfo;
            
            if (!readFile)
                return;
            
            Memory<byte> buffer = AFile.ReadFile(fileInfo, out Encoding encoding);

            this.Buffer = buffer;
            this.Encoding = encoding;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AFileObject"/> class with existing data.
        /// Detects encoding from binary data.
        /// </summary>
        /// <param name="fileInfo">The file info.</param>
        /// <param name="binaryData">The binary data.</param>
        public AFileObject(FileInfo fileInfo, Memory<byte> binaryData) : this(fileInfo, false)
        {
            Buffer = binaryData;
            Encoding = EncodingUtils.GetEncoding(binaryData);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AFileObject"/> class with existing data and encoding.
        /// </summary>
        /// <param name="fileInfo">The file info.</param>
        /// <param name="binaryData">The binary data.</param>
        /// <param name="encoding">The encoding.</param>
        public AFileObject(FileInfo fileInfo, Memory<byte> binaryData, Encoding encoding) : this(fileInfo, false)
        {
            Buffer = binaryData;
            Encoding = encoding;
        }

        /// <summary>
        /// Creates an AFileObject from a byte buffer.
        /// </summary>
        /// <param name="buffer">The byte buffer.</param>
        /// <param name="fileName">The mock file name.</param>
        /// <returns>A new AFileObject.</returns>
        public static AFileObject FromBuffer(byte[] buffer, string fileName = "buffer.bin") =>
            new AFileObject(new FileInfo(fileName), buffer);
        
        // COMPLAIN: I don't like this solution. 
        /// <summary>
        /// Converts the file content to a list of strings (lines).
        /// </summary>
        /// <returns>An AList of strings.</returns>
        public AList<string> ToList()
        {
            if (this.Buffer.IsEmpty)
                return new AList<string>();

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

        /// <summary>
        /// Decodes the buffer to a string using the stored encoding.
        /// </summary>
        /// <returns>The decoded string.</returns>
        public string ToStringData()
        {
            if (this.Buffer.IsEmpty)
                return string.Empty;

            return this.Encoding.GetString(this.Buffer.Span);
        }

        /// <summary>
        /// Returns the string representation of the file data.
        /// </summary>
        /// <returns>The file data as string.</returns>
        public override string ToString() => ToStringData();
    }
}
