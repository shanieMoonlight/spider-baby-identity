using ID.Infrastructure.Auth.JWT.LocalServices.Imps;
using ID.Infrastructure.Auth.JWT.Utils;

namespace ID.Infrastructure.Tests.Auth.JWT.LocalServices.Imps;

public class KeyIdBuilderTests
{
    private readonly KeyIdBuilder _builder = new();

    [Fact]
    public void GenerateKidFromPem_ReturnsExpectedKid_ForValidPem()
    {
        // Arrange: base64 for 'testkey' is 'dGVzdGtleQ=='
        string pem = $"{PemKeyConstants.PUBLIC_PEM_BEGIN}\ndGVzdGtleQ==\n{PemKeyConstants.PUBLIC_PEM_END}";
        // Act
        var kid = _builder.GenerateKidFromPem(pem);
        // Assert: Should be SHA256 of 'testkey', base64url encoded
        var expected = Base64UrlSha256("testkey");
        Assert.Equal(expected, kid);
    }

    //-----------------------------//

    [Fact]
    public void GenerateKidFromXml_ReturnsExpectedKid_ForXml()
    {
        string xml = "<RSAKeyValue><Modulus>abc</Modulus></RSAKeyValue>";
        var kid = _builder.GenerateKidFromXml(xml);
        var expected = Base64UrlSha256(xml);
        Assert.Equal(expected, kid);
    }

    //-----------------------------//

    [Fact]
    public void GenerateKidFromPem_ThrowsFormatException_ForInvalidBase64()
    {
        string pem = $"{PemKeyConstants.PUBLIC_PEM_BEGIN}\nnotbase64!\n{PemKeyConstants.PUBLIC_PEM_END}";
        Assert.Throws<FormatException>(() => _builder.GenerateKidFromPem(pem));
    }


    //-----------------------------//


    [Fact]
    public void GenerateKidFromPem_ProducesSameKid_ForSamePem()
    {
        string pem = $"{PemKeyConstants.PUBLIC_PEM_BEGIN}\ndGVzdGtleQ==\n{PemKeyConstants.PUBLIC_PEM_END}";
        var kid1 = _builder.GenerateKidFromPem(pem);
        var kid2 = _builder.GenerateKidFromPem(pem);
        Assert.Equal(kid1, kid2);
    }


    //-----------------------------//


    [Fact]
    public void GenerateKidFromPem_ProducesDifferentKid_ForDifferentPem()
    {
        string pem1 = $"{PemKeyConstants.PUBLIC_PEM_BEGIN}\ndGVzdGtleQ==\n{PemKeyConstants.PUBLIC_PEM_END}";
        string pem2 = $"{PemKeyConstants.PUBLIC_PEM_BEGIN}\nb3RoZXJrZXk=\n{PemKeyConstants.PUBLIC_PEM_END}"; // 'otherkey'
        var kid1 = _builder.GenerateKidFromPem(pem1);
        var kid2 = _builder.GenerateKidFromPem(pem2);
        Assert.NotEqual(kid1, kid2);
    }


    //-----------------------------//


    private static string Base64UrlSha256(string input)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        var hash = System.Security.Cryptography.SHA256.HashData(bytes);
        return Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

}//Cls
