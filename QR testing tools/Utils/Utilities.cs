using System;

namespace QRTestingTools.Utils
{
    static class Utilities
    {
        public static int AssembleBits(ReadOnlySpan<byte> data, int start, int bitCount)
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
    }
}
