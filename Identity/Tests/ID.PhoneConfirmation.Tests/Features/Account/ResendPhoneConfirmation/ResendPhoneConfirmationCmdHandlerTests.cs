using ID.Domain.Entities.AppUsers;
using ID.Domain.Utility.Messages;
using ID.PhoneConfirmation.Events.Integration.Bus;
using ID.PhoneConfirmation.Features.Account.ResendPhoneConfirmation;
using ID.Tests.Data.Factories;
using Moq;
using Shouldly;

namespace ID.PhoneConfirmation.Tests.Features.Account.ResendPhoneConfirmation;

public class ResendPhoneConfirmationCmdHandlerTests
{
    private readonly Mock<IPhoneConfirmationBus> _busMock;
    private readonly ResendPhoneConfirmationCmdHandler _handler;

    public ResendPhoneConfirmationCmdHandlerTests()
    {
        _busMock = new Mock<IPhoneConfirmationBus>();
        _handler = new ResendPhoneConfirmationCmdHandler(_busMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_UserNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var otherTeamId = Guid.NewGuid();
        var currentUser = AppUserDataFactory.Create(teamId: teamId);
        var phoneUser = AppUserDataFactory.Create(teamId: otherTeamId);
        var team = TeamDataFactory.Create(id: teamId, members: [currentUser]); //phonUser is in another team

        var dto = new ResendPhoneConfirmationDto { UserId = phoneUser .Id};
        var request = new ResendPhoneConfirmationCmd(dto) { PrincipalTeam = team };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
        result.Info.ShouldContain(IDMsgs.Error.NotFound<AppUser>(dto.Username ?? dto.Email ?? dto.UserId.ToString()));
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_PhoneAlreadyConfirmed_ReturnsPhoneAlreadyConfirmedResult()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        //var otherTeamId = Guid.NewGuid();
        var currentUser = AppUserDataFactory.Create(teamId: teamId);
        var phoneUser = AppUserDataFactory.Create(teamId: teamId, phoneNumberConfirmed:true);
        var team = TeamDataFactory.Create(id: teamId, members: [currentUser, phoneUser]);


        var dto = new ResendPhoneConfirmationDto { UserId = phoneUser.Id };
        var request = new ResendPhoneConfirmationCmd(dto) { PrincipalTeam = team };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Info.ShouldContain(IDMsgs.Info.Phone.PHONE_ALREADY_CONFIRMED(phoneUser.PhoneNumber));
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ValidRequest_GeneratesTokenAndPublishesEvent()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        //var otherTeamId = Guid.NewGuid();
        var currentUser = AppUserDataFactory.Create(teamId: teamId);
        var phoneUser = AppUserDataFactory.Create(teamId: teamId, phoneNumberConfirmed: false);
        var team = TeamDataFactory.Create(id: teamId, members: [currentUser, phoneUser]);



        var dto = new ResendPhoneConfirmationDto { UserId = phoneUser.Id };
        var request = new ResendPhoneConfirmationCmd(dto) { PrincipalTeam = team };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Info.ShouldContain("confirmed");
        _busMock.Verify(bus => bus.GenerateTokenAndPublishEventAsync(phoneUser, team, It.IsAny<CancellationToken>()), Times.Once);
    }

    //------------------------------------//

}//Cls