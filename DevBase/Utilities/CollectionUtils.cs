using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Exception;
using DevBase.Generic;

namespace DevBase.Utilities
{
    public class CollectionUtils
    {
        /// <summary>
        /// Appends to every item inside this list a given item of the other list
        ///
        /// List sizes should be equal or it throws
        /// <see cref="GenericListEntryException"/>
        /// </summary>
        ///
        /// <returns>Returns a new list with the merged entries</returns>
        public static GenericList<string> MergeList(List<string> first, List<string> second, string marker = "")
        {
            if (first.Count != second.Count)
                throw new GenericListEntryException(GenericListEntryException.Type.ListNotEqual);

            GenericList<string> returnList = new GenericList<string>();

            for (int i = 0; i < first.Count; i++)
            {
                returnList.Add(first[i] + marker + second[i]);
            }

            return returnList;
        }
    }
}
