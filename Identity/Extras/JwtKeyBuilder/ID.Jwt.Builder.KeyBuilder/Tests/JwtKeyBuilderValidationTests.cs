using System.Security.Cryptography;
using System.Text;

namespace ID.Jwt.KeyBuilder.Tests;

/// <summary>
/// Validation tests for the new BouncyCastle implementation
/// Use this to verify the new implementation works correctly before removing OpenSSL dependency
/// </summary>
public static class JwtKeyBuilderValidationTests
{
    /// <summary>
    /// Run all validation tests
    /// </summary>
    public static void RunAllTests()
    {
        Console.WriteLine("🚀 Starting JWT Key Builder Validation Tests");
        Console.WriteLine("=".PadRight(60, '='));

        try
        {
            Test1_BasicKeyGeneration();
            Test2_KeySizeVariations();
            Test3_PemStringGeneration();
            Test4_KeyCompatibilityWithDotNet();
            Test5_PemToXmlConversion();
            Test6_FileOperations();
            Test7_ErrorHandling();

            Console.WriteLine();
            Console.WriteLine("✅ ALL TESTS PASSED! The new BouncyCastle implementation is working correctly.");
            Console.WriteLine("✅ Safe to remove OpenSSL dependency.");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine($"❌ TESTS FAILED: {ex.Message}");
            Console.WriteLine("❌ Do not remove OpenSSL dependency yet.");
        }

        Console.WriteLine("=".PadRight(60, '='));
    }

