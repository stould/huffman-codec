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

        // Now expects Dictionary<byte, HuffmanEncoder.HuffmanCode>
        private static DecodeNode BuildDecodingTree(Dictionary<byte, HuffmanEncoder.HuffmanCode> encodingTable)
        {
            var root = new DecodeNode();
            foreach (var symbolEntry in encodingTable)
            {
                var node = root;
                var code = symbolEntry.Value;
                for (int i = code.BitLength - 1; i >= 0; i--)
                {
                    bool bitIsOne = ((code.Bits >> i) & 1) == 1;
                    if (!bitIsOne)
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

        public static byte[] Decode(byte[] byteArray, Dictionary<byte, HuffmanEncoder.HuffmanCode> encodingTable, int paddingRight)
        {
            var root = BuildDecodingTree(encodingTable);

            var decodedBytes = new List<byte>();
            var node = root;
            int totalBits = byteArray.Length * 8 - paddingRight;

            for (int i = 0; i < totalBits; i++)
            {
                int bytePos = i / 8;
                int bitPos = 7 - (i % 8);
                bool bitIsOne = ((byteArray[bytePos] >> bitPos) & 1) == 1;

                node = bitIsOne ? node.One : node.Zero;
                if (node == null)
                    throw new InvalidOperationException("Invalid bit sequence for the provided encoding table.");
                if (node.Symbol.HasValue)
                {
                    decodedBytes.Add(node.Symbol.Value);
                    node = root;
                }
            }
            return [.. decodedBytes];
        }
    }
}