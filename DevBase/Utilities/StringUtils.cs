using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Generics;

namespace DevBase.Utilities
{
    public class StringUtils
    {
        private readonly static Random Random = new Random();
        public static string RandomString(int length, string charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz")
        {
            if (length < 0)
                return string.Empty;

            return new string(Enumerable.Repeat(charset, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static string StringArrayToString(string[] array)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < array.Length; i++)
                sb.Append(array[i]);

            return sb.ToString();
        }

        public static string Separate(AList<string> elements, string separator = ", ") =>
            Separate(elements.GetAsArray(), separator);
        
        public static string Separate(string[] elements, string separator = ", ")
        {
            string pretty = string.Empty;

            for (int i = 0; i < elements.Length; i++)
                pretty += i == 0 ? elements[i] : separator + elements[i];

            return pretty;
        }
        
        public static string[] DeSeparate(string elements, string separator = ", ")
        {
            string[] splitted = elements.Split(separator);
            return splitted;
        }
    }
}
