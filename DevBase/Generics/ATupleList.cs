using DevBase.Utilities;

namespace DevBase.Generics
{
    /// <summary>
    /// A generic list of tuples with specialized search methods.
    /// </summary>
    /// <typeparam name="T1">The type of the first item in the tuple.</typeparam>
    /// <typeparam name="T2">The type of the second item in the tuple.</typeparam>
    public class ATupleList<T1, T2> : AList<Tuple<T1, T2>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ATupleList{T1, T2}"/> class.
        /// </summary>
        public ATupleList() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ATupleList{T1, T2}"/> class by copying elements from another list.
        /// </summary>
        /// <param name="list">The list to copy.</param>
        public ATupleList(ATupleList<T1, T2> list)
        {
            AddRange(list);
        }

        /// <summary>
        /// Adds a range of items from another ATupleList.
        /// </summary>
        /// <param name="anotherList">The list to add items from.</param>
        public void AddRange(ATupleList<T1, T2> anotherList) => this.AddRange(anotherList);

        /// <summary>
        /// Finds the full tuple entry where the first item matches the specified value.
        /// </summary>
        /// <param name="t1">The value of the first item to search for.</param>
        /// <returns>The matching tuple, or null if not found.</returns>
        public Tuple<T1, T2> FindFullEntry(T1 t1)
        {
            if (t1 == null)
                return null;

            long size = MemoryUtils.GetSize(t1);

            for (int i = 0; i < this.Length; i++)
            {
                T1 t1Object = this.Get(i).Item1;

                if (t1Object == null)
                    continue;

                if (size == MemoryUtils.GetSize(t1Object))
                {
                    if (t1.Equals(t1Object))
                    {
                        return this.Get(i);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the full tuple entry where the second item matches the specified value.
        /// </summary>
        /// <param name="t2">The value of the second item to search for.</param>
        /// <returns>The matching tuple, or null if not found.</returns>
        public Tuple<T1, T2> FindFullEntry(T2 t2)
        {
            if (t2 == null)
                return null;

            long size = MemoryUtils.GetSize(t2);

            for (int i = 0; i < this.Length; i++)
            {
                T2 t2Object = this.Get(i).Item2;

                if (t2Object == null)
                    continue;

                if (size == MemoryUtils.GetSize(t2Object))
                {
                    if (t2.Equals(t2Object))
                    {
                        return this.Get(i);
                    }
                }
            }

            return null;
        }
        
        /// <summary>
        /// Finds the second item of the tuple where the first item matches the specified value.
        /// </summary>
        /// <param name="t1">The value of the first item to search for.</param>
        /// <returns>The second item of the matching tuple, or null if not found.</returns>
        public dynamic FindEntry(T1 t1)
        {
            long size = MemoryUtils.GetSize(t1);

            for (int i = 0; i < this.Length; i++)
            {
                T1 t1Object = this.Get(i).Item1;

                if (t1Object == null)
                    continue;

                if (size == MemoryUtils.GetSize(t1Object))
                {
                    if (t1.Equals(t1Object))
                    {
                        return this.Get(i).Item2;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the first item of the tuple where the second item matches the specified value.
        /// </summary>
        /// <param name="t2">The value of the second item to search for.</param>
        /// <returns>The first item of the matching tuple, or null if not found.</returns>
        public dynamic FindEntry(T2 t2)
        {
            long size = MemoryUtils.GetSize(t2);

            for (int i = 0; i < this.Length; i++)
            {
                T2 t2Object = this.Get(i).Item2;

                if (t2Object == null)
                    continue;

                if (size == MemoryUtils.GetSize(t2Object))
                {
                    if (t2.Equals(t2Object))
                    {
                        return this.Get(i).Item1;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the second item of the tuple where the first item equals the specified value (without size check).
        /// </summary>
        /// <param name="t1">The value of the first item to search for.</param>
        /// <returns>The second item of the matching tuple, or null if not found.</returns>
        public dynamic FindEntrySafe(T1 t1)
        {
            if (t1 == null)
                return null;

            for (int i = 0; i < this.Length; i++)
            {
                T1 t1Object = this.Get(i).Item1;

                if (t1Object == null)
                    continue;

                if (t1.Equals(t1Object))
                {
                    return this.Get(i).Item2;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the first item of the tuple where the second item equals the specified value (without size check).
        /// </summary>
        /// <param name="t2">The value of the second item to search for.</param>
        /// <returns>The first item of the matching tuple, or null if not found.</returns>
        public dynamic FindEntrySafe(T2 t2)
        {
            if (t2 == null)
                return null;

            for (int i = 0; i < this.Length; i++)
            {
                T2 t2Object = this.Get(i).Item2;

                if (t2Object == null)
                    continue;

                if (t2.Equals(t2Object))
                {
                    return this.Get(i).Item1;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds all full tuple entries where the second item matches the specified value.
        /// </summary>
        /// <param name="t2">The value of the second item to search for.</param>
        /// <returns>A list of matching tuples.</returns>
        public AList<Tuple<T1, T2>> FindFullEntries(T2 t2)
        {
            if (t2 == null)
                return null;

            AList<Tuple<T1, T2>> t2AList = new AList<Tuple<T1, T2>>();

            long size = MemoryUtils.GetSize(t2);

            for (int i = 0; i < this.Length; i++)
            {
                T2 t2Object = this.Get(i).Item2;

                if (t2Object == null)
                    continue;

                if (size == MemoryUtils.GetSize(t2Object))
                {
                    if (t2.Equals(t2Object))
                    {
                        t2AList.Add(this.Get(i));
                    }
                }
            }

            return t2AList;
        }

        /// <summary>
        /// Finds all full tuple entries where the first item matches the specified value.
        /// </summary>
        /// <param name="t1">The value of the first item to search for.</param>
        /// <returns>A list of matching tuples.</returns>
        public AList<Tuple<T1, T2>> FindFullEntries(T1 t1)
        {
            if (t1 == null)
                return null;

            AList<Tuple<T1, T2>> t1AList = new AList<Tuple<T1, T2>>();

            long size = MemoryUtils.GetSize(t1);

            for (int i = 0; i < this.Length; i++)
            {
                T1 t1Object = this.Get(i).Item1;

                if (t1Object == null)
                    continue;

                if (size == MemoryUtils.GetSize(t1Object))
                {
                    if (t1.Equals(t1Object))
                    {
                        t1AList.Add(this.Get(i));
                    }
                }
            }

            return t1AList;
        }

        /// <summary>
        /// Finds all first items from tuples where the second item matches the specified value.
        /// </summary>
        /// <param name="t2">The value of the second item to search for.</param>
        /// <returns>A list of matching first items.</returns>
        public AList<T1> FindEntries(T2 t2)
        {
            if (t2 == null)
                return null;

            AList<T1> t1AList = new AList<T1>();

            long size = MemoryUtils.GetSize(t2);

            for (int i = 0; i < this.Length; i++)
            {
                T2 t2Object = this.Get(i).Item2;

                if (t2Object == null)
                    continue;

                if (size == MemoryUtils.GetSize(t2Object))
                {
                    if (t2.Equals(t2Object))
                    {
                        t1AList.Add(this.Get(i).Item1);
                    }
                }
            }

            return t1AList;
        }

        /// <summary>
        /// Finds all second items from tuples where the first item matches the specified value.
        /// </summary>
        /// <param name="t1">The value of the first item to search for.</param>
        /// <returns>A list of matching second items.</returns>
        public AList<T2> FindEntries(T1 t1)
        {
            if (t1 == null)
                return null;

            AList<T2> t2AList = new AList<T2>();

            long size = MemoryUtils.GetSize(t1);

            for (int i = 0; i < this.Length; i++)
            {
                T1 t1Object = this.Get(i).Item1;

                if (t1Object == null)
                    continue;

                if (size == MemoryUtils.GetSize(t1Object))
                {
                    if (t1.Equals(t1Object))
                    {
                        t2AList.Add(this.Get(i).Item2);
                    }
                }
            }

            return t2AList;
        }

        /// <summary>
        /// Adds a new tuple with the specified values to the list.
        /// </summary>
        /// <param name="t1">The first item.</param>
        /// <param name="t2">The second item.</param>
        public void Add(T1 t1, T2 t2)
        {
            this.Add(new Tuple<T1, T2>(t1, t2));
        }
    }
}