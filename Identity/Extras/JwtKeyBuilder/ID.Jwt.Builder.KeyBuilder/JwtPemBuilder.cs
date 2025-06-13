namespace ID.Jwt.KeyBuilder;

/// <summary>
/// JWT PEM Key Builder using pure BouncyCastle implementation
/// Cross-platform compatible with no external dependencies
/// </summary>
public class JwtPemBuilder
{

    /// <summary>
    /// Generate Private and Public PEM Keys
    /// </summary>
    /// <param name="jwtKeyDir">Directory where to save the key files</param>
    /// <param name="keySize">RSA key size in bits (default: 2048)</param>
    /// <returns>Data containing Private and Public Key file locations</returns>
    public static JwtKeyData GenerateJwtPemKeys(string jwtKeyDir, int keySize = 2048) =>
        JwtBouncyCastleKeyBuilder.GenerateJwtPemKeys(jwtKeyDir, keySize);

    //--------------------//

    /// <summary>
    /// Generate Private and Public PEM Keys as strings (in-memory)
    /// </summary>
    /// <param name="keySize">RSA key size in bits (default: 2048)</param>
    /// <returns>Tuple containing private key PEM and public key PEM as strings</returns>
    public static (string PrivateKeyPem, string PublicKeyPem) GenerateJwtPemStrings(int keySize = 2048) => 
        JwtBouncyCastleKeyBuilder.GenerateJwtPemStrings(keySize);

    //--------------------//

    /// <summary>
    /// Generate an RSA key pair using BouncyCastle
    /// </summary>
    /// <param name="keySize">Key size in bits (default: 2048)</param>
    /// <returns>Generated RSA key pair</returns>
    public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateRsaKeyPair(int keySize = 2048) =>
        JwtBouncyCastleKeyBuilder.GenerateRsaKeyPair(keySize);

}
