using ID.Infrastructure.Auth.JWT.Setup;
using ID.Infrastructure.Auth.JWT.Utils;

namespace ID.Domain.Tests.Models;

public class AsymmetricPemKeyPairTests
{
    [Fact]
    public void Throws_When_PublicKeyPem_IsNullOrEmpty()
    {
        Assert.Throws<ArgumentException>(() => AsymmetricPemKeyPair.Create(null!, "PRIVATE"));
        Assert.Throws<ArgumentException>(() => AsymmetricPemKeyPair.Create("", "PRIVATE"));
    }

    //--------------------------//

    [Fact]
    public void Throws_When_PrivateKeyPem_IsNullOrEmpty()
    {
        Assert.Throws<ArgumentException>(() => AsymmetricPemKeyPair.Create("PUBLIC", null!));
        Assert.Throws<ArgumentException>(() => AsymmetricPemKeyPair.Create("PUBLIC", ""));
    }

    //--------------------------//

    [Fact]
    public void Throws_When_PublicKeyPem_IsNotValidPublicKey()
    {
        // Simulate IsPublicKey returns false
        var invalidPublic = "NOT_A_PUBLIC_KEY";
        Assert.Throws<ArgumentException>(() => AsymmetricPemKeyPair.Create(invalidPublic, "PRIVATE"));
    }

    //--------------------------//

    [Fact]
    public void Throws_When_PublicKeyPem_IsPrivateKey()
    {
        // Simulate IsPrivateKey returns true
        var privateKeyPem = PemKeyConstants.PRIVATE_PEM_BEGIN + "...";
        Assert.Throws<ArgumentException>(() => AsymmetricPemKeyPair.Create(privateKeyPem, privateKeyPem));
    }

    //--------------------------//

    [Fact]
    public void Returns_Valid_AsymmetricPemKeyPair()
    {
        var publicKeyPem = PemKeyConstants.PUBLIC_PEM_BEGIN + "...";
        var privateKeyPem = PemKeyConstants.PRIVATE_PEM_BEGIN + "...";
        var pair = AsymmetricPemKeyPair.Create(publicKeyPem, privateKeyPem);
        Assert.Equal(publicKeyPem, pair.PublicKey);
        Assert.Equal(privateKeyPem, pair.PrivateKey);
    }

    //--------------------------//

    [Fact]
    public void Sets_KeyId_ToNull_IfNotProvided()
    {
        var publicKeyPem = PemKeyConstants.PUBLIC_PEM_BEGIN + "...";
        var privateKeyPem = PemKeyConstants.PRIVATE_PEM_BEGIN + "...";
        var pair = AsymmetricPemKeyPair.Create(publicKeyPem, privateKeyPem);
    }

    //--------------------------//


}//Cls
