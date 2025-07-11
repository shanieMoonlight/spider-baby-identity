using ID.Application.AppAbs.Setup;
using ID.Application.Features.System.Cmd.Init;
using ID.Domain.Utility.Messages;

namespace ID.Application.Tests.Features.System.Init;

public class InitializeCmdHandlerTests
{
    private readonly Mock<IIdentityInitializationService> _mockIdInitializationService;
    private readonly InitializeCmdHandler _handler;

    public InitializeCmdHandlerTests()
    {
        _mockIdInitializationService = new Mock<IIdentityInitializationService>();
        _handler = new InitializeCmdHandler(_mockIdInitializationService.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnInitializedMessage_WhenInitializationIsSuccessful()
    {
        // Arrange
        var superEmail = "newemail@yahoo.com";
        var dto = new InitializeDto("testPassword", superEmail);
        var request = new InitializeCmd(dto);
        _mockIdInitializationService.Setup(x => x.InitializeEverythingAsync(It.IsAny<string>(), It.IsAny<string?>())).ReturnsAsync(superEmail);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Info.ShouldBe(IDMsgs.Info.INITIALIZED(superEmail));
        _mockIdInitializationService.Verify(x => x.InitializeEverythingAsync(It.IsAny<string>(), It.IsAny<string?>()), Times.Once);
        result.Succeeded.ShouldBeTrue();
    }

}//Cls