using System.Collections;
using DevBase.Exception;
using DevBase.Utilities;

namespace DevBase.Generics
{
    public class AList<T> : IEnumerable<T>
    {
        private T[] _array;
        
        /// <summary>
        /// Constructs this class with an empty array
        /// </summary>
        public AList()
        {
            this._array = Array.Empty<T>();
        }

        /// <summary>
        /// Constructs this class and adds items from the given list
        /// </summary>
        /// <param name="list">The list which will be added</param>
        public AList(List<T> list) : this()
        {
            this.AddRange(list);
        }

        /// <summary>
        /// Constructs this class with the given array
        /// </summary>
        /// <param name="array">The given array</param>
        public AList(params T[] array)
        {
            this._array = array;
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
            long size = MemoryUtils.GetSize(searchObject);

            for (int i = 0; i < this._array.Length; i++)
            {
                if (size == MemoryUtils.GetSize(this.Get(i)))
                {
                    if (searchObject.Equals(this.Get(i)))
                    {
                        return this.Get(i);
                    }
                }
            }

            throw new AListEntryException(AListEntryException.Type.EntryNotFound);
        }

        /// <summary>
        /// Finds an elements by an given predicate
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <returns>The element matching the predicate</returns>
        public T Find(Predicate<T> predicate)
        {
            T? result = Array.Find(this._array, predicate);

            if (result == null)
                return default;

            return result;
        }
        
        /// <summary>
        /// Iterates through the list and executes an action
        /// </summary>
        /// <param name="action">The action</param>
        public void ForEach(Action<T> action)
        {
            for (int i = 0; i < this._array.Length; i++)
            {
                action(this.Get(i));
            }
        }

        /// <summary>
        /// Sorts this list with an comparer
        /// </summary>
        /// <param name="comparer">The given comparer</param>
        public void Sort(IComparer<T> comparer)
        {
            this.Sort(0, this.Length, comparer);
        }

        /// <summary>
        /// Sorts this list with an comparer
        /// </summary>
        /// <param name="comparer">The given comparer</param>
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            if (index < 0)
                throw new AListEntryException(AListEntryException.Type.OutOfBounds);

            if (count < 0)
                throw new AListEntryException(AListEntryException.Type.OutOfBounds);

            if (this._array.Length - index < count)
                throw new AListEntryException(AListEntryException.Type.InvalidRange);

            Array.Sort<T>(this._array, index, count, comparer);
        }

