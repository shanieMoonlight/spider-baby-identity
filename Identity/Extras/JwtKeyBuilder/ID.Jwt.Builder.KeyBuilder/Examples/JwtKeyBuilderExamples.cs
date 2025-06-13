using System.Security.Cryptography;

namespace ID.Jwt.KeyBuilder.Examples;

/// <summary>
/// Examples showing how to use the new BouncyCastle-based JWT key generation
/// </summary>
public static class JwtKeyBuilderExamples
{
    /// <summary>
    /// Example 1: Generate JWT keys to files (recommended approach)
    /// </summary>
    public static void GenerateKeysToFiles()
    {
        string keyDirectory = Path.Combine(Path.GetTempPath(), "jwt-keys");
        
        // Generate RSA 2048-bit keys using pure BouncyCastle (cross-platform)
        var keyData = JwtBouncyCastleKeyBuilder.GenerateJwtPemKeys(keyDirectory, 2048);
        
        Console.WriteLine($"Private key saved to: {keyData.PrivatePath}");
        Console.WriteLine($"Public key saved to: {keyData.PublicPath}");
        
        // Verify the keys work
        VerifyGeneratedKeys(keyData.PrivatePath, keyData.PublicPath);
    }

    /// <summary>
    /// Example 2: Generate JWT keys as strings (for in-memory usage)
    /// </summary>
    public static void GenerateKeysAsStrings()
    {
        // Generate 4096-bit keys for higher security
        var (privateKeyPem, publicKeyPem) = JwtBouncyCastleKeyBuilder.GenerateJwtPemStrings(4096);
        
        Console.WriteLine("Private Key PEM:");
        Console.WriteLine(privateKeyPem);
        Console.WriteLine("\nPublic Key PEM:");
        Console.WriteLine(publicKeyPem);
        
        // Convert to XML format if needed
        var privateXml = PemToXml.ConvertKeyContent(privateKeyPem);
        var publicXml = PemToXml.ConvertKeyContent(publicKeyPem);
        
        Console.WriteLine("\nPrivate Key XML:");
        Console.WriteLine(privateXml);
        Console.WriteLine("\nPublic Key XML:");
        Console.WriteLine(publicXml);
    }

    /// <summary>
    /// Example 3: Use the simplified API (wrapper around BouncyCastle)
    /// </summary>
    public static void UseSimplifiedApi()
    {
        string keyDirectory = Path.Combine(Path.GetTempPath(), "jwt-keys-simple");
        
        // This now uses BouncyCastle internally instead of OpenSSL
        var keyData = JwtPemBuilder.GenerateJwtPemKeys(keyDirectory);
        
        Console.WriteLine($"Keys generated using simplified API:");
        Console.WriteLine($"Private: {keyData.PrivatePath}");
        Console.WriteLine($"Public: {keyData.PublicPath}");
    }

    /// <summary>
    /// Example 4: Working with the raw BouncyCastle objects
    /// </summary>
    public static void WorkWithRawBouncyCastleObjects()
    {
        // Generate key pair as BouncyCastle objects
        var keyPair = JwtBouncyCastleKeyBuilder.GenerateRsaKeyPair(2048);
        
        // Convert to PEM strings
        var privateKeyPem = JwtBouncyCastleKeyBuilder.GetPrivateKeyAsPem(keyPair.Private);
        var publicKeyPem = JwtBouncyCastleKeyBuilder.GetPublicKeyAsPem(keyPair.Public);
        
        // Save to custom locations
        string customDir = Path.Combine(Path.GetTempPath(), "custom-jwt-keys");
        Directory.CreateDirectory(customDir);
        
        JwtBouncyCastleKeyBuilder.SavePrivateKeyToPem(keyPair.Private, Path.Combine(customDir, "my-private.pem"));
        JwtBouncyCastleKeyBuilder.SavePublicKeyToPem(keyPair.Public, Path.Combine(customDir, "my-public.pem"));
        
        Console.WriteLine($"Custom keys saved to: {customDir}");
    }

    /// <summary>
    /// Verify that generated keys can be used with .NET's RSA implementation
    /// </summary>
    private static void VerifyGeneratedKeys(string privateKeyPath, string publicKeyPath)
    {
        try
        {
            // Read the PEM files
            var privateKeyPem = File.ReadAllText(privateKeyPath);
            var publicKeyPem = File.ReadAllText(publicKeyPath);
            
            // Create RSA instances
            using var rsaPrivate = RSA.Create();
            using var rsaPublic = RSA.Create();
            
            // Import the keys
            rsaPrivate.ImportFromPem(privateKeyPem);
            rsaPublic.ImportFromPem(publicKeyPem);
            
            // Test encryption/decryption
            var testData = "Hello, JWT World!"u8.ToArray();
            var encrypted = rsaPublic.Encrypt(testData, RSAEncryptionPadding.Pkcs1);
            var decrypted = rsaPrivate.Decrypt(encrypted, RSAEncryptionPadding.Pkcs1);
            
            var decryptedText = System.Text.Encoding.UTF8.GetString(decrypted);
            
            if (decryptedText == "Hello, JWT World!")
            {
                Console.WriteLine("✅ Key verification successful - keys are working correctly!");
            }
            else
            {
                Console.WriteLine("❌ Key verification failed - decrypted text doesn't match");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Key verification failed: {ex.Message}");
        }
    }
}
