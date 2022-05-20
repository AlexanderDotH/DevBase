using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBaseFormat
{
    class LyricsUtils
    {
        public static string EditLine(string line)
        {
            if (line == null)
            {
                return "♪";
            }

            if (line.Length > 0)
            {
                if (line.Equals(""))
                {
                    line = line.Replace(string.Empty, "♪");
                }
            }
            else
            {
                line = "♪";
            }

            return line;
        }
    }
}
