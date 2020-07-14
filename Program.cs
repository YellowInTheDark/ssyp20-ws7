using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace QR
{
    class Program
    {
        static void Main(string[] args)
        {
            
            string input = Console.ReadLine();
            byte[] bytes = UTF8Encoding.UTF8.GetBytes(input);
            Console.WriteLine("Correction level:");
            var correctionLevel = int.Parse(Console.ReadLine());
            string data;
            string encodingMethod;


            if (Numeric(bytes))
            {
                data = NumericCoder(input);
                Console.WriteLine($"NumericEncoder: {data}");
                encodingMethod = "0001";
            }
            else if (Alphanumeric(bytes))
            {
                data = AlphanumericCoder(input);
                Console.WriteLine($"AlphanumericEncoder: {data}");
                encodingMethod = "0010";
            }
            //else if (Kanji(bytes))
            //{
            //    Console.WriteLine("3");
            //    string encodingMethod = "1000";
            //}
            else 
            {
                data = ByteCoder(bytes);
                Console.WriteLine($"ByteEncoder: {data}");
                encodingMethod = "0100";
            }

            string updateData = UpdateData(data, input.Length, encodingMethod, correctionLevel);
            Console.WriteLine($"Result: {updateData}");
        }
        static bool Numeric(byte[] bytes) =>
                    bytes.Where(b => (b > 57 || b < 48)).Count() == 0;
        static string NumericCoder(string input)
        {
            string BinaryString = " ";
            while (input.Length >= 3)
            {
                string sum = input.Remove(3, input.Length-3);
                var buff = Convert.ToString(int.Parse(sum), 2);
                for (; buff.Length < 10;)
                {
                    buff = buff.Insert(0,"0");
                }
                BinaryString = BinaryString.Insert(BinaryString.Length-1, buff);
                input = input.Remove(0,3);              
            }
            if (input.Length == 2)
            {
                var buff = Convert.ToString(int.Parse(input), 2);
                for (; buff.Length < 7;)
                {
                    buff = buff.Insert(0, "0");
                }
                BinaryString = BinaryString.Insert(BinaryString.Length - 1, buff);
                input = input.Remove(0, 2);
            }
            else if (input.Length == 1)
            {
                var buff = Convert.ToString(int.Parse(input), 2);
                for (; buff.Length < 4;)
                {
                    buff = buff.Insert(0, "0");
                }
                BinaryString = BinaryString.Insert(BinaryString.Length - 1, buff);
            }
            BinaryString = BinaryString.Replace(" ", "");
            return BinaryString;
        }


        static bool Alphanumeric(byte[] bytes) =>
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
        static string AlphanumericCoder(string input)
        {
            string BinaryString = " ";
            while (input.Length >= 2)
            {
                var Felement = AlphanumericDictionary(input[0]);
                var Selement = AlphanumericDictionary(input[1]);
                var sum = Felement * 45 + Selement;
                var buff = Convert.ToString(sum, 2);
                for (; buff.Length < 11;)
                {
                    buff = buff.Insert(0, "0");
                }
                BinaryString = BinaryString.Insert(BinaryString.Length - 1, buff);
                input = input.Remove(0, 2);
            }
            if (input.Length == 1)
            {
                var buff = Convert.ToString(AlphanumericDictionary(input[0]), 2);
                for (; buff.Length < 6;)
                {
                    buff = buff.Insert(0, "0");
                }
                BinaryString = BinaryString.Insert(BinaryString.Length - 1, buff);
            }
            BinaryString = BinaryString.Replace(" ", "");
            return BinaryString;

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


        static bool Kanji(byte[] bytes)
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
      
        
        static bool Byte(byte[] bytes)
        {
            return false;
        }
        static string ByteCoder(byte[] bytes)
        {
            string streamBits = " ";
            foreach (var item in bytes)
            {
                var buff = Convert.ToString(item, 2);
                    for (; buff.Length < 8;)
                    {
                        buff = buff.Insert(0, "0");
                    }
                streamBits = streamBits.Insert(streamBits.Length - 1, buff);
            }
            streamBits = streamBits.Replace(" ", "");
            return streamBits;
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
                        Console.WriteLine($"lengthData: {LODA}");
                        Console.WriteLine($"Max value for current Method and version: {maxValues[i]}");
                        updateData = encodingMethod + LODA + data;
                        break;
                    }

                }
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
                        for (; buff.Length < 10;)
                        {
                            buff = buff.Insert(0, "0");
                        }
                        return buff;
                    case "0010":
                        for (; buff.Length < 9;)
                        {
                            buff = buff.Insert(0, "0");
                        }
                        return buff;
                    case "0100":
                        for (; buff.Length < 8;)
                        {
                            buff = buff.Insert(0, "0");
                        }
                        return buff;
                }
            }
            if (i <= 26)
            {
                switch (encodingMethod)
                {
                    case "0001":
                        for (; buff.Length < 12;)
                        {
                            buff = buff.Insert(0, "0");
                        }
                        return buff;
                    case "0010":
                        for (; buff.Length < 11;)
                        {
                            buff = buff.Insert(0, "0");
                        }
                        return buff;
                    case "0100":
                        for (; buff.Length < 16;)
                        {
                            buff = buff.Insert(0, "0");
                        }
                        return buff;
                }
            }
            if (i <= 40)
            {
                switch (encodingMethod)
                {
                    case "0001":
                        for (; buff.Length < 14;)
                        {
                            buff = buff.Insert(0, "0");
                        }
                        return buff;
                    case "0010":
                        for (; buff.Length < 13;)
                        {
                            buff = buff.Insert(0, "0");
                        }
                        return buff;
                    case "0100":
                        for (; buff.Length < 16;)
                        {
                            buff = buff.Insert(0, "0");
                        }
                        return buff;
                }
            }
            return "";
        }

    }
}
