﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DevBaseFormat.Structure;

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

                if (Regex.IsMatch(line, RegexHolder.REGEX_TIMESTAMP) ||
                    Regex.IsMatch(line, RegexHolder.REGEX_DETAILED_TIMESTAMP))
                {
                    line = Regex.Replace(line, RegexHolder.REGEX_TIMESTAMP, string.Empty);
                    line = Regex.Replace(line, RegexHolder.REGEX_DETAILED_TIMESTAMP, string.Empty);
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
