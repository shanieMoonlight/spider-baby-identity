using ID.Infrastructure.Auth.JWT.LocalServices.Abs;
using ID.Infrastructure.Auth.JWT.LocalServices.Imps;
using ID.Infrastructure.Auth.JWT.Setup;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.Text;

namespace ID.Infrastructure.Tests.Auth.JWT.AppServices;

public class KeyHelperTests
{
    private const string _asymmetricPrivateKeyXml = "<RSAKeyValue>\r\n  <Modulus>vi/5SsPiRzR4YaqJnhwd6C1BccsLlSn2gt/tiikU6J7GLCf9OG/r5Pnrd3jC+kf3jdr9DTwsOMlb5u6443FchRoEBkKHQ8QqbfyzvvPkJX5CAOQR/dnV4NyvIrsQF+SgsrR6ER+XCtP2LFzdPVoXCj2MNT21MngzLs2q+qvNwSAmmHN7oCSbaN84HKZJs/f6H9vzbJVE8HOyASJF6vLI7KQp8N7FycWbUnf9BlG7Aj7ZGYdH9nARRZ1yYxY1Q17ON9rKJ5YhxL4/3NS0siR+LEUAyAMkj/00QopUH23fhpNRy75CqLI8B2doAD0EjkJWs5xMauI2RNckzOpyZBuiJQ==</Modulus>\r\n  <Exponent>AQAB</Exponent>\r\n  <P>73Hn4u8nw5ns1rGQluwjsVQ3WozQ2zvc1VhjYHok6ZYRPEheRntyuMp+ZA0oBS5fLNa02xFfeZrLA9MXNEsJ48uFCi7xEMXXmak6AfNlF0BbfDr6LYe5HXW4J5I1mqDrE+Dh9C+sP+gPiV8Ict2k7rTFqeJXuQqvZuwAMuQFg2k=</P>\r\n  <Q>y1Y55FEtPOzpDgjymYs6upJFFNGiCDD8tI/FrOFDTnZ+C7OVXdCLhFTSC3cLXf8nGCQvPqpMlrz6FNaK5vcYwAoegm4SpE9EnsEr6/8PHL9UgASwk/BMhdJd0MUQuAPMjrSewWruO03b9juCQ3T7IXfbSBXRmtRgU6qCBw14HV0=</Q>\r\n  <DP>6zMRGznd7mXpXa6Hn6gPG2XlBZ4ineb8cdhjrWXtkkElZviTGlqygs/tPOwrboNxW8L/Xdx/0xx45KXaOjSnX0oPwcQAaYBc2oR/BWGG4EMIWkw4aLbT4Bs9LCmTW/NLVnhkmw6k/RU/BaoCy4nqM8wACXLaxlm3l7qkK1kODVk=</DP>\r\n  <DQ>yZ1nraFn758gvo/UrLZGtzPNV4U82k5oY0ijQnXhXO1UsZmRIwJjNAU+d6vEE6GpS+ClD8egRV5/wSWxeK3NVq0x7zXhOhZ2/cqgGFJA08pKqNz9kNKVraMW7qhXmX362Azz1OiH6zmaPp5m3Sgi0d6cCO/Jc3HdSVgpsYCDduk=</DQ>\r\n  <InverseQ>FUzyxHZul54LMDwhlExHRsUizEE+Kh630gqBbZ9M/ei+Ow5jdXYQZG8dF5mgyk/oKwEpMTGaoeHlLyAuz6txDUwhyEAz9ElFHsJ9LUMcT3YGBpV2ySoDYIQzTbBwik4P0fV7OHGVvQ6OYKqwoPOe8pA/SevdBM5lvSZjUn2GalI=</InverseQ>\r\n  <D>HlrR0xkImIzLWes6I74hF9mBRIQ/yQL1kVXkN9TuvWH6BzoqtENIxzcMWfkwRXoPDNkS4nXkKPwaavVXFRYWyjeoxMcBh9NbYkqe9a8/jqxkJHhCVt3ZwRX37fclTmrzKxKbGPiNxvClrdb0iMJEQyInqqe6r993a9TvoSqioFwWoR0pSNF9yu3qHRO1B3MKvCV+kjLylWqZU8MTP/XBwcITfl+47LZRVqDNeSMWqaD8IxewXTjDXTLWvz7El8XejeZ7MA7GwhF+VslPd3DPpeXy30AcMMsWnKLNs1h8p9/ES0fvp3wrDkRUh0a6LdJsp0ae3QidOdlmHmDsAX8ugQ==</D>\r\n</RSAKeyValue>";
    private const string _asymmetricPublicKeyXml = "<RSAKeyValue>\r\n<Modulus>vi/5SsPiRzR4YaqJnhwd6C1BccsLlSn2gt/tiikU6J7GLCf9OG/r5Pnrd3jC+kf3jdr9DTwsOMlb5u6443FchRoEBkKHQ8QqbfyzvvPkJX5CAOQR/dnV4NyvIrsQF+SgsrR6ER+XCtP2LFzdPVoXCj2MNT21MngzLs2q+qvNwSAmmHN7oCSbaN84HKZJs/f6H9vzbJVE8HOyASJF6vLI7KQp8N7FycWbUnf9BlG7Aj7ZGYdH9nARRZ1yYxY1Q17ON9rKJ5YhxL4/3NS0siR+LEUAyAMkj/00QopUH23fhpNRy75CqLI8B2doAD0EjkJWs5xMauI2RNckzOpyZBuiJQ==</Modulus>\r\n<Exponent>AQAB</Exponent>\r\n</RSAKeyValue>";
    private const string _invalidRsaXml = "<InvalidKey>...</InvalidKey>";
    private const string _symmetricKey = "fdklgjlksdfjgopklznglkdjfgdfg987986q3457jbhjabdf8345afd983145jhgafhoijq3145";

