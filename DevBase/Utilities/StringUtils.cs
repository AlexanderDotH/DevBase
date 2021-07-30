using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBase.Utilities
{
    public class StringUtils
    {
        public static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();

            string buildSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvxyz";

            Random r = new Random();

            for (int i = 0; i < size; i++)
            {
                builder.Append(buildSet[r.Next(0, buildSet.Length)]);
            }

            Console.WriteLine(builder.ToString());

            return builder.ToString();
        }
    }
}
