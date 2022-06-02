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
        public Tuple<T1, T2> FindFullEntry(T1 t1)
        {
            long size = MemoryUtils.GetSize(t1);

            for (int i = 0; i < this.Length; i++)
            {
                if (size == MemoryUtils.GetSize(this.Get(i).Item1))
                {
                    if (t1.Equals(this.Get(i).Item1))
                    {
                        return this.Get(i);
                    }
                }
            }

            return null;
        }

        public Tuple<T1, T2> FindFullEntry(T2 t2)
        {
            long size = MemoryUtils.GetSize(t2);

            for (int i = 0; i < this.Length; i++)
            {
                if (size == MemoryUtils.GetSize(this.Get(i).Item2))
                {
                    if (t2.Equals(this.Get(i).Item2))
                    {
                        return this.Get(i);
                    }
                }
            }

            return null;
        }

        public T2 FindEntry(T1 t1)
        {
            long size = MemoryUtils.GetSize(t1);

            for (int i = 0; i < this.Length; i++)
            {
                if (size == MemoryUtils.GetSize(this.Get(i).Item1))
                {
                    if (t1.Equals(this.Get(i).Item1))
                    {
                        return this.Get(i).Item2;
                    }
                }
            }

            return (T2)new object();
        }

        public T1 FindEntry(T2 t2)
        {
            long size = MemoryUtils.GetSize(t2);

            for (int i = 0; i < this.Length; i++)
            {
                if (size == MemoryUtils.GetSize(this.Get(i).Item2))
                {
                    if (t2.Equals(this.Get(i).Item2))
                    {
                        return this.Get(i).Item1;
                    }
                }
            }

            return default;
        }


        public GenericList<Tuple<T1, T2>> FindFullEntries(T2 t2)
        {
            GenericList<Tuple<T1, T2>> t2GenericList = new GenericList<Tuple<T1, T2>>();

            long size = MemoryUtils.GetSize(t2);

            for (int i = 0; i < this.Length; i++)
            {
                if (size == MemoryUtils.GetSize(this.Get(i).Item2))
                {
                    if (t2.Equals(this.Get(i).Item2))
                    {
                        t2GenericList.Add(this.Get(i));
                    }
                }
            }

            return t2GenericList;
        }

        public GenericList<Tuple<T1, T2>> FindFullEntries(T1 t1)
        {
            GenericList<Tuple<T1, T2>> t1GenericList = new GenericList<Tuple<T1, T2>>();

            long size = MemoryUtils.GetSize(t1);

            for (int i = 0; i < this.Length; i++)
            {
                if (size == MemoryUtils.GetSize(this.Get(i).Item1))
                {
                    if (t1.Equals(this.Get(i).Item1))
                    {
                        t1GenericList.Add(this.Get(i));
                    }
                }
            }

            return t1GenericList;
        }

        public GenericList<T1> FindEntries(T2 t2)
        {
            GenericList<T1> t1GenericList = new GenericList<T1>();

            long size = MemoryUtils.GetSize(t2);

            for (int i = 0; i < this.Length; i++)
            {
                if (size == MemoryUtils.GetSize(this.Get(i).Item2))
                {
                    if (t2.Equals(this.Get(i).Item2))
                    {
                        t1GenericList.Add(this.Get(i).Item1);
                    }
                }
            }

            return t1GenericList;
        }

        public GenericList<T2> FindEntries(T1 t2)
        {
            GenericList<T2> t2GenericList = new GenericList<T2>();

            long size = MemoryUtils.GetSize(t2);

            for (int i = 0; i < this.Length; i++)
            {
                if (size == MemoryUtils.GetSize(this.Get(i).Item2))
                {
                    if (t2.Equals(this.Get(i).Item2))
                    {
                        t2GenericList.Add(this.Get(i).Item2);
                    }
                }
            }

            return t2GenericList;
        }
    }
}