    private readonly Mock<IOptions<JwtOptions>> _mockJwtOptions;
    private readonly JwtOptions _jwtOptions;
    private readonly IKeyHelper _keyHelper;
    public KeyHelperTests()
    {
        _jwtOptions = new JwtOptions
        {
            AsymmetricTokenPrivateKey_Xml = _asymmetricPrivateKeyXml,
            AsymmetricTokenPublicKey_Xml = _asymmetricPublicKeyXml,
            SymmetricTokenSigningKey = "", // Empty string makes UseAsymmetricCrypto = true
        };

        _mockJwtOptions = new Mock<IOptions<JwtOptions>>();
        _mockJwtOptions.Setup(x => x.Value).Returns(_jwtOptions);

        _keyHelper = new KeyHelper(_mockJwtOptions.Object);
    }

    //--------------------------------//

    [Fact]
    public void Constructor_WithValidOptions_ShouldInitialize()
    {
        // Act & Assert
        _keyHelper.ShouldNotBeNull();
        _mockJwtOptions.Verify(x => x.Value, Times.Once);
    }

    //--------------------------------//

    [Fact]
    public void ParseXmlString_WithValidPublicKeyXml_ShouldReturnRsaParameters()
    {
        // Act
        var result = _keyHelper.ParseXmlString(_asymmetricPublicKeyXml);

        // Assert
        result.Modulus.ShouldNotBeNull();
        result.Exponent.ShouldNotBeNull();
        result.P.ShouldBeNull(); // Public key doesn't have private components
        result.Q.ShouldBeNull();
    }

    //--------------------------------//

    [Fact]
    public void ParseXmlString_WithValidPrivateKeyXml_ShouldReturnCompleteRsaParameters()
    {
        // Act
        var result = _keyHelper.ParseXmlString(_asymmetricPrivateKeyXml);

        // Assert
        result.Modulus.ShouldNotBeNull();
        result.Exponent.ShouldNotBeNull();
        result.P.ShouldNotBeNull(); // Private key has all components
        result.Q.ShouldNotBeNull();
        result.DP.ShouldNotBeNull();
        result.DQ.ShouldNotBeNull();
        result.InverseQ.ShouldNotBeNull();
        result.D.ShouldNotBeNull();
    }

    //--------------------------------//

