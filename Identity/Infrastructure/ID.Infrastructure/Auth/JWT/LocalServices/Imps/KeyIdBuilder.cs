using ID.Infrastructure.Auth.JWT.LocalServices.Abs;
using ID.Infrastructure.Auth.JWT.Utils;
using System.Text;

namespace ID.Infrastructure.Auth.JWT.LocalServices.Imps;
internal class KeyIdBuilder : IKeyIdBuilder
{

    public string GenerateKidFromXml(string publicXmlKey)
    {
        var bytes = Encoding.UTF8.GetBytes(publicXmlKey);
        var hash = System.Security.Cryptography.SHA256.HashData(bytes);
        // Base64Url encode (no padding, +/ replaced with -_, no line breaks)
        return Base64UrlEncode(hash);
    }


    //-----------------------------//


    public string GenerateKidFromPem(string publicPem)
    {
        // Remove PEM header/footer and whitespace
        var key = publicPem.RemovePublicPemHeaderFooter();

        var keyBytes = Convert.FromBase64String(key);
        var hash = System.Security.Cryptography.SHA256.HashData(keyBytes);
        return Base64UrlEncode(hash);
    }


    //-----------------------------//


    private static string Base64UrlEncode(byte[] input) =>
        Convert.ToBase64String(input)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");

}//Cls
