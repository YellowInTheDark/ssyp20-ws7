using System;
using System.IO;
using System.Linq;
using System.Text;

namespace QR
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Write string to encode");
            string input = Console.ReadLine();
            Console.WriteLine("Choose error correction level: \n 1 - L(7%) | 2 - M(15%) | 3 - Q(25%) | 4 - H(30%)");
            int correctionLevel = int.Parse(Console.ReadLine());
            int C = 10;
            int M = 4;
            byte[] bytes = UTF8Encoding.UTF8.GetBytes(input);

            int[,] maxByteArr = new int[4, 40];
            maxByteArr = ReadCorrection();
            


            // Не уверен с порядком, если мы можем закодировать в канзи и в нумерик, к примеру, то что лучше выбрать? 
            if (isNumeric(bytes))
            {
                var bits = bytes.Length / 3 * 10; // 10 бит на каждые 3 числа
                bits += bytes.Length % 3 == 2 ? 7 : 4;
                Console.WriteLine($"{bits} bits");
                for (int i = 0; i < 40; i++)
                {
                    if(i+1 <= 10)
                        C = 10;
                    else if (i + 1 <= 26)
                        C = 12;
                    else if (i + 1 <= 47)
                        C = 14;

                    if (maxByteArr[correctionLevel - 1, i] - C - M >= bits)
                    {
                        Console.WriteLine($"Version {i + 1}");
                        break;
                    }
                }
            }
            else if (isAlphanumeric(bytes))
            {
                //...
            }
            else if (isKanji(bytes))
            {
                //...
            }

            //foreach (var item in bytes)
            //{
            //    // Вывод байтового кода каждого символа
            //    Console.Write($"{item} ");
            //}

        }

        static bool isNumeric(byte[] bytes) =>
                    bytes.Where(b => (b > 57 || b < 48)).Count() == 0;
        static bool isAlphanumeric(byte[] bytes) =>
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

        static bool isKanji(byte[] bytes)
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
