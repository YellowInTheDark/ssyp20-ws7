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
            string input = "";
            try
            {
                input = File.ReadAllText("./input.txt");
            }
            catch
            {
                Console.WriteLine("Input Error");
                Environment.Exit(0);
            }

            string bestMode = BestMode(input);
            string infoLine = "";
            if (bestMode == "numeric")
            {
                int len = input.Length;
                for (int i = 0; i <= len - 3; i += 3)
                {
                    int trio = int.Parse(input.Substring(i, 3));
                    string tempLine = Convert.ToString(trio, 2);
                    for (int j = tempLine.Length; j < 10; j++) infoLine += "0";
                    infoLine += tempLine;
                }
                if (len % 3 == 1)
                {
                    int ost = int.Parse(input.Substring(len - 1, 1));
                    string tempLine = Convert.ToString(ost, 2);
                    for (int j = tempLine.Length; j < 4; j++) infoLine += "0";
                    infoLine += tempLine;
                }
                else if (len % 3 == 2)
                {
                    int ost = int.Parse(input.Substring(len - 2, 2));
                    string tempLine = Convert.ToString(ost, 2);
                    for (int j = tempLine.Length; j < 7; j++) infoLine += "0";
                    infoLine += tempLine;
                }
            }
            Console.WriteLine(infoLine);
        }
    }
}
