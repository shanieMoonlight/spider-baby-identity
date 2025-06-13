using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Dtos.User;
using ID.Application.Features.Account.Cmd.Mfa.TwoFactorEnable;
using ID.Domain.Entities.AppUsers;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.Mfa.TwoFactorEnable;

public class TwoFactorEnableHandlerTests
{
    private readonly Mock<ITwoFactorVerificationService<AppUser>> _mockTwoFactorService;
    private readonly TwoFactorEnableHandler _handler;

    public TwoFactorEnableHandlerTests()
    {
        _mockTwoFactorService = new Mock<ITwoFactorVerificationService<AppUser>>();
        _handler = new TwoFactorEnableHandler(_mockTwoFactorService.Object);
    }

    [Fact]
    public async Task Handle_ShouldEnableTwoFactor_ForValidUser()
    {
        // Arrange
        var userID = Guid.NewGuid();
        var user = AppUserDataFactory.Create(userID);
        var request = new TwoFactorEnableCmd { PrincipalUser = user };
        var expectedResult = GenResult<AppUser>.Success(user);
        _mockTwoFactorService.Setup(s => s.EnableTwoFactorTokenAsync(user))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Value?.ShouldBeEquivalentTo(user.ToDto());
        _mockTwoFactorService.Verify(s => s.EnableTwoFactorTokenAsync(user), Times.Once);
    }

}//Cls