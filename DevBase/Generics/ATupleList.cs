using DevBase.Utilities;

namespace DevBase.Generics
{
    public class ATupleList<T1, T2> : AList<Tuple<T1, T2>>
    {
        public ATupleList() { }

        public ATupleList(ATupleList<T1, T2> list)
        {
            AddRange(list);
        }

        public void AddRange(ATupleList<T1, T2> anotherList) => this.AddRange(anotherList);

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

        public void Add(T1 t1, T2 t2)
        {
            this.Add(new Tuple<T1, T2>(t1, t2));
        }
    }
}