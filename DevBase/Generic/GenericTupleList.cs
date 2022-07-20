using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DevBase.Exception;
using DevBase.Utilities;

namespace DevBase.Generic
{
    public class GenericTupleList<T1, T2> : GenericList<Tuple<T1, T2>>
    {
        public GenericTupleList() { }

        public GenericTupleList(GenericTupleList<T1, T2> list)
        {
            AddRange(list);
        }

        public void AddRange(GenericTupleList<T1, T2> anotherList)
        {
            for (int i = 0; i < anotherList.Length; i++)
            {
                Tuple<T1, T2> e = anotherList.Get(i);
                this.Add(e);
            }
        }

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

        public GenericList<Tuple<T1, T2>> FindFullEntries(T2 t2)
        {
            if (t2 == null)
                return null;

            GenericList<Tuple<T1, T2>> t2GenericList = new GenericList<Tuple<T1, T2>>();

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
                        t2GenericList.Add(this.Get(i));
                    }
                }
            }

            return t2GenericList;
        }

        public GenericList<Tuple<T1, T2>> FindFullEntries(T1 t1)
        {
            if (t1 == null)
                return null;

            GenericList<Tuple<T1, T2>> t1GenericList = new GenericList<Tuple<T1, T2>>();

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
                        t1GenericList.Add(this.Get(i));
                    }
                }
            }

            return t1GenericList;
        }

        public GenericList<T1> FindEntries(T2 t2)
        {
            if (t2 == null)
                return null;

            GenericList<T1> t1GenericList = new GenericList<T1>();

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
                        t1GenericList.Add(this.Get(i).Item1);
                    }
                }
            }

            return t1GenericList;
        }

        public GenericList<T2> FindEntries(T1 t1)
        {
            if (t1 == null)
                return null;

            GenericList<T2> t2GenericList = new GenericList<T2>();

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
                        t2GenericList.Add(this.Get(i).Item2);
                    }
                }
            }

            return t2GenericList;
        }

        public void Add(T1 t1, T2 t2)
        {
            this.Add(new Tuple<T1, T2>(t1, t2));
        }
    }
}