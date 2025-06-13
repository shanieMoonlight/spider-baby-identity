using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.PhoneConfirmation.Events.Integration.Bus;
using ID.PhoneConfirmation.Features.Account.ResendPhoneConfirmationPrincipal;
using ID.Tests.Data.Factories;
using Moq;
using Shouldly;

namespace ID.PhoneConfirmation.Tests.Features.Account.ResendPhoneConfirmationPrincipal;
public class ResendEmailConfCmdHandlerTests
{

    //------------------------------------//

    [Fact]
    public async Task Handle_UserPhoneNumberConfirmed_ReturnsEmailAlreadyConfirmed()
    {
        // Arrange
        var user = AppUserDataFactory.Create(teamId: Guid.NewGuid(), phoneNumberConfirmed: true);

        var command = new ResendPhoneConfirmationPrincipalCmd { PrincipalUser = user };
        var mockBus = new Mock<IPhoneConfirmationBus>();
        var handler = new ResendPhoneConfirmationPrincipalHandler(mockBus.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Info.ShouldBe(IDMsgs.Info.Phone.PHONE_ALREADY_CONFIRMED(user.PhoneNumber));
    }

    //------------------------------------//


    [Fact]
    public async Task Handle_UserPhoneNumberNotConfirmed_PublishesEventAndReturnsEmailConfirmationSent()
    {
        // Arrange
        var user = AppUserDataFactory.Create(teamId: Guid.NewGuid(), phoneNumberConfirmed: false);

        var command = new ResendPhoneConfirmationPrincipalCmd { PrincipalUser = user };
        var mockBus = new Mock<IPhoneConfirmationBus>();
        mockBus.Setup(bus => bus.GenerateTokenAndPublishEventAsync(It.IsAny<AppUser>(), It.IsAny<Team>(), It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask);
        var handler = new ResendPhoneConfirmationPrincipalHandler(mockBus.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Info.ShouldBe(IDMsgs.Info.Phone.PHONE_CONFIRMED(user.PhoneNumber));
        mockBus.Verify(bus => bus.GenerateTokenAndPublishEventAsync(user, user.Team!, It.IsAny<CancellationToken>()), Times.Once);

    }
    //------------------------------------//



}//Cls
