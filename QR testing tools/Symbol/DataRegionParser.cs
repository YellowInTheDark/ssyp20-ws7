using System;
using System.Collections.Generic;
using System.IO;

namespace QRTestingTools.Symbol
{
    static class DataRegionParser
    {
        private static readonly ((int, int, int), (int, int))[][] LUT =
        {
         /*dummy*/new ((int, int, int), (int, int))[]{ },
            /*01*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((26, 19, 2), (1, 0)),
                  /*M*/((26, 16, 4), (1, 0)),
                  /*Q*/((26, 13, 6), (1, 0)),
                  /*H*/((26,  9, 8), (1, 0))
                  },
            /*02*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((44, 34,  4), (1, 0)),
                  /*M*/((44, 28,  8), (1, 0)),
                  /*Q*/((44, 22, 11), (1, 0)),
                  /*H*/((44, 16, 14), (1, 0))
                  },
            /*03*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((70, 55,  7), (1, 0)),
                  /*M*/((70, 44, 13), (1, 0)),
                  /*Q*/((35, 17,  9), (2, 0)),
                  /*H*/((35, 13, 11), (2, 0))
                  },
            /*04*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((100, 80, 10), (1, 0)),
                  /*M*/(( 50, 32,  9), (2, 0)),
                  /*Q*/(( 50, 24, 13), (2, 0)),
                  /*H*/(( 25,  9,  8), (4, 0))
                  },
            /*05*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((134, 108, 13), (1, 0)),
                  /*M*/(( 67,  43, 12), (2, 0)),
                  /*Q*/(( 33,  15,  9), (2, 2)),
                  /*H*/(( 33,  11, 11), (2, 2))
                  },
            /*06*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((86, 68,  9), (2, 0)),
                  /*M*/((43, 27,  8), (4, 0)),
                  /*Q*/((43, 19, 12), (4, 0)),
                  /*H*/((43, 15, 14), (4, 0))
                  },
            /*07*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((98, 78, 10), (2, 0)),
                  /*M*/((49, 31,  9), (4, 0)),
                  /*Q*/((32, 14,  9), (2, 4)),
                  /*H*/((39, 13, 13), (4, 1))
                  },
            /*08*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((121, 97, 12), (2, 0)),
                  /*M*/(( 60, 38, 11), (2, 2)),
                  /*Q*/(( 40, 18, 11), (4, 2)),
                  /*H*/(( 40, 14, 13), (4, 2))
                  },
            /*09*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((146, 116, 15), (2, 0)),
                  /*M*/(( 58,  36, 11), (3, 2)),
                  /*Q*/(( 36,  16, 10), (4, 4)),
                  /*H*/(( 36,  12, 12), (4, 4))
                  },
            /*10*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((86, 68,  9), (2, 2)),
                  /*M*/((69, 43, 13), (4, 1)),
                  /*Q*/((43, 19, 12), (6, 2)),
                  /*H*/((43, 15, 14), (6, 2))
                  },
            /*11*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((101, 81, 10), (4, 0)),
                  /*M*/(( 80, 50, 15), (1, 4)),
                  /*Q*/(( 50, 22, 14), (4, 4)),
                  /*H*/(( 36, 12, 12), (3, 8))
                  },
            /*12*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((116, 92, 12), (2, 2)),
                  /*M*/(( 58, 36, 11), (6, 2)),
                  /*Q*/(( 46, 20, 13), (4, 6)),
                  /*H*/(( 42, 14, 14), (7, 4))
                  },
            /*13*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((133, 107, 13), (4,  0)),
                  /*M*/(( 59,  37, 11), (8,  1)),
                  /*Q*/(( 44,  20, 12), (8,  4)),
                  /*H*/(( 33,  11, 11), (12, 4))
                  },
            /*14*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((145, 115, 15), (3,  1)),
                  /*M*/(( 64,  40, 12), (4,  5)),
                  /*Q*/(( 36,  16, 10), (11, 5)),
                  /*H*/(( 36,  12, 12), (11, 5))
                  },
            /*15*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((109, 87, 11), ( 5, 1)),
                  /*M*/(( 65, 41, 12), ( 5, 5)),
                  /*Q*/(( 54, 24, 15), ( 5, 7)),
                  /*H*/(( 36, 12, 12), (11, 7))
                  },
            /*16*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((122, 98, 12), ( 5,  1)),
                  /*M*/(( 73, 45, 14), ( 7,  3)),
                  /*Q*/(( 43, 19, 12), (15,  2)),
                  /*H*/(( 45, 15, 15), ( 3, 13))
                  },
            /*17*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((135, 107, 14), ( 1,  5)),
                  /*M*/(( 74,  46, 14), (10,  1)),
                  /*Q*/(( 50,  22, 14), ( 1, 15)),
                  /*H*/(( 42,  14, 14), ( 2, 17))
                  },
            /*18*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((150, 120, 15), ( 5,  1)),
                  /*M*/(( 69,  43, 13), ( 9,  4)),
                  /*Q*/(( 50,  22, 14), (17,  1)),
                  /*H*/(( 42,  14, 14), ( 2, 19))
                  },
            /*19*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((141, 113, 14), ( 3,  4)),
                  /*M*/(( 70,  44, 13), ( 3, 11)),
                  /*Q*/(( 47,  21, 13), (17,  4)),
                  /*H*/(( 39,  13, 13), ( 9, 16))
                  },
            /*20*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((135, 107, 14), ( 3,  5)),
                  /*M*/(( 67,  41, 13), ( 3, 13)),
                  /*Q*/(( 54,  24, 15), (15,  5)),
                  /*H*/(( 43,  15, 14), (15, 10))
                  },
            /*21*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((144, 116, 14), ( 4, 4)),
                  /*M*/(( 68,  42, 13), (17, 0)),
                  /*Q*/(( 50,  22, 14), (17, 6)),
                  /*H*/(( 46,  16, 15), (19, 6))
                  },
            /*22*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((139, 111, 14), ( 2,  7)),
                  /*M*/(( 74,  46, 14), (17,  0)),
                  /*Q*/(( 54,  24, 15), ( 7, 16)),
                  /*H*/(( 37,  13, 12), (34,  0))
                  },
            /*23*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((151, 121, 15), ( 4,  5)),
                  /*M*/(( 75,  47, 14), ( 4, 14)),
                  /*Q*/(( 54,  24, 15), (11, 14)),
                  /*H*/(( 45,  15, 15), (16, 14))
                  },
            /*24*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((147, 117, 15), ( 6,  4)),
                  /*M*/(( 73,  45, 14), ( 6, 14)),
                  /*Q*/(( 54,  24, 15), (11, 16)),
                  /*H*/(( 46,  16, 15), (30,  2))
                  },
            /*25*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((132, 106, 13), ( 8,  4)),
                  /*M*/(( 75,  47, 14), ( 8, 13)),
                  /*Q*/(( 54,  24, 15), ( 7, 22)),
                  /*H*/(( 45,  15, 15), (22, 13))
                  },
            /*26*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((142, 114, 14), (10, 2)),
                  /*M*/(( 74,  46, 14), (19, 4)),
                  /*Q*/(( 50,  22, 14), (28, 6)),
                  /*H*/(( 46,  16, 15), (33, 4))
                  },
            /*27*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((152, 122, 15), ( 8,  4)),
                  /*M*/(( 73,  45, 14), (22,  3)),
                  /*Q*/(( 53,  23, 15), ( 8, 26)),
                  /*H*/(( 45,  15, 15), (12, 28))
                  },
            /*28*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((147, 117, 15), ( 3, 10)),
                  /*M*/(( 73,  45, 14), ( 3, 23)),
                  /*Q*/(( 54,  24, 15), ( 4, 31)),
                  /*H*/(( 45,  15, 15), (11, 31))
                  },
            /*29*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((146, 116, 15), ( 7,  7)),
                  /*M*/(( 73,  45, 14), (21,  7)),
                  /*Q*/(( 53,  23, 15), ( 1, 37)),
                  /*H*/(( 45,  15, 15), (19, 26))
                  },
            /*30*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((145, 115, 15), ( 5, 10)),
                  /*M*/(( 75,  47, 14), (19, 10)),
                  /*Q*/(( 54,  24, 15), (15, 25)),
                  /*H*/(( 45,  15, 15), (23, 25))
                  },
            /*31*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((145, 115, 15), (13,  3)),
                  /*M*/(( 74,  46, 14), ( 2, 29)),
                  /*Q*/(( 54,  24, 15), (42,  1)),
                  /*H*/(( 45,  15, 15), (23, 28))
                  },
            /*32*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((145, 115, 15), (17,  0)),
                  /*M*/(( 74,  46, 14), (10, 23)),
                  /*Q*/(( 54,  24, 15), (10, 35)),
                  /*H*/(( 45,  15, 15), (19, 35))
                  },
            /*33*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((145, 115, 15), (17,  1)),
                  /*M*/(( 74,  46, 14), (14, 21)),
                  /*Q*/(( 54,  24, 15), (29, 19)),
                  /*H*/(( 45,  15, 15), (11, 46))
                  },
            /*34*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((145, 115, 15), (13,  6)),
                  /*M*/(( 74,  46, 14), (14, 23)),
                  /*Q*/(( 54,  24, 15), (44,  7)),
                  /*H*/(( 46,  16, 15), (59,  1))
                  },
            /*35*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((151, 121, 15), (12,  7)),
                  /*M*/(( 75,  47, 14), (12, 26)),
                  /*Q*/(( 54,  24, 15), (39, 14)),
                  /*H*/(( 45,  15, 15), (22, 41))
                  },
            /*36*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((151, 121, 15), ( 6, 14)),
                  /*M*/(( 75,  47, 14), ( 6, 34)),
                  /*Q*/(( 54,  24, 15), (46, 10)),
                  /*H*/(( 45,  15, 15), ( 2, 64))
                  },
            /*37*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((152, 122, 15), (17,  4)),
                  /*M*/(( 74,  46, 14), (29, 14)),
                  /*Q*/(( 54,  24, 15), (49, 10)),
                  /*H*/(( 45,  15, 15), (24, 46))
                  },
            /*38*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((152, 122, 15), ( 4, 18)),
                  /*M*/(( 74,  46, 14), (13, 32)),
                  /*Q*/(( 54,  24, 15), (48, 14)),
                  /*H*/(( 45,  15, 15), (42, 32))
                  },
            /*39*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((147, 117, 15), (20,  4)),
                  /*M*/(( 75,  47, 14), (40,  7)),
                  /*Q*/(( 54,  24, 15), (43, 22)),
                  /*H*/(( 45,  15, 15), (10, 67))
                  },
            /*40*/new ((int, int, int), (int, int))[]
                  {
                  /*L*/((148, 118, 15), (19,  6)),
                  /*M*/(( 75,  47, 14), (18, 31)),
                  /*Q*/(( 54,  24, 15), (34, 34)),
                  /*H*/(( 45,  15, 15), (20, 61))
                  }
        };

        public static ReadOnlyMemory<byte> Parse(ReadOnlySpan<byte> symbol, FormatInformation formatInfo, VersionInformation versionInfo)
        {
            return DecodeData(UnscrambleData(ExtractData(UnmaskQRMatrix(GetQRMatrix(symbol, versionInfo),
                                                         formatInfo, versionInfo),
                                             versionInfo),
                              formatInfo, versionInfo),
                   formatInfo, versionInfo);
        }

        private static byte[,] GetQRMatrix(ReadOnlySpan<byte> symbol, VersionInformation versionInfo)
        {
            int sideLength = QRSymbol.GetSideLength(versionInfo);
            if (sideLength * sideLength != symbol.Length)
                throw new Exception($"Symbol size does not match with the given version.{Environment.NewLine}Symbol length:{symbol.Length}{Environment.NewLine}Version:{versionInfo.Version}");
            byte[,] matrix = new byte[sideLength, sideLength];

            for (int i = 0; i < sideLength; i++)
                for (int j = 0; j < sideLength; j++)
                    matrix[i, j] = symbol[i * sideLength + j];

            return matrix;
        }

        private static byte[,] UnmaskQRMatrix(byte[,] matrix, FormatInformation formatInfo, VersionInformation versionInfo)
        {
            var sideLength = QRSymbol.GetSideLength(versionInfo);
            var map = QRSymbol.GetNondataPatternsMap(versionInfo);
            var pattern = formatInfo.MaskPattern;

            for (int i = 0; i < sideLength; i++)
                for (int j = 0; j < sideLength; j++)
                    if (map[i, j] == false)
                        matrix[i, j] ^= pattern(i, j) switch { true => (byte)1, false => (byte)0 };

            return matrix;
        }

        private static byte[] ExtractData(byte[,] unmaskedMatrix, VersionInformation versionInfo)
        {       

            List<byte> data = new List<byte>();
            int sideLength = QRSymbol.GetSideLength(versionInfo);
            bool[,] map = QRSymbol.GetNondataPatternsMap(versionInfo);

            int col = sideLength - 1;
            int row = sideLength - 1;
            int rowOffset = -1;
            while (col > 0)
            {
                if (col == 6)
                    col--;

                while (row >= 0 && row < sideLength)
                {
                    if (map[row, col] == false)
                        data.Add(unmaskedMatrix[row, col]);
                    if (map[row, col - 1] == false)
                        data.Add(unmaskedMatrix[row, col - 1]);
                    row += rowOffset;
                }

                col -= 2;
                rowOffset = -rowOffset;
                row += rowOffset;
            }

            return data.ToArray();
        }

        private static byte[][] UnscrambleData(byte[] data, FormatInformation formatInfo, VersionInformation versionInfo)
        {
            List<byte[]> unscrambledBlocks = new List<byte[]>();

            int correctionLevelIndex = formatInfo.ErrorCorrectionLevel switch 
            { 
                ErrorCorrectionLevel.L => 0,
                ErrorCorrectionLevel.M => 1,
                ErrorCorrectionLevel.Q => 2,
                ErrorCorrectionLevel.H => 3,
                _ => throw new Exception($"Unknown error correction level{Environment.NewLine}Correction level:{formatInfo.ErrorCorrectionLevel}")
            };
            var ((totalWordsInGroup1Block, dataWordsInGroup1Block, _), (group1BlockCount, group2BlockCount)) = LUT[versionInfo.Version][correctionLevelIndex];
            int wordSize = 8;
            int totalBlockCount = group1BlockCount + group2BlockCount;
            int totalWordsInGroup2Block = totalWordsInGroup1Block + 1;
            int dataWordsInGroup2Block = dataWordsInGroup1Block + 1;
            int correctionWordsInGroup1Block = totalWordsInGroup1Block - dataWordsInGroup1Block;

            for (int i = 0; i < group1BlockCount; i++)
                unscrambledBlocks.Add(new byte[totalWordsInGroup1Block * wordSize]);
            for (int i = 0; i < group2BlockCount; i++)
                unscrambledBlocks.Add(new byte[totalWordsInGroup2Block * wordSize]);

            using (MemoryStream ms = new MemoryStream(data, false))
            {
                for (int col = 0; col < dataWordsInGroup1Block; col++)
                    for (int row = 0; row < totalBlockCount; row++)
                        for (int bit = 0; bit < wordSize; bit++)
                            unscrambledBlocks[row][col * 8 + bit] = (byte)ms.ReadByte();
                for (int row = group1BlockCount; row < totalBlockCount; row++)
                    for (int bit = 0; bit < wordSize; bit++)
                        unscrambledBlocks[row][dataWordsInGroup1Block * 8 + bit] = (byte)ms.ReadByte();
                for (int col = 0; col < correctionWordsInGroup1Block; col++)
                    for (int row = 0; row < totalBlockCount; row++)
                        for (int bit = 0; bit < wordSize; bit++)
                            if(row < group1BlockCount)
                                unscrambledBlocks[row][(dataWordsInGroup1Block + col) * 8 + bit] = (byte)ms.ReadByte();
                            else
                                unscrambledBlocks[row][(dataWordsInGroup2Block + col) * 8 + bit] = (byte)ms.ReadByte();
            }

            return unscrambledBlocks.ToArray();
        }

        private static byte[] DecodeData(byte[][] blocks, FormatInformation formatInfo, VersionInformation versionInfo)
        {
            int correctionLevelIndex = formatInfo.ErrorCorrectionLevel switch
            {
                ErrorCorrectionLevel.L => 0,
                ErrorCorrectionLevel.M => 1,
                ErrorCorrectionLevel.Q => 2,
                ErrorCorrectionLevel.H => 3,
                _ => throw new Exception($"Unknown error correction level{Environment.NewLine}Correction level:{formatInfo.ErrorCorrectionLevel}")
            };
            var ((_, dataWordsInGroup1Block, _), (group1BlockCount, group2BlockCount)) = LUT[versionInfo.Version][correctionLevelIndex];
            List<byte> result = new List<byte>();

            for (int i = 0; i < group1BlockCount; i++)
                result.AddRange(blocks[i][0..(dataWordsInGroup1Block * 8)]);
            for (int i = 0; i < group2BlockCount; i++)
                result.AddRange(blocks[group1BlockCount + i][0..((dataWordsInGroup1Block + 1) * 8)]);

            return result.ToArray();
        }
    }
}
