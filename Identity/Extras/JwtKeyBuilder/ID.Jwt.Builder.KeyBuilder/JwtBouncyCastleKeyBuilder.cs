using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace ID.Jwt.KeyBuilder;

/// <summary>
/// Pure BouncyCastle implementation for JWT key generation
/// Replaces OpenSSL dependency for cross-platform support
/// </summary>
public class JwtBouncyCastleKeyBuilder
{
    private const int _defaultKeySize = 2048;

    //-------------------------------//

    /// <summary>
    /// Generate Private and Public PEM Keys using BouncyCastle
    /// </summary>
    /// <param name="jwtKeyDir">Directory where to save the key files</param>
    /// <param name="keySize">RSA key size in bits (default: 2048)</param>
    /// <returns>Data containing Private and Public Key file locations</returns>
    public static JwtKeyData GenerateJwtPemKeys(string jwtKeyDir, int keySize = _defaultKeySize)
    {
        if (string.IsNullOrWhiteSpace(jwtKeyDir))
            throw new ArgumentException("Key directory cannot be null or empty", nameof(jwtKeyDir));

        if (keySize < 1024)
            throw new ArgumentException("Key size must be at least 1024 bits", nameof(keySize));

        // Generate RSA key pair
        var keyPair = GenerateRsaKeyPair(keySize);

        // Create directory if it doesn't exist
        Directory.CreateDirectory(jwtKeyDir);

        // Generate file paths
        var privateFilePath = Path.Combine(jwtKeyDir, "private.pem");
        var publicFilePath = Path.Combine(jwtKeyDir, "public.pem");

        // Save private key
        SavePrivateKeyToPem(keyPair.Private, privateFilePath);

        // Save public key
        SavePublicKeyToPem(keyPair.Public, publicFilePath);

        return new JwtKeyData(privateFilePath, publicFilePath);
    }

    //-------------------------------//    /// <summary>
    /// Generate an RSA key pair using BouncyCastle
    /// </summary>
    /// <param name="keySize">Key size in bits</param>
    /// <returns>Generated RSA key pair</returns>
    public static AsymmetricCipherKeyPair GenerateRsaKeyPair(int keySize = _defaultKeySize)
    {
        if (keySize < 1024)
            throw new ArgumentException("Key size must be at least 1024 bits", nameof(keySize));

        var keyGenerator = new RsaKeyPairGenerator();
        var parameters = new RsaKeyGenerationParameters(
            publicExponent: Org.BouncyCastle.Math.BigInteger.ValueOf(65537), // Common public exponent
            random: new SecureRandom(),
            strength: keySize,
            certainty: 25);

        keyGenerator.Init(parameters);
        return keyGenerator.GenerateKeyPair();
    }

    //-------------------------------//

    /// <summary>
    /// Save private key to PEM format file
    /// </summary>
    /// <param name="privateKey">The private key to save</param>
    /// <param name="filePath">File path where to save the key</param>
    public static void SavePrivateKeyToPem(AsymmetricKeyParameter privateKey, string filePath)
    {
        ArgumentNullException.ThrowIfNull(privateKey);

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        using var writer = new StringWriter();
        var pemWriter = new PemWriter(writer);
        pemWriter.WriteObject(privateKey);
        pemWriter.Writer.Flush();

        File.WriteAllText(filePath, writer.ToString());
    }

    //-------------------------------//

    /// <summary>
    /// Save public key to PEM format file
    /// </summary>
    /// <param name="publicKey">The public key to save</param>
    /// <param name="filePath">File path where to save the key</param>
    public static void SavePublicKeyToPem(AsymmetricKeyParameter publicKey, string filePath)
    {
        ArgumentNullException.ThrowIfNull(publicKey);

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        using var writer = new StringWriter();
        var pemWriter = new PemWriter(writer);
        pemWriter.WriteObject(publicKey);
        pemWriter.Writer.Flush();

        File.WriteAllText(filePath, writer.ToString());
    }

    //-------------------------------//

    /// <summary>
    /// Get private key as PEM string
    /// </summary>
    /// <param name="privateKey">The private key</param>
    /// <returns>PEM formatted private key string</returns>
    public static string GetPrivateKeyAsPem(AsymmetricKeyParameter privateKey)
    {
        ArgumentNullException.ThrowIfNull(privateKey);

        using var writer = new StringWriter();
        var pemWriter = new PemWriter(writer);
        pemWriter.WriteObject(privateKey);
        pemWriter.Writer.Flush();

        return writer.ToString();
    }

    //-------------------------------//

    /// <summary>
    /// Get public key as PEM string
    /// </summary>
    /// <param name="publicKey">The public key</param>
    /// <returns>PEM formatted public key string</returns>
    public static string GetPublicKeyAsPem(AsymmetricKeyParameter publicKey)
    {
        ArgumentNullException.ThrowIfNull(publicKey);

        using var writer = new StringWriter();
        var pemWriter = new PemWriter(writer);
        pemWriter.WriteObject(publicKey);
        pemWriter.Writer.Flush();

        return writer.ToString();
    }

    //-------------------------------//

    /// <summary>
    /// Generate JWT keys and return them as strings instead of files
    /// </summary>
    /// <param name="keySize">RSA key size in bits (default: 2048)</param>
    /// <returns>Tuple containing private key PEM and public key PEM as strings</returns>
    public static (string PrivateKeyPem, string PublicKeyPem) GenerateJwtPemStrings(int keySize = _defaultKeySize)
    {
        var keyPair = GenerateRsaKeyPair(keySize);
        var privateKeyPem = GetPrivateKeyAsPem(keyPair.Private);
        var publicKeyPem = GetPublicKeyAsPem(keyPair.Public);

        return (privateKeyPem, publicKeyPem);
    }

}
