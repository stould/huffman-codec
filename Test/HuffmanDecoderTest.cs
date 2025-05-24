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
            // Pattern: 10011001 (A=0, B=1, "BAABBAAB", no padding)
            var table = new Dictionary<byte, HuffmanEncoder.HuffmanCode>
            {
                [(byte)'A'] = new HuffmanEncoder.HuffmanCode { Bits = 0b0, BitLength = 1 },
                [(byte)'B'] = new HuffmanEncoder.HuffmanCode { Bits = 0b1, BitLength = 1 }
            };
            byte[] encoded = { 0b10011001 };
            var result = HuffmanDecoder.Decode(encoded, table, 0);
            Assert.Equal(new byte[] { (byte)'B', (byte)'A', (byte)'A', (byte)'B', (byte)'B', (byte)'A', (byte)'A', (byte)'B'}, result);
        }

        [Fact]
        public void Decode_WithPadding()
        {
            // Pattern: 010110__ (X=0, Y=10, Z=11, "XYZ", 3 bits padding)
            var table = new Dictionary<byte, HuffmanEncoder.HuffmanCode>
            {
                [(byte)'X'] = new HuffmanEncoder.HuffmanCode { Bits = 0b0, BitLength = 1 },
                [(byte)'Y'] = new HuffmanEncoder.HuffmanCode { Bits = 0b10, BitLength = 2 },
                [(byte)'Z'] = new HuffmanEncoder.HuffmanCode { Bits = 0b11, BitLength = 2 }
            };
            byte[] encoded = { 0b01011000 };
            var result = HuffmanDecoder.Decode(encoded, table, 3);
            Assert.Equal(new byte[] { (byte)'X', (byte)'Y', (byte)'Z' }, result);
        }

        [Fact]
        public void Decode_EmptyInput_ReturnsEmpty()
        {
            // Pattern: (empty)
            var table = new Dictionary<byte, HuffmanEncoder.HuffmanCode>
            {
                [(byte)'A'] = new HuffmanEncoder.HuffmanCode { Bits = 0b0, BitLength = 1 }
            };
            byte[] encoded = Array.Empty<byte>();
            var result = HuffmanDecoder.Decode(encoded, table, 0);
            Assert.Empty(result);
        }

        [Fact]
        public void Decode_EmptyTable_Throws()
        {
            // Pattern: 00000000 (no valid codes)
            var table = new Dictionary<byte, HuffmanEncoder.HuffmanCode>();
            byte[] encoded = { 0b00000000 };
            Assert.Throws<InvalidOperationException>(() =>
                HuffmanDecoder.Decode(encoded, table, 0)
            );
        }

        [Fact]
        public void Decode_AllPadding_ReturnsAB()
        {
            // Pattern: 010_____ (A=0, B=10, "AB", 5 bits padding)
            var table = new Dictionary<byte, HuffmanEncoder.HuffmanCode>
            {
                [(byte)'A'] = new HuffmanEncoder.HuffmanCode { Bits = 0b0, BitLength = 1 },
                [(byte)'B'] = new HuffmanEncoder.HuffmanCode { Bits = 0b10, BitLength = 2 }
            };
            byte[] encoded = { 0b01000000 }; // A B -> 0 10 (padding: 5 bits)
            var result = HuffmanDecoder.Decode(encoded, table, 5);
            Assert.Equal(new byte[] { (byte)'A', (byte)'B' }, result);
        }

        [Fact]
        public void Decode_ComplexTable()
        {
            // Pattern: 00011011 (A=00, B=01, C=10, D=11, "ABCD", no padding)
            var table = new Dictionary<byte, HuffmanEncoder.HuffmanCode>
            {
                [(byte)'A'] = new HuffmanEncoder.HuffmanCode { Bits = 0b00, BitLength = 2 },
                [(byte)'B'] = new HuffmanEncoder.HuffmanCode { Bits = 0b01, BitLength = 2 },
                [(byte)'C'] = new HuffmanEncoder.HuffmanCode { Bits = 0b10, BitLength = 2 },
                [(byte)'D'] = new HuffmanEncoder.HuffmanCode { Bits = 0b11, BitLength = 2 }
            };
            byte[] encoded = { 0b00011011 };
            var result = HuffmanDecoder.Decode(encoded, table, 0);
            Assert.Equal(new byte[] { (byte)'A', (byte)'B', (byte)'C', (byte)'D' }, result);
        }

        [Fact]
        public void Decode_MultiByteInput_Works()
        {
            // Pattern: 01011100 11110000 (A=0, B=10, C=11, "ABCBACC", 4 bits padding)
            var table = new Dictionary<byte, HuffmanEncoder.HuffmanCode>
            {
                [(byte)'A'] = new HuffmanEncoder.HuffmanCode { Bits = 0b0, BitLength = 1 },
                [(byte)'B'] = new HuffmanEncoder.HuffmanCode { Bits = 0b10, BitLength = 2 },
                [(byte)'C'] = new HuffmanEncoder.HuffmanCode { Bits = 0b11, BitLength = 2 }
            };
            byte[] encoded = { 0b01011100, 0b11110000 };
            var result = HuffmanDecoder.Decode(encoded, table, 4);
            Assert.Equal(new byte[] { (byte)'A', (byte)'B', (byte)'C', (byte)'B', (byte)'A', (byte)'C', (byte)'C' }, result);
        }
    }
}