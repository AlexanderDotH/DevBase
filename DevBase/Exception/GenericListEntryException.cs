using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace DevBase.Exception
{
    /// <summary>
    /// Exception thrown for errors related to AList entries.
    /// </summary>
    public class AListEntryException : SystemException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AListEntryException"/> class.
        /// </summary>
        /// <param name="type">The type of error.</param>
        public AListEntryException(Type type)
        {
            switch (type)
            {
                case Type.EntryNotFound:
                {
                    throw new System.Exception("AListEntry not found");
                }
                case Type.ListNotEqual:
                {
                    throw new System.Exception("AListEntry size is not equal to given list size");
                }
                case Type.OutOfBounds:
                {
                    throw new System.Exception("The given index is out of bounds");
                }
                case Type.InvalidRange:
                {
                    throw new System.Exception("Given range is invalid");
                }
            }
        }

        /// <summary>
        /// Specifies the type of list entry error.
        /// </summary>
        public enum Type
        {
            /// <summary>Entry not found.</summary>
            EntryNotFound,
            /// <summary>List sizes are not equal.</summary>
            ListNotEqual,
            /// <summary>Index out of bounds.</summary>
            OutOfBounds,
            /// <summary>Invalid range.</summary>
            InvalidRange
        }
    }
}
