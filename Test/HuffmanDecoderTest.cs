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
            // Pattern: 0110____ (A=0, B=1, "ABBA", 4 bits padding)
            var table = new Dictionary<byte, string>
            {
                [(byte)'A'] = "0",
                [(byte)'B'] = "1"
            };
            byte[] encoded = { 0b01100000 };
            var result = HuffmanDecoder.Decode(encoded, table, 4);
            Assert.Equal(new byte[] { (byte)'A', (byte)'B', (byte)'B', (byte)'A' }, result);
        }

        [Fact]
        public void Decode_WithPadding()
        {
            // Pattern: 010110__ (X=0, Y=10, Z=11, "XYZ", 3 bits padding)
            var table = new Dictionary<byte, string>
            {
                [(byte)'X'] = "0",
                [(byte)'Y'] = "10",
                [(byte)'Z'] = "11"
            };
            byte[] encoded = { 0b01011000 };
            var result = HuffmanDecoder.Decode(encoded, table, 3);
            Assert.Equal(new byte[] { (byte)'X', (byte)'Y', (byte)'Z' }, result);
        }

        [Fact]
        public void Decode_EmptyInput_ReturnsEmpty()
        {
            // Pattern: (empty)
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
            // Pattern: 00000000 (no valid codes)
            var table = new Dictionary<byte, string>();
            byte[] encoded = { 0b00000000 };
            Assert.Throws<InvalidOperationException>(() =>
                HuffmanDecoder.Decode(encoded, table, 0)
            );
        }

        [Fact]
        public void Decode_AllPadding_ReturnsAB()
        {
            // Pattern: 010_____
            var table = new Dictionary<byte, string>
            {
                [(byte)'A'] = "0",
                [(byte)'B'] = "10"
            };
            byte[] encoded = { 0b01000000 }; // A B -> 0 10 (padding: 5 bits)
            var result = HuffmanDecoder.Decode(encoded, table, 5);
            Assert.Equal(new byte[] { (byte)'A', (byte)'B' }, result);
        }

        [Fact]
        public void Decode_ComplexTable()
        {
            // Pattern: 00011011 (A=00, B=01, C=10, D=11, "ABCD", no padding)
            var table = new Dictionary<byte, string>
            {
                [(byte)'A'] = "00",
                [(byte)'B'] = "01",
                [(byte)'C'] = "10",
                [(byte)'D'] = "11"
            };
            byte[] encoded = { 0b00011011 };
            var result = HuffmanDecoder.Decode(encoded, table, 0);
            Assert.Equal(new byte[] { (byte)'A', (byte)'B', (byte)'C', (byte)'D' }, result);
        }

        [Fact]
        public void Decode_MultiByteInput_Works()
        {
            // Pattern: 01011100 1111____ (A=0, B=10, C=11, "ABCBACC", 4 bits padding)
            var table = new Dictionary<byte, string>
            {
                [(byte)'A'] = "0",
                [(byte)'B'] = "10",
                [(byte)'C'] = "11"
            };
            byte[] encoded = { 0b01011100, 0b11110000 };
            var result = HuffmanDecoder.Decode(encoded, table, 4);
            Assert.Equal(new byte[] { (byte)'A', (byte)'B', (byte)'C', (byte)'B', (byte)'A', (byte)'C', (byte)'C' }, result);
        }
    }
}