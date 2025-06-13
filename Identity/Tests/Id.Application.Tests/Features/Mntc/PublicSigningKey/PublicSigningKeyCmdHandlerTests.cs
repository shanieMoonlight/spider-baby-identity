using ID.Application.Features.Mntc.Qry.EmailRoutes;
using ID.Application.Features.Mntc.Qry.PublicSigningKey;
using ID.Domain.Utility.Messages;
using Moq;
using Shouldly;
using ID.Application.JWT;

namespace ID.Application.Tests.Features.Mntc.PublicSigningKey;

public class PublicSigningKeyCmdHandlerTests
{
    private readonly Mock<IJwtKeyService> _mockKeyService;
    private readonly GetPublicSigningKeyCmdHandler _handler;

    public PublicSigningKeyCmdHandlerTests()
    {
        _mockKeyService = new Mock<IJwtKeyService>();
        _handler = new GetPublicSigningKeyCmdHandler(_mockKeyService.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnNotFoundResult_WhenKeyIsNull()
    {
        // Arrange
        _mockKeyService.Setup(x => x.GetPublicSigningKeyAsync()).ReturnsAsync((string?)null);

        // Act
        var result = await _handler.Handle(new GetPublicSigningKeyCmd(), CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.Jwt.SYMETRIC_CRYPTO_HAS_NO_PUBLIC_KEY);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnBasicResult_WhenKeyIsNotNull()
    {
        // Arrange
        var publicKey = "publicKey";
        _mockKeyService.Setup(x => x.GetPublicSigningKeyAsync()).ReturnsAsync(publicKey);

        // Act
        var result = await _handler.Handle(new GetPublicSigningKeyCmd(), CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(new PublicSigningKeyDto(publicKey));
    }

    //------------------------------------//

}//Cls