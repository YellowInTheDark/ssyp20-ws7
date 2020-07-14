using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QR_testing_tools
{
    class SegmentDecoder
    {
        public static string DecodeFirstSegment(ReadOnlySpan<byte> bitArray, int codeVersion)
        {
            SegmentMode mode = (SegmentMode)AssembleBits(bitArray.Slice(0, 4), 0, 4); // BitConverter.ToInt32(bitArray.Slice(0, 4));
            ReadOnlySpan<byte> countIndicator = bitArray.Slice(4, GetCountIndicatorLength(codeVersion, mode));
            int count = AssembleBits(countIndicator, 0, countIndicator.Length);
            int bitCount = GetBitCount(mode, count);
            ReadOnlySpan<byte> data = bitArray.Slice(4 + countIndicator.Length, bitCount);

            return mode switch
            {
                SegmentMode.Numeric => DecodeNumeric(data, count),
                SegmentMode.Alphanumeric => DecodeAlphanumeric(data, count),
                SegmentMode.Byte => DecodeByte(data, count),
                SegmentMode.Kanji => DecodeKanji(data, count),
                _ => throw new Exception()
            };
        }

        private static int GetBitCount(SegmentMode mode, int count)
        {
            return mode switch
            {
                SegmentMode.Numeric => (count / 3) * 10 +
                                (count % 3) switch
                                {
                                    0 => 0,
                                    1 => 4,
                                    2 => 7
                                },
                SegmentMode.Alphanumeric => 11 * (count / 2) + 6 * (count % 2),
                SegmentMode.Byte => 8 * count,
                SegmentMode.Kanji => 13 * count,
                _ => throw new Exception()
            };
        }

        private static int GetCountIndicatorLength(int codeVersion, SegmentMode mode)
        {
            return (codeVersion, mode) switch
            {
                (_, _) when codeVersion <= 0 => throw new Exception(),
                (_, SegmentMode.Numeric) when codeVersion <= 9 => 10,
                (_, SegmentMode.Numeric) when codeVersion <= 26 => 12,
                (_, SegmentMode.Numeric) when codeVersion <= 40 => 14,
                (_, SegmentMode.Alphanumeric) when codeVersion <= 9 => 9,
                (_, SegmentMode.Alphanumeric) when codeVersion <= 26 => 11,
                (_, SegmentMode.Alphanumeric) when codeVersion <= 40 => 13,
                (_, SegmentMode.Byte) when codeVersion <= 9 => 8,
                (_, SegmentMode.Byte) when codeVersion <= 26 => 16,
                (_, SegmentMode.Byte) when codeVersion <= 40 => 16,
                (_, SegmentMode.Kanji) when codeVersion <= 9 => 8,
                (_, SegmentMode.Kanji) when codeVersion <= 26 => 10,
                (_, SegmentMode.Kanji) when codeVersion <= 40 => 12,
                _ => throw new Exception()
            };
        }

        private static int AssembleBits(ReadOnlySpan<byte> data, int start, int bitCount)
        {
            int res = 0;
            for (int i = 0; i < bitCount - 1; i++)
                res = (res + data[start + i]) << 1;
            res += data[start + bitCount - 1];
            return res;
        }

        private static string DecodeNumeric(ReadOnlySpan<byte> data, int count)
        {
            List<byte> bytes = new List<byte>();
            for (int i = 0, j = count; j >= 3; i++, j -= 3)
                bytes.AddRange(Encoding.ASCII.GetBytes(AssembleBits(data, i * 10, 10).ToString()));
            if (count % 3 == 2)
                bytes.AddRange(Encoding.ASCII.GetBytes(AssembleBits(data, (count / 3) * 10, 7).ToString()));
            else if (count % 3 == 1)
                bytes.AddRange(Encoding.ASCII.GetBytes(AssembleBits(data, (count / 3) * 10, 4).ToString()));

            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        private static string DecodeAlphanumeric(ReadOnlySpan<byte> data, int count)
        { throw new Exception(); }

        private static string DecodeByte(ReadOnlySpan<byte> data, int count)
        {
            List<byte> bytes = new List<byte>();
            foreach (int i in Enumerable.Range(0, count))
                bytes.Add((byte)AssembleBits(data, i * 8, 8));
            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        private static string DecodeKanji(ReadOnlySpan<byte> data, int count)
        { throw new Exception(); }
    }
}
