using System;


namespace QR
{
    public class Encoders
    {
        public static string NumericCoder(string input)
        {
            string BinaryString = " ";
            while (input.Length >= 3)
            {
                string sum = input.Remove(3, input.Length - 3);
                var buff = Convert.ToString(int.Parse(sum), 2);
                buff = buff.PadLeft(10, '0');
                BinaryString = BinaryString.Insert(BinaryString.Length - 1, buff);
                input = input.Remove(0, 3);
            }
            if (input.Length == 2)
            {
                var buff = Convert.ToString(int.Parse(input), 2);
                buff = buff.PadLeft(7, '0');
                BinaryString = BinaryString.Insert(BinaryString.Length - 1, buff);
                input = input.Remove(0, 2);
            }
            else if (input.Length == 1)
            {
                var buff = Convert.ToString(int.Parse(input), 2);
                buff = buff.PadLeft(4, '0');
                BinaryString = BinaryString.Insert(BinaryString.Length - 1, buff);
            }
            BinaryString = BinaryString.Replace(" ", "");
            return BinaryString;
        }


        public static string AlphanumericCoder(string input)
        {
            string BinaryString = " ";
            while (input.Length >= 2)
            {
                var Felement = AllDictionaries.AlphanumericDictionary(input[0]);
                var Selement = AllDictionaries.AlphanumericDictionary(input[1]);
                var sum = Felement * 45 + Selement;
                var buff = Convert.ToString(sum, 2);
                buff = buff.PadLeft(11, '0');
                BinaryString = BinaryString.Insert(BinaryString.Length - 1, buff);
                input = input.Remove(0, 2);
            }
            if (input.Length == 1)
            {
                var buff = Convert.ToString(AllDictionaries.AlphanumericDictionary(input[0]), 2);
                buff = buff.PadLeft(6, '0');
                BinaryString = BinaryString.Insert(BinaryString.Length - 1, buff);
            }
            BinaryString = BinaryString.Replace(" ", "");
            return BinaryString;

        }


        public static string ByteCoder(byte[] bytes)
        {
            string streamBits = " ";
            foreach (var item in bytes)
            {
                var buff = Convert.ToString(item, 2);
                buff = buff.PadLeft(8, '0');
                streamBits = streamBits.Insert(streamBits.Length - 1, buff);
            }
            streamBits = streamBits.Replace(" ", "");
            return streamBits;
        }
    }
}
