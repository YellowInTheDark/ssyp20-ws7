using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QRTestingTools.Symbol.Data
{
    class SegmentDecoder
    {
        /// <summary>
        /// Decodes all segments in <paramref name="bitArray"/>
        /// until an unknown or a terminator segment is encountered or the end is reached
        /// </summary>
        /// <param name="bitArray">Span of bits to be decoded</param>
        /// <param name="codeVersion">QR Code version from 1 to 40</param>
        /// <returns>List of successfully decoded segments</returns>
        public static List<Segment> DecodeAllSegments(QRSymbol symbol)
        {
            var bitArray = symbol.Data;
            List<Segment> segments = new List<Segment>();
            while (TryDecodeFirstSegment(bitArray.Span, symbol.VersionInfo.Version, out Segment segment, out int bitsRead))
            {
                segments.Add(segment);
                bitArray = bitArray.Slice(bitsRead);
                if (segment.Mode == SegmentMode.Terminator)
                    break;
            }
            return segments;
        }

        /// <summary>
        /// Tries to decode the first segment in <paramref name="bitArray"/>
        /// </summary>
        /// <param name="bitArray">Span of bits to be decoded</param>
        /// <param name="codeVersion">QR Code version from 1 to 40</param>
        /// <param name="result">Decoded segment</param>
        /// <param name="bitsRead">Amount of used bits in <paramref name="bitArray"/></param>
        /// <returns>Inndicates if a segment was successfully decoded</returns>
        static bool TryDecodeFirstSegment(ReadOnlySpan<byte> bitArray, int codeVersion, out Segment result, out int bitsRead)
        {
            int modeIndicatorLength = Math.Min(4, bitArray.Length);
            SegmentMode mode = (SegmentMode)AssembleBits(bitArray.Slice(0, modeIndicatorLength), 0, modeIndicatorLength);

            switch (mode)
            {
                case SegmentMode.ECI:
                    {
                        Console.WriteLine("ECI segments are not supported");
                        result = new Segment();
                        bitsRead = 0;
                        return false;
                    }
                case SegmentMode.Numeric:
                case SegmentMode.Alphanumeric:
                case SegmentMode.Byte:
                case SegmentMode.Kanji:
                    {
                        try //TODO: replace this try/catch with something decent
                        {
                            ReadOnlySpan<byte> countIndicator = bitArray.Slice(modeIndicatorLength, GetCountIndicatorLength(codeVersion, mode));
                            int count = AssembleBits(countIndicator, 0, countIndicator.Length);
                            int bitCount = GetBitCount(mode, count);
                            ReadOnlySpan<byte> data = bitArray.Slice(modeIndicatorLength + countIndicator.Length, bitCount);
                            result =
                            new Segment
                            {
                                Mode = mode,
                                CharacterCount = count,
                                Content = mode switch
                                {
                                    SegmentMode.Numeric => DecodeNumeric(data, count),
                                    SegmentMode.Alphanumeric => DecodeAlphanumeric(data, count),
                                    SegmentMode.Byte => DecodeByte(data, count),
                                    SegmentMode.Kanji => DecodeKanji(data, count),
                                    _ => throw new Exception("wtf")
                                }
                            };
                            bitsRead = modeIndicatorLength + countIndicator.Length + bitCount;
                            return true;
                        }
                        catch 
                        {
                            result = new Segment();
                            bitsRead = 0;
                            return false;
                        }
                    }
                case SegmentMode.StructuredAppend:
                    {
                        Console.WriteLine("Structured Append segments are not supported");
                        result = new Segment();
                        bitsRead = 0;
                        return false;
                    }
                case SegmentMode.FNC1FirstPosition:
                    {
                        Console.WriteLine("FNC1 segments are not supported");
                        result = new Segment();
                        bitsRead = 0;
                        return false;
                    }
                case SegmentMode.FNC1SecondPosition:
                    {
                        Console.WriteLine("FNC1 segments are not supported");
                        result = new Segment();
                        bitsRead = 0;
                        return false;
                    }
                case SegmentMode.Terminator:
                    {
                        result =
                        new Segment
                        {
                            Mode = mode,
                            CharacterCount = 0,
                            Content = new byte[0]
                        };
                        bitsRead = modeIndicatorLength;
                        return true;
                    }
                default:
                    {
                        Console.WriteLine("Unknown segments are not supported");
                        result = new Segment();
                        bitsRead = 0;
                        return false;
                    }
            }
        }

        /// <summary>
        /// Calculates the amount of bits needed to encode <paramref name="count"/> characters
        /// in the specified segment mode
        /// </summary>
        /// <param name="mode">Segment mode</param>
        /// <param name="count">Amount of characters to encode</param>
        /// <returns></returns>
        private static int GetBitCount(SegmentMode mode, int count)
        {
            return mode switch
            {
                SegmentMode.Numeric => (count / 3) * 10 +
                                (count % 3) switch
                                {
                                    0 => 0,
                                    1 => 4,
                                    2 => 7,
                                    _ => throw new Exception("wtf")
                                },
                SegmentMode.Alphanumeric => 11 * (count / 2) + 6 * (count % 2),
                SegmentMode.Byte => 8 * count,
                SegmentMode.Kanji => 13 * count,
                _ => throw new Exception($"Can't calculate bit count for: {mode}")
            };
        }

        /// <summary>
        /// Calculates the length of the character count indicator for given QR Code version and segment mode
        /// </summary>
        /// <param name="codeVersion">QR Code version from 1 to 40</param>
        /// <param name="mode">Segment mode</param>
        private static int GetCountIndicatorLength(int codeVersion, SegmentMode mode)
        {
            return (codeVersion, mode) switch
            {
                (_, _) when codeVersion <= 0 => throw new Exception($"Code version can't be less than 0. Given version: {codeVersion}"),
                (_, _) when codeVersion > 40 => throw new Exception($"Code version can't be greater than 40. Given version: {codeVersion}"),
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
                _ => throw new Exception($"Can't get count indicator length for version: {codeVersion} and mode: {mode}")
            };
        }

        /// <summary>
        /// Assembles up to 32 bits from a given span into an <c>Int32</c> value
        /// </summary>
        /// <param name="data">Span of bits to be assembled</param>
        /// <param name="start">Starting position in <paramref name="data"/></param>
        /// <param name="bitCount">Amount of bits to be assembled</param>
        private static int AssembleBits(ReadOnlySpan<byte> data, int start, int bitCount)
        {
            if (data.Length < start + bitCount)
                throw new Exception("Given data is too short");
            if (bitCount > 32)
                throw new Exception("Can't assemble more than 32 bits at a time");
            if (bitCount == 0)
                return 0;
            int res = 0;
            for (int i = 0; i < bitCount - 1; i++)
                res = (res + data[start + i]) << 1;
            res += data[start + bitCount - 1];
            return res;
        }

        /// <summary>
        /// Decodes data from a Numeric segment
        /// </summary>
        /// <param name="data">Data to be decoded</param>
        /// <param name="count">Count of encoded Numeric characters</param>
        private static byte[] DecodeNumeric(ReadOnlySpan<byte> data, int count)
        {
            List<byte> bytes = new List<byte>();
            for (int i = 0, j = count; j >= 3; i++, j -= 3)
                bytes.AddRange(Encoding.ASCII.GetBytes(AssembleBits(data, i * 10, 10).ToString()));
            if (count % 3 == 2)
                bytes.AddRange(Encoding.ASCII.GetBytes(AssembleBits(data, (count / 3) * 10, 7).ToString()));
            else if (count % 3 == 1)
                bytes.AddRange(Encoding.ASCII.GetBytes(AssembleBits(data, (count / 3) * 10, 4).ToString()));
            return bytes.ToArray();
        }

        /// <summary>
        /// Decodes data from an Alphanumeric segment
        /// </summary>
        /// <param name="data">Data to be decoded</param>
        /// <param name="count">Count of encoded Alphanumeric characters</param>
        private static byte[] DecodeAlphanumeric(ReadOnlySpan<byte> data, int count)
        {
            byte[] table = { 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, //Numbers
                             0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, //Letters
                             0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x51, 0x52, 0x53, 0x54, //Letters
                             0x55, 0x56, 0x57, 0x58, 0x59, 0x5A,                         //Letters
                             0x20, 0x24, 0x25, 0x2A, 0x2B, 0x2D, 0x2E, 0x2F, 0x3A };     //Punctuation, Symbols
            List<byte> bytes = new List<byte>();
            for (int i = 0, j = count; j >= 2; i++, j -= 2)
            {
                var tmp = AssembleBits(data, i * 11, 11);
                bytes.Add(table[tmp / 45]);
                bytes.Add(table[tmp % 45]);
            }
            if (count % 2 == 1)
                bytes.Add(table[AssembleBits(data, (count / 2) * 11, 6)]);

            return bytes.ToArray();
        }

        /// <summary>
        /// Decodes data from a Byte segment
        /// </summary>
        /// <param name="data">Data to be decoded</param>
        /// <param name="count">Count of encoded Byte characters</param>
        private static byte[] DecodeByte(ReadOnlySpan<byte> data, int count)
        {
            List<byte> bytes = new List<byte>();
            foreach (int i in Enumerable.Range(0, count))
                bytes.Add((byte)AssembleBits(data, i * 8, 8));
            return bytes.ToArray();
        }

        /// <summary>
        /// Decodes data from a Kanji segment
        /// </summary>
        /// <param name="data">Data to be decoded</param>
        /// <param name="count">Count of encoded Kanji characters</param>
        private static byte[] DecodeKanji(ReadOnlySpan<byte> data, int count)
        {
            List<byte> bytes = new List<byte>();
            foreach (int i in Enumerable.Range(0, count))
            {
                var tmp = AssembleBits(data, i * 13, 13);
                tmp = (((tmp / 0xC0) << 8) | (tmp % 0xC0));
                if (tmp >= 0x1F00)
                    tmp += 0xC140;
                else
                    tmp += 0x8140;
                bytes.Add((byte)((tmp & 0xff00) >> 8));
                bytes.Add((byte)(tmp & 0xff));
            }

            return bytes.ToArray();
        }
    }
}
