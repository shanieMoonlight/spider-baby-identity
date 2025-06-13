using ID.Application.AppAbs.ApplicationServices.Principal;
using ID.Application.Features.Teams;
using ID.Application.Features.Teams.Cmd.Create;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.AppServices.Abs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Tests.Data.Factories;
using Moq;
using Shouldly;


namespace ID.Application.Tests.Features.Teams.Cmd.CreateCustomer;
public class CreateCustomerTeamsCmdHandlerTests
{
    private readonly Mock<IIdPrincipalInfo> _userInfoMock;
    private readonly Mock<IIdentityTeamManager<AppUser>> _teamMgrMock;
    private readonly CreateCustomerTeamsCmdHandler _handler;
    private readonly Mock<ITeamBuilderService> _teamBuilderMock;

    //- - - - - - - - - - - - - - - - - - //

    public CreateCustomerTeamsCmdHandlerTests()
    {
        _userInfoMock = new Mock<IIdPrincipalInfo>();
        _teamMgrMock = new Mock<IIdentityTeamManager<AppUser>>();
        _teamBuilderMock = new Mock<ITeamBuilderService>();

        _handler = new CreateCustomerTeamsCmdHandler(_teamMgrMock.Object, _teamBuilderMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task OnAuthenticated_ValidRequest_ReturnsTeamDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userInfoMock.Setup(u => u.IsMntcMinimum()).Returns(true);

        var dtoName = "Test Team";
        var dtoDescription = "This is a test team.";
        var request = new CreateCustomerTeamCmd(new TeamDto { Name = dtoName, Description = dtoDescription });
        var expectedTeamId = Guid.NewGuid();
        var expectedTeam = TeamDataFactory.Create(expectedTeamId, dtoName, dtoDescription);

        _teamMgrMock.Setup(tm => tm.AddTeamAsync(It.IsAny<Team>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTeam);

        var handler = new CreateCustomerTeamsCmdHandler(_teamMgrMock.Object, _teamBuilderMock.Object);


        // Act
        var result = await handler.Handle(request, CancellationToken.None);


        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value?.Id.ShouldBe(expectedTeamId);
        result.Value?.Name.ShouldBe(dtoName);
        result.Value?.Description.ShouldBe(dtoDescription);

        _teamMgrMock.Verify(tm => tm.AddTeamAsync(It.IsAny<Team>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    //------------------------------------//

}//Cls
