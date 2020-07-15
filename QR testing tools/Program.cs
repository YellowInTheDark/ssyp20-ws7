using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QR_testing_tools
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Bit string:\n");
                    var input = ParseBitString(Console.ReadLine());
                    Console.WriteLine("Code version:\n");
                    var version = Int32.Parse(Console.ReadLine());
                    var segments = SegmentDecoder.DecodeAllSegments(input, version);
                    PrintSegments(segments);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine("___________________");
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

            foreach (Segment s in segments)
                Console.WriteLine($"|{s.Mode.ToString().PadRight(14)}|{s.CharacterCount.ToString().PadRight(14)}|{Encoding.UTF8.GetString(s.Content)}");
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

    }
}
