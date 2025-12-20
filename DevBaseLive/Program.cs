using System.Globalization;
using System.Text;
using CsvHelper;
using DevBaseLive.Objects;
using DevBaseLive.Tracks;
using Newtonsoft.Json;

namespace DevBaseLive
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            StringBuilder charBuilder = new StringBuilder();
            StringBuilder hexBuilder = new StringBuilder();

            while (true)
            {
                // Ausgabe
                Console.SetCursorPosition(0, 0);

                Console.Write(hexBuilder + "\n");
                Console.Write(charBuilder);

                char lastKey = Console.ReadKey().KeyChar;

                if (lastKey == '\r')
                {
                    Console.WriteLine();
                    continue;
                }

                // Unicode-Zeichencode ermitteln

                // Hexadezimale Darstellung mit führenden Nullen

                byte[] buffer = Encoding.UTF8.GetBytes(lastKey.ToString());

                string hex = string.Concat(buffer.Select(b => b.ToString("X2")));

                if (hex.Length == 1)
                {
                    hex = $"0x0{hex}, ";
                }
                else
                {
                    hex = $"0x{hex}, ";
                }

                charBuilder.Append(lastKey);
                hexBuilder.Append(hex);
            }
        }
    }
}