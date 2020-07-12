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
            bytes.Where(b => (b > 57 || b < 48)).Count() == 0 ||
            bytes.Where(b => (b > 57) || (b < 48)).Count() == 0 ||
            bytes.Where(b => (b < 65) || (b > 90)).Count() == 0 ||
            bytes.Where(b => (b < 36) || (b > 37)).Count() == 0 ||
            bytes.Where(b => (b < 42) || (b > 43)).Count() == 0 ||
            bytes.Where(b => (b < 43) && (b > 47)).Count() == 0 ||
            bytes.Where(b => (b != 58)).Count() == 0 ||
            bytes.Where(b => (b != 32)).Count() == 0;
    }
}
