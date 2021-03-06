﻿using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;


namespace QR
{
    public class MainClass
    {
        public static void Main()
        {
            Console.InputEncoding = System.Text.Encoding.GetEncoding(1200);
            Console.OutputEncoding = System.Text.Encoding.GetEncoding(1200);                            
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            int correctionLevel = 0;
            string input = string.Empty;
            Console.WriteLine("Do you want to use any payload? (y/N): ");
            if (Console.ReadLine().ToLower() == "y")
            {
                input = ChoosePayload();
            }
            else
            {
                Console.WriteLine("Write string to encode");
                input = Console.ReadLine();
            }

            Console.WriteLine("Add logo? (y/N): ");
            bool withLogo = false;
            if (Console.ReadLine().ToLower() == "y")
            {
                withLogo = true;
                correctionLevel = 4;
            }
            else
            {
                Console.WriteLine("Choose error correction level: \n 1 - L(7%) | 2 - M(15%) | 3 - Q(25%) | 4 - H(30%)");
                if (!int.TryParse(Console.ReadLine(), out correctionLevel) || correctionLevel < 1 || correctionLevel > 4)
                {
                    throw new Exception("Correction level must be number from 1 to 4");
                }
            }
            //input = "ሐ`"; // СТРОКА ДЛЯ ТЕСТА
            //input = "\\000009ΑΒΓΔ"; //тест2
            string encodedLine = string.Empty;
            byte[] bytes = UTF8Encoding.UTF8.GetBytes(input);
            foreach (var item in bytes)
            {
                Console.Write($"{item} ");
            }
            int version = Encoding.GetVersion(bytes, correctionLevel);
            bool ECI = false;
            if (input.Contains('\\'))
            {
                Console.WriteLine("Enable ECI? (y/N): ");
                if (Console.ReadLine().ToLower() == "y")
                {
                    ECI = true;
                    encodedLine = Encoding.EncodeECI(input, version, correctionLevel);
                    Console.WriteLine(encodedLine);
                }
            }
            if (!ECI) {
                if (Encoding.IsNumeric(bytes))
                {
                    encodedLine = Encoding.EncodeNumeric(input, version, correctionLevel);
                    Console.WriteLine(encodedLine);
                }
                else if (Encoding.IsAlphanumeric(bytes))
                {
                    encodedLine = Encoding.EncodeAlphaNumeric(input, version, correctionLevel);
                    Console.WriteLine(encodedLine);
                }
                else if (Encoding.IsKanji(bytes))
                {
                    encodedLine = Encoding.EncodeKanji(input, version, correctionLevel);
                    Console.WriteLine(encodedLine);
                }
                else
                {
                    encodedLine = Encoding.EncodeByte(input, version, correctionLevel);
                    Console.WriteLine(encodedLine);
                }
            } 
            int[,] matrix = Matrix.CreateMatrix(encodedLine, version, correctionLevel);

            if (withLogo)
            {
                Save.SaveWithLogo(matrix);
            }
            else Save.RequestImageSave(matrix);
        }

        public static string ChoosePayload()
        {
            string payload = string.Empty;
            Console.WriteLine("Choose payload number: ");
            Console.WriteLine("1. Skype\n2. URL\n3. E-Mail\n4. Phone call\n5. SMS\n6. Discord\n7. Wi-Fi");
            if(int.TryParse(Console.ReadLine(), out int number))
            {
                payload = Payload.UsePayload(number);
            }
            return payload;
        }

        public int[,] ReadCorrection()
        {
            String file = File.ReadAllText(@"CorrectionLevel.txt");

            int i = 0, j = 0;
            int[,] result = new int[4, 40];
            foreach (var row in file.Split('\n'))
            {
                j = 0;
                foreach (var col in row.Split(' '))
                {
                    result[i, j] = int.Parse(col.Trim());
                    j++;
                }
                i++;
            }
            return result;
        }

        public int[,] ReadBlocks()
        {
            String file = File.ReadAllText(@"Blocks.txt");

            int i = 0, j = 0;
            int[,] result = new int[4, 40];
            foreach (var row in file.Split('\n'))
            {
                j = 0;
                foreach (var col in row.Split(' '))
                {
                    result[i, j] = int.Parse(col.Trim());
                    j++;
                }
                i++;
            }
            return result;
        }

        public int[,] ReadCorrectionBytePerBlock()
        {
            String file = File.ReadAllText(@"CorrectionBytePerBlock.txt");

            int i = 0, j = 0;
            int[,] result = new int[4, 40];
            foreach (var row in file.Split('\n'))
            {
                j = 0;
                foreach (var col in row.Split(' '))
                {
                    result[i, j] = int.Parse(col.Trim());
                    j++;
                }
                i++;
            }
            return result;
        }

        public int[,] ReadAlignment()
        {
            String file = File.ReadAllText(@"AlignmentPatterns.txt");

            int i = 0, j = 0;
            int[,] result = new int[40, 7];
            foreach (var row in file.Split('\n'))
            {
                j = 0;
                foreach (var col in row.Split(' '))
                {
                    result[i, j] = int.Parse(col.Trim());
                    j++;
                }
                i++;
            }
            return result;
        }
        public string[,] ReadVersionCode()
        {
            // Коды версий для qr кода 7+ версии
            String file = File.ReadAllText(@"VersionCode.txt"); 

            int i = 0, j = 0;
            string[,] result = new string[34, 3];
            foreach (var row in file.Split('\n'))
            {
                j = 0;
                foreach (var col in row.Split(' '))
                {
                    result[i, j] = col.Trim();
                    j++;
                }
                i++;
            }
            return result;
        }
        public string[,] ReadMaskCode()
        {
            // Коды версий для qr кода 7+ версии
            String file = File.ReadAllText(@"MaskCode.txt");

            string[,] result = new string[4, 8];
            var data = file.Split("\n");
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    result[i, j] = data[i*8+j].Trim();
                }
            }
            return result;
        }

        public byte[] ReadGaloisField()
        {
            String file = File.ReadAllText(@"GaloisField.txt");
            int i = 0;
            byte[] result = new byte[256];
            foreach (var row in file.Split('\n'))
            {
                result[i] = byte.Parse(row.Trim());
                i++;
            }
            return result;
        }

        public byte[] ReadBackGaloisField()
        {
            String file = File.ReadAllText(@"BackGaloisField.txt");
            int i = 0;
            byte[] result = new byte[256];
            foreach (var row in file.Split('\n'))
            {
                result[i] = byte.Parse(row.Trim());
                i++;
            }
            return result;
        }

    }
}
