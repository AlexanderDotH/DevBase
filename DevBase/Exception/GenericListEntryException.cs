using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace DevBase.Exception
{
    public class GenericListEntryException : SystemException
    {
        public GenericListEntryException(Type type)
        {
            switch (type)
            {
                case Type.EntryNotFound:
                {
                    throw new System.Exception("GenericListEntry not found");
                }
                case Type.ListNotEqual:
                {
                    throw new System.Exception("GenericListEntry size is not equal to given list size");
                }
                case Type.OutOfBounds:
                {
                    throw new System.Exception("GenericListEntry the given index is out of bounds");
                }
            }
        }

        public enum Type
        {
            EntryNotFound,
            ListNotEqual,
            OutOfBounds
        }
    }
}
