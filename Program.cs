using System;
using System.IO;
using System.Linq;
using System.Text;


namespace QR
{
    class Program
    {

        static void Main()
        {
            
            string input = Console.ReadLine();
            byte[] bytes = UTF8Encoding.UTF8.GetBytes(input);
            
            Console.WriteLine("Correction level:");
            var correctionLevel = int.Parse(Console.ReadLine());

            string data;
            string encodingMethod;


            if (Check.Numeric(bytes))
            {
                data = Encoders.NumericCoder(input);
                Console.WriteLine($"NumericEncoder: {data}");
                encodingMethod = "0001";
            }
            else if (Check.Alphanumeric(bytes))
            {
                data = Encoders.AlphanumericCoder(input);
                Console.WriteLine($"AlphanumericEncoder: {data}");
                encodingMethod = "0010";
            }
            //else if (Check.Kanji(bytes))
            //{
            //    Console.WriteLine("3");
            //    string encodingMethod = "1000";
            //}
            else 
            {
                data = Encoders.ByteCoder(bytes);
                Console.WriteLine($"ByteEncoder: {data}");
                encodingMethod = "0100";
            }

            string updateData = UpdateData(data, input.Length, encodingMethod, correctionLevel);
            Console.WriteLine($"Result: {updateData}");
        }
      

        static string UpdateData(string data, int lengthString, string encodingMethod, int correctionLevel)
        {
            var lengthData = data.Length;
            string updateData = "";
            string[] maxValues = File.ReadLines("versions.txt").ElementAt(correctionLevel-1).Split();

            for (var i = 0; i < maxValues.Length; i++)
            {
                if (int.Parse(maxValues[i]) > lengthData)
                {

                    var LODA = LengthOfDataAmount(encodingMethod, lengthString, i + 1);
                    if (int.Parse(maxValues[i]) < lengthData + 4 + LODA.Length)
                    {
                        continue;

                    }
                    else
                    {
                        Console.WriteLine($"String length in binary system: {LODA}");
                        Console.WriteLine($"Max value for current Method and version: {maxValues[i]}");
                        updateData = encodingMethod + LODA + data;
                        break;
                    }

                }
            }
            if (updateData == "")
            {
                Console.Clear();
                Console.WriteLine("Your string is too big. Try smaller one");
                Main();
            }


            return updateData;
        }


        static string LengthOfDataAmount(string encodingMethod, int lengthString, int i)
        {
            var buff = Convert.ToString(lengthString, 2);
            if (i <= 9)
            {
                switch (encodingMethod)
                {
                    case "0001":
                        return buff.PadLeft(10, '0');
                    case "0010":
                        return buff.PadLeft(9, '0');
                    case "0100":
                        return buff.PadLeft(8, '0');
                }
            }
            if (i <= 26)
            {
                switch (encodingMethod)
                {
                    case "0001":
                        return buff.PadLeft(12, '0');
                    case "0010":
                        return buff.PadLeft(11, '0');
                    case "0100":
                        return buff.PadLeft(16, '0');
                }
            }
            if (i <= 40)
            {
                switch (encodingMethod)
                {
                    case "0001":
                        return buff.PadLeft(14, '0');
                    case "0010":
                        return buff.PadLeft(13, '0');
                    case "0100":
                        return buff.PadLeft(16, '0');
                }
            }
            return "Exception";
        }

    }
}
