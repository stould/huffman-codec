using System;
using System.IO;

namespace Huffman
{
    public static class Helper
    {
        public static byte[] GenerateRandomAsciiBytes(long length)
        {
            const string lettersAndDigits = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var rand = new Random();
            var bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                // 90% chance to pick a letter or digit, 10% for any ASCII character
                if (rand.NextDouble() < 0.90)
                {
                    bytes[i] = (byte)lettersAndDigits[rand.Next(lettersAndDigits.Length)];
                }
                else
                {
                    bytes[i] = (byte)rand.Next(' ', '~'); // Printable ASCII (space to ~)
                }
            }
            return bytes;
        }
    }
}