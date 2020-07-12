using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace qr
{
    public class Program
    {
        public static void Main()
        {
            var input = Console.ReadLine();
            int B;
            int M = 4;
            int D = input.Length;
            byte[] bytes = UTF8Encoding.UTF8.GetBytes(input);

            foreach (var item in bytes)
            {
                Console.WriteLine(item);
            }
            static bool numberor(byte[] bytes) =>
                    bytes.Where(b => (b > 57 || b < 48)).Count() == 0;
            static bool alphanumericor(byte[] bytes) =>
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
            static bool kanjior(byte[] bytes)
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

}