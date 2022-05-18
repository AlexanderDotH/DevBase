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
                if (size == Marshal.SizeOf(this.Get(i)))
                {
                    if (searchObject.Equals(this.Get(i)))
                    {
                        return this.Get(i);
                    }
                }
            }

            throw new GenericListEntryException(GenericListEntryException.Type.EntryNotFound);
        }

        /// <summary>
        /// Gets an T type from an given index
        /// </summary>
        /// <param name="index">The index of the array</param>
        /// <returns>A T-Object from the given index</returns>
        /// <exception cref="GenericListEntryException">When the index is out of bounds</exception>
        public T Get(int index)
        {
            if (index > this._array.Length)
                throw new GenericListEntryException(GenericListEntryException.Type.OutOfBounds);

            return this._array[index];
        }

        /// <summary>
        /// Gets a range of item as array
        /// </summary>
        /// <param name="min">The minimum range</param>
        /// <param name="max">The maximum range</param>
        /// <returns>An array of type T from the given range</returns>
        /// <exception cref="GenericListEntryException">When the min value is bigger than the max value</exception>
        public T[] GetRangeAsArray(int min, int max)
        {
            if (min > max)
                throw new GenericListEntryException(GenericListEntryException.Type.InvalidRange);

            T[] newArray = new T[max - min + 1];

            int counter = -1;
            for (int i = min; i < max; i++)
            {
                newArray[counter++] = this.Get(i);
            }

            return newArray;
        }

        /// <summary>
        /// Gets a range of item as list
        /// </summary>
        /// <param name="min">The minimum range</param>
        /// <param name="max">The maximum range</param>
        /// <returns>An array of type T from the given range</returns>
        /// <exception cref="GenericListEntryException">When the min value is bigger than the max value</exception>
        public List<T> GetRangeAsList(int min, int max)
        {
            if (min > max)
                throw new GenericListEntryException(GenericListEntryException.Type.InvalidRange);

            List<T> list = new List<T>();

            for (int i = min; i < max; i++)
            {
                list.Add(this.Get(i));
            }

            return list;
        }

        /// <summary>
        /// Adds an item to the array by creating a new array and the new item to it.
        /// </summary>
        /// <param name="item">The new item</param>
        public void Add(T item)
        {
            T[] newArray = new T[this._array.Length + 1];

            for (int i = 0; i < this._array.Length; i++)
            {
                newArray[i] = this._array[i];
            }

            newArray[this._array.Length + 1] = item;
            this._array = newArray;
        }

        /// <summary>
        /// Adds a array of T values to the array
        /// </summary>
        /// <param name="array">The given array</param>
        public void AddRange(T[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                this.Add(array[i]);
            }
        }

        /// <summary>
        /// Adds a list if T values to the array
        /// </summary>
        /// <param name="arrayList">The given list</param>
        public void AddRange(List<T> arrayList)
        {
            for (int i = 0; i < arrayList.Count; i++)
            {
                this.Add(arrayList[i]);
            }
        }

        public List<T> GetAsList()
        {
            return new List<T>(this._array);
        }

        public int Length
        {
            get { return this._array.Length; }
        }
    }
}
