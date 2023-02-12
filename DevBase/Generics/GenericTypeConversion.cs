namespace DevBase.Generics
{
    public class GenericTypeConversion<F, T>
    {
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
