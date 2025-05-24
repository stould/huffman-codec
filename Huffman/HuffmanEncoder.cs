#nullable enable
namespace Huffman
{
    /// <summary>
    /// HuffmanEncoding is not thread-safe.
    /// </summary>
    public sealed class HuffmanEncoder
    {
        public struct HuffmanCode
        {
            public uint Bits;      // The bits of the code (up to 32 bits)
            public int BitLength;  // How many bits are valid

            public HuffmanCode(uint bits, int bitLength)
            {
                Bits = bits;
                BitLength = bitLength;
            }

            public HuffmanCode()
            {
                Bits = 0;
                BitLength = 0;
            }
        }

        private readonly record struct Node(
            byte? Symbol,
            int Frequency,
            int? Left,
            int? Right
        )
        {
            public bool IsLeaf => Symbol.HasValue;
        }

        // Struct to represent a Huffman code as bits and length
        private readonly byte[] input;
        private int rootIndex;
        private readonly Dictionary<byte, HuffmanCode> encodingTable = [];
        private readonly Dictionary<byte, int> frequency = [];
        private readonly List<Node> tree = [];
        private int paddingRight;
        private int totalBits;
        public int PaddingRight => paddingRight;

        public HuffmanEncoder(byte[] input)
        {
            if (input == null || input.Length == 0)
                throw new ArgumentException("Input byte array cannot be null or empty.", nameof(input));

            this.input = input;
            ComputeSymbolFrequency(input);
            Encode();
            GenerateCodes(rootIndex);
        }

        /// <summary>
        /// Gets the Huffman encoding code for a given symbol.
        /// </summary>
        /// <param name="symbol">The symbol to encode. This is slow.</param>
        /// <returns>The Huffman code as a string, or null if not found.</returns>
        public HuffmanCode? GetEncodingCode(byte symbol)
        {
            if (!frequency.ContainsKey(symbol))
                throw new ArgumentException($"Symbol '{symbol}' was not found in the input.", nameof(symbol));

            if (encodingTable.TryGetValue(symbol, out var code))
                return code;
            return null;
        }

        /// <summary>
        /// Encodes the specified input byte array using the Huffman encoding table.
        /// </summary>
        /// <returns>The encoded string as a sequence of bits.</returns>
        /// <exception cref="ArgumentException">Thrown if a symbol in the input is not in the encoding table.</exception>
        public byte[] EncodeBytes()
        {
            int byteLength = (totalBits + 7) / 8;
            byte[] result = new byte[byteLength];
            int bitIndex = 0;

            foreach (var value in input)
            {
                var code = encodingTable[value];
                for (int i = code.BitLength - 1; i >= 0; i--)
                {
                    if (((code.Bits >> i) & 1) == 1)
                        result[bitIndex / 8] |= (byte)(1 << (7 - (bitIndex % 8)));
                    bitIndex++;
                }
            }

            paddingRight = byteLength * 8 - totalBits;
            return result;
        }

        /// <summary>
        /// Returns a copy of the Huffman encoding table.
        /// </summary>
        public Dictionary<byte, HuffmanCode> GetEncodingTable()
        {
            return new Dictionary<byte, HuffmanCode>(encodingTable);
        }

        /// <summary>
        /// Sets the encoding table for this encoder.
        /// </summary>
        /// <param name="table">A dictionary mapping symbols to HuffmanCode.</param>
        public void SetEncodingTable(Dictionary<byte, HuffmanCode> table)
        {
            if (table == null || table.Count == 0)
                throw new ArgumentException("Encoding table cannot be null or empty.", nameof(table));
            encodingTable.Clear();
            foreach (var entry in table)
                encodingTable[entry.Key] = entry.Value;
        }

        private void ComputeSymbolFrequency(byte[] input)
        {
            foreach (byte b in input)
            {
                if (frequency.TryGetValue(b, out int value))
                    frequency[b] = value + 1;
                else
                    frequency[b] = 1;
            }
        }

        private void Encode()
        {
            // Build all nodes and prepare for bulk queue construction
            var nodeTuples = new List<(int, int)>();
            foreach (var symbol in frequency)
            {
                tree.Add(new Node(symbol.Key, symbol.Value, null, null));
                int nodeIndex = tree.Count - 1;
                nodeTuples.Add((nodeIndex, symbol.Value));
            }

            // Bulk initialize the priority queue (requires .NET 7+)
            PriorityQueue<int, int> nodeQueue = new(nodeTuples);

            if (nodeQueue.Count == 1)
            {
                rootIndex = 0;
                return;
            }

            while (nodeQueue.Count > 1)
            {
                nodeQueue.TryDequeue(out int leftIndex, out int leftValue);
                nodeQueue.TryDequeue(out int rightIndex, out int rightValue);

                var parentNode = new Node
                {
                    Frequency = leftValue + rightValue,
                    Left = leftIndex,
                    Right = rightIndex
                };
                tree.Add(parentNode);
                nodeQueue.Enqueue(tree.Count - 1, parentNode.Frequency);
            }
            rootIndex = tree.Count - 1;
        }

        private void GenerateCodes(int index)
        {
            if (index < 0 || index >= tree.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            var node = tree[index];
            if (node.IsLeaf)
            {
                encodingTable[node.Symbol!.Value] = new HuffmanCode { Bits = 0, BitLength = 1 };
                return;
            }
            PreOrder(index, 0, 0);
        }

        // PreOrder traversal to generate the encoding table as bits
        private void PreOrder(int index, uint bits, int bitLength)
        {
            var node = tree[index];
            if (node.IsLeaf)
            {
                encodingTable[node.Symbol!.Value] = new HuffmanCode { Bits = bits, BitLength = bitLength };
                totalBits += bitLength * node.Frequency;
                if (bitLength > 32)
                    throw new InvalidOperationException("Bit length exceeds 32 bits.");
                return;
            }

            PreOrder(node.Left!.Value, (bits << 1) | 0u, bitLength + 1);
            PreOrder(node.Right!.Value, (bits << 1) | 1u, bitLength + 1);
        }

        private void PrintTree()
        {
            for (int i = 0; i < tree.Count; i++)
            {
                var node = tree[i];
                string symbol = node.Symbol.HasValue ? $"'{node.Symbol}'" : "internal";
                Console.WriteLine($"Node {i}: Symbol={symbol}, Freq={node.Frequency}, Left={node.Left}, Right={node.Right}");
            }
        }
    }
}