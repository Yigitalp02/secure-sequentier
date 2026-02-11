using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SignerApp
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage: SignerApp <input_file> <output_directory>");
                    return 1;
                }

                string inputFile = args[0];
                string outputDir = args[1];

                if (!File.Exists(inputFile))
                {
                    Console.WriteLine($"Error: Input file '{inputFile}' does not exist.");
                    return 1;
                }

                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                // Generate SHA-256 hash of the file
                string fileHash = GenerateFileHash(inputFile);
                string fileName = Path.GetFileNameWithoutExtension(inputFile);
                string fileExtension = Path.GetExtension(inputFile);

                // Create signature data
                var signatureData = new
                {
                    OriginalFile = Path.GetFileName(inputFile),
                    FileHash = fileHash,
                    HashAlgorithm = "SHA-256",
                    SignedAt = DateTime.UtcNow,
                    SignerApp = "SecureSolution2 SignerApp v1.0",
                    FileSize = new FileInfo(inputFile).Length
                };

                // Create output files
                string signatureFile = Path.Combine(outputDir, $"{fileName}.signature");
                string hashFile = Path.Combine(outputDir, $"{fileName}.hash");
                string signedFile = Path.Combine(outputDir, $"{fileName}_signed{fileExtension}");

                // Write signature file (JSON format)
                string signatureJson = JsonSerializer.Serialize(signatureData, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                File.WriteAllText(signatureFile, signatureJson);

                // Write hash file (plain text)
                File.WriteAllText(hashFile, fileHash);

                // Copy original file to output directory with "_signed" suffix
                File.Copy(inputFile, signedFile, true);

                // Create a summary report
                string reportFile = Path.Combine(outputDir, $"{fileName}_report.txt");
                var report = new StringBuilder();
                report.AppendLine("=== FILE SIGNATURE REPORT ===");
                report.AppendLine($"Original File: {Path.GetFileName(inputFile)}");
                report.AppendLine($"File Hash (SHA-256): {fileHash}");
                report.AppendLine($"Signed At: {signatureData.SignedAt:yyyy-MM-dd HH:mm:ss UTC}");
                report.AppendLine($"File Size: {signatureData.FileSize} bytes");
                report.AppendLine($"Output Directory: {outputDir}");
                report.AppendLine();
                report.AppendLine("Generated Files:");
                report.AppendLine($"- {Path.GetFileName(signatureFile)} (Signature metadata)");
                report.AppendLine($"- {Path.GetFileName(hashFile)} (File hash)");
                report.AppendLine($"- {Path.GetFileName(signedFile)} (Signed copy)");
                report.AppendLine($"- {Path.GetFileName(reportFile)} (This report)");

                File.WriteAllText(reportFile, report.ToString());

                Console.WriteLine("=== SIGNATURE SUCCESSFUL ===");
                Console.WriteLine($"File: {Path.GetFileName(inputFile)}");
                Console.WriteLine($"Hash: {fileHash}");
                Console.WriteLine($"Output: {outputDir}");
                Console.WriteLine("Files generated successfully!");

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        }

        static string GenerateFileHash(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = sha256.ComputeHash(stream);
                    return Convert.ToHexString(hashBytes).ToLower();
                }
            }
        }
    }
}

