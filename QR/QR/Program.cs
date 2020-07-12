using System;
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
            Console.WriteLine(isNumeric(bytes));
            Console.WriteLine(isAlphanumeric(bytes));

        }

        static bool isNumeric(byte[] bytes)
        {
            foreach (var item in bytes)
            {
                // 48 - 57
                if (item > 57 || item < 48)
                    return false;
            }
            return true;
        }
        static bool isAlphanumeric(byte[] bytes)
        {
            foreach (var item in bytes)
            {
                bool isInBounds = false;
                // 48 - 57; 65 - 90
                if ((item <= 57) && (item >= 48))
                    isInBounds = true;
                else if ((item >= 65) && (item <= 90))
                    isInBounds = true;
                else if ((item >= 36) && (item <= 37))
                    isInBounds = true;
                else if ((item >= 42) && (item <= 43))
                    isInBounds = true;
                else if ((item >= 45) && (item <= 47))
                    isInBounds = true;
                else if (item == 58)
                    isInBounds = true;
                else if (item == 32)
                    isInBounds = true;
                if (!isInBounds) return false;
            }
            return true;
        }
    }
}
