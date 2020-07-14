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
            bool containsNumeric = false;
            bool containsAlphanumeric = false;
            bool containsKanji = false;
            bool containsByte = false;
            foreach (char i in line)
            {
                if (Char.IsNumber(i)) containsNumeric = true;
                else if (Char.IsLetter(i)) containsAlphanumeric = true;
                else if (8140 >= (int)i && (int)i >= 40956 || 57408 >= (int)i && (int)i <= 60351) containsKanji = true;
            }
            if (containsNumeric && !containsAlphanumeric && !containsKanji && !containsByte) return "numeric";
            else if (containsAlphanumeric && !containsKanji && !containsByte) return "alphanumeric";
            else if (containsKanji && !containsByte) return "kanji";
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
