using System;
using System.IO;
using System.Linq;
using System.Text;

namespace QR
{
    public class MainClass
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

            int version = Encoding.GetVersion(bytes, correctionLevel);
            if (Encoding.IsNumeric(bytes)) Console.WriteLine(Encoding.EncodeNumeric(input, version));
            else if (Encoding.IsAlphanumeric(bytes)) Console.WriteLine(Encoding.EncodeAlphaNumeric(input, version));
        }

        public int[,] ReadCorrection()
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
