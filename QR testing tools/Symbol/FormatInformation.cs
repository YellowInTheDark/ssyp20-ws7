using QRTestingTools.Utils;
using System;
using System.Linq;
using System.Numerics;

namespace QRTestingTools.Symbol
{
    class FormatInformation
    {
        public int Mask { get; }
        public int MinMask { get; } = 0;
        public int MaxMask { get; } = 7;
        public Func<int, int, bool> MaskPattern { get; }
        public ErrorCorrectionLevel ErrorCorrectionLevel { get; }

        public FormatInformation(int mask, ErrorCorrectionLevel errorCorrectionLevel)
        {
            if (0 <= mask && mask <= 7)
            {
                Mask = mask;
                MaskPattern = GetMaskPattern(mask);
                ErrorCorrectionLevel = errorCorrectionLevel;
            }
            else
                throw new Exception($"Attempt to create FormatInformation with mask not in the range [{MinMask}:{MaxMask}]. Given mask: {mask}");
        }

        public static FormatInformation CreateFrom(ReadOnlySpan<byte> symbol)
        {
            var (firstFormat, secondFormat) = DecodeFormatInformationBlocks(ExtractFormatInformationBlocks(symbol));
            if (firstFormat != secondFormat)
                throw new Exception($"Decoded Format Information is not consistent.{Environment.NewLine}First:{firstFormat}{Environment.NewLine}Second:{secondFormat}");

            return new FormatInformation(firstFormat.mask, firstFormat.errorCorrectionLevel);
        }

        private static (ReadOnlyMemory<byte>, ReadOnlyMemory<byte>) ExtractFormatInformationBlocks(ReadOnlySpan<byte> symbol)
        {
            byte[] first = new byte[15];
            byte[] second = new byte[15];

            int sideLength = 4 * QRSymbol.GetVersion(symbol) + 17;

            for (int i = 0; i < 6; i++)
                first[i] = symbol[8 * sideLength + i];
            first[6] = symbol[7 + 8 * sideLength];
            first[7] = symbol[8 + 8 * sideLength];
            first[8] = symbol[8 + 7 * sideLength];
            for (int i = 5; i >= 0; i--)
                first[14 - i] = symbol[8 + i * sideLength];

            for (int i = 0; i < 7; i++)
                second[i] = symbol[8 + (sideLength - 1 - i) * sideLength];
            for (int i = 0; i < 8; i++)
                second[14 - i] = symbol[(sideLength - 1 - i) + 8 * sideLength];

            return (new ReadOnlyMemory<byte>(first), new ReadOnlyMemory<byte>(second));
        }

        private static ((int mask, ErrorCorrectionLevel errorCorrectionLevel), (int mask, ErrorCorrectionLevel errorCorrectionLevel))
        DecodeFormatInformationBlocks((ReadOnlyMemory<byte>, ReadOnlyMemory<byte>) blocks)
        {
            static int GetIndexOfNearest(int value)
            {
                int[] lut = { 0x5412, 0x5125, 0x5E7C, 0x5B4B, 0x45F9, 0x40CE, 0x4F97, 0x4AA0,
                              0x77C4, 0x72F3, 0x7DAA, 0x789D, 0x662F, 0x6318, 0x6C41, 0x6976,
                              0x1689, 0x13BE, 0x1CE7, 0x19D0, 0x0762, 0x0255, 0x0D0C, 0x083B,
                              0x355F, 0x3068, 0x3F31, 0x3A06, 0x24B4, 0x2183, 0x2EDA, 0x2BED };

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
                    throw new Exception($"Unable to determine correct format information{Environment.NewLine}Format block:{Convert.ToString(value, 2)}");
                }
            }
            var (a, b) = blocks;
            int first = Utilities.AssembleBits(a.Span, 0, 15);
            int second = Utilities.AssembleBits(b.Span, 0, 15);

            int firstValue = GetIndexOfNearest(first);
            int secondValue = GetIndexOfNearest(second);
            return ((firstValue & 7, (ErrorCorrectionLevel)((firstValue & 24) >> 3)), (secondValue & 7, (ErrorCorrectionLevel)((secondValue & 24) >> 3)));

        }

        private static Func<int, int, bool> GetMaskPattern(int mask)
        {
            return mask switch
            {
                0 => (i, j) => (i + j) % 2 == 0,
                1 => (i, j) => i % 2 == 0,
                2 => (i, j) => j % 3 == 0,
                3 => (i, j) => (i + j) % 3 == 0,
                4 => (i, j) => ((i / 2) + (j / 3)) % 2 == 0,
                5 => (i, j) => (i * j) % 2 + (i * j) % 3 == 0,
                6 => (i, j) => ((i * j) % 2 + (i * j) % 3) % 2 == 0,
                7 => (i, j) => ((i + j) % 2 + (i * j) % 3) % 2 == 0,
                _ => throw new Exception($"Unknown mask number.{Environment.NewLine}Mask number:{mask}")
            };
        }
    }
}
