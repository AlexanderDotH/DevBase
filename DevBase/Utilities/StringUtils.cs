using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DevBase.Generics;

namespace DevBase.Utilities
{
    /// <summary>
    /// Provides utility methods for string manipulation.
    /// </summary>
    public class StringUtils
    {
        private static readonly Random _random = new Random();

        protected StringUtils() { }
        
        /// <summary>
        /// Generates a random string of a specified length using a given charset.
        /// </summary>
        /// <param name="length">The length of the random string.</param>
        /// <param name="charset">The characters to use for generation.</param>
        /// <returns>A random string.</returns>
        public static string RandomString(int length, string charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz")
        {
            if (length < 0)
                return string.Empty;

            return new string(Enumerable.Repeat(charset, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Joins list elements into a single string using a separator.
        /// </summary>
        /// <param name="elements">The list of strings.</param>
        /// <param name="separator">The separator string.</param>
        /// <returns>The joined string.</returns>
        public static string Separate(AList<string> elements, string separator = ", ") =>
            Separate(elements.GetAsArray(), separator);
 
        #pragma warning disable S1643
        /// <summary>
        /// Joins array elements into a single string using a separator.
        /// </summary>
        /// <param name="elements">The array of strings.</param>
        /// <param name="separator">The separator string.</param>
        /// <returns>The joined string.</returns>
        public static string Separate(string[] elements, string separator = ", ")
        {
            string pretty = string.Empty;
        
            for (int i = 0; i < elements.Length; i++)
                pretty += i == 0 ? elements[i] : separator + elements[i];
        
            return pretty;
        }
        #pragma warning restore S1643
        
        /// <summary>
        /// Splits a string into an array using a separator.
        /// </summary>
        /// <param name="elements">The joined string.</param>
        /// <param name="separator">The separator string.</param>
        /// <returns>The array of strings.</returns>
        public static string[] DeSeparate(string elements, string separator = ", ")
        {
            string[] splitted = elements.Split(separator);
            return splitted;
        }
    }
}
