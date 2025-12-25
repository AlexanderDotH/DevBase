using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Exception;
using DevBase.Generics;

namespace DevBase.Utilities
{
    /// <summary>
    /// Provides utility methods for collections.
    /// </summary>
    public class CollectionUtils
    {
        /// <summary>
        /// Appends to every item inside this list a given item of the other list
        ///
        /// List sizes should be equal or it throws
        /// <see cref="AListEntryException"/>
        /// </summary>
        /// <param name="first">The first list.</param>
        /// <param name="second">The second list to merge with.</param>
        /// <param name="marker">The separator string between merged items.</param>
        /// <returns>Returns a new list with the merged entries</returns>
        public static AList<string> MergeList(List<string> first, List<string> second, string marker = "")
        {
            if (first.Count != second.Count)
                throw new AListEntryException(AListEntryException.Type.ListNotEqual);

            AList<string> returnList = new AList<string>();

            for (int i = 0; i < first.Count; i++)
            {
                returnList.Add(first[i] + marker + second[i]);
            }

            return returnList;
        }
    }
}
