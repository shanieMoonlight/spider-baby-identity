using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Dtos.User;
using ID.Application.Features.Account.Cmd.Mfa.TwoFactorDisable;
using ID.Domain.Entities.AppUsers;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.Mfa.TwoFactorDisable;

public class TwoFactorDisableHandlerTests
{
    private readonly Mock<ITwoFactorVerificationService<AppUser>> _mockTwoFactorService;
    private readonly TwoFactorDisableHandler _handler;

    public TwoFactorDisableHandlerTests()
    {
        _mockTwoFactorService = new Mock<ITwoFactorVerificationService<AppUser>>();
        _handler = new TwoFactorDisableHandler(_mockTwoFactorService.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldDisableTwoFactorToken()
    {
        // Arrange
        var userID = Guid.NewGuid();
        var user = AppUserDataFactory.Create(userID);
        var request = new TwoFactorDisableCmd { PrincipalUser = user };
        var cancellationToken = new CancellationToken();
        var expectedResult = GenResult<AppUser>.Success(user);

        _mockTwoFactorService
            .Setup(service => service.DisableTwoFactorTokenAsync(user))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Value?.ShouldBeEquivalentTo(user.ToDto());
        _mockTwoFactorService.Verify(service => service.DisableTwoFactorTokenAsync(user), Times.Once);
    }

    //------------------------------------//

}//Cls