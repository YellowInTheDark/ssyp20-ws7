using System;
using System.Linq;

namespace QR_testing_tools
{
    class Program
    {
        //TODO
        //Sanitize input
        //Handle exceptions
        //Add comments
        static void Main(string[] args)
        {
            Console.WriteLine("Bit string:\n");
            var input = Console.ReadLine()
                .Where(c => !Char.IsWhiteSpace(c))
                .Select(
                (c, i) => c switch
                     { 
                        '0' => (byte)0,
                        '1' => (byte)1,
                         _  => throw new Exception()
                     })
                .ToArray()
                .AsSpan();

            Console.WriteLine("Code version:\n");
            var version = Int32.Parse(Console.ReadLine());
            Console.WriteLine(SegmentDecoder.DecodeFirstSegment(input, version));
        }


    }
}