    /// <summary>
    /// Test 1: Basic key generation to files
    /// </summary>
    private static void Test1_BasicKeyGeneration()
    {
        Console.WriteLine("Test 1: Basic Key Generation...");
        
        string testDir = Path.Combine(Path.GetTempPath(), $"jwt-test-{Guid.NewGuid():N}");
        
        try
        {
            // Generate keys
            var keyData = JwtPemBuilder.GenerateJwtPemKeys(testDir);
            
            // Verify files exist
            if (!File.Exists(keyData.PrivatePath))
                throw new Exception($"Private key file not created: {keyData.PrivatePath}");
            
            if (!File.Exists(keyData.PublicPath))
                throw new Exception($"Public key file not created: {keyData.PublicPath}");
            
            // Verify content looks like PEM
            var privateContent = File.ReadAllText(keyData.PrivatePath);
            var publicContent = File.ReadAllText(keyData.PublicPath);
            
            if (!privateContent.Contains("-----BEGIN RSA PRIVATE KEY-----"))
                throw new Exception("Private key doesn't appear to be in PEM format");
            
            if (!publicContent.Contains("-----BEGIN PUBLIC KEY-----"))
                throw new Exception("Public key doesn't appear to be in PEM format");
            
            Console.WriteLine("   ✅ Keys generated successfully");
            Console.WriteLine($"   ✅ Private key: {keyData.PrivatePath}");
            Console.WriteLine($"   ✅ Public key: {keyData.PublicPath}");
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(testDir))
                Directory.Delete(testDir, true);
        }
    }

    /// <summary>
    /// Test 2: Different key sizes
    /// </summary>
    private static void Test2_KeySizeVariations()
    {
        Console.WriteLine();
        Console.WriteLine("Test 2: Key Size Variations...");
        
        int[] keySizes = { 1024, 2048, 4096 };
        
        foreach (var keySize in keySizes)
        {
            Console.WriteLine($"   Testing {keySize}-bit keys...");
            
            var (privateKeyPem, publicKeyPem) = JwtPemBuilder.GenerateJwtPemStrings(keySize);
            
            // Verify keys were generated
            if (string.IsNullOrEmpty(privateKeyPem))
                throw new Exception($"Failed to generate {keySize}-bit private key");
            
            if (string.IsNullOrEmpty(publicKeyPem))
                throw new Exception($"Failed to generate {keySize}-bit public key");
            
            // Verify they can be imported by .NET
            using var rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyPem);
            
            // Check key size
            if (rsa.KeySize != keySize)
                throw new Exception($"Expected {keySize}-bit key, got {rsa.KeySize}-bit key");
            
            Console.WriteLine($"   ✅ {keySize}-bit keys generated and verified");
        }
    }

    /// <summary>
    /// Test 3: PEM string generation (in-memory)
    /// </summary>
    private static void Test3_PemStringGeneration()
    {
        Console.WriteLine();
        Console.WriteLine("Test 3: PEM String Generation...");
        
        var (privateKeyPem, publicKeyPem) = JwtPemBuilder.GenerateJwtPemStrings();
        
        if (string.IsNullOrEmpty(privateKeyPem))
            throw new Exception("Private key PEM string is empty");
        
        if (string.IsNullOrEmpty(publicKeyPem))
            throw new Exception("Public key PEM string is empty");
        
        // Verify format
        if (!privateKeyPem.Contains("-----BEGIN RSA PRIVATE KEY-----"))
            throw new Exception("Private key PEM string doesn't have correct format");
        
        if (!publicKeyPem.Contains("-----BEGIN PUBLIC KEY-----"))
            throw new Exception("Public key PEM string doesn't have correct format");
        
        Console.WriteLine("   ✅ PEM strings generated successfully");
        Console.WriteLine($"   ✅ Private key length: {privateKeyPem.Length} characters");
        Console.WriteLine($"   ✅ Public key length: {publicKeyPem.Length} characters");
    }

    /// <summary>
    /// Test 4: Compatibility with .NET RSA implementation
    /// </summary>
    private static void Test4_KeyCompatibilityWithDotNet()
    {
        Console.WriteLine();
        Console.WriteLine("Test 4: .NET RSA Compatibility...");
        
        var (privateKeyPem, publicKeyPem) = JwtPemBuilder.GenerateJwtPemStrings();
        
        // Test encryption/decryption
        using var rsaPrivate = RSA.Create();
        using var rsaPublic = RSA.Create();
        
        rsaPrivate.ImportFromPem(privateKeyPem);
        rsaPublic.ImportFromPem(publicKeyPem);
        
        // Test data
        var testMessage = "Hello, JWT World! 🚀"u8.ToArray();
        
        // Encrypt with public key
        var encrypted = rsaPublic.Encrypt(testMessage, RSAEncryptionPadding.Pkcs1);
        
        // Decrypt with private key
        var decrypted = rsaPrivate.Decrypt(encrypted, RSAEncryptionPadding.Pkcs1);
        
        var decryptedMessage = Encoding.UTF8.GetString(decrypted);
        
        if (decryptedMessage != "Hello, JWT World! 🚀")
            throw new Exception($"Encryption/decryption failed. Expected: 'Hello, JWT World! 🚀', Got: '{decryptedMessage}'");
        
        Console.WriteLine("   ✅ Encryption/decryption successful");
        
        // Test signing/verification
        var signature = rsaPrivate.SignData(testMessage, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var isValid = rsaPublic.VerifyData(testMessage, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        
        if (!isValid)
            throw new Exception("Digital signature verification failed");
        
        Console.WriteLine("   ✅ Digital signature verification successful");
    }

    /// <summary>
    /// Test 5: PEM to XML conversion
    /// </summary>
    private static void Test5_PemToXmlConversion()
    {
        Console.WriteLine();
        Console.WriteLine("Test 5: PEM to XML Conversion...");
        
        var (privateKeyPem, publicKeyPem) = JwtPemBuilder.GenerateJwtPemStrings();
        
        // Test conversion
        var privateXml = PemToXml.ConvertKeyContent(privateKeyPem);
        var publicXml = PemToXml.ConvertKeyContent(publicKeyPem);
        
        if (string.IsNullOrEmpty(privateXml))
            throw new Exception("Private key XML conversion failed");
        
        if (string.IsNullOrEmpty(publicXml))
            throw new Exception("Public key XML conversion failed");
        
        // Verify XML format
        if (!privateXml.Contains("<RSAKeyValue>"))
            throw new Exception("Private key XML doesn't have correct format");
        
        if (!publicXml.Contains("<RSAKeyValue>"))
            throw new Exception("Public key XML doesn't have correct format");
        
        // Test that XML keys work with .NET
        using var rsaFromXml = RSA.Create();
        rsaFromXml.FromXmlString(privateXml);
        
        // Quick test
        var testData = "XML Test"u8.ToArray();
        var signature = rsaFromXml.SignData(testData, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var isValid = rsaFromXml.VerifyData(testData, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        
        if (!isValid)
            throw new Exception("XML-converted key doesn't work properly");
        
        Console.WriteLine("   ✅ PEM to XML conversion successful");
        Console.WriteLine($"   ✅ Private XML length: {privateXml.Length} characters");
        Console.WriteLine($"   ✅ Public XML length: {publicXml.Length} characters");
    }

    /// <summary>
    /// Test 6: File operations
    /// </summary>
    private static void Test6_FileOperations()
    {
        Console.WriteLine();
        Console.WriteLine("Test 6: File Operations...");
        
        string testDir = Path.Combine(Path.GetTempPath(), $"jwt-file-test-{Guid.NewGuid():N}");
        
        try
        {
            // Test direct key pair generation
            var keyPair = JwtPemBuilder.GenerateRsaKeyPair(2048);
            
            Directory.CreateDirectory(testDir);
            
            var privateKeyPath = Path.Combine(testDir, "test-private.pem");
            var publicKeyPath = Path.Combine(testDir, "test-public.pem");
            
            // Save keys using BouncyCastle methods
            JwtBouncyCastleKeyBuilder.SavePrivateKeyToPem(keyPair.Private, privateKeyPath);
            JwtBouncyCastleKeyBuilder.SavePublicKeyToPem(keyPair.Public, publicKeyPath);
            
            // Verify files exist and have content
            if (!File.Exists(privateKeyPath))
                throw new Exception("Private key file was not saved");
            
            if (!File.Exists(publicKeyPath))
                throw new Exception("Public key file was not saved");
            
            var privateContent = File.ReadAllText(privateKeyPath);
            var publicContent = File.ReadAllText(publicKeyPath);
            
            if (privateContent.Length < 100)
                throw new Exception("Private key file seems too short");
            
            if (publicContent.Length < 100)
                throw new Exception("Public key file seems too short");
            
            Console.WriteLine("   ✅ Direct file save operations successful");
            Console.WriteLine($"   ✅ Private key saved: {privateKeyPath}");
            Console.WriteLine($"   ✅ Public key saved: {publicKeyPath}");
        }
        finally
        {
            if (Directory.Exists(testDir))
                Directory.Delete(testDir, true);
        }
    }

    /// <summary>
    /// Test 7: Error handling
    /// </summary>
    private static void Test7_ErrorHandling()
    {
        Console.WriteLine();
        Console.WriteLine("Test 7: Error Handling...");
        
        // Test invalid key size
        try
        {
            JwtPemBuilder.GenerateJwtPemStrings(512); // Too small
            throw new Exception("Should have thrown exception for key size too small");
        }
        catch (ArgumentException)
        {
            Console.WriteLine("   ✅ Correctly rejected key size too small (512 bits)");
        }
        
        // Test null/empty directory
        try
        {
            JwtPemBuilder.GenerateJwtPemKeys(""); // Empty directory
            throw new Exception("Should have thrown exception for empty directory");
        }
        catch (ArgumentException)
        {
            Console.WriteLine("   ✅ Correctly rejected empty directory");
        }
        
        // Test null key parameter
        try
        {
            JwtBouncyCastleKeyBuilder.SavePrivateKeyToPem(null!, "test.pem");
            throw new Exception("Should have thrown exception for null key");
        }
        catch (ArgumentNullException)
        {
            Console.WriteLine("   ✅ Correctly rejected null key parameter");
        }
        
        Console.WriteLine("   ✅ Error handling tests passed");
    }
}