        /// <summary>
        /// Checks if this list contains a given item
        /// </summary>
        /// <param name="item">The given item</param>
        /// <returns>True if the item is in the list. False if the item is not in the list</returns>
        public bool Contains(T item)
        {
            long size = MemoryUtils.GetSize(item);

            for (int i = 0; i < this._array.Length; i++)
            {
                if (size == MemoryUtils.GetSize(this.Get(i)))
                {
                    if (item.Equals(this.Get(i)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a random object from the array 
        /// </summary>
        /// <returns>A random object</returns>
        public T GetRandom()
        {
            return this.Get(new Random().Next(0, this.Length - 1));
        }
        
        /// <summary>
        /// Returns a random object from the array with an given random number generator
        /// </summary>
        /// <returns>A random object</returns>
        public T GetRandom(Random random)
        {
            return this.Get(random.Next(0, this.Length - 1));
        }

        /// <summary>
        /// This function slices the list into smaller given pieces.
        /// </summary>
        /// <param name="size">Is the size of the chunks inside the list</param>
        /// <returns>A freshly sliced list</returns>
        public AList<AList<T>> Slice(int size)
        {
            AList<AList<T>> slicedArray = new AList<AList<T>>();

            for (int i = 0; i < this._array.Length; i += size)
            {
                int remainingItems = this._array.Length - i;
                int currentChunkSize = remainingItems < size ? remainingItems : size;
                
                T[] chunk = new T[currentChunkSize];
                Array.Copy(this._array, i, chunk, 0, currentChunkSize);
                
                slicedArray.Add(new AList<T>(chunk));
            }

            return slicedArray;
        }

        /// <summary>
        /// Checks if this list contains a given item
        /// </summary>
        /// <param name="item">The given item</param>
        /// <returns>True if the item is in the list. False if the item is not in the list</returns>
        public bool SafeContains(T item)
        {
            for (int i = 0; i < this._array.Length; i++)
            {
                if (item.Equals(this.Get(i)))
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// Gets and sets the items with an given index
        /// </summary>
        /// <param name="index">The given index</param>
        /// <returns>A requested item based on the index</returns>
        public T this[int index]
        {
            get { return this.Get(index); }
            set { this.Set(index, value); }
        }

        /// <summary>
        /// Gets an T type from an given index
        /// </summary>
        /// <param name="index">The index of the array</param>
        /// <returns>A T-Object from the given index</returns>
        public T Get(int index)
        {
            return this._array[index];
        }

        /// <summary>
        /// Sets the value at a given index
        /// </summary>
        /// <param name="index">The given index</param>
        /// <param name="value">The given value</param>
        public void Set(int index, T value)
        {
            this._array[index] = value;
        }

        /// <summary>
        /// Clears the list
        /// </summary>
        public void Clear()
        {
            Array.Clear(this._array, 0, this._array.Length);
        }

        /// <summary>
        /// Gets a range of item as array
        /// </summary>
        /// <param name="min">The minimum range</param>
        /// <param name="max">The maximum range</param>
        /// <returns>An array of type T from the given range</returns>
        /// <exception cref="AListEntryException">When the min value is bigger than the max value</exception>
        public T[] GetRangeAsArray(int min, int max)
        {
            if (min > max)
                throw new AListEntryException(AListEntryException.Type.InvalidRange);

            T[] newArray = new T[max - min + 1];

            int counter = 0;
            for (int i = min; i <= max; i++)
            {
                newArray[counter++] = this.Get(i);
            }

            return newArray;
        }

        /// <summary>
        /// Gets a range of items as AList.
        /// </summary>
        /// <param name="min">The minimum index.</param>
        /// <param name="max">The maximum index.</param>
        /// <returns>An AList of items in the range.</returns>
        public AList<T> GetRangeAsAList(int min, int max) => new AList<T>(GetRangeAsArray(min, max));

        /// <summary>
        /// Gets a range of item as list
        /// </summary>
        /// <param name="min">The minimum range</param>
        /// <param name="max">The maximum range</param>
        /// <returns>An array of type T from the given range</returns>
        /// <exception cref="AListEntryException">When the min value is bigger than the max value</exception>
        public List<T> GetRangeAsList(int min, int max)
        {
            if (min > max)
                throw new AListEntryException(AListEntryException.Type.InvalidRange);

            return new List<T>(GetRangeAsArray(min, max));
        }

        /// <summary>
        /// Adds an item to the array by creating a new array and the new item to it.
        /// </summary>
        /// <param name="item">The new item</param>
        public void Add(T item)
        {
            T[] newArray = new T[this._array.Length + 1];
            this._array.CopyTo(newArray, 0);
            newArray[newArray.Length - 1] = item;
            this._array = newArray;
        }

        /// <summary>
        /// Adds an array of T values to this collection.
        /// </summary>
        /// <param name="array"></param>
        public void AddRange(params T[] array)
        {
            T[] newArray = new T[this._array.Length + array.Length];
            this._array.CopyTo(newArray, 0);

            for (int i = 0; i < array.Length; i++)
                newArray[this._array.Length + i] = array[i];

            this._array = newArray;
        }

        /// <summary>
        /// Adds an array of T values to the array
        /// </summary>
        /// <param name="array">The given array</param>
        public void AddRange(AList<T> array) => AddRange(array.GetAsArray());

        /// <summary>
        /// Adds a list if T values to the array
        /// </summary>
        /// <param name="arrayList">The given list</param>
        public void AddRange(List<T> arrayList) => AddRange(arrayList.ToArray());

        /// <summary>
        /// Removes an item of the array with an given item as type
        /// </summary>
        /// <param name="item">The given item which will be removed</param>
        public void Remove(T item)
        {
            if (!Contains(item))
                return;

            long sizeOfItem = MemoryUtils.GetSize(item);

            T[] newArray = new T[this._array.Length - 1];

            int position = 0;

            for (int i = 0; i < this._array.Length; i++)
            {
                if (sizeOfItem == MemoryUtils.GetSize(this._array[i]))
                {
                    T currentItem = this._array[i];

                    if (!item.Equals(currentItem))
                    {
                        newArray[position] = this._array[i];
                        position++;
                    }
                }
            }

            Array.Resize(ref this._array, this._array.Length - 1);
            Array.Copy(newArray, _array, newArray.Length);
        }

        /// <summary>
        /// Removes an entry without checking the size before identifying it
        /// </summary>
        /// <param name="item">The item which will be deleted</param>
        public void SafeRemove(T item)
        {
            if (!SafeContains(item))
                return;

            T[] newArray = new T[this._array.Length - 1];

            int position = 0;

            for (int i = 0; i < this._array.Length; i++)
            {
                T currentItem = this._array[i];

                if (!item.Equals(currentItem))
                {
                    newArray[position] = this._array[i];
                    position++;
                }
            }

            Array.Copy(newArray, _array, newArray.Length);
            Array.Resize(ref this._array, newArray.Length);
        }

        /// <summary>
        /// Removes an item of this list at an given index
        /// </summary>
        /// <param name="index">The given index</param>
        public void Remove(int index)
        {
            if (index > this._array.Length)
                return;

            T[] newArray = new T[this._array.Length - 1];

            int position = 0;

            for (int i = 0; i < this._array.Length; i++)
            {
                if (i != index)
                {
                    newArray[position] = this._array[i];
                    position++;
                }
            }

            Array.Resize(ref this._array, this._array.Length - 1);
            Array.Copy(newArray, _array, newArray.Length);
        }
        
        /// <summary>
        /// Removes items in an given range
        /// </summary>
        /// <param name="min">Minimum range</param>
        /// <param name="max">Maximum range</param>
        /// <exception cref="AListEntryException">Throws if the range is invalid</exception>
        public void RemoveRange(int minIndex, int maxIndex)
        {
            if (minIndex > maxIndex)
                throw new AListEntryException(AListEntryException.Type.InvalidRange);

            int diff = maxIndex - minIndex + 1;
    
            if (minIndex < 0 || maxIndex >= this._array.Length)
                throw new AListEntryException(AListEntryException.Type.InvalidRange);

            int newSize = this._array.Length - diff;

            for (int i = minIndex; i < newSize; i++)
            {
                this._array[i] = this._array[i + diff];
            }
    
            Array.Resize(ref this._array, newSize);
        }

        /// <summary>
        /// Converts this Generic list array to an List<T>
        /// </summary>
        /// <returns></returns>
        public List<T> GetAsList()
        {
            return new List<T>(this._array);
        }

        /// <summary>
        /// Returns the internal array for this list
        /// </summary>
        /// <returns>An array from type T</returns>
        public T[] GetAsArray()
        {
            return this._array;
        }

        /// <summary>
        /// Is empty check
        /// </summary>
        /// <returns>True, if this list is empty, False if not</returns>
        public bool IsEmpty()
        {
            return this._array.Length == 0;
        }

        /// <summary>
        /// Returns the length of this list
        /// </summary>
        public int Length
        {
            get { return this._array.Length; }
        }

        // The dirtiest way I know
        public IEnumerator<T> GetEnumerator()
        {
            return this.GetAsList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
