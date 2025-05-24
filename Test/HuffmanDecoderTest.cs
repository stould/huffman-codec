using System;
using System.Collections.Generic;
using Xunit;

namespace Huffman
{
    public class HuffmanDecoderTest
    {
        [Fact]
        public void Decode_SimpleTable_NoPadding()
        {
            // Encoding: A=0, B=1
            var table = new Dictionary<byte, string>
            {
                [(byte)'A'] = "0",
                [(byte)'B'] = "1"
            };
            // Encoded: "ABBA" -> 0 1 1 0 -> 0110 (pad to 8 bits: 01100000)
            byte[] encoded = { 0b01100000 };
            var result = HuffmanDecoder.Decode(encoded, table, 4);
            Assert.Equal(new byte[] { (byte)'A', (byte)'B', (byte)'B', (byte)'A' }, result);
        }

        [Fact]
        public void Decode_WithPadding()
        {
            // Encoding: X=0, Y=10, Z=11
            var table = new Dictionary<byte, string>
            {
                [(byte)'X'] = "0",
                [(byte)'Y'] = "10",
                [(byte)'Z'] = "11"
            };
            // X Y Z -> 0 10 11 (padding: 3 bits)
            byte[] encoded = { 0b01011000 };
            var result = HuffmanDecoder.Decode(encoded, table, 3);
            Assert.Equal(new byte[] { (byte)'X', (byte)'Y', (byte)'Z' }, result);
        }

        [Fact]
        public void Decode_EmptyInput_ReturnsEmpty()
        {
            var table = new Dictionary<byte, string>
            {
                [(byte)'A'] = "0"
            };
            byte[] encoded = Array.Empty<byte>();
            var result = HuffmanDecoder.Decode(encoded, table, 0);
            Assert.Empty(result);
        }

        [Fact]
        public void Decode_EmptyTable_Throws()
        {
            var table = new Dictionary<byte, string>();
            byte[] encoded = { 0b00000000 };
            Assert.Throws<InvalidOperationException>(() =>
                HuffmanDecoder.Decode(encoded, table, 0)
            );
        }

        [Fact]
        public void Decode_AllPadding_ReturnsAB()
        {
            var table = new Dictionary<byte, string>
            {
                [(byte)'A'] = "0",
                [(byte)'B'] = "10"
            };
            byte[] encoded = [0b01000000]; // A B -> 0 10 (padding: 5 bits)
            var result = HuffmanDecoder.Decode(encoded, table, 5);
            Assert.Equal(new byte[] { (byte)'A', (byte)'B' }, result);
        }

        [Fact]
        public void Decode_ComplexTable()
        {
            // Encoding: A=00, B=01, C=10, D=11
            var table = new Dictionary<byte, string>
            {
                [(byte)'A'] = "00",
                [(byte)'B'] = "01",
                [(byte)'C'] = "10",
                [(byte)'D'] = "11"
            };
            // Encoded: "ABCD" -> 00 01 10 11 -> 00011011 (no padding)
            byte[] encoded = { 0b00011011 };
            var result = HuffmanDecoder.Decode(encoded, table, 0);
            Assert.Equal(new byte[] { (byte)'A', (byte)'B', (byte)'C', (byte)'D' }, result);
        }
    }
}