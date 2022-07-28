using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevBase.Generic
{
    public class GenericTypeConversion<F, T>
    {
        public GenericList<T> MergeToList(GenericList<F> inputList, Action<F, GenericList<T>> action)
        {
            GenericList<T> convertToList = new GenericList<T>();

            for (int i = 0; i < inputList.Length; i++)
            {
                F input = inputList[i];
                action(input, convertToList);
            }

            return convertToList;
        }

        public GenericList<T> MergeToList(List<F> inputList, Action<F, GenericList<T>> action)
        {
            GenericList<T> convertToList = new GenericList<T>();

            for (int i = 0; i < inputList.Count; i++)
            {
                F input = inputList[i];
                action(input, convertToList);
            }

            return convertToList;
        }
    }
}
