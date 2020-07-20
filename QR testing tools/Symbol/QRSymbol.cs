using System;

namespace QRTestingTools.Symbol
{
    class QRSymbol
    {
        public VersionInformation VersionInfo { get; }
        public FormatInformation FormatInfo { get; }
        public ReadOnlyMemory<byte> Data { get; }

        public QRSymbol(ReadOnlySpan<byte> symbol)
        {
            VersionInfo = VersionInformation.CreateFrom(symbol);
            FormatInfo = FormatInformation.CreateFrom(symbol);
            Data = DataRegionParser.Parse(symbol, FormatInfo, VersionInfo);
        }

        public static int GetVersion(ReadOnlySpan<byte> symbol)
        {
            int[] validLengths = new int[] {   441,   625,   841,  1089,  1369,  1681,  2025,  2401,  2809,  3249,
                                              3721,  4225,  4761,  5329,  5929,  6561,  7225,  7921,  8649,  9409,
                                             10201, 11025, 11881, 12769, 13689, 14641, 15625, 16641, 17689, 18769,
                                             19881, 21025, 22201, 23409, 24649, 25921, 27225, 28561, 29929, 31329 };
            return validLengths.AsSpan().IndexOf(symbol.Length) switch
                   { 
                       -1             => throw new Exception($"Can't determine the symbol's length because it doesn't correspond to any known code version. Length: {symbol.Length}"),
                       int validIndex => validIndex + 1
                   };
        }

        public bool[,] GetNondataPatternsMap()
        {
            return GetNondataPatternsMap(VersionInfo);
        }