    [Fact]
    public void ParseXmlString_WithInvalidXml_ShouldThrowArgumentException()
    {
        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() => _keyHelper.ParseXmlString(_invalidRsaXml));
        exception.Message.ShouldContain("Invalid XML RSA key");
    }

    //--------------------------------//

    [Fact]
    public void ParseXmlString_WithEmptyString_ShouldThrowXmlException()
    {
        // Act & Assert
        Should.Throw<System.Xml.XmlException>(() => _keyHelper.ParseXmlString(""));
    }

    //--------------------------------//

    [Fact]
    public void ParseXmlString_WithNullString_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _keyHelper.ParseXmlString(null!));
    }

    //--------------------------------//

    [Fact]
    public void BuildRsaSigningKey_WithValidPrivateKeyXml_ShouldReturnRsaSecurityKey()
    {
        // Act
        var result = _keyHelper.BuildRsaSigningKey(_asymmetricPrivateKeyXml);

        // Assert
        result.ShouldBeOfType<RsaSecurityKey>();
        result.KeySize.ShouldBe(2048);
    }

    //--------------------------------//

    [Fact]
    public void BuildRsaSigningKey_WithValidPublicKeyXml_ShouldReturnRsaSecurityKey()
    {
        // Act
        var result = _keyHelper.BuildRsaSigningKey(_asymmetricPublicKeyXml);

        // Assert
        result.ShouldBeOfType<RsaSecurityKey>();
        result.KeySize.ShouldBe(2048);
    }

    //--------------------------------//

    [Fact]
    public void BuildRsaSigningKey_WithInvalidXml_ShouldThrowArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => _keyHelper.BuildRsaSigningKey(_invalidRsaXml));
    }

    //--------------------------------//

    [Fact]
    public void BuildPrivateRsaSigningKey_ShouldUsePrivateKeyFromOptions()
    {
        // Act
        var result = _keyHelper.BuildPrivateRsaSigningKey();
        var expected = _keyHelper.BuildRsaSigningKey(_asymmetricPrivateKeyXml);

        // Assert
        result.ShouldBeOfType<RsaSecurityKey>();
        AreRsaSecurityKeysEqual(result, expected).ShouldBeTrue();
    }

    //--------------------------------//

    [Fact]
    public void BuildPublicRsaSigningKey_ShouldUsePublicKeyFromOptions()
    {
        // Act
        var result = _keyHelper.BuildPublicRsaSigningKey();
        var expected = _keyHelper.BuildRsaSigningKey(_asymmetricPublicKeyXml);

        // Assert
        result.ShouldBeOfType<RsaSecurityKey>();
        AreRsaSecurityKeysEqual(result, expected).ShouldBeTrue();
    }   
    
     //--------------------------------//

    [Fact]
    public void BuildSymmetricSigningKey_ShouldUseSymmetricKeyFromOptions()
    {
        // Arrange - Set the symmetric key in options
        _jwtOptions.SymmetricTokenSigningKey = _symmetricKey;
        
        // Act
        var result = _keyHelper.BuildSymmetricSigningKey();
        var expected = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_symmetricKey));

        // Assert
        result.ShouldBeOfType<SymmetricSecurityKey>();
        AreSymmetricSecurityKeysEqual(result, expected).ShouldBeTrue();
    }

    //--------------------------------//

    [Fact]
    public void BuildSigningKey_WhenUseAsymmetricCryptoIsTrue_ShouldReturnPrivateRsaKey()
    {
        // Arrange - UseAsymmetricCrypto = true when SymmetricTokenSigningKey is empty
        _jwtOptions.SymmetricTokenSigningKey = "";

        // Act
        var result = _keyHelper.BuildSigningKey();
        var expected = _keyHelper.BuildPrivateRsaSigningKey();

        // Assert
        result.ShouldBeOfType<RsaSecurityKey>();
        AreRsaSecurityKeysEqual((RsaSecurityKey)result, expected).ShouldBeTrue();
    }

    //--------------------------------//

    [Fact]
    public void BuildSigningKey_WhenUseAsymmetricCryptoIsFalse_ShouldReturnSymmetricKey()
    {
        // Arrange - UseAsymmetricCrypto = false when SymmetricTokenSigningKey has value
        _jwtOptions.SymmetricTokenSigningKey = _symmetricKey;

        // Act
        var result = _keyHelper.BuildSigningKey();
        var expected = _keyHelper.BuildSymmetricSigningKey();

        // Assert
        result.ShouldBeOfType<SymmetricSecurityKey>();
        AreSymmetricSecurityKeysEqual((SymmetricSecurityKey)result, expected).ShouldBeTrue();
    }

    //--------------------------------//

    [Fact]
    public void BuildValidationSigningKey_WhenUseAsymmetricCryptoIsTrue_ShouldReturnPublicRsaKey()
    {
        // Arrange - UseAsymmetricCrypto = true when SymmetricTokenSigningKey is empty
        _jwtOptions.SymmetricTokenSigningKey = "";

        // Act
        var result = _keyHelper.BuildValidationSigningKey();
        var expected = _keyHelper.BuildPublicRsaSigningKey();

        // Assert
        result.ShouldBeOfType<RsaSecurityKey>();
        AreRsaSecurityKeysEqual((RsaSecurityKey)result, expected).ShouldBeTrue();
    }

    //--------------------------------//

    [Fact]
    public void BuildValidationSigningKey_WhenUseAsymmetricCryptoIsFalse_ShouldReturnSymmetricKey()
    {
        // Arrange - UseAsymmetricCrypto = false when SymmetricTokenSigningKey has value
        _jwtOptions.SymmetricTokenSigningKey = _symmetricKey;

        // Act
        var result = _keyHelper.BuildValidationSigningKey();
        var expected = _keyHelper.BuildSymmetricSigningKey();

        // Assert
        result.ShouldBeOfType<SymmetricSecurityKey>();
        AreSymmetricSecurityKeysEqual((SymmetricSecurityKey)result, expected).ShouldBeTrue();
    }

    //--------------------------------//

    [Fact]
    public void ExportPublicKey_ShouldReturnValidPemFormat()
    {
        // Act
        var result = _keyHelper.ExportPublicKey();

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldContain("-----BEGIN PUBLIC KEY-----");
        result.ShouldContain("-----END PUBLIC KEY-----");
        result.Split('\n').Length.ShouldBeGreaterThan(3); // Header, content lines, footer
    }

    //--------------------------------//    
     
    [Fact]
    public void ExportPublicKey_ShouldUsePublicKeyFromOptions()
    {
        // Arrange - Use a different valid public key to verify it reads from options
        var differentValidPublicKey = "<RSAKeyValue>\r\n<Modulus>jAadS3T9EVDda48fbF9ANVPcZ9g65LqLJiHudaPmq88YYkdo5zJzzFPcdNqvNixVWBA6O45JAjmIZus21YFWbD5DiP1AA2217KYBzmmwmcfbI7BQYjkSTSm2/hPyAMMYW+Gg7pshPTFORtLVD/019SQ3SHWZ0MvUu62ggUWYDh8AchYE019gVjBPEh3fwFLFmMqaa7ST9sfcJzNoWy22ee78HaqU4e7JIaXrE5ldV2X8+1K6YFX4OQ4S31S3BCw68h9lHSTdgs0ResCef8jEDSL7WGTVIKW/BvCAak/5XFFMNMhdk0KDeKNe8/JObpXiij7tCwk2T/D0MQw8aqDPuw==</Modulus>\r\n<Exponent>AQAB</Exponent>\r\n</RSAKeyValue>";
        _jwtOptions.AsymmetricTokenPublicKey_Xml = differentValidPublicKey;

        // Act
        var result = _keyHelper.ExportPublicKey();

        // Assert - Should not throw and should return valid PEM format
        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldContain("-----BEGIN PUBLIC KEY-----");
        result.ShouldContain("-----END PUBLIC KEY-----");
        
        // The result should be different from what we'd get with the original key
        var originalKey = new JwtOptions
        {
            AsymmetricTokenPublicKey_Xml = _asymmetricPublicKeyXml
        };
        var mockOriginalOptions = new Mock<IOptions<JwtOptions>>();
        mockOriginalOptions.Setup(x => x.Value).Returns(originalKey);
        var originalKeyHelper = new KeyHelper(mockOriginalOptions.Object);
        var originalResult = originalKeyHelper.ExportPublicKey();
        
        result.ShouldNotBe(originalResult, "Should use the different public key from options");
    }

    //--------------------------------//


    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CryptoMethods_ShouldConsistentlyUseSameKeySource(bool useAsymmetric)
    {
        // Arrange - Control UseAsymmetricCrypto by setting/clearing SymmetricTokenSigningKey
        _jwtOptions.SymmetricTokenSigningKey = useAsymmetric ? "" : _symmetricKey;

        // Act
        var signingKey = _keyHelper.BuildSigningKey();
        var validationKey = _keyHelper.BuildValidationSigningKey();

        // Assert
        if (useAsymmetric)
        {
            signingKey.ShouldBeOfType<RsaSecurityKey>();
            validationKey.ShouldBeOfType<RsaSecurityKey>();

            // For asymmetric, signing uses private key, validation uses public key
            // They should be different instances but cryptographically related
            signingKey.ShouldNotBeSameAs(validationKey);
        }
        else
        {
            signingKey.ShouldBeOfType<SymmetricSecurityKey>();
            validationKey.ShouldBeOfType<SymmetricSecurityKey>();

            // For symmetric, both should be identical
            AreSymmetricSecurityKeysEqual((SymmetricSecurityKey)signingKey, (SymmetricSecurityKey)validationKey)
                .ShouldBeTrue();
        }
    }

    //--------------------------------//

    [Fact]
    public void JwtOptions_ChangesAfterConstruction_ShouldBeReflectedInKeyGeneration()
    {
        // Arrange - Start with asymmetric crypto (empty symmetric key)
        var originalSymmetricKey = _jwtOptions.SymmetricTokenSigningKey;

        // Act - Switch to symmetric crypto by setting symmetric key
        _jwtOptions.SymmetricTokenSigningKey = _symmetricKey;
        var key1 = _keyHelper.BuildSigningKey();

        // Switch back to asymmetric crypto by clearing symmetric key
        _jwtOptions.SymmetricTokenSigningKey = "";
        var key2 = _keyHelper.BuildSigningKey();

        // Assert - Keys should be different types based on the changed options
        key1.GetType().ShouldNotBe(key2.GetType());
        key1.ShouldBeOfType<SymmetricSecurityKey>();
        key2.ShouldBeOfType<RsaSecurityKey>();
    }

    //--------------------------------//

    [Fact]
    public void BuildSigningKey_WhenCalledMultipleTimes_ShouldReturnNewInstances()
    {
        // Act
        var key1 = _keyHelper.BuildSigningKey();
        var key2 = _keyHelper.BuildSigningKey();

        // Assert
        key1.ShouldNotBeSameAs(key2); // Different instances
        key1.GetType().ShouldBe(key2.GetType()); // Same type
    }

    [Theory]
    [InlineData("", true)]
    [InlineData(" ", true)]
    [InlineData(null, true)]
    [InlineData("valid_symmetric_key", false)]
    [InlineData("another_key_value", false)]
    public void UseAsymmetricCrypto_ShouldBeComputedCorrectly(string? symmetricKey, bool expectedUseAsymmetric)
    {
        // Arrange
        _jwtOptions.SymmetricTokenSigningKey = symmetricKey!;

        // Act
        var actualUseAsymmetric = _jwtOptions.UseAsymmetricCrypto;

        // Assert
        actualUseAsymmetric.ShouldBe(expectedUseAsymmetric);
    }

    // Helper methods similar to the original tests
    private static bool AreRsaSecurityKeysEqual(RsaSecurityKey? key1, RsaSecurityKey? key2)
    {
        if (key1 == null || key2 == null)
            return false;

        var parameters1 = key1.Rsa?.ExportParameters(false) ?? key1.Parameters;
        var parameters2 = key2.Rsa?.ExportParameters(false) ?? key2.Parameters;

        return parameters1.Modulus?.SequenceEqual(parameters2.Modulus ?? Array.Empty<byte>()) == true &&
               parameters1.Exponent?.SequenceEqual(parameters2.Exponent ?? Array.Empty<byte>()) == true;
    }

    private static bool AreSymmetricSecurityKeysEqual(SymmetricSecurityKey? key1, SymmetricSecurityKey? key2)
    {
        if (key1 == null || key2 == null)
            return false;

        return key1.Key.SequenceEqual(key2.Key);
    }
}
