namespace DevBase.Generics
{
    /// <summary>
    /// Provides functionality to convert and merge lists of one type into another using a conversion action.
    /// </summary>
    /// <typeparam name="F">The source type.</typeparam>
    /// <typeparam name="T">The target type.</typeparam>
    public class GenericTypeConversion<F, T>
    {
        /// <summary>
        /// Merges an AList of type F into an AList of type T using the provided action.
        /// </summary>
        /// <param name="inputList">The source list.</param>
        /// <param name="action">The action to perform conversion and addition to the target list.</param>
        /// <returns>The resulting list of type T.</returns>
        public AList<T> MergeToList(AList<F> inputList, Action<F, AList<T>> action)
        {
            AList<T> convertToList = new AList<T>();

            for (int i = 0; i < inputList.Length; i++)
            {
                F input = inputList[i];
                action(input, convertToList);
            }

            return convertToList;
        }

        /// <summary>
        /// Merges a List of type F into an AList of type T using the provided action.
        /// </summary>
        /// <param name="inputList">The source list.</param>
        /// <param name="action">The action to perform conversion and addition to the target list.</param>
        /// <returns>The resulting list of type T.</returns>
        public AList<T> MergeToList(List<F> inputList, Action<F, AList<T>> action)
        {
            AList<T> convertToList = new AList<T>();

            for (int i = 0; i < inputList.Count; i++)
            {
                F input = inputList[i];
                action(input, convertToList);
            }

            return convertToList;
        }
    }
}
