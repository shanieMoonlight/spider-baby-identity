using ID.Application.Features.MemberMgmt.Qry.GetMntcTeamMember;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Tests.Data.Factories;
using Moq;
using Shouldly;

namespace ID.Application.Tests.Features.MemberMgmt.Qry.GetMntcTeamMember;

public class GetMntcTeamMemberQryHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _teamManagerMock;
    private readonly GetMntcTeamMemberQryHandler _handler;

    public GetMntcTeamMemberQryHandlerTests()
    {
        _teamManagerMock = new Mock<IIdentityTeamManager<AppUser>>();
        _handler = new GetMntcTeamMemberQryHandler(_teamManagerMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSuccessResult_WhenMemberExists()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var member = AppUserDataFactory.Create(teamId: teamId, id: memberId);
        var team = TeamDataFactory.Create(id: teamId, members: [member]);

        _teamManagerMock
            .Setup(tm => tm.GetMntcTeamWithMemberAsync(memberId, It.IsAny<int>()))
            .ReturnsAsync(team);

        var request = new GetMntcTeamMemberQry(memberId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(memberId);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnNotFoundResult_WhenMemberDoesNotExist()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var team = TeamDataFactory.Create();

        _teamManagerMock
            .Setup(tm => tm.GetMntcTeamWithMemberAsync(memberId, It.IsAny<int>()))
            .ReturnsAsync(team);

        var request = new GetMntcTeamMemberQry(memberId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_Should_Call_GetMntcTeamWithMemberAsync_With_Position_1000_When_IsSuper()
    {
        // Arrange
        var request = new GetMntcTeamMemberQry(Guid.NewGuid()) { IsSuper = true, MemberId = Guid.NewGuid() };
        var cancellationToken = CancellationToken.None;

        _teamManagerMock
            .Setup(mgr => mgr.GetMntcTeamWithMemberAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(TeamDataFactory.AnyTeam);

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _teamManagerMock.Verify(mgr => mgr.GetMntcTeamWithMemberAsync(request.MemberId, 1000), Times.Once);
    }

    //------------------------------------//

}//Cls
