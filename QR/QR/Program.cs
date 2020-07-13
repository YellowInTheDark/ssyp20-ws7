using System;
using System.IO;
using System.Linq;
using System.Text;

namespace QR
{
    class Program
    {
        public static void Main()
        {
            Console.WriteLine("Write string to encode");
            string input = Console.ReadLine();
            Console.WriteLine("Choose error correction level: \n 1 - L(7%) | 2 - M(15%) | 3 - Q(25%) | 4 - H(30%)");
            if (!int.TryParse(Console.ReadLine(), out int correctionLevel) || correctionLevel < 1 || correctionLevel > 4)
            {
                throw new Exception("Correction level must be number from 1 to 4");
            }
            byte[] bytes = UTF8Encoding.UTF8.GetBytes(input);

            int version = GetVersion(bytes, correctionLevel);
        }
        
        public static int GetVersion(byte[] bytes, int correctionLevel)
        {
            int[,] maxByteArr = new int[4, 40];
            maxByteArr = ReadCorrection();
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
                       int _ when i <= 10 => 10,
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
                       int _ when i <= 10 => 9,
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
                       int _ when i <= 10 => 8,
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
                       int _ when i <= 10 => 8,
                       int _ when i <= 40 => 16,
                       _ => throw new Exception("Error")
                   } >= bits);
                if (version == 0) throw new Exception("Your text is too big. Try to choose smaller level correction");
                Console.WriteLine($"Byte | Version {version}");
                return version;
            }
        }
        static bool IsNumeric(byte[] bytes) =>
                bytes.Count(b => (b > 57 || b < 48)) == 0;
        static bool IsAlphanumeric(byte[] bytes) =>
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

        static bool IsKanji(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i += 2)
            {
                if ((bytes[i] >= 129 && bytes[i] <= 159) && (bytes[i + 1] >= 64 && bytes[i + 1] <= 126) ||
                    (bytes[i] >= 129 && bytes[i] <= 159) && (bytes[i + 1] >= 128 && bytes[i + 1] <= 252) ||

                    (bytes[i] >= 224 && bytes[i] <= 234) && (bytes[i + 1] >= 64 && bytes[i + 1] <= 126) ||
                    (bytes[i] >= 224 && bytes[i] <= 234) && (bytes[i + 1] >= 128 && bytes[i + 1] <= 252) ||
                    
                    (bytes[i] >= 234 && bytes[i] <= 235) && (bytes[i + 1] >= 64 && bytes[i + 1] <= 126) ||
                    (bytes[i] >= 224 && bytes[i] <= 234) && (bytes[i + 1] >= 128 && bytes[i + 1] <= 191))
                    return false;
            }
            return true;
        }
        
        static int[,] ReadCorrection()
        {
            String file = File.ReadAllText(@"CorrectionLevel.txt");

            int i = 0, j = 0;
            int[,] result = new int[4, 40];
            foreach (var row in file.Split('\n'))
            {
                j = 0;
                foreach (var col in row.Split(' '))
                {
                    result[i, j] = int.Parse(col.Trim());
                    j++;
                }
                i++;
            }
            return result;
        }
    }
}
