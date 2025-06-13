using ID.Jwt.KeyBuilder.Utility;
using System.Security.Cryptography;
using System.Xml;

namespace ID.Jwt.KeyBuilder;

/// <summary>
/// Converts XML RSA keys to PEM format
/// Opposite of PemToXml - useful for converting legacy XML keys to modern PEM format
/// </summary>
public class XmlToPem
{

    /// <summary>
    /// Check if XML content contains a private key (has private key parameters)
    /// </summary>
    private static bool IsPrivateKey(string xmlContent) =>
        xmlContent.Contains("<D>")
        && xmlContent.Contains("<P>")
        && xmlContent.Contains("<Q>");

    //-------------------------------//

    /// <summary>
    /// Save XML key as PEM files
    /// </summary>
    /// <param name="xmlKeyPath">Path to XML key file</param>
    /// <param name="pemFileNamePrefix">Prefix for PEM files (default: "key")</param>
    /// <returns>Tuple with (PrivatePemPath, PublicPemPath)</returns>
    public static (string PrivatePemPath, string PublicPemPath) Save(string xmlKeyPath, string pemFileNamePrefix = "key")
    {
        var keyDirPath = Path.GetDirectoryName(xmlKeyPath);
        var xmlContent = File.ReadAllText(xmlKeyPath);

        var (privatePem, publicPem) = ConvertKey(xmlContent);

        var privatePemPath = Path.Combine(keyDirPath!, $"{pemFileNamePrefix}_private.pem");
        var publicPemPath = Path.Combine(keyDirPath!, $"{pemFileNamePrefix}_public.pem");

        File.WriteAllText(privatePemPath, privatePem);
        File.WriteAllText(publicPemPath, publicPem);

        return (privatePemPath, publicPemPath);
    }

    //-------------------------------//

    /// <summary>
    /// Convert XML key file to PEM format
    /// </summary>
    /// <param name="xmlKeyPath">Path to XML key file</param>
    /// <returns>Tuple with (PrivatePem, PublicPem)</returns>
    public static (string PrivatePem, string PublicPem) ConvertKey(string xmlKeyPath)
    {
        string xmlContent = File.ReadAllText(xmlKeyPath).Trim();
        return ConvertKeyContent(xmlContent);
    }

    //-------------------------------//

    /// <summary>
    /// Convert XML key content to PEM format
    /// </summary>
    /// <param name="xmlContent">XML RSA key content</param>
    /// <returns>Tuple with (PrivatePem, PublicPem)</returns>
    public static (string PrivatePem, string PublicPem) ConvertKeyContent(string xmlContent)
    {
        xmlContent = xmlContent.Trim();

        // Validate XML format
        if (!xmlContent.StartsWith("<RSAKeyValue>") || !xmlContent.EndsWith("</RSAKeyValue>"))
            throw new ArgumentException("Invalid XML RSA key format. Expected <RSAKeyValue> root element.");

        // Create RSA instance and import XML
        using var rsa = RSA.Create();
        rsa.FromXmlString(xmlContent);

        // Export both private and public keys
        var privatePem = rsa.ExportRSAPrivateKeyPem();
        var publicPem = rsa.ExportRSAPublicKeyPem();

        return (privatePem, publicPem);
    }

    //-------------------------------//

    /// <summary>
    /// Convert XML private key to PEM private key
    /// </summary>
    /// <param name="xmlContent">XML RSA private key content</param>
    /// <returns>PEM private key string</returns>
    public static string ConvertPrivateKey(string xmlContent)
    {
        xmlContent = xmlContent.Trim();

        if (!IsPrivateKey(xmlContent))
            throw new ArgumentException("XML content does not contain a private key (missing private key parameters).");

        using var rsa = RSA.Create();
        rsa.FromXmlString(xmlContent);
        return rsa.ExportRSAPrivateKeyPem();
    }

    //-------------------------------//

