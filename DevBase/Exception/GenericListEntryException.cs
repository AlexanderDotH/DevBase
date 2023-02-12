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
    public class AListEntryException : SystemException
    {
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

        public enum Type
        {
            EntryNotFound,
            ListNotEqual,
            OutOfBounds,
            InvalidRange
        }
    }
}
