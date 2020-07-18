using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;


namespace QR
{
    class Program
    {

        static void Main()
        {
            int c = (byte)((255 % 255 + 1 % 255) % 255);
            byte b = (byte)((255 % 255 + 1 % 255) % 255);
            Console.WriteLine(11^17);
            Console.WriteLine(b);

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
                        Console.WriteLine($"Max value for current Method and version {i+1}: {maxValues[i]}");
                        updateData = encodingMethod + LODA + data;
                        updateData = filling(updateData, int.Parse(maxValues[i]));
                        updateData = DivisionIntoBlocks(updateData, int.Parse(maxValues[i]), i, correctionLevel);
                        string matrix = GenerateMatrix(i+1, correctionLevel);
                        updateData = LastAlgorithm(updateData, correctionLevel,i);
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
                char[] versionCode = File.ReadLines("versionCode.txt").ElementAt(version - 1).ToCharArray();
                
                for (var i = 0; i < 3; i++)
                {
                    for (var j = 0; j < 6; j++)
                    {
                        if (Convert.ToInt32(versionCode[6 * i + j]) == 48)
                        {
                            matrix[size - 11 + i, j] = 0;
                            matrix[j, size - 11 + i] = 0;
                        }
                        else
                        {
                            matrix[size - 11 + i, j] = 1;
                            matrix[j, size - 11 + i] = 1;
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


        static string LastAlgorithm(string data, int correctionLevel, int version)// версия минус один
        {
            var data2 = data;
            string[] CountOfBlocks = File.ReadLines("versionsBlocks.txt").ElementAt(correctionLevel - 1).Split();
            var NumberOfBlocks = int.Parse(CountOfBlocks[version]);
            string[] CountOfCorrectionBytes = File.ReadLines("NumberOfCorrectionBytersPerBlock.txt").ElementAt(correctionLevel - 1).Split();
            var NumberOfCorrectionBytes = int.Parse(CountOfCorrectionBytes[version]);
            string[] ArrayCorrectionBytes = new string[NumberOfBlocks* NumberOfCorrectionBytes];

            var nb = 1;
            for (var i = 0; i < NumberOfBlocks; i++)
            {
                string block;
                if (data2.IndexOf(' ') == -1)
                    block = data2;
                else
                    block = data2.Remove(data2.IndexOf(' '));
                int[] bytes = new int[block.Length/8];
                var j = 0;
                while (block.Length != 0)
                { 
                if (block.Length == 8)
                        bytes[j] = Convert.ToInt32(block, 2);
                else
                        bytes[j] = Convert.ToInt32(block.Remove(8), 2);
                    block = block.Remove(0, 8);
                    j++;
                }
                var g = NumberOfCorrectionBytes;
                if (bytes.Length > NumberOfCorrectionBytes)
                    g = bytes.Length;
                int[] BArray = new int[g];
                for (j = 0; j < bytes.Length; j++)
                {
                    BArray[j] = bytes[j];
                }
                int[] arrayOfCorrectionBytes = AllDictionaries.PolynomialDict(NumberOfCorrectionBytes);
                //foreach (var item in arrayOfCorrectionBytes)
                //    Console.Write(item + " ");
               // Console.WriteLine();
               // foreach (var item in BArray)
                //    Console.Write(item + " ");
               // Console.WriteLine();




                for (int h = 0; h < bytes.Length; h++)
                {
                   // Console.WriteLine(BArray[0]);
                    var firstItem = int.Parse(File.ReadLines("ReverseTable.txt").ElementAt(BArray[0]));

                    for (int b = 0; b < g-1; b++)

                    {
                        BArray[b] = BArray[b + 1];
                    }
                    BArray[g - 1] = 0;
                   // foreach (var item in BArray)
                    //    Console.Write(item + " ");
                   // Console.WriteLine(firstItem);



                    if (firstItem == -1)
                        continue;
                    var B = 0;
                    for (int b = 0; b < NumberOfCorrectionBytes; b++)
                    {
                        B = arrayOfCorrectionBytes[b] + firstItem;
                        if (B > 254)
                            B = B % 255;
                        B = int.Parse(File.ReadLines("Table.txt").ElementAt(B));
                        B = B ^ BArray[b];
                        BArray[b] = B;
                        //if (arrayOfCorrectionBytes[b] == 0)
                        //    arrayOfCorrectionBytes[b] = 255;
                        // Console.WriteLine(arrayOfCorrectionBytes[b] + " ");

                        //Console.WriteLine(arrayOfCorrectionBytes[b]);
                        //arrayOfCorrectionBytes[b] = int.Parse(File.ReadLines("Table.txt").ElementAt(arrayOfCorrectionBytes[b]));
                        //Console.WriteLine(arrayOfCorrectionBytes[b]);

                        //Console.WriteLine($"{arrayOfCorrectionBytes[b]} {BArray[b]} {arrayOfCorrectionBytes[b] ^ BArray[b]} ");
                        //arrayOfCorrectionBytes[b] = arrayOfCorrectionBytes[b] ^ BArray[b];
                        //Console.WriteLine(arrayOfCorrectionBytes[b]);
                    }
                    
                }
                string y;
                
                for (var item = 1; item<= NumberOfCorrectionBytes; item++)
                {
                    var f  = Convert.ToString(BArray[item-1], 2);
                    f = f.PadLeft(8, '0');

                    ArrayCorrectionBytes[item*nb-1] = f;
                }
                
                data2 = data2.Remove(0, data2.IndexOf(' ')+1);
                nb++;
            }
            foreach (var item in ArrayCorrectionBytes)
                Console.Write(item + " ");
            while (data.Length > 0)
            {
                data2 = 
            }
             

            return data;
        }

    }
}
