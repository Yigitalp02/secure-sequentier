using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AdvancedSignerApp
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage: AdvancedSignerApp <input_file> <output_directory>");
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

                // Generate multiple hash types for enhanced security
                string sha256Hash = GenerateFileHash(inputFile, "SHA256");
                string sha512Hash = GenerateFileHash(inputFile, "SHA512");
                string md5Hash = GenerateFileHash(inputFile, "MD5");

                string fileName = Path.GetFileNameWithoutExtension(inputFile);
                string fileExtension = Path.GetExtension(inputFile);

                // Create enhanced signature data
                var signatureData = new
                {
                    OriginalFile = Path.GetFileName(inputFile),
                    FileHashes = new
                    {
                        SHA256 = sha256Hash,
                        SHA512 = sha512Hash,
                        MD5 = md5Hash
                    },
                    SignedAt = DateTime.UtcNow,
                    SignerApp = "SecureSolution2 AdvancedSignerApp v2.0",
                    FileSize = new FileInfo(inputFile).Length,
                    FileExtension = fileExtension,
                    SecurityLevel = "Enhanced Multi-Hash"
                };

                // Create output files
                string signatureFile = Path.Combine(outputDir, $"{fileName}_advanced.signature");
                string hashFile = Path.Combine(outputDir, $"{fileName}_advanced.hash");
                string signedFile = Path.Combine(outputDir, $"{fileName}_advanced_signed{fileExtension}");

                // Write signature file (JSON format)
                string signatureJson = JsonSerializer.Serialize(signatureData, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                File.WriteAllText(signatureFile, signatureJson);

                // Write hash file with all hash types
                var hashContent = new StringBuilder();
                hashContent.AppendLine($"SHA-256: {sha256Hash}");
                hashContent.AppendLine($"SHA-512: {sha512Hash}");
                hashContent.AppendLine($"MD5: {md5Hash}");
                File.WriteAllText(hashFile, hashContent.ToString());

                // Copy original file to output directory with "_advanced_signed" suffix
                File.Copy(inputFile, signedFile, true);

                // Create a detailed report
                string reportFile = Path.Combine(outputDir, $"{fileName}_advanced_report.txt");
                var report = new StringBuilder();
                report.AppendLine("=== ADVANCED FILE SIGNATURE REPORT ===");
                report.AppendLine($"Original File: {Path.GetFileName(inputFile)}");
                report.AppendLine($"File Size: {signatureData.FileSize} bytes");
                report.AppendLine($"File Extension: {fileExtension}");
                report.AppendLine($"Signed At: {signatureData.SignedAt:yyyy-MM-dd HH:mm:ss UTC}");
                report.AppendLine($"Security Level: {signatureData.SecurityLevel}");
                report.AppendLine();
                report.AppendLine("File Hashes:");
                report.AppendLine($"  SHA-256: {sha256Hash}");
                report.AppendLine($"  SHA-512: {sha512Hash}");
                report.AppendLine($"  MD5: {md5Hash}");
                report.AppendLine();
                report.AppendLine($"Output Directory: {outputDir}");
                report.AppendLine();
                report.AppendLine("Generated Files:");
                report.AppendLine($"- {Path.GetFileName(signatureFile)} (Advanced signature metadata)");
                report.AppendLine($"- {Path.GetFileName(hashFile)} (Multi-hash file)");
                report.AppendLine($"- {Path.GetFileName(signedFile)} (Advanced signed copy)");
                report.AppendLine($"- {Path.GetFileName(reportFile)} (This detailed report)");

                File.WriteAllText(reportFile, report.ToString());

                Console.WriteLine("=== ADVANCED SIGNATURE SUCCESSFUL ===");
                Console.WriteLine($"File: {Path.GetFileName(inputFile)}");
                Console.WriteLine($"SHA-256: {sha256Hash}");
                Console.WriteLine($"SHA-512: {sha512Hash}");
                Console.WriteLine($"MD5: {md5Hash}");
                Console.WriteLine($"Output: {outputDir}");
                Console.WriteLine("Advanced signature files generated successfully!");

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        }

        static string GenerateFileHash(string filePath, string algorithm)
        {
            HashAlgorithm hashAlgorithm = algorithm.ToUpper() switch
            {
                "SHA256" => SHA256.Create(),
                "SHA512" => SHA512.Create(),
                "MD5" => MD5.Create(),
                _ => throw new ArgumentException($"Unsupported hash algorithm: {algorithm}")
            };

            using (hashAlgorithm)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = hashAlgorithm.ComputeHash(stream);
                    return Convert.ToHexString(hashBytes).ToLower();
                }
            }
        }
    }
}