    /// <summary>
    /// Convert XML public key to PEM public key
    /// </summary>
    /// <param name="xmlContent">XML RSA key content (can be private or public)</param>
    /// <returns>PEM public key string</returns>
    public static string ConvertPublicKey(string xmlContent)
    {
        xmlContent = xmlContent.Trim();

        using var rsa = RSA.Create();
        rsa.FromXmlString(xmlContent);
        return rsa.ExportRSAPublicKeyPem();
    }

    //-------------------------------//

    /// <summary>
    /// Convert XML key to PKCS#8 format (more standard)
    /// </summary>
    /// <param name="xmlContent">XML RSA key content</param>
    /// <returns>Tuple with (PrivatePkcs8Pem, PublicPem)</returns>
    public static (string PrivatePkcs8Pem, string PublicPem) ConvertKeyToPkcs8(string xmlContent)
    {
        xmlContent = xmlContent.Trim();

        using var rsa = RSA.Create();
        rsa.FromXmlString(xmlContent);

        // Export in PKCS#8 format (more standard than RSA format)
        var privatePkcs8Pem = rsa.ExportPkcs8PrivateKeyPem();
        var publicPem = rsa.ExportSubjectPublicKeyInfoPem();

        return (privatePkcs8Pem, publicPem);
    }

    //-------------------------------//

    /// <summary>
    /// Validate that XML to PEM conversion works correctly
    /// </summary>
    /// <param name="xmlContent">XML RSA key content</param>
    /// <returns>True if conversion is valid (round-trip test passes)</returns>
    public static bool ValidateConversion(string xmlContent)
    {
        try
        {
            var (privatePem, publicPem) = ConvertKeyContent(xmlContent);

            // Test round-trip conversion
            using var originalRsa = RSA.Create();
            originalRsa.FromXmlString(xmlContent);

            using var convertedRsa = RSA.Create();
            convertedRsa.ImportFromPem(privatePem);

            // Compare key parameters
            var originalParams = originalRsa.ExportParameters(true);
            var convertedParams = convertedRsa.ExportParameters(true);

            return CompareRsaParameters(originalParams, convertedParams);
        }
        catch
        {
            return false;
        }
    }

    //-------------------------------//

    /// <summary>
    /// Compare RSA parameters for equality
    /// </summary>
    private static bool CompareRsaParameters(RSAParameters params1, RSAParameters params2)
    {
        return CompareByteArrays(params1.Modulus, params2.Modulus) &&
               CompareByteArrays(params1.Exponent, params2.Exponent) &&
               CompareByteArrays(params1.D, params2.D) &&
               CompareByteArrays(params1.P, params2.P) &&
               CompareByteArrays(params1.Q, params2.Q) &&
               CompareByteArrays(params1.DP, params2.DP) &&
               CompareByteArrays(params1.DQ, params2.DQ) &&
               CompareByteArrays(params1.InverseQ, params2.InverseQ);
    }

    //-------------------------------//

    /// <summary>
    /// Compare byte arrays for equality (handles null values)
    /// </summary>
    private static bool CompareByteArrays(byte[]? array1, byte[]? array2)
    {
        if (array1 == null && array2 == null) return true;
        if (array1 == null || array2 == null) return false;
        if (array1.Length != array2.Length) return false;

        for (int i = 0; i < array1.Length; i++)
        {
            if (array1[i] != array2[i]) return false;
        }

        return true;
    }

    //-------------------------------//

    /// <summary>
    /// Get information about the XML key
    /// </summary>
    /// <param name="xmlContent">XML RSA key content</param>
    /// <returns>Key information string</returns>
    public static string GetKeyInfo(string xmlContent)
    {
        try
        {
            using var rsa = RSA.Create();
            rsa.FromXmlString(xmlContent);

            var hasPrivateKey = IsPrivateKey(xmlContent);
            var keySize = rsa.KeySize;

            return $"RSA Key Information:\n" +
                   $"  Type: {(hasPrivateKey ? "Private Key" : "Public Key Only")}\n" +
                   $"  Size: {keySize} bits\n" +
                   $"  Format: XML (Legacy)\n" +
                   $"  Can convert to: PEM (Modern)";
        }
        catch (Exception ex)
        {
            return $"Error reading XML key: {ex.Message}";
        }
    }

}//Cls
