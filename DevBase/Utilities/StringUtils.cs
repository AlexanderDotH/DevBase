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
        private readonly static Random _random;
        private readonly static Regex _regexBase64;

        static StringUtils()
        {
            _random = new Random();

            _regexBase64 = new Regex(@"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }
        
        public static string RandomString(int length, string charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz")
        {
            if (length < 0)
                return string.Empty;

            return new string(Enumerable.Repeat(charset, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
        
        // TODO: Unit test
        public static bool IsBase64String(string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && _regexBase64.IsMatch(s);
        }

        // TODO: Unit test
        public static string DecodeBase64(string input)
        {
            if (!IsBase64String(input))
                return string.Empty;

            byte[] decoded = Convert.FromBase64String(input);
            string rawText = Convert.ToString(decoded);

            return rawText;
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
