using ID.Infrastructure.Auth.JWT.AppServiceImps;
using ID.Infrastructure.Auth.JWT.LocalServices.Abs;
using ID.Infrastructure.Auth.JWT.Setup;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.Security.Cryptography;

namespace ID.Infrastructure.Tests.Auth.JWT.AppServiceImps;

public class JsonWebKeyProviderTests
{
    private static RsaSecurityKey CreateRsaSecurityKey(string keyId = "test-kid")
    {
        using var rsa = RSA.Create(2048);
        var key = new RsaSecurityKey(rsa.ExportParameters(false)) { KeyId = keyId };
        return key;
    }

    [Fact]
    public async Task GetJwks_Returns_Expected_JwkListDto_With_OneKey()
    {
        // Arrange
        var rsaKey = CreateRsaSecurityKey("kid-1");
        var keyProvider = new Mock<IKeyProvider>();
        keyProvider.Setup(x => x.GetAsymmetricValidationSigningKeys())
            .Returns([rsaKey]);

        var jwtOptions = new JwtOptions { AsymmetricAlgorithm = "RS256" };
        var options = Options.Create(jwtOptions);
        var provider = new JsonWebKeyProvider(keyProvider.Object, options);

        // Act
        var result = await provider.GetJwks();

        // Assert
        result.ShouldNotBeNull();
        result.Keys.Count.ShouldBe(1);
        var jwk = result.Keys[0];
        jwk.Kty.ShouldBe("RSA");
        jwk.Use.ShouldBe("sig");
        jwk.Alg.ShouldBe("RS256");
        jwk.Kid.ShouldBe("kid-1");
        jwk.N.ShouldNotBeNullOrWhiteSpace();
        jwk.E.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetJwks_Returns_Empty_When_No_Keys()
    {
        // Arrange
        var keyProvider = new Mock<IKeyProvider>();
        keyProvider.Setup(x => x.GetAsymmetricValidationSigningKeys())
            .Returns(new List<RsaSecurityKey>());
        var jwtOptions = new JwtOptions { AsymmetricAlgorithm = "RS256" };
        var options = Options.Create(jwtOptions);
        var provider = new JsonWebKeyProvider(keyProvider.Object, options);

        // Act
        var result = await provider.GetJwks();

        // Assert
        result.ShouldNotBeNull();
        result.Keys.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetJwks_Returns_Multiple_Keys()
    {
        // Arrange
        var rsaKey1 = CreateRsaSecurityKey("kid-1");
        var rsaKey2 = CreateRsaSecurityKey("kid-2");
        var keyProvider = new Mock<IKeyProvider>();
        keyProvider.Setup(x => x.GetAsymmetricValidationSigningKeys())
            .Returns(new List<RsaSecurityKey> { rsaKey1, rsaKey2 });
        var jwtOptions = new JwtOptions { AsymmetricAlgorithm = "RS384" };
        var options = Options.Create(jwtOptions);
        var provider = new JsonWebKeyProvider(keyProvider.Object, options);

        // Act
        var result = await provider.GetJwks();

        // Assert
        result.ShouldNotBeNull();
        result.Keys.Count.ShouldBe(2);
        result.Keys.ShouldContain(k => k.Kid == "kid-1");
        result.Keys.ShouldContain(k => k.Kid == "kid-2");
        result.Keys.ShouldAllBe(k => k.Alg == "RS384");
    }
}
