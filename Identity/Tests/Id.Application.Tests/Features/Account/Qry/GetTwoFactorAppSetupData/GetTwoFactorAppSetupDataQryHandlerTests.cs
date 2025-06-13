using ID.Application.AppAbs.MFA.AuthenticatorApps;
using ID.Application.Features.Account.Qry.GetTwoFactorAppSetupData;
using ID.Tests.Data.Factories;
using Moq;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Qry.GetTwoFactorAppSetupData;

public class GetTwoFactorAppSetupDataQryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnAuthAppSetupDto()
    {
        // Arrange
        var mockAuthAppService = new Mock<IAuthenticatorAppService>();
        var user = AppUserDataFactory.Create(Guid.NewGuid());
        var setupDto = new AuthAppSetupDto("setupKey", "qrCodeData", "customerSecret", "account");
        mockAuthAppService.Setup(service => service.Setup(user)).ReturnsAsync(setupDto);

        var handler = new GetTwoFactorAppSetupDataQryHandler(mockAuthAppService.Object);
        var request = new GetTwoFactorAppSetupDataQry { PrincipalUser = user };
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldBe(setupDto);
    }
}