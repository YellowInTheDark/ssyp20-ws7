using QRTestingTools.Symbol;
using QRTestingTools.Symbol.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace QRTestingTools
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            while (true)
            {
                try
                {
                    var x = Encoding.GetEncoding("iso-8859-7").GetBytes("ΑΒΓΔ");
                    Console.InputEncoding = System.Text.Encoding.GetEncoding(1200);
                    Console.OutputEncoding = System.Text.Encoding.GetEncoding(1200);
                    Console.WriteLine("ሐ`");
                    Console.WindowWidth = Console.LargestWindowWidth;
                    Console.SetIn(new StreamReader(Console.OpenStandardInput(32768), Console.InputEncoding, false, 32768));
                    Console.WriteLine("Bit string:\n");
                    var inputs = Console.ReadLine();
                    var input = ParseBitString(inputs);
                    Console.Write($"{Environment.NewLine}___________________{Environment.NewLine}");
                    DrawImageInConsole(input, 4);
                    Console.Write($"{Environment.NewLine}___________________{Environment.NewLine}");
                    var decodedSymbol = new QRSymbol(input);
                    Console.Write($"{Environment.NewLine}___________________{Environment.NewLine}");
                    Console.WriteLine($"Symbol deconstruction: SUCCESS{Environment.NewLine}");
                    Console.WriteLine($"Version:  {decodedSymbol.VersionInfo.Version}");
                    Console.WriteLine($"Mask:     {decodedSymbol.FormatInfo.Mask}");
                    Console.WriteLine($"EC Level: {decodedSymbol.FormatInfo.ErrorCorrectionLevel}");
                    Console.WriteLine();
                    Console.WriteLine("Data segments:");
                    var segments = SegmentDecoder.DecodeAllSegments(decodedSymbol);
                    PrintSegments(segments);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Console.Write($"{Environment.NewLine}___________________{Environment.NewLine}");
            }
        }

        /// <summary>
        /// Prints out a table of <c>Segment</c>'s
        /// </summary>
        /// <param name="segments">Segments to be printed</param>
        private static void PrintSegments(List<Segment> segments)
        {
            Console.Write("|MODE".PadRight(15));
            Console.Write("|CHARACTERS".PadRight(15));
            Console.WriteLine("|CONTENT");

            foreach (Segment seg in segments)
            {
                Console.Write($"|{seg.Mode, -14}|{seg.CharacterCount, -14}|");
                string content = Encoding.UTF8.GetString(seg.Content);
                StringBuilder sb = new StringBuilder(content);
                for (int i = 0; i < content.Length; i++)
                {
                    if (Char.IsControl(content[i]))
                    {
                        var a = @$"\u{Char.ConvertToUtf32(content,  i):X4}";
                        sb.Replace(content[i].ToString(), a);
                    }
                }
                content = sb.ToString();
                foreach (string str in Split(content, Console.WindowWidth - 40))
                {
                    if(!content.StartsWith(str))
                        Console.WriteLine(str.Insert(0, "|--------------|--------------|"));
                    else
                        Console.WriteLine(str);
                }
            }

            var x = Encoding.GetEncoding("ISO-8859-1").GetBytes("TEST");
        }

        /// <summary>
        /// Parses a string of ones and zeros possibly delimited by whitespace characters
        /// </summary>
        /// <param name="bitString">String to be parsed</param>
        /// <returns>Span of bytes where each byte represents a single bit from the <c>bitString</c></returns>
        /// <exception cref="Exception">Throws if a printable character other from '0' or '1' is encountered</exception>
        private static Span<byte> ParseBitString(string bitString)
        {
            return bitString
                   .Where(c => !Char.IsWhiteSpace(c))
                   .Select(
                   (c, i) => c switch
                   {
                       '0' => (byte)0,
                       '1' => (byte)1,
                       _   => throw new Exception($"Unexpected character: {c}")
                   })
                   .ToArray()
                   .AsSpan();
        }

        private static IEnumerable<string> Split(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }

        private static void DrawImageInConsole(ReadOnlySpan<byte> modules, int quietZoneSize)
        {
            int sideLength = QRSymbol.GetSideLength(new VersionInformation(QRSymbol.GetVersion(modules)));
            for (int i = 0; i < quietZoneSize; i++)
                Console.WriteLine();

            for (int i = 0; i < sideLength; i++)
            {
                Console.Write("".PadRight(quietZoneSize));

                for (int j = 0; j < sideLength; j++)
                    Console.Write(modules[i * sideLength + j] switch { 0 => "░░", 1 => "██", _ => throw new Exception($"Unexpected byte: {modules[i * sideLength + j]}") });

                Console.WriteLine("".PadRight(quietZoneSize));
            }

            for (int i = 0; i < quietZoneSize; i++)
                Console.WriteLine();
        }
    }
}
