using System;
using System.Linq;
using System.Text;

namespace QR
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = Console.ReadLine();
            byte[] bytes = UTF8Encoding.UTF8.GetBytes(input);

            foreach (var item in bytes)
            {
                // Вывод байтового кода каждого символа
                Console.WriteLine(item);
            }
            // Вывод принадлежности к разрядам
            
            // Проверка для кандзи
            //bytes = new byte[2];
            //bytes[0] = 159;
            //bytes[1] = 126;
            Console.WriteLine(isNumeric(bytes));
            Console.WriteLine(isAlphanumeric(bytes));
            Console.WriteLine(isKanji(bytes));

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
    }
}
