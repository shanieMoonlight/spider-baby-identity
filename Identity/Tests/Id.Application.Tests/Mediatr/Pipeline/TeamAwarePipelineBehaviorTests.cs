using ID.Application.Mediatr.Behaviours;
using ID.Application.Mediatr.CqrsAbs;
using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Tests.Data.Factories;
using MediatR;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Mediatr.Pipeline;

public class TeamAwarePipelineBehaviorTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _teamManagerMock;
    private readonly IdTeamAwarePipelineBehavior<TestUserAndTeamAwareCommand<AppUser, object>, BasicResult> _behavior;

    //- - - - - - - - - - - - - - - - - - //

    public TeamAwarePipelineBehaviorTests()
    {
        _teamManagerMock = new Mock<IIdentityTeamManager<AppUser>>();
        _behavior = new IdTeamAwarePipelineBehavior<TestUserAndTeamAwareCommand<AppUser, object>, BasicResult>(_teamManagerMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_TeamNotFound_ReturnsNotFoundResponse()
    {
        // Arrange
        var request = new TestUserAndTeamAwareCommand<AppUser, object>();

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        _teamManagerMock.Setup(tm => tm.GetByIdWithEverythingAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync((Team)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        var next = new Mock<RequestHandlerDelegate<BasicResult>>();

        // Act
        var result = await _behavior.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
    }


    //------------------------------------//


    [Fact]
    public async Task Handle_TeamFound_SetsPrincipalTeamAndCallsNext()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        //var requestMock = new Mock<TestUserAndTeamAwareCommand<AppUser, object>>();
        //requestMock.Setup(r => r.PrincipalTeamId).Returns(Guid.NewGuid());
        var request = new TestUserAndTeamAwareCommand<AppUser, object>
        {
            PrincipalTeamId = Guid.NewGuid()
        };

        _teamManagerMock.Setup(tm => tm.GetByIdWithEverythingAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(team);

        var next = new Mock<RequestHandlerDelegate<BasicResult>>();
        next.Setup(n => n(It.IsAny<CancellationToken>())).ReturnsAsync(BasicResult.Success());

        // Act
        var result = await _behavior.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        //request.VerifySet(r => r.PrincipalTeam = team, Times.Once);
        request.PrincipalTeam.ShouldNotBeNull();
        request.PrincipalTeam.Id.ShouldBe(team.Id);
        next.Verify(n => n(It.IsAny<CancellationToken>()), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_TeamFound_SetsLeaderID_IfPrincipalIsLeader()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        //var requestMock = new Mock<TestUserAndTeamAwareCommand<AppUser, object>>();
        //requestMock.Setup(r => r.PrincipalTeamId).Returns(Guid.NewGuid());
        var request = new TestUserAndTeamAwareCommand<AppUser, object>
        {
            PrincipalTeamId = team.Id,
            PrincipalUserId = team.LeaderId
        };

        _teamManagerMock.Setup(tm => tm.GetByIdWithEverythingAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(team);

        var next = new Mock<RequestHandlerDelegate<BasicResult>>();
        next.Setup(n => n(It.IsAny<CancellationToken>())).ReturnsAsync(BasicResult.Success());

        // Act
        var result = await _behavior.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        request.PrincipalTeam.ShouldNotBeNull();
        request.IsLeader.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_TeamFound_DOES_NOT_SetLeaderID_IfPrincipalIsLeader()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        //var requestMock = new Mock<TestUserAndTeamAwareCommand<AppUser, object>>();
        //requestMock.Setup(r => r.PrincipalTeamId).Returns(Guid.NewGuid());
        var request = new TestUserAndTeamAwareCommand<AppUser, object>
        {
            PrincipalTeamId = team.Id,
            PrincipalUserId = Guid.NewGuid()
        };

        _teamManagerMock.Setup(tm => tm.GetByIdWithEverythingAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(team);

        var next = new Mock<RequestHandlerDelegate<BasicResult>>();
        next.Setup(n => n(It.IsAny<CancellationToken>())).ReturnsAsync(BasicResult.Success());

        // Act
        var result = await _behavior.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        request.PrincipalTeam.ShouldNotBeNull();
        request.IsLeader.ShouldBeFalse();
    }

    //------------------------------------//

}//Cls







//=============================================================================//

public record TestUserAndTeamAwareCommand<TUser, TResponse>
    : AIdUserAndTeamAwareCommand<TUser, TResponse>, IIdUserAndTeamAwareRequest<TUser> where TUser : AppUser
{ }

//=============================================================================//


