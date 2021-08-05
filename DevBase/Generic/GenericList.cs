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
    }
}
