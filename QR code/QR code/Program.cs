using System;
using System.Globalization;
using System.IO;

namespace QR_code
{
    class Program
    {
        static string BestMode(string line)
        {
            // numeric, alphanumeric, byte, kanji
            bool numeric = false;
            bool alphanumeric = false;
            bool kanji = false;
            bool other = false;
            foreach (char i in line)
            {
                int j = (int)i;
                if (48 <= j && j <= 57) numeric = true;
                else if (65 <= j && j <= 90 || j == 32 || j == 36 || j == 37 || j == 42 || j == 43 || j == 43 || j == 45 || j == 46 || j == 47 || j == 58) alphanumeric = true;
                else if (8140 >= (int)i && (int)i >= 40956 || 57408 >= (int)i && (int)i <= 60351) kanji = true;
                else other = true;
            }
            if (numeric && !alphanumeric && !kanji && !other) return "numeric";
            else if (alphanumeric && !kanji && !other) return "alphanumeric";
            else if (!numeric && !alphanumeric && kanji && !other) return "kanji";
            else return "byte";
        }
        static void Main()
        {
            string line = "";
            try
            {
                line = File.ReadAllText("./input.txt");
            }
            catch
            {
                Console.WriteLine("Input Error");
                Environment.Exit(0);
            }

            Console.WriteLine(BestMode(line));
        }
    }
}
