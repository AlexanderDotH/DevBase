using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Generics;

namespace DevBase.Typography
{
    public class AString
    {
        protected string _value;
        
        public AString(string value)
        {
            this._value = value;
        }

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

        public string CapitalizeFirst()
        {
            return this._value.Substring(0, 1).ToUpper() + this._value.Substring(1, this._value.Length - 1);
        }

        public override string ToString()
        {
            return this._value;
        }
    }
}
