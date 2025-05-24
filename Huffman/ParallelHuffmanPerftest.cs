using System.Collections.Concurrent;
using System.Diagnostics;

namespace Huffman
{
    public static class ParallelHuffmanPerftest
    {
        public static void Run()
        {
            var stopwatch = Stopwatch.StartNew();

            var results = new ConcurrentBag<(int InputLength, int avgCompressedBits, double avgRatio, double avgMs)>();
            int total = 10001 - 10;
            int completed = 0;

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount * 2 // Try 2x logical cores
            };

            Parallel.For(10, 10001, options, InputLength =>
            {
                var (avgCompressedBits, avgRatio, avgMs) = RunRandomHuffmanTests(InputLength);
                results.Add((InputLength, avgCompressedBits, avgRatio, avgMs));

                int done = Interlocked.Increment(ref completed);
                if (done % 100 == 0 || done == total)
                {
                    Console.WriteLine($"Progress: {done}/{total} ({(done * 100.0 / total):F2}%)");
                }
            });

            using var writer = new StreamWriter("huffman_results.csv");
            writer.WriteLine("InputLength,AvgCompressedBits,AvgCompressionRatio,AvgCompressionTimeMs");
            foreach (var (InputLength, avgCompressedBits, avgRatio, avgMs) in results.OrderBy(r => r.InputLength))
            {
                writer.WriteLine($"{InputLength},{avgCompressedBits},{avgRatio:F4},{avgMs:F4}");
            }

            Console.WriteLine($"Total elapsed time: {stopwatch.Elapsed}");
        }

        private static (int avgCompressedBits, double avgRatio, double avgMs) RunRandomHuffmanTests(int InputLength, int numTests = 300)
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
                byte[] inputBytes = Helper.GenerateRandomAsciiBytes(InputLength);

                var sw = Stopwatch.StartNew();
                HuffmanEncoder huffman = new(inputBytes);
                byte[] encodedByteArray = huffman.EncodeBytes();
                sw.Stop();

                int originalBits = inputBytes.Length;
                int compressedBits = encodedByteArray.Length;
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

            return (avgCompressedBits, avgRatio, avgMs);
        }
    }
}