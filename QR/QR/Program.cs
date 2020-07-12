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
            Console.WriteLine(isNumeric(bytes));
            Console.WriteLine(isAlphanumeric(bytes));

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

    }
}
