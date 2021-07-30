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
            }
        }

        public enum Type
        {
            EntryNotFound
        }
    }
}
