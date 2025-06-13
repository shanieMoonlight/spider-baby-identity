using ID.Jwt.KeyBuilder.Utility;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Xml;


namespace ID.Jwt.KeyBuilder;
public class PemToXml
{

    //-------------------------------//

    private static bool IsPublicKey(string pemContent) =>
        pemContent.Trim().StartsWith(JwtKeyBuilderConstants.PUBLIC_PEM_BEGIN)
        || pemContent.Trim().StartsWith(JwtKeyBuilderConstants.PUBLIC_PEM_RSA_BEGIN);

    //-------------------------------//

    public static string Save(string pemKeyPath, string xmlFileName = "key.xml")
    {
        var keyDirPath = Path.GetDirectoryName(pemKeyPath);
        var xmlString = ConvertKeyFromPath(pemKeyPath);

        XmlDocument xmlDoc = new();
        xmlDoc.LoadXml(xmlString);

        string xmlPath = Path.Combine(keyDirPath!, xmlFileName);
        xmlDoc.Save(xmlPath);

        return xmlPath;
    }

    //-------------------------------//

    public static string ConvertKeyFromPath(string pemKeyPath)
    {
        string pemContent = File.ReadAllText(pemKeyPath).Trim();

        if (IsPublicKey(pemContent))
            return ConvertPublicKey(pemContent);
        else
            return ConvertPrivateKey(pemContent);
    }


    //-------------------------------//

    public static string ConvertKeyContent(string pemContent) => 
        IsPublicKey(pemContent) 
        ? ConvertPublicKey(pemContent) 
        : ConvertPrivateKey(pemContent);

    //-------------------------------//

    public static string ConvertPrivateKey(string pemContent)
    {
        pemContent = pemContent.Trim();

        // Create an RSA instance and import the PEM key
        RSA rsaKey = RSA.Create();
        rsaKey.ImportFromPem(pemContent);

        return rsaKey.ToXmlString(true);
    }


    //-------------------------------//    
    // 
    public static string ConvertPublicKey(string pemContent)
    {
        pemContent = pemContent.Trim();
        
        // Use .NET's native PEM support (cross-platform)
        RSA rsaKey = RSA.Create();
        rsaKey.ImportFromPem(pemContent);
        
        return rsaKey.ToXmlString(false);
    }


}//Cls
