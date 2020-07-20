using QRTestingTools.Utils;
using System;
using System.Linq;
using System.Numerics;

namespace QRTestingTools.Symbol
{
    class VersionInformation
    {
        public int Version { get; }
        public int MinVersion { get; } = 0;
        public int MaxVersion { get; } = 40;

        public VersionInformation(int version)
        {
            if (MinVersion <= version && version <= MaxVersion)
                Version = version;
            else
                throw new Exception($"Attempt to create VersionInformation with version not in the range [{MinVersion}:{MaxVersion}]. Given version: {version}");
        }

        public static VersionInformation CreateFrom(ReadOnlySpan<byte> symbol)
        {
            int version = QRSymbol.GetVersion(symbol);
            
            if (version >= 7)
            {
                (var firstVersion, var secondVersion) = DecodeVersionInformationBlocks(ExtractVersionInformationBlocks(symbol));
                if (firstVersion != secondVersion)
                    throw new Exception($"Decoded Version Information is not consistent.{Environment.NewLine}First:{firstVersion}{Environment.NewLine}Second:{secondVersion}");
                else if (firstVersion != version)
                    throw new Exception($"Decoded Version Information does not match with the symbol's size{Environment.NewLine}Decoded:{firstVersion}{Environment.NewLine}Derived from size:{version}");
            }

            return new VersionInformation(version);
        }

        private static (ReadOnlyMemory<byte>, ReadOnlyMemory<byte>) ExtractVersionInformationBlocks(ReadOnlySpan<byte> symbol)
        {
            byte[] first = new byte[18];
            byte[] second = new byte[18];

            int sideLength = 4 * QRSymbol.GetVersion(symbol) + 17;

            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 3; j++)
                    first[i * 3 + j] = symbol[(sideLength - 9 - j) + (5 - i) * sideLength];

            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 3; j++)
                    second[i * 3 + j] = symbol[(sideLength - 9 - j) * sideLength + (5 - i)];

            return (new ReadOnlyMemory<byte>(first), new ReadOnlyMemory<byte>(second));
        }

        private static (int, int) DecodeVersionInformationBlocks((ReadOnlyMemory<byte>, ReadOnlyMemory<byte>) blocks)
        {
            static int GetIndexOfNearest(int value)
            {
                int[] lut = { 0x07C94, 0x085BC, 0x09A99, 0x0A4D3, 0x0BBF6, 0x0C762, 0x0D847, 0x0E60D,
                              0x0F928, 0x10B78, 0x1145D, 0x12A17, 0x13532, 0x149A6, 0x15683, 0x168C9,
                              0x177EC, 0x18EC4, 0x191E1, 0x1AFAB, 0x1B08E, 0x1CC1A, 0x1D33F, 0x1ED75,
                              0x1F250, 0x209D5, 0x216F0, 0x228BA, 0x2379F, 0x24B0B, 0x2542E, 0x26A64,
                              0x27541, 0x28C69 };

                try
                {
                    return Array.IndexOf(
                    lut, lut.Zip(
                    lut.Select(i => BitOperations.PopCount((uint)(i ^ value))))
                                                 .Where(val => val.Second <= 3)
                                                 .OrderBy(val => val.Second)
                                                 .FirstOrDefault()
                                                 .First);
                }
                catch
                {
                    throw new Exception($"Unable to determine correct version information{Environment.NewLine}Version block:{Convert.ToString(value, 2)}");
                }
            }
            var (a, b) = blocks;
            int first = Utilities.AssembleBits(a.Span, 0, 18);
            int second = Utilities.AssembleBits(b.Span, 0, 18);

            int firstValue = GetIndexOfNearest(first) + 7;
            int secondValue = GetIndexOfNearest(second) + 7;
            return (firstValue, secondValue);

        }
    }
}
