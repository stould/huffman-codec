using System.Collections.Concurrent;
using System.Diagnostics;

namespace Huffman
{
    public static class ParallelHuffmanPerftest
    {
        public static void Run()
        {
            var stopwatch = Stopwatch.StartNew();

            var results = new ConcurrentBag<(int inputLen, int avgCompressedBits, int avgOriginalBits, double avgRatio, double avgMs)>();
            int total = 10001 - 10;
            int completed = 0;

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount * 2 // Try 2x logical cores
            };

            Parallel.For(10, 10001, options, inputLen =>
            {
                var (avgOriginalBits, avgCompressedBits, avgRatio, avgMs) = RunRandomHuffmanTests(inputLen);
                results.Add((inputLen, avgCompressedBits, avgOriginalBits, avgRatio, avgMs));

                int done = Interlocked.Increment(ref completed);
                if (done % 100 == 0 || done == total)
                {
                    Console.WriteLine($"Progress: {done}/{total} ({(done * 100.0 / total):F2}%)");
                }
            });

            using var writer = new StreamWriter("huffman_results.csv");
            writer.WriteLine("InputLength,AvgOriginalBits,AvgCompressedBits,AvgCompressionRatio,AvgCompressionTimeMs");
            foreach (var result in results.OrderBy(r => r.inputLen))
            {
                writer.WriteLine($"{result.inputLen},{result.avgOriginalBits},{result.avgCompressedBits},{result.avgRatio:F4},{result.avgMs:F4}");
            }

            Console.WriteLine($"Total elapsed time: {stopwatch.Elapsed}");
        }

        private static (int avgOriginalBits, int avgCompressedBits, double avgRatio, double avgMs) RunRandomHuffmanTests(int inputLen, int numTests = 500)
        {
            // Thread-local Random for thread safety and speed
            var rand = new Random(Guid.NewGuid().GetHashCode());
            double totalOriginalBits = 0;
            double totalCompressedBits = 0;
            double totalRatio = 0;
            double totalMilliseconds = 0;

            for (int i = 0; i < numTests; i++)
            {
                // Generate random byte array
                byte[] input = new byte[inputLen];
                rand.NextBytes(input);

                var sw = Stopwatch.StartNew();
                HuffmanEncoding huffman = new(input);
                string encodedString = huffman.EncodeBytes();
                sw.Stop();

                int originalBits = input.Length * 8;
                int compressedBits = encodedString.Length; // Each bit is a char in the encoded string
                double compressionRatio = (double)compressedBits / originalBits;

                totalOriginalBits += originalBits;
                totalCompressedBits += compressedBits;
                totalRatio += compressionRatio;
                totalMilliseconds += sw.Elapsed.TotalMilliseconds;
            }

            int avgOriginalBits = (int)(totalOriginalBits / numTests);
            int avgCompressedBits = (int)(totalCompressedBits / numTests);
            double avgRatio = totalRatio / numTests;
            double avgMs = totalMilliseconds / numTests;

            return (avgOriginalBits, avgCompressedBits, avgRatio, avgMs);
        }
    }
}