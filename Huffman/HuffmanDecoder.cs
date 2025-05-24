using System;
using System.Collections.Generic;
using System.Linq;

namespace Huffman
{
    public static class HuffmanDecoder
    {
        private class DecodeNode
        {
            public byte? Symbol;
            public DecodeNode? Zero;
            public DecodeNode? One;
        }

        private static DecodeNode BuildDecodingTree(Dictionary<byte, string> encodingTable)
        {
            var root = new DecodeNode();
            foreach (var symbolEntry in encodingTable)
            {
                var node = root;
                foreach (char bit in symbolEntry.Value)
                {
                    if (bit == '0')
                    {
                        node.Zero ??= new DecodeNode();
                        node = node.Zero;
                    }
                    else
                    {
                        node.One ??= new DecodeNode();
                        node = node.One;
                    }
                }
                node.Symbol = symbolEntry.Key;
            }
            return root;
        }

        public static byte[] Decode(byte[] byteArray, Dictionary<byte, string> encodingTable, int paddingRight)
        {
            // Convert byte array to bit string
            var bitString = string.Concat(byteArray.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
            if (paddingRight > 0 && bitString.Length > paddingRight)
                bitString = bitString.Substring(0, bitString.Length - paddingRight);

            var root = BuildDecodingTree(encodingTable);

            var decodedBytes = new List<byte>();
            var node = root;
            foreach (char bit in bitString)
            {
                node = (bit == '0') ? node.Zero : node.One;
                if (node == null)
                    throw new InvalidOperationException("Invalid bit sequence for the provided encoding table.");
                if (node.Symbol.HasValue)
                {
                    decodedBytes.Add(node.Symbol.Value);
                    node = root;
                }
            }
            return decodedBytes.ToArray();
        }
    }
}