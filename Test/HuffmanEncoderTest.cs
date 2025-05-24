using System;
using System.Collections.Generic;
using Xunit;

namespace Huffman
{
    public class HuffmanEncoderTest
    {
        [Fact]
        public void Encode_AlternatingPattern_NoPadding()
        {
            // Pattern: 10011001 (X=0, Y=10, Z=11, "BAABBAAB", no padding)
            var table = new Dictionary<byte, HuffmanEncoder.HuffmanCode>
            {
                [(byte)'A'] = new HuffmanEncoder.HuffmanCode { Bits = 0b0, BitLength = 1 },
                [(byte)'B'] = new HuffmanEncoder.HuffmanCode { Bits = 0b1, BitLength = 1 }
            };

            byte[] input = { (byte)'B', (byte)'A', (byte)'A', (byte)'B', (byte)'B', (byte)'A', (byte)'A', (byte)'B'};
            var expectedEncoded = new byte[] { 0b10011001 };
            int expectedPadding = 0;

            var encoder = new HuffmanEncoder(input);
            encoder.SetEncodingTable(table);
            var encodedBytes = encoder.EncodeBytes();

            Assert.Equal(expectedEncoded, encodedBytes);
            Assert.Equal(expectedPadding, encoder.PaddingRight);
        }

        [Fact]
        public void Encode_ComplexTable_WithPadding()
        {
            // Pattern: 01011___ (X=0, Y=10, Z=11, "XYZ", 3 bits padding)
            var table = new Dictionary<byte, HuffmanEncoder.HuffmanCode>
            {
                [(byte)'X'] = new HuffmanEncoder.HuffmanCode(0b0, 1),
                [(byte)'Y'] = new HuffmanEncoder.HuffmanCode(0b10, 2),
                [(byte)'Z'] = new HuffmanEncoder.HuffmanCode(0b11, 2)
            };

            byte[] input = { (byte)'X', (byte)'Y', (byte)'Z' };
            var expectedEncoded = new byte[] { 0b01011000 };
            int expectedPadding = 3;

            var encoder = new HuffmanEncoder(input);
            encoder.SetEncodingTable(table);
            var encodedBytes = encoder.EncodeBytes();

            Assert.Equal(expectedEncoded, encodedBytes);
            Assert.Equal(expectedPadding, encoder.PaddingRight);
        }
    }
}