using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Xml;

namespace ID.Infrastructure.Auth.JWT.Utils;
internal static class XmlExtensions
{
    public static bool IsValidRsaPublicKeyXml(this string xml)
    {
        try
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            if (xmlDoc.DocumentElement?.Name != "RSAKeyValue")
                return false;
            var modulus = xmlDoc.DocumentElement["Modulus"];
            var exponent = xmlDoc.DocumentElement["Exponent"];
            return modulus != null && exponent != null;
        }
        catch
        {
            return false;
        }
    }

    //-----------------------------//

    public static bool IsValidRsaPrivateKeyXml(this string xml)
    {
        try
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            if (xmlDoc.DocumentElement?.Name != "RSAKeyValue")
                return false;
            // Check for private key elements
            var d = xmlDoc.DocumentElement["D"];
            var p = xmlDoc.DocumentElement["P"];
            var q = xmlDoc.DocumentElement["Q"];
            return d != null && p != null && q != null;
        }
        catch
        {
            return false;
        }
    }

    //-----------------------------//

    public static RSAParameters GetRSAParameters(this string xml)
    {
        RSAParameters parameters = new();

        XmlDocument xmlDoc = new();
        xmlDoc.LoadXml(xml);

        if (xmlDoc.DocumentElement is null || !xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            throw new XmlException("Invalid XML RSA key.");

        foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
        {
            byte[]? value = string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText);
            switch (node.Name)
            {
                case "Modulus":
                    parameters.Modulus = value;
                    break;
                case "Exponent":
                    parameters.Exponent = value;
                    break;
                case "P":
                    parameters.P = value;
                    break;
                case "Q":
                    parameters.Q = value;
                    break;
                case "DP":
                    parameters.DP = value;
                    break;
                case "DQ":
                    parameters.DQ = value;
                    break;
                case "InverseQ":
                    parameters.InverseQ = value;
                    break;
                case "D":
                    parameters.D = value;
                    break;
            }
        }

        return parameters;
    }

    //----------------------------//


    public static RsaSecurityKey BuildRsaSigningKey(this string xml)
    {
        RSAParameters parameters = xml.GetRSAParameters();
        RSACryptoServiceProvider rsaProvider = new(2048);
        rsaProvider.ImportParameters(parameters);

        return new RsaSecurityKey(rsaProvider);
    }


    //-----------------------------//




}