        public static bool[,] GetNondataPatternsMap(VersionInformation versionInfo)
        {
            #region Visualization
            /*             1                        
             *        7   1|                    3 1   7
             *     ┌-----┐||                   ┌-┐|┌-----┐
             *   ┌─◘◘◘◘◘◘◘○▪░░░░░░░░░░░░░░░░░░┌▫▫▫○◘◘◘◘◘◘◘─┐
             *  7│ ◘◘◘◘◘◘◘○▪░░░░░░░░░░░░░░░░░6│▫▫▫○◘◘◘◘◘◘◘ │7
             *   │ ◘◘◘◘◘◘◘○▪░░░░░░░░░░░░░░░░░░└▫▫▫○◘◘◘◘◘◘◘ │
             *   └─◘◘◘◘◘◘◘○●●●●●●●●●●●●●●●●●●●●●●●○◘◘◘◘◘◘◘─┘
             *  1──○○○○○○○○▪░░░░░░░░░░░░░░░░░░░░░░○○○○○○○○──1
             * 1───▪▪▪▪▪▪●▪▪░░░░░░░░░░░░░░░░░░░░░░▪▪▪▪▪▪▪▪───1
             *     ░░░░░░●░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
             *     ░░░░░░●░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
             *     ░░░░░░●░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
             *     ┌░6-░┐●░░░░░░░░░░░░░░░┌░5░┐░░░░░░░░░░░░
             *   ┌─▫▫▫▫▫▫●░░░░░░░░░░░░░░░◙◙◙◙◙┐░░░░░░░░░░░
             *  3│ ▫▫▫▫▫▫●░░░░░░░░░░░░░░░◙◙◙◙◙│5░░░░░░░░░░
             *   └─▫▫▫▫▫▫●░░░░░░░░░░░░░░░◙◙◙◙◙┘░░░░░░░░░░░
             *     ○○○○○○○○▪░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
             *   ┌─◘◘◘◘◘◘◘○▪░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
             *   │ ◘◘◘◘◘◘◘○▪░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
             *  7│ ◘◘◘◘◘◘◘○▪░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
             *   └─◘◘◘◘◘◘◘○▪░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
             *     └-----┘||
             *        7   1|
             *             1
             * 
             * ◘ Finder Patterns
             * ○ Separators
             * ● Timing Patterns
             * ◙ Alignment Patterns
             * ▪ Format Information
             * ▫ Version Information
             * ░ Encoding Region
             */
            #endregion

            static void PlaceRectangle(bool[,] map, (int row, int column) start, (int height, int width) dimensions)
            {
                for (int i = 0; i < dimensions.height; i++)
                    for (int j = 0; j < dimensions.width; j++)
                        map[start.row + i, start.column + j] = true;
            }
            static void PlaceFinderPatterns(bool[,] map, int sideLength)
            {
                PlaceRectangle(map, (0, 0), (7, 7));
                PlaceRectangle(map, (0, sideLength - 7), (7, 7));
                PlaceRectangle(map, (sideLength - 7, 0), (7, 7));
            }
            static void PlaceSeparators(bool[,] map, int sideLength)
            {
                PlaceRectangle(map, (7, 0), (1, 8));
                PlaceRectangle(map, (0, 7), (7, 1));
                PlaceRectangle(map, (0, sideLength - 8), (7, 1));
                PlaceRectangle(map, (7, sideLength - 8), (1, 8));
                PlaceRectangle(map, (sideLength - 8, 0), (1, 8));
                PlaceRectangle(map, (sideLength - 7, 7), (7, 1));
            }
            static void PlaceTimingPatterns(bool[,] map, int sideLength)
            {
                PlaceRectangle(map, (6, 8), (1, sideLength - 16));
                PlaceRectangle(map, (8, 6), (sideLength - 16, 1));
            }
            static void PlaceAlignmentPatterns(bool[,] map, int sideLength)
            {
                // Row/Column cordinates of center module of alignment patterns
                int[][] lut =
                {
                 /*dummy*/new int[]{ },
                    /*01*/new int[]{ },
                    /*02*/new int[]{ 6, 18 },
                    /*03*/new int[]{ 6, 22 },
                    /*04*/new int[]{ 6, 26 },
                    /*05*/new int[]{ 6, 30 },
                    /*06*/new int[]{ 6, 34 },
                    /*07*/new int[]{ 6, 22, 38 },
                    /*08*/new int[]{ 6, 24, 42 },
                    /*09*/new int[]{ 6, 26, 46 },
                    /*10*/new int[]{ 6, 28, 50 },
                    /*11*/new int[]{ 6, 30, 54 },
                    /*12*/new int[]{ 6, 32, 58 },
                    /*13*/new int[]{ 6, 34, 62 },
                    /*14*/new int[]{ 6, 26, 46, 66 },
                    /*15*/new int[]{ 6, 26, 48, 70 },
                    /*16*/new int[]{ 6, 26, 50, 74 },
                    /*17*/new int[]{ 6, 30, 54, 78 },
                    /*18*/new int[]{ 6, 30, 56, 82 },
                    /*19*/new int[]{ 6, 30, 58, 86 },
                    /*20*/new int[]{ 6, 34, 62, 90 },
                    /*21*/new int[]{ 6, 28, 50, 72, 94 },
                    /*22*/new int[]{ 6, 26, 50, 74, 98 },
                    /*23*/new int[]{ 6, 30, 54, 78, 102 },
                    /*24*/new int[]{ 6, 28, 54, 80, 106 },
                    /*25*/new int[]{ 6, 32, 58, 84, 110 },
                    /*26*/new int[]{ 6, 30, 58, 86, 114 },
                    /*27*/new int[]{ 6, 34, 62, 90, 118 },
                    /*28*/new int[]{ 6, 26, 50, 74, 98, 122 },
                    /*29*/new int[]{ 6, 30, 54, 78, 102, 126 },
                    /*30*/new int[]{ 6, 26, 52, 78, 104, 130 },
                    /*31*/new int[]{ 6, 30, 56, 82, 108, 134 },
                    /*32*/new int[]{ 6, 34, 60, 86, 112, 138 },
                    /*33*/new int[]{ 6, 30, 58, 86, 114, 142 },
                    /*34*/new int[]{ 6, 34, 62, 90, 118, 146 },
                    /*35*/new int[]{ 6, 30, 54, 78, 102, 126, 150 },
                    /*36*/new int[]{ 6, 24, 50, 76, 102, 128, 154 },
                    /*37*/new int[]{ 6, 28, 54, 80, 106, 132, 158 },
                    /*38*/new int[]{ 6, 32, 58, 84, 110, 136, 162 },
                    /*39*/new int[]{ 6, 26, 54, 82, 110, 138, 166 },
                    /*40*/new int[]{ 6, 30, 58, 86, 114, 142, 170 }
                    };

                int version = (sideLength - 17) >> 2;
                for (int i = 0; i < lut[version].Length; i++)
                {
                    for (int j = 0; j < lut[version].Length; j++)
                    {
                        if ((i, j) switch
                        {
                            _ when i == 0 && j == 0 => false,
                            _ when i == 0 && j == lut[version].Length - 1 => false,
                            _ when i == lut[version].Length - 1 && j == 0 => false,
                            _ => true
                        })
                        {
                            PlaceRectangle(map, (lut[version][i] - 2, lut[version][j] - 2), (5, 5));
                        }
                    }
                }
            }
            static void PlaceFormatInformation(bool[,] map, int sideLength)
            {
                PlaceRectangle(map, (8, 0), (1, 6));
                PlaceRectangle(map, (8, 7), (1, 2));
                PlaceRectangle(map, (7, 8), (1, 1));
                PlaceRectangle(map, (0, 8), (6, 1));
                PlaceRectangle(map, (8, sideLength - 8), (1, 8));
                PlaceRectangle(map, (sideLength - 8, 8), (8, 1));
            }
            static void PlaceVersionInformation(bool[,] map, int sideLength)
            {
                PlaceRectangle(map, (sideLength - 11, 0), (3, 6));
                PlaceRectangle(map, (0, sideLength - 11), (6, 3));
            }

            int sideLength = GetSideLength(versionInfo);
            bool[,] map = new bool[sideLength, sideLength];

            PlaceFinderPatterns(map, sideLength);
            PlaceSeparators(map, sideLength);
            PlaceTimingPatterns(map, sideLength);
            PlaceAlignmentPatterns(map, sideLength);
            PlaceFormatInformation(map, sideLength);
            if (versionInfo.Version >= 7)
                PlaceVersionInformation(map, sideLength);

            return map;
        }

        public static int GetSideLength(VersionInformation versionInfo)
        {
            return 17 + 4 * versionInfo.Version;
        }
    }
}