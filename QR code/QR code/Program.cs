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
                int j = (int)i;
                if (48 <= j && j <= 57) containsNumeric = true;
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
            // Console.WriteLine($"{(int)' '} {(int)'$'} {(int)'%'} {(int)'*'} {(int)'+'} {(int)'-'} {(int)'.'} {(int)'/'} {(int)':'}");
            /*
            0	1	2	3	4	5	6	7	8	9	// [48; 57]
            A	B	C	D	E   F	G	H	I	J	K	L	M	N	O	P	Q	R	S	T   U	V	W	X	Y	Z	// [65; 90]
            Пробел	$	%	*	+	-	.	/	: // 32 36 37 42 43 45 46 47 58
            */
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
