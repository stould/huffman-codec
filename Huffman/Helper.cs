using System;
using System.IO;

namespace Huffman
{
    public static class Helper
    {
        public static (byte[], int) ConvertBinaryStringToByteArray(string binaryString)
        {
            int remainder = binaryString.Length % 8;
            int paddingRight = remainder == 0 ? 0 : 8 - remainder;
            if (paddingRight > 0)
            {
                binaryString = binaryString.PadRight(binaryString.Length + paddingRight, '0');
            }

            byte[] byteArray = new byte[binaryString.Length / 8];

            for (int i = 0; i < byteArray.Length; i++)
            {
                byteArray[i] = Convert.ToByte(binaryString.Substring(i * 8, 8), 2);
            }

            return (byteArray, paddingRight);
        }
        public static string GenerateRandomAsciiString(long length)
        {
            const string lettersAndDigits = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var rand = new Random();
            var chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                // 90% chance to pick a letter or digit, 20% for any ASCII character
                if (rand.NextDouble() < 0.90)
                {
                    chars[i] = lettersAndDigits[rand.Next(lettersAndDigits.Length)];
                }
                else
                {
                    chars[i] = (char)rand.Next(' ', '~'); // Printable ASCII (space to ~)
                }
            }
            return new string(chars);
        }
    }
}