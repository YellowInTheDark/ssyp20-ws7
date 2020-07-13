using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QR
{
    class Encoding
    {
        public static string EncodeNumeric(string input, int version)
        {
            string encodedLine = string.Empty;
            for (int i = 0; i < input.Length / 3; i++)
            {
                int tmp = int.Parse(input.Substring(3 * i, 3));
                encodedLine += $"{Convert.ToString(tmp, 2).PadLeft(10, '0')} ";
            }
            if (input.Length % 3 == 2)
            {
                int tmp = int.Parse(input.Substring(3 * (input.Length / 3), 2));
                encodedLine += $"{Convert.ToString(tmp, 2).PadLeft(7, '0')} ";
            }
            if (input.Length % 3 == 1)
            {
                int tmp = int.Parse(input.Substring(3 * (input.Length / 3), 1));
                encodedLine += $"{Convert.ToString(tmp, 2).PadLeft(4, '0')} ";
            }

            switch (version)
            {
                case int _ when version <= 9:
                    encodedLine = encodedLine.Insert(0, $"{Convert.ToString(input.Length, 2).PadLeft(10, '0')} ");
                    break;
                case int _ when version <= 26:
                    encodedLine = encodedLine.Insert(0, $"{Convert.ToString(input.Length, 2).PadLeft(12, '0')} ");
                    break;
                case int _ when version <= 40:
                    encodedLine = encodedLine.Insert(0, $"{Convert.ToString(input.Length, 2).PadLeft(14, '0')} ");
                    break;
            }

            encodedLine = encodedLine.Insert(0, "0001 ");
            encodedLine.TrimEnd();
            return encodedLine;
        }

        public static string EncodeAlphaNumeric(string input, int version)
        {
            string encodedLine = string.Empty;
            for (int i = 0; i < input.Length-1; i+=2)
            {
                var fElement = AlphanumericDictionary(input[i]);
                var sElement = AlphanumericDictionary(input[i+1]);
                var sum = fElement * 45 + sElement;
                encodedLine += $"{Convert.ToString(sum, 2).PadLeft(11, '0')} ";
            }
            if (input.Length % 2 == 1)
            {
                int tmp = AlphanumericDictionary(input[input.Length-1]);
                encodedLine += $"{Convert.ToString(tmp, 2).PadLeft(6, '0')} ";
            }

            switch (version)
            {
                case int _ when version <= 9:
                    encodedLine = encodedLine.Insert(0, $"{Convert.ToString(input.Length, 2).PadLeft(9, '0')} ");
                    break;
                case int _ when version <= 26:
                    encodedLine = encodedLine.Insert(0, $"{Convert.ToString(input.Length, 2).PadLeft(11, '0')} ");
                    break;
                case int _ when version <= 40:
                    encodedLine = encodedLine.Insert(0, $"{Convert.ToString(input.Length, 2).PadLeft(13, '0')} ");
                    break;
            }

            encodedLine = encodedLine.Insert(0, "0010 ");
            encodedLine.TrimEnd();
            return encodedLine;
        }

        public static string EncodeByte(string input, int version)
        {
            string encodedLine = string.Empty;
            for (int i = 0; i < input.Length; i++)
            {
                var tmp = input[i];
                encodedLine += $"{Convert.ToString(tmp, 2).PadLeft(8, '0')} ";
            }
            switch (version)
            {
                case int _ when version <= 9:
                    encodedLine = encodedLine.Insert(0, $"{Convert.ToString(input.Length, 2).PadLeft(8, '0')} ");
                    break;
                case int _ when version <= 40:
                    encodedLine = encodedLine.Insert(0, $"{Convert.ToString(input.Length, 2).PadLeft(16, '0')} ");
                    break;
            }

            encodedLine = encodedLine.Insert(0, "0100 ");
            encodedLine.TrimEnd();
            return encodedLine;
        }

        public static string EncodeKanji(string input, int version)
        {
            byte[] bytes = UTF8Encoding.UTF8.GetBytes(input);
            string encodedLine = string.Empty;
            for (int i = 0; i < input.Length; i++)
            {
                //encodedLine += $"{Convert.ToString(sum, 2).PadLeft(11, '0')} ";
                return "too hard";

            }
            if (input.Length % 2 == 1)
            {
                int tmp = AlphanumericDictionary(input[input.Length - 1]);
                encodedLine += $"{Convert.ToString(tmp, 2).PadLeft(6, '0')} ";
            }

            switch (version)
            {
                case int _ when version <= 9:
                    encodedLine = encodedLine.Insert(0, $"{Convert.ToString(input.Length, 2).PadLeft(9, '0')} ");
                    break;
                case int _ when version <= 26:
                    encodedLine = encodedLine.Insert(0, $"{Convert.ToString(input.Length, 2).PadLeft(11, '0')} ");
                    break;
                case int _ when version <= 40:
                    encodedLine = encodedLine.Insert(0, $"{Convert.ToString(input.Length, 2).PadLeft(13, '0')} ");
                    break;
            }

            encodedLine = encodedLine.Insert(0, "0010 ");
            encodedLine.TrimEnd();
            return encodedLine;
        }

            static int AlphanumericDictionary(char keyValue)
        {
            Dictionary<char, int> AD = new Dictionary<char, int>
            {

                {'0', 0 },
                {'1', 1 },
                {'2', 2 },
                {'3', 3 },
                {'4', 4 },
                {'5', 5 },
                {'6', 6 },
                {'7', 7 },
                {'8', 8 },
                {'9', 9 },
                {'A', 10 },
                {'B', 11 },
                {'C', 12 },
                {'D', 13 },
                {'E', 14 },
                {'F', 15 },
                {'G', 16 },
                {'H', 17 },
                {'I', 18 },
                {'J', 19 },
                {'K', 20 },
                {'L', 21 },
                {'M', 22 },
                {'N', 23 },
                {'O', 24 },
                {'P', 25 },
                {'Q', 26 },
                {'R', 27 },
                {'S', 28 },
                {'T', 29 },
                {'U', 30 },
                {'V', 31 },
                {'W', 32 },
                {'X', 33 },
                {'Y', 34 },
                {'Z', 35 },
                {' ', 36 },
                {'$', 37 },
                {'%', 38 },
                {'*', 39 },
                {'+', 40 },
                {'-', 41 },
                {'.', 42 },
                {'/', 43 },
                {':', 44 },
            };
            return AD[keyValue];

        }

        public static int GetVersion(byte[] bytes, int correctionLevel)
        {
            MainClass main = new MainClass();
            int[,] maxByteArr = new int[4, 40];
            maxByteArr = main.ReadCorrection();
            const int M = 4;
            if (IsNumeric(bytes))
            {
                var bits = bytes.Length / 3 * 10; // 10 бит на каждые 3 числа
                bits += bytes.Length % 3 == 2 ? 7 : 4;
                Console.WriteLine($"{bits} bits");
                int version =
                Enumerable.Range(1, 40).FirstOrDefault(i =>
                   maxByteArr[correctionLevel - 1, i - 1] - M -
                   i switch
                   {
                       int _ when i <= 9 => 10,
                       int _ when i <= 26 => 12,
                       int _ when i <= 40 => 14,
                       _ => throw new Exception("Error")
                   } >= bits);
                if (version == 0) throw new Exception("Your text is too big. Try to choose smaller level correction");
                Console.WriteLine($"Numeric | Version {version}");
                return version;
            }
            else if (IsAlphanumeric(bytes))
            {
                var bits = bytes.Length / 2 * 11;
                bits += (bytes.Length % 2) * 6;
                Console.WriteLine($"{bits} bits");
                int version =
                Enumerable.Range(1, 40).FirstOrDefault(i =>
                   maxByteArr[correctionLevel - 1, i - 1] - M -
                   i switch
                   {
                       int _ when i <= 9 => 9,
                       int _ when i <= 26 => 11,
                       int _ when i <= 40 => 13,
                       _ => throw new Exception("Error")
                   } >= bits);
                if (version == 0) throw new Exception("Your text is too big. Try to choose smaller level correction");
                Console.WriteLine($"AlphaNumeric | Version {version}");
                return version;
            }
            else if (IsKanji(bytes))
            {
                var bits = bytes.Length * 13;
                Console.WriteLine($"{bits} bits");
                int version =
                Enumerable.Range(1, 40).FirstOrDefault(i =>
                   maxByteArr[correctionLevel - 1, i - 1] - M -
                   i switch
                   {
                       int _ when i <= 9 => 8,
                       int _ when i <= 26 => 10,
                       int _ when i <= 40 => 12,
                       _ => throw new Exception("Error")
                   } >= bits);
                if (version == 0) throw new Exception("Your text is too big. Try to choose smaller level correction");
                Console.WriteLine($"Kanji | Version {version}");
                return version;
            }
            else
            {
                var bits = bytes.Length * 8;
                Console.WriteLine($"{bits} bits");
                int version =
                Enumerable.Range(1, 40).FirstOrDefault(i =>
                   maxByteArr[correctionLevel - 1, i - 1] - M -
                   i switch
                   {
                       int _ when i <= 9 => 8,
                       int _ when i <= 40 => 16,
                       _ => throw new Exception("Error")
                   } >= bits);
                if (version == 0) throw new Exception("Your text is too big. Try to choose smaller level correction");
                Console.WriteLine($"Byte | Version {version}");
                return version;
            }
        }
        public static bool IsNumeric(byte[] bytes) =>
                bytes.Count(b => (b > 57 || b < 48)) == 0;
        public static bool IsAlphanumeric(byte[] bytes) =>
            bytes.Count(b => b switch
            {
                byte x when
                    b <= 57 && b >= 48 ||
                    b >= 36 && b <= 37 ||
                    b >= 42 && b <= 43 ||
                    b >= 45 && b <= 47 ||
                    b >= 65 && b <= 90 ||
                    b == 58 ||
                    b == 32
                    => false,
                _ => true
            }) == 0;

        public static bool IsKanji(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i += 2)
            {
                if (bytes[i] >= 129 && bytes[i] <= 159 && bytes[i + 1] >= 64 && bytes[i + 1] <= 126 ||
                    bytes[i] >= 129 && bytes[i] <= 159 && bytes[i + 1] >= 128 && bytes[i + 1] <= 252 ||

                    bytes[i] >= 224 && bytes[i] <= 234 && bytes[i + 1] >= 64 && bytes[i + 1] <= 126 ||
                    bytes[i] >= 224 && bytes[i] <= 234 && bytes[i + 1] >= 128 && bytes[i + 1] <= 252 ||

                    bytes[i] >= 234 && bytes[i] <= 235 && bytes[i + 1] >= 64 && bytes[i + 1] <= 126 ||
                    bytes[i] >= 224 && bytes[i] <= 234 && bytes[i + 1] >= 128 && bytes[i + 1] <= 191)
                { }
                else 
                    return false;
            }
            return true;
        }

    }
}
