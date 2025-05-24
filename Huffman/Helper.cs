using System;
using System.IO;

namespace Huffman
{
    public static class Helper
    {
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