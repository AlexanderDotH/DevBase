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
        public Tuple<T1, T2> FindEntry(T1 t1)
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

        public Tuple<T1, T2> FindEntry(T2 t2)
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
    }
}