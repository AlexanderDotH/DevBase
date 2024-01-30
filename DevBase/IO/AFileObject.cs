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
        public FileInfo FileInfo { get; protected set; }
        public Memory<byte> Buffer { get; protected set; }
        public Encoding Encoding { get; protected set; }

        // WARNING: For internal purposes you need to know what you are doing
        protected AFileObject() {}

        public AFileObject(FileInfo fileInfo, bool readFile = false)
        {
            FileInfo = fileInfo;
            
            if (!readFile)
                return;
            
            Memory<byte> buffer = AFile.ReadFile(fileInfo, out Encoding encoding);

            this.Buffer = buffer;
            this.Encoding = encoding;
        }

        public AFileObject(FileInfo fileInfo, Memory<byte> binaryData) : this(fileInfo, false)
        {
            Buffer = binaryData;
            Encoding = EncodingUtils.GetEncoding(binaryData);
        }
        
        public AFileObject(FileInfo fileInfo, Memory<byte> binaryData, Encoding encoding) : this(fileInfo, false)
        {
            Buffer = binaryData;
            Encoding = encoding;
        }

        // COMPLAIN: I don't like this solution. 
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

        public string ToStringData()
        {
            if (this.Buffer.IsEmpty)
                return string.Empty;

            return this.Encoding.GetString(this.Buffer.Span);
        }

        public override string ToString() => ToStringData();
    }
}
