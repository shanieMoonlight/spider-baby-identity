using ID.Infrastructure.Auth.JWT.AppServiceImps;
using ID.Infrastructure.Auth.JWT.LocalServices.Abs;
using ID.Infrastructure.Auth.JWT.Setup;
using Microsoft.Extensions.Options;
using Moq;

namespace ID.Infrastructure.Tests.Auth.JWT;

public class JwtKeyServiceTests
{
    private const string _samplePublicKey = "-----BEGIN PUBLIC KEY-----\nSAMPLEKEY\n-----END PUBLIC KEY-----";
    
    [Fact]
    public async Task GetPublicSigningKeyAsync_ShouldReturnKey_WhenAsymmetricCryptoIsUsed()
    {
        // Arrange
        var keyHelperMock = new Mock<IKeyHelper>();
        keyHelperMock.Setup(x => x.ExportPublicKey()).Returns(_samplePublicKey);
        
        var jwtOptions = new JwtOptions
        {
            // Setting SymmetricTokenSigningKey to empty makes UseAsymmetricCrypto return true
            SymmetricTokenSigningKey = string.Empty
        };
        
        var optionsMock = new Mock<IOptions<JwtOptions>>();
        optionsMock.Setup(o => o.Value).Returns(jwtOptions);
        
        var keyService = new JwtKeyService(keyHelperMock.Object, optionsMock.Object);
        
        // Act
        var result = await keyService.GetPublicSigningKeyAsync();
        
        // Assert
        result.ShouldBe(_samplePublicKey);
        keyHelperMock.Verify(x => x.ExportPublicKey(), Times.Once);
    }




    
    [Fact]
    public async Task GetPublicSigningKeyAsync_ShouldReturnNull_WhenSymmetricCryptoIsUsed()
    {
        // Arrange
        var keyHelperMock = new Mock<IKeyHelper>();
        keyHelperMock.Setup(x => x.ExportPublicKey()).Returns(_samplePublicKey);
        
        var jwtOptions = new JwtOptions
        {
            // Providing a non-empty symmetric key makes UseAsymmetricCrypto return false
            SymmetricTokenSigningKey = "some-symmetric-key-for-testing"
        };
        
        var optionsMock = new Mock<IOptions<JwtOptions>>();
        optionsMock.Setup(o => o.Value).Returns(jwtOptions);
        
        var keyService = new JwtKeyService(keyHelperMock.Object, optionsMock.Object);
        
        // Act
        var result = await keyService.GetPublicSigningKeyAsync();
        
        // Assert
        result.ShouldBeNull();
        keyHelperMock.Verify(x => x.ExportPublicKey(), Times.Never);
    }



    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task GetPublicSigningKeyAsync_ShouldReturnKey_WhenSymmetricKeyIsNullOrWhitespace(string symmetricKey)
    {
        // Arrange
        var keyHelperMock = new Mock<IKeyHelper>();
        keyHelperMock.Setup(x => x.ExportPublicKey()).Returns(_samplePublicKey);
        
        var jwtOptions = new JwtOptions
        {
            SymmetricTokenSigningKey = symmetricKey
        };
        
        var optionsMock = new Mock<IOptions<JwtOptions>>();
        optionsMock.Setup(o => o.Value).Returns(jwtOptions);
        
        var keyService = new JwtKeyService(keyHelperMock.Object, optionsMock.Object);
        
        // Act
        var result = await keyService.GetPublicSigningKeyAsync();
        
        // Assert
        result.ShouldBe(_samplePublicKey);
        keyHelperMock.Verify(x => x.ExportPublicKey(), Times.Once);
    }



    
    [Fact]
    public async Task GetPublicSigningKeyAsync_ShouldReturnExactValueFromKeyHelper()
    {
        // Arrange
        var dynamicKey = $"dynamic-key-{Guid.NewGuid()}";
        var keyHelperMock = new Mock<IKeyHelper>();
        keyHelperMock.Setup(x => x.ExportPublicKey()).Returns(dynamicKey);
        
        var jwtOptions = new JwtOptions
        {
            SymmetricTokenSigningKey = string.Empty
        };
        
        var optionsMock = new Mock<IOptions<JwtOptions>>();
        optionsMock.Setup(o => o.Value).Returns(jwtOptions);
        
        var keyService = new JwtKeyService(keyHelperMock.Object, optionsMock.Object);
        
        // Act
        var result = await keyService.GetPublicSigningKeyAsync();
        
        // Assert
        result.ShouldBe(dynamicKey);
    }
}