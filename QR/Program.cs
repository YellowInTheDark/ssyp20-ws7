using System;
using System.ComponentModel;
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
                        continue;
                    else
                    {
                        Console.WriteLine($"String length in binary system: {LODA}");
                        Console.WriteLine($"Max value for current Method and version: {maxValues[i]}");
                        updateData = encodingMethod + LODA + data;
                        updateData = filling(updateData, int.Parse(maxValues[i]));
                        updateData = DivisionIntoBlocks(updateData, int.Parse(maxValues[i]), i, correctionLevel);
                        string matrix = GenerateMatrix(i+1, correctionLevel);

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


        static string filling(string data, int targetLength)
        {
            while (data.Length % 8 != 0)
            {
                data = data.Insert(data.Length,"0");
            }
            var i = 2;
            while (data.Length != targetLength)
            {
                if (i % 2 == 0)
                    data = data.Insert(data.Length , "11101100");
                else
                    data = data.Insert(data.Length , "00010001");
                i++;
            }
            return data;
        }


        static string DivisionIntoBlocks(string data, int maxBits, int version, int correctionLevel)
        {
            var maxBytes = maxBits / 8;
            string[] CountOfBlocks = File.ReadLines("versionsBlocks.txt").ElementAt(correctionLevel - 1).Split();
            var blocks = int.Parse(CountOfBlocks[version]);
            if (blocks == 1)
                return data;
            var value = maxBytes / blocks;
            int[] blocksValue = new int[blocks];
            for (var i = 0; i < blocks; i++)
            {
                blocksValue[i] = value;
            }
            var rest = maxBytes % blocks;
            var j = blocks-1;
            while (rest > 0)
            {
                blocksValue[j] += 1; 
                j--;
                rest--;
            }
           for (var k = 0; k < blocks- maxBytes % blocks; k++)
            {
                data = data.Insert((blocksValue[k]*8*(k+1)+k), " ");

            }
            var gh = 1;
            for (var k = data.Length - 8 * blocksValue[blocks - 1]; k > data.Length - (maxBytes % blocks*8* blocksValue[blocks - 1]); k-= 8 * blocksValue[blocks - 1])
            {
                data = data.Insert(k, " ");

            }
            return data;
        }


        static string GenerateMatrix(int version, int correctionLevel)
        {
            string d = "";
            var size = AllDictionaries.SizeDictionary(version-1);
            int[,] matrix = new int[size, size];
            int[,] searchPatterns = {{1,1,1,1,1,1,1}, {1,0,0,0,0,0,1}, {1,0,1,1,1,0,1}, {1,0,1,1,1,0,1}, {1,0,1,1,1,0,1}, {1,0,0,0,0,0,1}, {1,1,1,1,1,1,1}};
            int[,] alignmentPatterns = { { 1, 1, 1, 1, 1 }, { 1, 0, 0, 0, 1 }, { 1, 0, 1, 0, 1 }, { 1, 0, 0, 0, 1 }, { 1, 1, 1, 1, 1 } };
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    matrix[i, j] = searchPatterns[i, j];
                }
            }
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    matrix[i+size-7, j] = searchPatterns[i, j];
                }
            }
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    matrix[i, j+size-7] = searchPatterns[i, j];
                }
            }
            if (version == 1)
            {
            }
            else if (version < 7)
            {
                var alignmentPatternspos = int.Parse(File.ReadLines("alignmentPatternsPos.txt").ElementAt(version - 1));
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        matrix[i + alignmentPatternspos - 2, j + alignmentPatternspos - 2] = alignmentPatterns[i, j];
                    }
                }
            }
            else
            {
                string[] alignmentPatternspos = File.ReadLines("alignmentPatternsPos.txt").ElementAt(version - 1).Split();
                for (int i = 0; i < alignmentPatternspos.Length; i++)
                {
                    for (int j = 0; j < alignmentPatternspos.Length; j++)
                    {
                        if ((i == 0 && j == alignmentPatternspos.Length - 1) || (i == 0 && j == 0) || (j == 0 && i == alignmentPatternspos.Length - 1))
                            continue;
                        else
                        {
                            int X = int.Parse(alignmentPatternspos[j]);
                            int Y = int.Parse(alignmentPatternspos[i]);
                            int check = 0;
                            for (int k = 0; k < 5; k++)
                            {
                                for (int g = 0; g < 5; g++)
                                {
                                    if (matrix[k + X - 2, g + Y - 2] == 1)
                                    {
                                        check = 1;
                                        break;
                                    }    
                                    matrix[k + X - 2, g + Y - 2] = alignmentPatterns[k, g];
                                }
                                if (check == 1)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            int l = 2;
            for (int i = 7; i < size - 7; i++)
            {
                if (l % 2 == 0)
                    matrix[i, 6] = 0;
                else
                    matrix[i, 6] = 1;
                l++;
            }
            for (int i = 8; i < size - 7; i++)
            {
                if (l % 2 == 0)
                    matrix[6, i] = 0;
                else
                    matrix[6, i] = 1;
                l++;
            }
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (matrix[j, i] ==0)
                        Console.Write("  ");
                    else
                        Console.Write("██");
                }
                
                Console.WriteLine();
            }
            //matrix = matrix.
            return d;

        
        }

    }
}
