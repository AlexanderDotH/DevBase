using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Generics;

namespace DevBase.Typography
{
    /// <summary>
    /// Represents a string wrapper with utility methods.
    /// </summary>
    public class AString
    {
        protected string _value;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AString"/> class.
        /// </summary>
        /// <param name="value">The string value.</param>
        public AString(string value)
        {
            this._value = value;
        }

        /// <summary>
        /// Converts the string to a list of lines.
        /// </summary>
        /// <returns>An AList of lines.</returns>
        public AList<string> AsList()
        {
            AList<string> genericList = new AList<string>();

            using (StringReader reader = new StringReader(this._value))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    genericList.Add(line);
                }
            }

            return genericList;
        }

        /// <summary>
        /// Capitalizes the first letter of the string.
        /// </summary>
        /// <returns>The string with the first letter capitalized.</returns>
        public string CapitalizeFirst()
        {
            return this._value.Substring(0, 1).ToUpper() + this._value.Substring(1, this._value.Length - 1);
        }

        /// <summary>
        /// Returns the string value.
        /// </summary>
        /// <returns>The string value.</returns>
        public override string ToString()
        {
            return this._value;
        }
    }
}
