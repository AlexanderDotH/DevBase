using System.Text.RegularExpressions;
using DevBase.Format.Structure;

namespace DevBase.Format.Utilities
{
    /// <summary>
    /// Provides utility methods for manipulating lyric text lines.
    /// </summary>
    class LyricsUtils
    {
        /// <summary>
        /// Edits and cleans a lyric line, optionally replacing music symbols with a standard note symbol.
        /// </summary>
        /// <param name="line">The lyric line to edit.</param>
        /// <param name="prettify">If true, replaces various music symbols with '♪' and ensures empty lines have a note symbol.</param>
        /// <returns>The cleaned lyric line.</returns>
        public static string EditLine(string line, bool prettify = true)
        {
            string lineTrimmed = line.Trim();

            if (lineTrimmed.Contains("\\n"))
                lineTrimmed = lineTrimmed.Replace("\\n", string.Empty);

            if (!prettify)
                return lineTrimmed;
            
            if (String.IsNullOrEmpty(lineTrimmed))
                lineTrimmed = "♪";
            
            if (lineTrimmed.Contains("🎵"))
                lineTrimmed = lineTrimmed.Replace("🎵", "♪");
            
            if (lineTrimmed.Contains("🎶"))
                lineTrimmed = lineTrimmed.Replace("🎶", "♪");

            if (lineTrimmed.Contains("\ud834\udd60"))
                lineTrimmed = lineTrimmed.Replace("\ud834\udd60", "♪");

            if (lineTrimmed.Contains("\ud834\udd61"))
                lineTrimmed = lineTrimmed.Replace("\ud834\udd61", "♪");

            if (lineTrimmed.Contains("\ud834\udd62"))
                lineTrimmed = lineTrimmed.Replace("\ud834\udd62", "♪");
            
            if (lineTrimmed.Contains("\ud834\udd64"))
                lineTrimmed = lineTrimmed.Replace("\ud834\udd64", "♪");

            return lineTrimmed;
        }
    }
}
