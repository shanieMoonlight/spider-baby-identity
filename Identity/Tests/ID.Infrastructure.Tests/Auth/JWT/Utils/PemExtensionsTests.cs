using ID.Infrastructure.Auth.JWT.Utils;

namespace ID.Infrastructure.Tests.Auth.JWT.Utils;

public class PemExtensionsTests
{

    [Theory]
    [InlineData("-----BEGIN PUBLIC KEY-----\nMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA7\n-----END PUBLIC KEY-----", "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA7")]
    [InlineData("-----BEGIN RSA PUBLIC KEY-----\nABC123==\n-----END RSA PUBLIC KEY-----", "ABC123==")]
    [InlineData("-----BEGIN PUBLIC KEY-----\r\nXYZ==\r\n-----END PUBLIC KEY-----", "XYZ==")]
    [InlineData("   -----BEGIN PUBLIC KEY-----\nDATA==\n-----END PUBLIC KEY-----   ", "DATA==")]
    [InlineData("DATAONLY", "DATAONLY")]
    public void RemovePublicPemHeaderFooter_RemovesHeadersAndWhitespace(string input, string expected)
    {
        var result = input.RemovePublicPemHeaderFooter();
        Assert.Equal(expected, result);
    }



    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void RemovePublicPemHeaderFooter_ThrowsOnNullOrEmpty(string input)
    {
        Assert.Throws<ArgumentException>(() => input.RemovePublicPemHeaderFooter());
    }



    [Theory]
    [InlineData("-----BEGIN PRIVATE KEY-----\nABCDEF==\n-----END PRIVATE KEY-----", "ABCDEF==")]
    [InlineData("-----BEGIN RSA PRIVATE KEY-----\n123456==\n-----END RSA PRIVATE KEY-----", "123456==")]
    [InlineData("-----BEGIN PRIVATE KEY-----\r\nQWERTY==\r\n-----END PRIVATE KEY-----", "QWERTY==")]
    [InlineData("   -----BEGIN PRIVATE KEY-----\nSECRET==\n-----END PRIVATE KEY-----   ", "SECRET==")]
    [InlineData("PRIVATEKEYDATA", "PRIVATEKEYDATA")]
    public void RemovePrivatePemHeaderFooter_RemovesHeadersAndWhitespace(string input, string expected)
    {
        var result = input.RemovePrivatePemHeaderFooter();
        Assert.Equal(expected, result);
    }



    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void RemovePrivatePemHeaderFooter_ThrowsOnNullOrEmpty(string input)
    {
        Assert.Throws<ArgumentException>(() => input.RemovePrivatePemHeaderFooter());
    }

    [Theory]
    [InlineData("-----BEGIN PUBLIC KEY-----\nABCDEF==\n-----END PUBLIC KEY-----", true)]
    [InlineData("-----BEGIN RSA PUBLIC KEY-----\nABCDEF==\n-----END RSA PUBLIC KEY-----", true)]
    [InlineData("-----BEGIN PRIVATE KEY-----\nABCDEF==\n-----END PRIVATE KEY-----", true)]
    [InlineData("-----BEGIN RSA PRIVATE KEY-----\nABCDEF==\n-----END RSA PRIVATE KEY-----", true)]
    [InlineData("SOMEKEYDATA", false)]
    public void IsPemKey_DetectsPublicKey(string input, bool expected)
    {
        input.IsPemKey().ShouldBe(expected);
    }



    [Theory]
    [InlineData("-----BEGIN PRIVATE KEY-----\nABCDEF==\n-----END PRIVATE KEY-----", true)]
    [InlineData("-----BEGIN RSA PRIVATE KEY-----\nABCDEF==\n-----END RSA PRIVATE KEY-----", true)]
    [InlineData("-----BEGIN PUBLIC KEY-----\nABCDEF==\n-----END PUBLIC KEY-----", false)]
    [InlineData("-----BEGIN RSA PUBLIC KEY-----\nABCDEF==\n-----END RSA PUBLIC KEY-----", false)]
    [InlineData("SOMEKEYDATA", false)]
    public void IsPrivateKey_DetectsPrivateKey(string input, bool expected)
    {
        input.IsPrivateKey().ShouldBe(expected);
    }


    [Theory]
    [InlineData("-----BEGIN PUBLIC KEY-----\nABCDEF==\n-----END PUBLIC KEY-----", true)]
    [InlineData("-----BEGIN RSA PUBLIC KEY-----\nABCDEF==\n-----END RSA PUBLIC KEY-----", true)]
    [InlineData("-----BEGIN PRIVATE KEY-----\nABCDEF==\n-----END PRIVATE KEY-----", false)]
    [InlineData("-----BEGIN RSA PRIVATE KEY-----\nABCDEF==\n-----END RSA PRIVATE KEY-----", false)]
    [InlineData("SOMEKEYDATA", false)]
    public void IsPublicKey_DetectsPublicKey(string input, bool expected)
    {
        input.IsPublicKey().ShouldBe(expected);
    }

    [Theory]
    [InlineData("-----BEGIN RSA PUBLIC KEY-----\nABCDEF==\n-----END RSA PUBLIC KEY-----", true)]
    [InlineData("-----BEGIN RSA PRIVATE KEY-----\nABCDEF==\n-----END RSA PRIVATE KEY-----", true)]
    [InlineData("-----BEGIN PUBLIC KEY-----\nABCDEF==\n-----END PUBLIC KEY-----", false)]
    [InlineData("-----BEGIN PRIVATE KEY-----\nABCDEF==\n-----END PRIVATE KEY-----", false)]
    [InlineData("SOMEKEYDATA", false)]
    public void IsRsaKey_DetectsRsaKey(string input, bool expected)
    {
        input.IsRsaKey().ShouldBe(expected);
    }



    [Theory]
    [InlineData("-----BEGIN RSA PUBLIC KEY-----\nABCDEF==\n-----END RSA PUBLIC KEY-----", true)]
    [InlineData("-----BEGIN PUBLIC KEY-----\nABCDEF==\n-----END PUBLIC KEY-----", false)]
    [InlineData("-----BEGIN PRIVATE KEY-----\nABCDEF==\n-----END PRIVATE KEY-----", false)]
    [InlineData("SOMEKEYDATA", false)]
    public void IsRsaPublicKey_DetectsRsaPublicKey(string input, bool expected)
    {
        input.IsRsaPublicKey().ShouldBe(expected);
    }



    [Theory]
    [InlineData("-----BEGIN RSA PRIVATE KEY-----\nABCDEF==\n-----END RSA PRIVATE KEY-----", true)]
    [InlineData("-----BEGIN PRIVATE KEY-----\nABCDEF==\n-----END PRIVATE KEY-----", false)]
    [InlineData("-----BEGIN PUBLIC KEY-----\nABCDEF==\n-----END PUBLIC KEY-----", false)]
    [InlineData("SOMEKEYDATA", false)]
    public void IsRsaPrivateKey_DetectsRsaPrivateKey(string input, bool expected)
    {
        input.IsRsaPrivateKey().ShouldBe(expected);
    }


}
