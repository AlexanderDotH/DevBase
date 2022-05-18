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
    public class GenericList<T>
    {
        private T[] _array;

        public GenericList()
        {
            this._array = new T[] { };
        }

        /// <summary>
        /// A faster and optimized way to search entries inside this generic list
        ///
        /// It iterates through the list and firstly checks
        /// the size of the object to the corresponding searchObject.
        /// 
        /// </summary>
        /// <param name="searchObject">The object to search for</param>
        /// <returns></returns>
        public T FindEntry(T searchObject)
        {
            int size = Marshal.SizeOf(searchObject);

            for (int i = 0; i < this._array.Length; i++)
            {
                if (size == Marshal.SizeOf(this.get(i)))
                {
                    if (searchObject.Equals(this.get(i)))
                    {
                        return this.get(i);
                    }
                }
            }

            throw new GenericListEntryException(GenericListEntryException.Type.EntryNotFound);
        }

        public T get(int index)
        {
            if (index > this._array.Length)
                throw new GenericListEntryException(GenericListEntryException.Type.OutOfBounds);

            return this._array[index];
        }



    }
}
