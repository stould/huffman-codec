using System.Diagnostics;
using Huffman;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Enter working directory path: ");
        string workingPath = Console.ReadLine() ?? "";
        Console.Write("Enter file name: ");
        string fileName = Console.ReadLine() ?? "";

        string inputPath = Path.Combine(workingPath, fileName);
        string outputEncodedPath = Path.Combine(workingPath, "encoded_" + fileName + ".bin");
        string outputDecodedPath = Path.Combine(workingPath, "decoded_" + fileName);

        // Read file as bytes
        byte[] inputBytes = File.ReadAllBytes(inputPath);

        Console.WriteLine($"Starting encoding...");
        var encodeTest = Stopwatch.StartNew();

        // Encode bytes using Huffman encoding
        HuffmanEncoding huffman = new(inputBytes);

        // Encode bytes to binary string
        var encodedString = huffman.EncodeBytes();

        // Convert to byte array
        (byte[] encodedByteArray, int paddingRight) = Helper.ConvertBinaryStringToByteArray(encodedString.ToString());

        // Compression ratio calculation
        long originalBits = inputBytes.Length;
        long compressedBits = encodedByteArray.Length;
        double compressionRatio = 100.0 * (compressedBits / (double)originalBits);

        // Save compressed data
        File.WriteAllBytes(outputEncodedPath, encodedByteArray);
        Console.WriteLine($"Encoding completed. Compressed data saved to {outputEncodedPath}");
        encodeTest.Stop();

        var decodeTest = Stopwatch.StartNew();

        Console.WriteLine($"Starting decoding...");
        // Decode the compressed data
        Dictionary<byte, string> encodingTable = huffman.GetEncodingTable();
        byte[] compressedByteDataFromFile = File.ReadAllBytes(outputEncodedPath);
        byte[] decodedBytes = HuffmanDecoder.Decode(compressedByteDataFromFile, encodingTable, paddingRight);
        File.WriteAllBytes(outputDecodedPath, decodedBytes);
        Console.WriteLine($"Decoding completed. Decoded data saved to {outputDecodedPath}");
        decodeTest.Stop();

        Console.WriteLine($"Encoding time: {encodeTest.ElapsedMilliseconds:F2} ms");
        Console.WriteLine($"Decoding time: {decodeTest.ElapsedMilliseconds:F2} ms");
        Console.WriteLine($"Input size: {originalBits:F2} bytes");
        Console.WriteLine($"Compressed size: {compressedBits:F2} bytes");
        Console.WriteLine($"Compression ratio: {100.0 - compressionRatio:F4}%");
    }
}