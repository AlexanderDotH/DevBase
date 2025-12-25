using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DevBase.Utilities
{
    /// <summary>
    /// Provides utility methods for memory and serialization operations.
    /// </summary>
    public class MemoryUtils
    {
        
        #pragma warning disable SYSLIB0011
        /// <summary>
        /// Calculates the approximate size of an object in bytes using serialization.
        /// Returns 0 if serialization is not allowed or object is null.
        /// </summary>
        /// <param name="obj">The object to measure.</param>
        /// <returns>The size in bytes.</returns>
        public static long GetSize(Object obj)
        {
            if (!Globals.ALLOW_SERIALIZATION)
                return 0;
            
            if (obj == null)
                return 0;
            
            using (Stream s = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(s, obj);
                return s.Length;
            }
        }
        
        /// <summary>
        /// Reads a stream and converts it to a byte array.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <returns>The byte array containing the stream data.</returns>
        public static byte[] StreamToByteArray(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
