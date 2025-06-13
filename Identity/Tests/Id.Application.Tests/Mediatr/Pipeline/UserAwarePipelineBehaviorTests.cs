using ID.Tests.Data.Factories;
using MediatR;
using Moq;
using MyResults;
using Shouldly;
using ID.Application.Mediatr.Behaviours;
using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Application.Tests.Mediatr.Pipeline;

public class UserAwarePipelineBehaviorTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _teamManagerMock;
    private readonly IdUserAwarePipelineBehavior<TestUserAwareCommand<AppUser, object>, BasicResult> _behavior;

    //- - - - - - - - - - - - - - - - - - //

    public UserAwarePipelineBehaviorTests()
    {
        _teamManagerMock = new Mock<IIdentityTeamManager<AppUser>>();
        _behavior = new IdUserAwarePipelineBehavior<TestUserAwareCommand<AppUser, object>, BasicResult>(_teamManagerMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_TeamNotFound_ReturnsNotFoundResponse()
    {
        // Arrange
        var request = new TestUserAwareCommand<AppUser, object>();

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        _teamManagerMock.Setup(tm => tm.GetMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync((AppUser)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        var next = new Mock<RequestHandlerDelegate<BasicResult>>();

        // Act
        var result = await _behavior.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Unauthorized.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_TeamFound_SetsPrincipalTeamAndCallsNext()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team.Id);
        var request = new TestUserAwareCommand<AppUser, object>
        {
            PrincipalTeamId = team.Id,
            PrincipalUserId = user.Id
        };

        _teamManagerMock.Setup(tm => tm.GetMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(user);

        var next = new Mock<RequestHandlerDelegate<BasicResult>>();
        next.Setup(n => n(It.IsAny<CancellationToken>())).ReturnsAsync(BasicResult.Success());

        // Act
        var result = await _behavior.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        //request.VerifySet(r => r.PrincipalTeam = team, Times.Once);
        request.PrincipalUser.ShouldNotBeNull();
        request.PrincipalUser.Id.ShouldBe(user.Id);
        next.Verify(n => n(It.IsAny<CancellationToken>()), Times.Once);
    }

    //------------------------------------//

}//Cls







//=============================================================================//


/// <summary>
/// Class for creating requests that contain the Pincipal info and the actual AppUser with their Full Team. 
/// Will cause a NotFoundResult to be returned in the Pipeline if the user was not found.
/// So User will NOT be null in the Handler
/// </summary>
/// <typeparam name="TUser">Type of AppUser</typeparam>
public record TestUserAwareCommand<TUser, TResponse> : AIdUserAwareCommand<TUser, TResponse> where TUser : AppUser
{
}



//=============================================================================//


