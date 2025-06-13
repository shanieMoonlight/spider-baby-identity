using ID.Application.AppAbs.TokenVerificationServices;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Utility.Messages;
using ID.PhoneConfirmation.Features.Account.ConfirmPhone;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.PhoneConfirmation.Tests.Features.Account.ConfirmPhone;

public class ConfirmPhoneCmdHandlerTests
{
    private readonly Mock<IPhoneConfirmationService<AppUser>> _phoneConfServiceMock;
    private readonly ConfirmPhoneCmdHandler _handler;

    public ConfirmPhoneCmdHandlerTests()
    {
        _phoneConfServiceMock = new Mock<IPhoneConfirmationService<AppUser>>();
        _handler = new ConfirmPhoneCmdHandler(_phoneConfServiceMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_PhoneAlreadyConfirmed_ReturnsPhoneConfirmedMessage()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var principalUser = AppUserDataFactory.Create(teamId);
        var phoneUser = AppUserDataFactory.Create(teamId, phoneNumberConfirmed: true);
        var team = TeamDataFactory.Create(teamId, members: [principalUser, phoneUser]);
        var request = new ConfirmPhoneCmd(new ConfirmPhoneDto("token")) { PrincipalUser = principalUser };

        _phoneConfServiceMock.Setup(x => x.IsPhoneConfirmedAsync(principalUser))
            .ReturnsAsync(true);

        _phoneConfServiceMock.Setup(x => x.ConfirmPhoneAsync(team, principalUser, "token", principalUser.PhoneNumber ?? ""))
            .ReturnsAsync(BasicResult.Success("Phone confirmed"));


        // Act
        var result = await _handler.Handle(request, CancellationToken.None);


        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Info.Phone.PHONE_CONFIRMED(principalUser.PhoneNumber ?? "no-phone"));
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_PhoneNotConfirmed_ConfirmsPhone()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team.Id);
        var request = new ConfirmPhoneCmd(new ConfirmPhoneDto("token"))
        {
            PrincipalUser = user,
            PrincipalTeam = team,
            PrincipalTeamId = team.Id
        };
        _phoneConfServiceMock.Setup(x => x.IsPhoneConfirmedAsync(user))
            .ReturnsAsync(false);

        _phoneConfServiceMock.Setup(x => x.ConfirmPhoneAsync(team, user, "token", user.PhoneNumber ?? ""))
            .ReturnsAsync(BasicResult.Success("Phone confirmed"));


        // Act
        var result = await _handler.Handle(request, CancellationToken.None);


        // Assert
        result.Info.ShouldBe("Phone confirmed");
    }


}//Cls