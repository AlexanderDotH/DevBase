using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DevBase.Generics;

namespace DevBase.Utilities
{
    public class StringUtils
    {
        private static readonly Random _random = new Random();

        protected StringUtils() { }
        
        public static string RandomString(int length, string charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz")
        {
            if (length < 0)
                return string.Empty;

            return new string(Enumerable.Repeat(charset, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public static string Separate(AList<string> elements, string separator = ", ") =>
            Separate(elements.GetAsArray(), separator);
 
        #pragma warning disable S1643
        public static string Separate(string[] elements, string separator = ", ")
        {
            string pretty = string.Empty;
        
            for (int i = 0; i < elements.Length; i++)
                pretty += i == 0 ? elements[i] : separator + elements[i];
        
            return pretty;
        }
        #pragma warning restore S1643
        
        public static string[] DeSeparate(string elements, string separator = ", ")
        {
            string[] splitted = elements.Split(separator);
            return splitted;
        }
    }
}
