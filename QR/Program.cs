using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;


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
            string updateData;

            if (Check.Numeric(bytes))
            {
                data = Encoders.NumericCoder(input);
                //Console.WriteLine($"NumericEncoder: {data}");
                encodingMethod = "0001";
                updateData = UpdateData(data, input.Length, encodingMethod, correctionLevel);
            }
            else if (Check.Alphanumeric(bytes))
            {
                data = Encoders.AlphanumericCoder(input);
                //Console.WriteLine($"AlphanumericEncoder: {data}");
                encodingMethod = "0010";
                updateData = UpdateData(data, input.Length, encodingMethod, correctionLevel);
            }
            //else if (Check.Kanji(bytes))
            //{
            //    data = Encoders.KanjiCoder(input, bytes);
            //    Console.WriteLine($"KanjiEncoder: {data}");
            //    encodingMethod = "1000";
            //}
            else 
            {
                data = Encoders.ByteCoder(bytes);
                //Console.WriteLine($"ByteEncoder: {data}");
                encodingMethod = "0100";
                updateData = UpdateData(data, (data.Length)/8, encodingMethod, correctionLevel);
            }
            
            ////Console.WriteLine($"Result: {updateData}");

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
                        //Console.WriteLine($"String length in binary system: {LODA}");
                        //Console.WriteLine($"Max value for current Method and version {i+1}: {maxValues[i]}");
                        updateData = encodingMethod + LODA + data;
                        //Console.WriteLine(updateData);
                        updateData = filling(updateData, int.Parse(maxValues[i]));
                        //Console.WriteLine(updateData);
                        updateData = DivisionIntoBlocks(updateData, int.Parse(maxValues[i]), i, correctionLevel);
                        //Console.WriteLine(updateData);
                        updateData = LastAlgorithm(updateData, correctionLevel,i);
                        //Console.WriteLine(updateData);
                        string matrix = GenerateMatrix(i + 1, correctionLevel, updateData);
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
                    case "1000":
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
                    case "1000":
                        return buff.PadLeft(10, '0');
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
                    case "1000":
                        return buff.PadLeft(12, '0');
                }
            }
            return "Exception";
        }


        static string filling(string data, int targetLength)
        {

            if (data.Length + 4 > targetLength)
            {
                data = data.PadRight(targetLength, '0');
                return data;
            }
            else
                data = data.Insert(data.Length, "0000");
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
            for (var k = data.Length - 8 * blocksValue[blocks - 1]; k > data.Length - (maxBytes % blocks*8* blocksValue[blocks - 1]); k-= 8 * blocksValue[blocks - 1])
            {
                data = data.Insert(k, " ");
            }
            return data;
        }


        static string GenerateMatrix(int version, int correctionLevel, string data)
        {
            string d = "";
            var size = AllDictionaries.SizeDictionary(version-1);
            int[,] matrix1 = new int[size, size];
            int[,] searchPatterns = {{1,1,1,1,1,1,1}, {1,0,0,0,0,0,1}, {1,0,1,1,1,0,1}, {1,0,1,1,1,0,1}, {1,0,1,1,1,0,1}, {1,0,0,0,0,0,1}, {1,1,1,1,1,1,1}};
            int[,] alignmentPatterns = { { 1, 1, 1, 1, 1 }, { 1, 0, 0, 0, 1 }, { 1, 0, 1, 0, 1 }, { 1, 0, 0, 0, 1 }, { 1, 1, 1, 1, 1 } };
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    matrix1[i, j] = searchPatterns[i, j];
                }
            }
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    matrix1[i+size-7, j] = searchPatterns[i, j];
                }
            }
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    matrix1[i, j+size-7] = searchPatterns[i, j];
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
                        matrix1[i + alignmentPatternspos - 2, j + alignmentPatternspos - 2] = alignmentPatterns[i, j];
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
                                    if (matrix1[k + X - 2, g + Y - 2] == 1)
                                    {
                                        check = 1;
                                        break;
                                    }    
                                    matrix1[k + X - 2, g + Y - 2] = alignmentPatterns[k, g];
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
                            matrix1[size - 11 + i, j] = 0;
                            matrix1[j, size - 11 + i] = 0;
                        }
                        else
                        {
                            matrix1[size - 11 + i, j] = 1;
                            matrix1[j, size - 11 + i] = 1;
                        }

                    }
                }
            }
            int l = 2;
            for (int i = 7; i < size - 7; i++)
            {
                if (l % 2 == 0)
                    matrix1[i, 6] = 0;
                else
                    matrix1[i, 6] = 1;
                l++;
            }
            for (int i = 8; i < size - 7; i++)
            {
                if (l % 2 == 0)
                    matrix1[6, i] = 0;
                else
                    matrix1[6, i] = 1;
                l++;
            }
            matrix1[8, size - 8] = 1;

            int[,] matrixResult = new int[size, size];
            int h;
            int gf;
            for (var maskCount = 0; maskCount <= 7; maskCount++) // data
            {

                h = 0;
                gf = 1;
                int[,] matrix = new int[size, size];
                Array.Copy(matrix1, matrix, size * size);

                for (var p = 0; p < 2; p++)
                {
                    for (var i = size - 1; i > 8; i--)
                    {
                        if (matrix[size - gf, i] == 1)
                            i -= 5;
                        matrix[size - gf, i] = Masks(Convert.ToInt32(new string(data[h], 1)), size - gf, i, maskCount);
                        h++;
                        matrix[size - gf - 1, i] = Masks(Convert.ToInt32(new string(data[h], 1)), size - gf - 1, i, maskCount);
                        h++;
                    }
                    gf += 2;

                    for (var i = 9; i < size; i++)
                    {
                        if (matrix[size - gf, i] == 1)
                            i += 5;
                        matrix[size - gf, i] = Masks(Convert.ToInt32(new string(data[h], 1)), size - gf, i, maskCount);
                        h++;
                        matrix[size - gf - 1, i] = Masks(Convert.ToInt32(new string(data[h], 1)), size - gf - 1, i, maskCount);
                        h++;
                    }
                    gf += 2;
                }
                for (var j = 0; j < (size - 17) / 4; j++)
                {
                    for (var i = size - 1; i >= 0; i--)
                    {

                        if (i == 6)
                            i -= 1;
                        if (version > 6 && i == 5 && gf == 9)
                            break;

                        if (matrix[size - gf, i] == 1 && matrix[size - gf - 1, i] == 0)
                        {
                            matrix[size - gf - 1, i] = Masks(Convert.ToInt32(new string(data[h], 1)), size - gf - 1, i, maskCount);
                            h++;
                        }
                        else
                        {
                            if (matrix[size - gf, i] == 1 && matrix[size - gf - 1, i] == 1)
                                i -= 5;
                            matrix[size - gf, i] = Masks(Convert.ToInt32(new string(data[h], 1)), size - gf, i, maskCount);
                            h++;
                            matrix[size - gf - 1, i] = Masks(Convert.ToInt32(new string(data[h], 1)), size - gf - 1, i, maskCount);
                            h++;
                        }
                    }
                    gf += 2;

                    for (var i = 0; i < size; i++)
                    {

                        if (version > 6 && i < 6 && gf == 11)
                        {
                            matrix[size - gf - 1, i] = Masks(Convert.ToInt32(new string(data[h], 1)), size - gf - 1, i, maskCount);
                            h++;
                            continue;
                        }
                        if (i == 6)
                            i += 1;
                        if (matrix[size - gf, i] == 1 && matrix[size - gf - 1, i] == 0)
                        {
                            matrix[size - gf - 1, i] = Masks(Convert.ToInt32(new string(data[h], 1)), size - gf - 1, i, maskCount);
                            h++;
                        }
                        else
                        {
                            if (matrix[size - gf, i] == 1 && matrix[size - gf - 1, i] == 1)
                                i += 5;
                            matrix[size - gf, i] = Masks(Convert.ToInt32(new string(data[h], 1)), size - gf, i, maskCount);
                            h++;
                            matrix[size - gf - 1, i] = Masks(Convert.ToInt32(new string(data[h], 1)), size - gf - 1, i, maskCount);
                            h++;
                        }
                    }
                    gf += 2;

                }

                for (var i = size - 9; i > 8; i--)
                {
                    if (matrix[size - gf, i] == 1 && matrix[size - gf - 1, i] == 1)
                        i -= 5;
                    matrix[size - gf, i] = Masks(Convert.ToInt32(new string(data[h], 1)), size - gf, i, maskCount);
                    h++;
                    matrix[size - gf - 1, i] = Masks(Convert.ToInt32(new string(data[h], 1)), size - gf - 1, i, maskCount);
                    h++;
                }
                gf += 3;
                bool ch = false;
                for (var p = 0; p < 2; p++)
                {
                    if (ch)
                        break;
                    for (var i = 9; i < size - 8; i++)
                    {
                        if (version > 6 && i >= size - 11)
                            break;
                        if (matrix[size - gf, i] == 1 && matrix[size - gf - 1, i] == 1)
                            i += 5;
                        matrix[size - gf, i] = Masks(Convert.ToInt32(new string(data[h], 1)), size - gf, i, maskCount);
                        if (h == data.Length - 1)
                        {
                            ch = true;
                            break;
                        }
                        h++;
                        matrix[size - gf - 1, i] = Masks(Convert.ToInt32(new string(data[h], 1)), size - gf - 1, i, maskCount);
                        if (h == data.Length - 1)
                        {
                            ch = true;
                            break;
                        }
                        h++;

                    }
                    gf += 2;
                    if (ch)
                        break;
                    for (var i = size - 9; i > 8; i--)
                    {
                        if (version > 6 && i >= size - 11)
                            continue;

                        matrix[size - gf, i] = Masks(Convert.ToInt32(new string(data[h], 1)), size - gf, i, maskCount);
                        if (h == data.Length - 1)
                        {
                            ch = true;
                            break;
                        }
                        h++;
                        matrix[size - gf - 1, i] = Masks(Convert.ToInt32(new string(data[h], 1)), size - gf - 1, i, maskCount);
                        if (h == data.Length - 1)
                        {
                            ch = true;
                            break;
                        }
                        h++;

                    }
                    gf += 2;

                }

                matrix = CodeMask(matrix, maskCount, correctionLevel);
                if (Points(matrixResult) > Points(matrix))
                {
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            matrixResult[i, j] = matrix[i, j];
                        }
                    }
                }
                ////Points(matrix);
            }

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < size + 8; j++)
                    {
                        Console.Write("██");
                    }
                    Console.WriteLine();
                }
                for (int i = 0; i < size; i++)
                {
                    Console.Write("████████");
                    for (int j = 0; j < size; j++)
                    {
                        if (matrixResult[j, i] == 0)
                            Console.Write("██");
                        else
                            Console.Write("  ");
                    }
                    Console.Write("████████");
                    Console.WriteLine();
                }
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < size + 8; j++)
                    {
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
            string[,] ArrayCorrectionBytes = new string[NumberOfBlocks, NumberOfCorrectionBytes];
            string[,] info;
            if (NumberOfBlocks == 1)
                info = new string[NumberOfBlocks, data.Length/8];
            else
                info = new string[NumberOfBlocks, (data.IndexOf(' ') + 8) / 8];

            
            var nb = 0;
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
                    int firstItem;
                    if (BArray[0] == 255)
                        firstItem = int.Parse(File.ReadLines("ReverseTable.txt").ElementAt(1));
                    else
                        firstItem = int.Parse(File.ReadLines("ReverseTable.txt").ElementAt(BArray[0]));


                    for (int b = 0; b < g-1; b++)

                    {
                        BArray[b] = BArray[b + 1];
                    }
                    BArray[g - 1] = 0;
                    if (firstItem == -1)
                        continue;
                    // foreach (var item in BArray)
                    //    Console.Write(item + " ");
                    // Console.WriteLine(firstIte
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
                for (var item = 0; item< NumberOfCorrectionBytes; item++)
                {
                    var f  = Convert.ToString(BArray[item], 2);
                    f = f.PadLeft(8, '0');

                    ArrayCorrectionBytes[nb, item] = f;
                }
                //for (var item = 0; item < NumberOfCorrectionBytes; item++)
                //    Console.WriteLine(ArrayCorrectionBytes[nb, item]);
                data2 = data2.Remove(0, data2.IndexOf(' ')+1);
                nb++;
            }
            data2 = data;
            data = data.Insert(data.Length, " ");
            for (var i = 0; i < NumberOfBlocks; i++)
            {
                
                if (data[0] == ' ')
                    data = data.Remove(0, 1);
                var t = data.IndexOf(' ');
                for (var j = 0; j <  t / 8; j++)
                {
                    if (data[0] == ' ')
                    {
                        data = data.Remove(0,1);
                        break;
                    }
                    info[i, j] = data.Remove(8, data.Length - 8);
                    data = data.Remove(0, 8);
                    //Console.WriteLine();
                    //Console.Write(info[i, j] + " ");

                }
                //Console.WriteLine();

            }
            var data3 = "";
            //Console.WriteLine();
            for (var i = 0; i < info.GetLength(1); i++)
            {
                for (var j = 0; j < info.GetLength(0); j++)
                {
                    if (info[j, i] == null)
                        continue;
                    data3 += info[j, i]; 
                }
            }
            
            for (var i = 0; i < ArrayCorrectionBytes.GetLength(1); i++)
            {
                for (var j = 0; j < ArrayCorrectionBytes.GetLength(0); j++)
                {
                    
                    data3 += ArrayCorrectionBytes[j, i];
                }
            }

            return data3;
        }


        static int Masks(int value, int X, int Y, int maskCount)
        {
            int g;
            if (value == 1)
                g = 0;
            else
                g = 1;
            switch (maskCount)
            {
                case 0:
                    if ((X + Y) % 2 == 0)
                        return g;
                    return value;
                case 1:
                    if (Y % 2 == 0)
                        return g;
                    return value;
                case 2:
                    if (X % 3 == 0)
                        return g;
                    return value;
                case 3:
                    if ((X + Y) % 3 == 0)
                        return g;
                    return value;
                case 4:
                    if ((X / 3 + Y / 2) % 2 == 0)
                        return g;
                    return value;
                case 5:
                    if ((X * Y) % 2 + (X * Y) % 3 == 0)
                        return g;
                    return value;
                case 6:
                    if (((X * Y) % 2 + (X * Y) % 3) % 2 == 0)
                        return g;
                    return value;
                case 7:
                    if (((X * Y) % 3 + (X + Y) % 2) % 2 == 0)
                        return g;
                    return value;
                case 8:
                    return value;

            }
            return 0;

        }


        static int Points(int[,] matrix)
        {
            var points = 0;
            var B = 0;
            var W = 0;
            var B1 = 0;
            var W1 = 0;

            // first rule

            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                for (var j = 0; j < matrix.GetLength(0); j++)
                {
                    if (matrix[j, i] == 0)
                    {
                        W++;
                        if (B >= 5)
                            points += B - 2;
                        if (W >= 5 && j == matrix.GetLength(0) - 1)
                            points += W - 2;
                        B = 0;
                    }
                    else
                    {
                        B++;
                        if (W >= 5)
                            points += W - 2;
                        if (B >= 5 && j == matrix.GetLength(0) - 1)
                            points += B - 2;
                        W = 0;
                    }
                    if (matrix[i, j] == 0)
                    {
                        W1++;
                        if (B1 >= 5)
                            points += B1 - 2;
                        if (W1 >= 5 && j == matrix.GetLength(0) - 1)
                            points += W1 - 2;
                        B1 = 0;
                    }
                    else
                    {
                        B1++;
                        if (W1 >= 5)
                            points += W1 - 2;
                        if (B1 >= 5 && j == matrix.GetLength(0) - 1)
                            points += B1 - 2;
                        W1 = 0;
                    }

                }
                B = 0;
                W = 0;
                B1 = 0;
                W1 = 0;
            }
            //Console.WriteLine(points);
            
            // second rule

            for (var i = 0; i < matrix.GetLength(0)-1; i++)
            {
                for (var j = 0; j < matrix.GetLength(0)-1; j++)
                {
                    if (matrix[j, i] == matrix[j + 1, i] && matrix[j, i + 1] == matrix[j + 1, i + 1] && matrix[j, i] == matrix[j, i + 1])
                        points += 3;
                }
            }

            // third rule

            //Console.WriteLine(points);
            string modules1 = "000010111010000";
            string modules2 = "00001011101";
            string modules3 = "10111010000";
            int[] sd = { 1, 0, 1 };
            int count = 0;
            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                string lineArray = "";
                string columnArray = "";
                for (var j = 0; j < matrix.GetLength(0); j++)
                {
                    lineArray += matrix[j, i];
                    columnArray += matrix[i, j];
                }
                count = (lineArray.Length - lineArray.Replace(modules1, "").Length) / modules1.Length;
                count += (lineArray.Length - lineArray.Replace(modules2, "").Length) / modules2.Length - count + (lineArray.Length - lineArray.Replace(modules3, "").Length) / modules3.Length - count;
                points += count * 40;
                
                count = (columnArray.Length - columnArray.Replace(modules1, "").Length) / modules1.Length;
                count += (columnArray.Length - columnArray.Replace(modules2, "").Length) / modules2.Length - count + (columnArray.Length - columnArray.Replace(modules3, "").Length) / modules3.Length - count;
                points += count * 40;
            }
            //Console.WriteLine(points);

            // fourth rule

            B = 0;
            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                for (var j = 0; j < matrix.GetLength(0); j++)
                {
                    if (matrix[j,i] == 1)
                        B++;
                }
            }
            int h = (int)Math.Abs(Math.Truncate(((double)B / (double)matrix.Length)*100 - 50));
           
            points += h * 2;
            //Console.WriteLine(points);
            return points;
        }


        static int[,] CodeMask(int[,] matrix, int maskVersion, int correctionVersion)
        { 
        string CodeMask = File.ReadLines("MaskCode.txt").ElementAt((correctionVersion-1)*8 + maskVersion);
            var h = 0;
            for (var i = 0; i < 9; i++)
            {
                if (matrix[i, 8] == 1)
                    i++;
                matrix[i, 8] = Convert.ToInt32(new string(CodeMask[h], 1));
                h++;
            }
            //h = 8;
            for (var i = 7; i >= 0; i--)
            {
                if (matrix[8, i] == 1)
                    i--;
                matrix[8, i] = Convert.ToInt32(new string(CodeMask[h], 1));
                h++;
            }
            h = 0;
            var length = matrix.GetLength(1);
            for (var i = length - 1; i > length - 8; i--)
            {
                matrix[8, i] = Convert.ToInt32(new string(CodeMask[h], 1));
                h++;
            }
            h = 7;
            for (var i = length - 8; i < length; i++)
            {
                matrix[i, 8] = Convert.ToInt32(new string(CodeMask[h], 1));
                h++;
            }
            return matrix;
        }

        
    }
}
