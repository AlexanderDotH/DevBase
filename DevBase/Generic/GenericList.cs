using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DevBase.Exception;

namespace DevBase.Generic
{
    public class GenericList<T> : List<T>
    {

        /// <summary>
        /// A faster and optimized way to search entries inside this generic list
        /// </summary>
        /// <param name="searchObject">The object to search for</param>
        /// <returns></returns>
        public T FindEntry(T searchObject)
        {
            int size = Marshal.SizeOf(searchObject);

            for (int i = 0; i < this.Count; i++)
            {
                if (size == Marshal.SizeOf(this[i]))
                {
                    if (searchObject.Equals(this[i]))
                    {
                        return this[i];
                    }
                }
            }

            throw new GenericListEntryException(GenericListEntryException.Type.EntryNotFound);
        }

        /// <summary>
        /// Appends to every item inside this list a given item of the other list
        ///
        /// List sizes should be equal or it throws
        /// <see cref="GenericListEntryException"/>
        /// </summary>
        ///
        /// <param name="append">A list which will be appended</param>
        /// <param name="marker">A marker which will be placed between the items</param>
        public void AppendToEveryItem(List<T> append, object marker = null)
        {
            if (this.Count != append.Count)
                throw new GenericListEntryException(GenericListEntryException.Type.EntryNotFound);

            GenericList<T> genericList = new GenericList<T>();

            for (int i = 0; i < this.Count; i++)
            {
                object typeObject = null;

                IntPtr pointerObject = GCHandle.Alloc(typeObject, GCHandleType.Pinned).AddrOfPinnedObject();
                IntPtr thisPtr = GCHandle.Alloc(this[i], GCHandleType.Pinned).AddrOfPinnedObject();

            }

           
        }
    }
}
