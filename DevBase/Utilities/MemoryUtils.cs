using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DevBase.Utilities
{
    /// <summary>
    /// Provides utility methods for memory and serialization operations.
    /// </summary>
    public class MemoryUtils
    {
        /// <summary>
        /// Estimates the approximate size of an object in bytes using reflection.
        /// For primitive types and structs, uses Marshal.SizeOf when possible.
        /// For reference types, estimates based on field types.
        /// </summary>
        /// <param name="obj">The object to measure.</param>
        /// <returns>The estimated size in bytes, or 0 if measurement fails.</returns>
        public static long GetSize(Object obj)
        {
            if (obj == null)
                return 0;

            try
            {
                return EstimateSize(obj, obj.GetType());
            }
            catch
            {
                return 0;
            }
        }

        private static long EstimateSize(object obj, Type type)
        {
            if (obj == null)
                return 0;

            // Primitives and value types with known sizes
            if (type == typeof(bool)) return 1;
            if (type == typeof(byte) || type == typeof(sbyte)) return 1;
            if (type == typeof(char)) return 2;
            if (type == typeof(short) || type == typeof(ushort)) return 2;
            if (type == typeof(int) || type == typeof(uint)) return 4;
            if (type == typeof(long) || type == typeof(ulong)) return 8;
            if (type == typeof(float)) return 4;
            if (type == typeof(double)) return 8;
            if (type == typeof(decimal)) return 16;
            if (type == typeof(IntPtr) || type == typeof(UIntPtr)) return IntPtr.Size;

            // String: 2 bytes per character + overhead
            if (type == typeof(string))
            {
                var str = (string)obj;
                return str.Length * 2 + IntPtr.Size;
            }

            // Arrays
            if (type.IsArray)
            {
                var array = (Array)obj;
                var elementType = type.GetElementType();
                long elementSize = elementType != null ? GetPrimitiveSize(elementType) : IntPtr.Size;
                return array.Length * elementSize + IntPtr.Size;
            }

            // Try Marshal.SizeOf for blittable structs
            if (type.IsValueType && !type.IsEnum)
            {
                try
                {
                    return Marshal.SizeOf(type);
                }
                catch
                {
                    // Not a blittable type, estimate via reflection
                }
            }

            // Reference types: estimate based on fields
            long size = IntPtr.Size; // Object header
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            
            foreach (var field in fields)
            {
                size += GetPrimitiveSize(field.FieldType);
            }

            return size;
        }

        private static long GetPrimitiveSize(Type type)
        {
            if (type == typeof(bool)) return 1;
            if (type == typeof(byte) || type == typeof(sbyte)) return 1;
            if (type == typeof(char)) return 2;
            if (type == typeof(short) || type == typeof(ushort)) return 2;
            if (type == typeof(int) || type == typeof(uint)) return 4;
            if (type == typeof(long) || type == typeof(ulong)) return 8;
            if (type == typeof(float)) return 4;
            if (type == typeof(double)) return 8;
            if (type == typeof(decimal)) return 16;
            if (type.IsValueType)
            {
                try { return Marshal.SizeOf(type); } catch { }
            }
            return IntPtr.Size; // Reference type pointer
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
