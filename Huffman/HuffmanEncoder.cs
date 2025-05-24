#nullable enable
namespace Huffman
{
    /// <summary>
    /// HuffmanEncoding is not thread-safe.
    /// </summary>
    public sealed class HuffmanEncoding
    {
        private readonly record struct Node(
            byte? Symbol,
            int Frequency,
            int? Left,
            int? Right
        )
        {
            public bool IsLeaf => Symbol.HasValue;
        }

        private readonly byte[] input;
        private int rootIndex;
        private readonly Dictionary<byte, string> encodingTable = [];
        private readonly Dictionary<byte, int> frequency = [];
        private readonly List<Node> tree = [];

        public HuffmanEncoding(byte[] input)
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
        public string? GetEncodingCode(byte symbol)
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
        public string EncodeBytes()
        {
            var result = new System.Text.StringBuilder();
            foreach (var value in input)
            {
                if (!encodingTable.TryGetValue(value, out var code))
                    throw new ArgumentException($"Symbol '{value}' was not found in the encoding table.", nameof(input));
                result.Append(code);
            }
            return result.ToString();
        }

        /// <summary>
        /// Returns a copy of the Huffman encoding table.
        /// </summary>
        public Dictionary<byte, string> GetEncodingTable()
        {
            return new Dictionary<byte, string>(encodingTable);
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
                encodingTable[node.Symbol!.Value] = tree.Count == 1 ? "0" : "";
                return;
            }
            PreOrder(index, new System.Text.StringBuilder());
            // PrintTree();
        }

        // PreOrder traversal to generate the encoding table
        private void PreOrder(int index, System.Text.StringBuilder prefix)
        {
            var node = tree[index];
            if (node.IsLeaf)
            {
                encodingTable[node.Symbol!.Value] = prefix.ToString();
                return;
            }

            prefix.Append('0');
            PreOrder(node.Left!.Value, prefix);
            prefix.Length--;

            prefix.Append('1');
            PreOrder(node.Right!.Value, prefix);
            prefix.Length--;
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