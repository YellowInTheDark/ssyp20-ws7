using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QR
{
    class Encoding
    {
        public static string EncodeECI(string input, int version, int correctionLevel)
        {
            string encodedLine = string.Empty;
            string encodedData = string.Empty;
            string ECIAssignmentNumber = input.Substring(0, 7).Remove(0, 1);
            input = input.Remove(0, 7);
            var ECIAssignmentNumberBinary = Convert.ToString(int.Parse(ECIAssignmentNumber), 2).PadLeft(8, '0');
            encodedLine = encodedLine.Insert(0, ECIAssignmentNumberBinary);
            encodedLine = encodedLine.Insert(0, "0111 ");

            byte[] bytes = System.Text.Encoding.GetEncoding(Dicts.ECI(ECIAssignmentNumber)).GetBytes(input);
            for (int i = 0; i < bytes.Length; i++)
            {
                var tmp = bytes[i];
                encodedData += $"{Convert.ToString(tmp, 2).PadLeft(8, '0')} ";
            }
            Console.WriteLine($"INPUT BITS: {encodedData}");

            switch (version)
            {
                case int _ when version <= 9:
                    encodedData = encodedData.Insert(0, $"{Convert.ToString(bytes.Length, 2).PadLeft(8, '0')} ");
                    break;
                case int _ when version <= 40:
                    encodedData = encodedData.Insert(0, $"{Convert.ToString(bytes.Length, 2).PadLeft(16, '0')} ");
                    break;
            }

            encodedData = encodedData.Insert(0, "0100 ");
            encodedLine += $" {encodedData}";
            encodedLine = encodedLine.TrimEnd();
            encodedLine = AddEndOfLine(encodedLine, version, correctionLevel);
            encodedLine = DivideLine(encodedLine);
            encodedLine = AddCodewords(encodedLine, version, correctionLevel);
            Console.WriteLine($"+ Codewords {encodedLine}");

            encodedLine = DivideBlocks(encodedLine, version, correctionLevel);
            encodedLine = CreateCorrectionByte(encodedLine, version, correctionLevel);

            encodedLine = ShuffleCodewords(encodedLine);
            return encodedLine;
        }

        public static string EncodeNumeric(string input, int version, int correctionLevel)
        {

            string encodedLine = string.Empty;
            for (int i = 0; i < input.Length / 3; i++)
            {
                int tmp = int.Parse(input.Substring(3 * i, 3));
                encodedLine += $"{Convert.ToString(tmp, 2).PadLeft(10, '0')} ";
            }
            if (input.Length % 3 == 2)
            {
                int tmp = int.Parse(input.Substring(3 * (input.Length / 3), 2));
                encodedLine += $"{Convert.ToString(tmp, 2).PadLeft(7, '0')} ";
            }
            if (input.Length % 3 == 1)
            {
                int tmp = int.Parse(input.Substring(3 * (input.Length / 3), 1));
                encodedLine += $"{Convert.ToString(tmp, 2).PadLeft(4, '0')} ";
            }
            Console.WriteLine($"INPUT BITS: {encodedLine}");

            switch (version)
            {
                case int _ when version <= 9:
                    encodedLine = encodedLine.Insert(0, $"{Convert.ToString(input.Length, 2).PadLeft(10, '0')} ");
                    break;
                case int _ when version <= 26:
                    encodedLine = encodedLine.Insert(0, $"{Convert.ToString(input.Length, 2).PadLeft(12, '0')} ");
                    break;
                case int _ when version <= 40:
                    encodedLine = encodedLine.Insert(0, $"{Convert.ToString(input.Length, 2).PadLeft(14, '0')} ");
                    break;
            }
            encodedLine = encodedLine.Insert(0, "0001 ");
            encodedLine = AddEndOfLine(encodedLine, version, correctionLevel);
            encodedLine = DivideLine(encodedLine);
            encodedLine = AddCodewords(encodedLine, version, correctionLevel);

            encodedLine = DivideBlocks(encodedLine, version, correctionLevel);
            encodedLine = CreateCorrectionByte(encodedLine, version, correctionLevel);
            encodedLine = ShuffleCodewords(encodedLine);

            return encodedLine;
        }

        public static string ShuffleCodewords(string encodedLine)
        {
            string[] data = encodedLine.Split('|')[0].Split(' ');
            string[] correction = encodedLine.Split('|')[1].TrimEnd().Split(' ');
            string result = string.Empty;
            int i = 0;
            while (data.Last() != "")
            {
                if (i >= data.Length) i = 0;
                if (data[i] != "")
                {
                    result += data[i].Substring(0, 8);
                    data[i] = data[i].Remove(0, 8);
                }
                i++;
            }
            while (correction.Last() != "")
            {
                if (i >= correction.Length) i = 0;
                if (correction[i] != "")
                {
                    result += correction[i].Substring(0, 8);
                    correction[i] = correction[i].Remove(0, 8);
                }
                i++;
            }

            return result;
        }

        public static string CreateCorrectionByte(string encodedLine, int version, int correctionLevel)
        {
            MainClass main = new MainClass();
            int[,] corrBytePerBlockArr = new int[4, 40];
            corrBytePerBlockArr = main.ReadCorrectionBytePerBlock();
            byte[] GaloisField = main.ReadGaloisField();
            byte[] BackGaloisField = main.ReadBackGaloisField();

            string correctionBytes = string.Empty;

            int corrBytePerBlock = corrBytePerBlockArr[correctionLevel - 1, version - 1];
            byte[] Polynomial = Dicts.PolynomialDict(corrBytePerBlock);
            string[] blocks = encodedLine.Split(' ');
            byte[] newArr = default;
            encodedLine += "|";
            for (int i = 0; i < blocks.Length; i++)
            {
                correctionBytes = string.Empty;

                newArr = new byte[Math.Max(corrBytePerBlock, blocks[i].Length / 8)];

                byte[] bytes = new byte[blocks[i].Length / 8];
                for (int j = 0; j < blocks[i].Length / 8; ++j)
                {
                    bytes[j] = Convert.ToByte(blocks[i].Substring(8 * j, 8), 2);
                }

                for (int j = 0; j < bytes.Length; j++)
                {
                    newArr[j] = bytes[j];
                }

                for (int j = 0; j < blocks[i].Length / 8; ++j)
                {

                    byte A = newArr[0];
                    //newArr = newArr.Where((val, idx) => idx != 0).Append(0).ToArray();
                    for (int l = 1; l < newArr.Length; l++)
                    {
                        newArr[l - 1] = newArr[l];
                        newArr[l] = 0;
                    }
                    if (A == 0) continue;
                    byte B = BackGaloisField[A];
                    byte BPolynomial;
                    for (int k = 0; k < corrBytePerBlock; k++)
                    {
                        //BPolynomial[k] = (byte)(Polynomial[k] + B);
                        //newArr[k] = (byte)(GaloisField[B] ^ BPolynomial[k]);
                        BPolynomial = (byte)((Polynomial[k] % 255 + B % 255) % 255);
                        newArr[k] = (byte)(GaloisField[BPolynomial] ^ newArr[k]);

                    }
                }

                Console.WriteLine($"Correction Bytes start of block {i+1}");
                for (int k = 0; k < corrBytePerBlock; k++)
                {
                    correctionBytes += $"{Convert.ToString(newArr[k], 2).PadLeft(8, '0')}";

                    Console.Write($" {Convert.ToString(newArr[k], 2).PadLeft(8, '0')}");
                }
                Console.WriteLine($"\nCorrection Bytes end of block {i+1}");
                correctionBytes += " ";
                encodedLine += correctionBytes;
            }
            return encodedLine;
        }


        public static string AddEndOfLine(string encodedLine, int version, int correctionLevel)
        {
            MainClass main = new MainClass();
            int[,] maxByteArr = new int[4, 40];
            maxByteArr = main.ReadCorrection();
            int bits = encodedLine.Count(c => !Char.IsWhiteSpace(c));
            if (maxByteArr[correctionLevel - 1, version - 1] - bits >= 4)
            {
                encodedLine += "0000";
            }
            else encodedLine += "0000".Substring(0, maxByteArr[correctionLevel - 1, version - 1] - bits);
            return encodedLine;
        }

        public static string DivideLine(string encodedLine)
        {
            string divided = new string(encodedLine.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
            for (int i = 8; i <= divided.Length; i += 8)
            {
                divided = divided.Insert(i, " ");
                i++;
            }
            if (divided.Count(c => !Char.IsWhiteSpace(c)) % 8 != 0)
            {
                divided += "00000000".Substring(0, 8 - divided.Count(c => !Char.IsWhiteSpace(c)) % 8);
            }
            return divided;
        }

        public static string AddCodewords(string encodedLine, int version, int correctionLevel)
        {
            MainClass main = new MainClass();
            int[,] maxByteArr = new int[4, 40];
            maxByteArr = main.ReadCorrection();
            int numOfCodeword = maxByteArr[correctionLevel - 1, version - 1] / 8;
            int codewordsMissing = numOfCodeword - encodedLine.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            for (int i = 0; i < codewordsMissing; i++)
            {
                if (i % 2 == 0)
                    encodedLine += " 11101100";
                else
                    encodedLine += " 00010001";
            }

            return encodedLine;
        }

        public static string DivideBlocks(string encodedLine, int version, int correctionLevel)
        {
            MainClass main = new MainClass();
            int[,] maxByteArr = new int[4, 40];
            maxByteArr = main.ReadCorrection();
            int[,] blocksCountArr = new int[4, 40];
            blocksCountArr = main.ReadBlocks();
            if (blocksCountArr[correctionLevel - 1, version - 1] == 1) return new string(encodedLine.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray()); ;
            int bytesPerBlock = (maxByteArr[correctionLevel - 1, version - 1] / 8) / blocksCountArr[correctionLevel - 1, version - 1];
            int blockMod = (maxByteArr[correctionLevel - 1, version - 1] / 8) % blocksCountArr[correctionLevel - 1, version - 1];
            int[] bytesPerBlockArr = new int[blocksCountArr[correctionLevel - 1, version - 1]];

            string blocks = string.Empty;
            string noSpaces = new string(encodedLine.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
            for (int i = 0; i < bytesPerBlockArr.Length; i++)
            {
                bytesPerBlockArr[i] = bytesPerBlock;
            }
            for (int i = 0; i < blockMod; i++)
            {
                bytesPerBlockArr[bytesPerBlockArr.Length - 1 - i] += 1;
            }

            for (int i = 0; i < blocksCountArr[correctionLevel - 1, version - 1]; i++)
            {
                blocks += $"{noSpaces.Substring(0, bytesPerBlockArr[i] * 8)} ";
                noSpaces = noSpaces.Remove(0, bytesPerBlockArr[i] * 8);
            }
            blocks = blocks.TrimEnd();
            return blocks;
        }

        public static string EncodeAlphaNumeric(string input, int version, int correctionLevel)
        {
            string encodedLine = string.Empty;
            for (int i = 0; i < input.Length - 1; i += 2)
            {
                var fElement = Dicts.AlphanumericDictionary(input[i]);
                var sElement = Dicts.AlphanumericDictionary(input[i + 1]);
                var sum = fElement * 45 + sElement;
                encodedLine += $"{Convert.ToString(sum, 2).PadLeft(11, '0')} ";
            }
            if (input.Length % 2 == 1)
            {
                int tmp = Dicts.AlphanumericDictionary(input[input.Length - 1]);
                encodedLine += $"{Convert.ToString(tmp, 2).PadLeft(6, '0')} ";
            }
            Console.WriteLine($"INPUT BITS: {encodedLine}");

            switch (version)
            {
                case int _ when version <= 9:
                    encodedLine = encodedLine.Insert(0, $"{Convert.ToString(input.Length, 2).PadLeft(9, '0')} ");
                    break;
                case int _ when version <= 26:
                    encodedLine = encodedLine.Insert(0, $"{Convert.ToString(input.Length, 2).PadLeft(11, '0')} ");
                    break;
                case int _ when version <= 40:
                    encodedLine = encodedLine.Insert(0, $"{Convert.ToString(input.Length, 2).PadLeft(13, '0')} ");
                    break;
            }

            
            encodedLine = encodedLine.Insert(0, "0010 ");
            encodedLine = AddEndOfLine(encodedLine, version, correctionLevel);
            encodedLine = DivideLine(encodedLine);
            encodedLine = AddCodewords(encodedLine, version, correctionLevel);
            encodedLine = DivideBlocks(encodedLine, version, correctionLevel);
            encodedLine = CreateCorrectionByte(encodedLine, version, correctionLevel);

            encodedLine = ShuffleCodewords(encodedLine);

            return encodedLine;
        }

        public static string EncodeByte(string input, int version, int correctionLevel)
        {
            string encodedLine = string.Empty;
            byte[] bytes = UTF8Encoding.UTF8.GetBytes(input);
            for (int i = 0; i < bytes.Length; i++)
            {
                var tmp = bytes[i];
                encodedLine += $"{Convert.ToString(tmp, 2).PadLeft(8, '0')} ";
            }
            Console.WriteLine($"INPUT BITS: {encodedLine}");

            switch (version)
            {
                case int _ when version <= 9:
                    encodedLine = encodedLine.Insert(0, $"{Convert.ToString(bytes.Length, 2).PadLeft(8, '0')} ");
                    break;
                case int _ when version <= 40:
                    encodedLine = encodedLine.Insert(0, $"{Convert.ToString(bytes.Length, 2).PadLeft(16, '0')} ");
                    break;
            }

            encodedLine = encodedLine.Insert(0, "0100 ");
            encodedLine = AddEndOfLine(encodedLine, version, correctionLevel);
            encodedLine = DivideLine(encodedLine);
            encodedLine = AddCodewords(encodedLine, version, correctionLevel);
            Console.WriteLine($"+ Codewords {encodedLine}");

            encodedLine = DivideBlocks(encodedLine, version, correctionLevel);
            encodedLine = CreateCorrectionByte(encodedLine, version, correctionLevel);

            encodedLine = ShuffleCodewords(encodedLine);

            return encodedLine;
        }

        public static string EncodeKanji(string input, int version, int correctionLevel)
        {
            byte[] bytes = UTF8Encoding.UTF8.GetBytes(input);

            string encodedLine = string.Empty;
            for (int i = 0; i < bytes.Length; i += 2)
            {
                ushort group = (ushort)((int)(bytes[i] << 8) | (int)bytes[i + 1]);

                if (group >= 33088 && group <= 40956)
                {
                    group -= 0x8140;
                    byte HighByte = (byte)(group >> 8);
                    ushort result = (ushort)(HighByte * 0xC0);
                    byte LowByte = (byte)(group & 0xFF);
                    result += LowByte;
                    encodedLine += $"{Convert.ToString(result, 2).PadLeft(13, '0')} ";
                }
                else if (group >= 57408 && group <= 60351)
                {
                    group -= 0xC140;
                    byte HighByte = (byte)(group >> 8);
                    ushort result = (ushort)(HighByte * 0xC0);
                    byte LowByte = (byte)(group & 0xFF);
                    result += LowByte;
                    encodedLine += $"{Convert.ToString(result, 2).PadLeft(13, '0')} ";
                }
            }
            Console.WriteLine($"INPUT BITS: {encodedLine}");

            switch (version)
            {
                case int _ when version <= 9:
                    encodedLine = encodedLine.Insert(0, $"{Convert.ToString(input.Length, 2).PadLeft(8, '0')} ");
                    break;
                case int _ when version <= 26:
                    encodedLine = encodedLine.Insert(0, $"{Convert.ToString(input.Length, 2).PadLeft(10, '0')} ");
                    break;
                case int _ when version <= 40:
                    encodedLine = encodedLine.Insert(0, $"{Convert.ToString(input.Length, 2).PadLeft(12, '0')} ");
                    break;
            }

            Console.WriteLine($"INPUT BITS: {encodedLine}");
            encodedLine = encodedLine.Insert(0, "1000 ");
            encodedLine = AddEndOfLine(encodedLine, version, correctionLevel);
            encodedLine = DivideLine(encodedLine);
            encodedLine = AddCodewords(encodedLine, version, correctionLevel);
            encodedLine = DivideBlocks(encodedLine, version, correctionLevel);
            encodedLine = CreateCorrectionByte(encodedLine, version, correctionLevel);

            encodedLine = ShuffleCodewords(encodedLine);

            return encodedLine;
        }

        public static int GetVersion(byte[] bytes, int correctionLevel)
        {
            MainClass main = new MainClass();
            int[,] maxByteArr = new int[4, 40];
            maxByteArr = main.ReadCorrection();
            const int M = 4;
            if (IsNumeric(bytes))
            {
                var bits = bytes.Length / 3 * 10; // 10 бит на каждые 3 числа
                bits += bytes.Length % 3 == 2 ? 7 : 4;
                Console.WriteLine($"\n{bits} bits");
                int version =
                Enumerable.Range(1, 40).FirstOrDefault(i =>
                   maxByteArr[correctionLevel - 1, i - 1] - M -
                   i switch
                   {
                       int _ when i <= 9 => 10,
                       int _ when i <= 26 => 12,
                       int _ when i <= 40 => 14,
                       _ => throw new Exception("Error")
                   } >= bits);
                if (version == 0) throw new Exception("Your text is too big. Try to choose smaller correction level");
                Console.WriteLine($"Numeric | Version {version}");
                return version;
            }
            else if (IsAlphanumeric(bytes))
            {
                var bits = bytes.Length / 2 * 11;
                bits += (bytes.Length % 2) * 6;
                Console.WriteLine($"\n{bits} bits");
                int version =
                Enumerable.Range(1, 40).FirstOrDefault(i =>
                   maxByteArr[correctionLevel - 1, i - 1] - M -
                   i switch
                   {
                       int _ when i <= 9 => 9,
                       int _ when i <= 26 => 11,
                       int _ when i <= 40 => 13,
                       _ => throw new Exception("Error")
                   } >= bits);
                if (version == 0) throw new Exception("Your text is too big. Try to choose smaller correction level");
                Console.WriteLine($"AlphaNumeric | Version {version}");
                return version;
            }
            else if (IsKanji(bytes))
            {
                var bits = bytes.Length * 13;
                Console.WriteLine($"\n{bits} bits");
                int version =
                Enumerable.Range(1, 40).FirstOrDefault(i =>
                   maxByteArr[correctionLevel - 1, i - 1] - M -
                   i switch
                   {
                       int _ when i <= 9 => 8,
                       int _ when i <= 26 => 10,
                       int _ when i <= 40 => 12,
                       _ => throw new Exception("Error")
                   } >= bits);
                if (version == 0) throw new Exception("Your text is too big. Try to choose smaller correction level");
                Console.WriteLine($"Kanji | Version {version}");
                return version;
            }
            else
            {
                var bits = bytes.Length * 8;
                Console.WriteLine($"\n{bits} bits");
                int version =
                Enumerable.Range(1, 40).FirstOrDefault(i =>
                   maxByteArr[correctionLevel - 1, i - 1] - M -
                   i switch
                   {
                       int _ when i <= 9 => 8,
                       int _ when i <= 40 => 16,
                       _ => throw new Exception("Error")
                   } >= bits);
                if (version == 0) throw new Exception("Your text is too big. Try to choose smaller correction level");
                Console.WriteLine($"Byte | Version {version}");
                return version;
            }
        }
        public static bool IsNumeric(byte[] bytes) =>
                bytes.Count(b => (b > 57 || b < 48)) == 0;
        public static bool IsAlphanumeric(byte[] bytes) =>
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

        public static bool IsKanji(byte[] bytes)
        {
            if (bytes.Length % 2 == 0)
                for (int i = 0; i < bytes.Length; i += 2)
                {
                    if (bytes[i] >= 129 && bytes[i] <= 159 && bytes[i + 1] >= 64 && bytes[i + 1] <= 126 ||
                        bytes[i] >= 129 && bytes[i] <= 159 && bytes[i + 1] >= 128 && bytes[i + 1] <= 252 ||

                        bytes[i] >= 224 && bytes[i] <= 234 && bytes[i + 1] >= 64 && bytes[i + 1] <= 126 ||
                        bytes[i] >= 224 && bytes[i] <= 234 && bytes[i + 1] >= 128 && bytes[i + 1] <= 252 ||

                        bytes[i] >= 234 && bytes[i] <= 235 && bytes[i + 1] >= 64 && bytes[i + 1] <= 126 ||
                        bytes[i] >= 224 && bytes[i] <= 234 && bytes[i + 1] >= 128 && bytes[i + 1] <= 191)
                    { }
                    else
                        return false;
                }
            else return false;

            return true;
        }
    }
}
