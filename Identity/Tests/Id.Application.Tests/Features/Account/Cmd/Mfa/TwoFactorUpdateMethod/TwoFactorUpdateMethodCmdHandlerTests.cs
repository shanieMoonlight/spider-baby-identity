namespace ID.Application.Tests.Features.Account.Cmd.Mfa.TwoFactorUpdateMethod;

public class TwoFactorUpdateMethodCmdHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _teamManagerMock;
    private readonly TwoFactorUpdateMethodCmdHandler _handler;

    public TwoFactorUpdateMethodCmdHandlerTests()
    {
        _teamManagerMock = new Mock<IIdentityTeamManager<AppUser>>();
        _handler = new TwoFactorUpdateMethodCmdHandler(_teamManagerMock.Object);
    }


    //------------------------------------//


    [Fact]
    public async Task Handle_Should_Update_TwoFactorProvider_And_Return_Success()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var user = AppUserDataFactory.Create(teamId);
        var team = TeamDataFactory.Create(id: teamId, members: [user]);
        var provider = TwoFactorProvider.Email;
        var dto = new UpdateTwoFactorProviderDto(provider);
       var request = new UpdateTwoFactorProviderCmd(dto)
        {
            PrincipalUser = user,
            PrincipalTeam = team
        };

        _teamManagerMock.Setup(x => x.UpdateMemberAsync(team, user))
            .ReturnsAsync(GenResult<AppUser>.Success(user));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.TwoFactorProvider.ShouldBe(provider);
        _teamManagerMock.Verify(x => x.UpdateMemberAsync(team, user), Times.Once);
    }

    //------------------------------------//


    [Fact]
    public async Task Handle_Should_Return_Failure_When_Update_Fails()
    {
        // Arrange
        var user = AppUserDataFactory.Create(Guid.NewGuid());
        var team = TeamDataFactory.Create();
        var provider = TwoFactorProvider.Email;
        var dto = new UpdateTwoFactorProviderDto(provider);
        var request = new UpdateTwoFactorProviderCmd(dto)
        {
            PrincipalUser = user,
            PrincipalTeam = team
        };

        _teamManagerMock.Setup(x => x.UpdateMemberAsync(team, user))
            .ReturnsAsync(GenResult<AppUser>.Failure("Update failed"));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe("Update failed");
        _teamManagerMock.Verify(x => x.UpdateMemberAsync(team, user), Times.Once);
    }

    //------------------------------------//

}//Cls