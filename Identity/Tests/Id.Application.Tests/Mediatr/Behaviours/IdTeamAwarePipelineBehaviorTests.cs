using ID.Application.Mediatr.Behaviours;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Tests.Data.Factories;
using MediatR;
using Moq;
using MyResults;
using Shouldly;
using static ID.Application.Tests.Mediatr.Behaviours.IdTeamAwarePipelineBehaviorTests;
using static MyResults.BasicResult;

namespace ID.Application.Tests.Mediatr.Behaviours;

// Add this interface to your test class
public interface ITestUserAndTeamRequest : IIdUserAndTeamAwareRequest<AppUser>, IRequest<TestResponse> { }

public class IdTeamAwarePipelineBehaviorTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _teamManagerMock;
    private readonly Mock<RequestHandlerDelegate<TestResponse>> _nextMock;
    private readonly Mock<ITestUserAndTeamRequest> _requestMock;
    private readonly IdTeamAwarePipelineBehavior<ITestUserAndTeamRequest, TestResponse> _behavior;

    public IdTeamAwarePipelineBehaviorTests()
    {
        _teamManagerMock = new Mock<IIdentityTeamManager<AppUser>>();
        _requestMock = new Mock<ITestUserAndTeamRequest>();
        _nextMock = new Mock<RequestHandlerDelegate<TestResponse>>();
        _behavior = new IdTeamAwarePipelineBehavior<ITestUserAndTeamRequest, TestResponse>(_teamManagerMock.Object);
    }

    //------------------------------------//


    [Fact]
    public async Task Handle_Should_CallGetByIdWithEverythingAsync_WithCorrectParameters()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var teamPosition = 2;
        var team = TeamDataFactory.Create(teamId);

        _requestMock.Setup(r => r.PrincipalTeamId).Returns(teamId);
        _requestMock.Setup(r => r.PrincipalTeamPosition).Returns(teamPosition);
        _teamManagerMock.Setup(m => m.GetByIdWithEverythingAsync(teamId, teamPosition))
            .ReturnsAsync(team);
        _nextMock.Setup(n => n(It.IsAny<CancellationToken>())).ReturnsAsync(new TestResponse { Succeeded = true });

        // Act
        await _behavior.Handle(_requestMock.Object, _nextMock.Object, CancellationToken.None);

        // Assert
        _teamManagerMock.Verify(m => m.GetByIdWithEverythingAsync(teamId, teamPosition), Times.Once);
    }

    //------------------------------------//


    [Fact]
    public async Task Handle_Should_SetTeamOnRequest_WhenTeamIsFound()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = TeamDataFactory.Create(teamId);

        _requestMock.Setup(r => r.PrincipalTeamId).Returns(teamId);
        _teamManagerMock.Setup(m => m.GetByIdWithEverythingAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(team);
        _nextMock.Setup(n => n(It.IsAny<CancellationToken>())).ReturnsAsync(new TestResponse { Succeeded = true });

        // Act
        await _behavior.Handle(_requestMock.Object, _nextMock.Object, CancellationToken.None);

        // Assert
        _requestMock.Verify(r => r.SetTeam(team), Times.Once);
    }

    //------------------------------------//


    [Fact]
    public async Task Handle_Should_CallNextDelegate_WhenTeamIsFound()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = TeamDataFactory.Create(teamId);
        var expectedResponse = new TestResponse { Succeeded = true };

        _requestMock.Setup(r => r.PrincipalTeamId).Returns(teamId);
        _teamManagerMock.Setup(m => m.GetByIdWithEverythingAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(team);
        _nextMock.Setup(n => n(It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

        // Act
        var result = await _behavior.Handle(_requestMock.Object, _nextMock.Object, CancellationToken.None);

        // Assert
        _nextMock.Verify(n => n(It.IsAny<CancellationToken>()), Times.Once);
        result.ShouldBe(expectedResponse);
    }

    //------------------------------------//

    // Test with BasicResult (not your custom TestResponse)
    [Fact]
    public async Task Handle_WithBasicResult_Should_ReturnNotFound_WhenTeamIsNotFound()
    {
        // Setup
        var teamManagerMock = new Mock<IIdentityTeamManager<AppUser>>();
        var requestMock = new Mock<ITestUserAndTeamRequest>();
        var nextMock = new Mock<RequestHandlerDelegate<BasicResult>>();
        var behavior = new IdTeamAwarePipelineBehavior<ITestUserAndTeamRequest, BasicResult>(teamManagerMock.Object);

        var teamId = Guid.NewGuid();
        requestMock.Setup(r => r.PrincipalTeamId).Returns(teamId);
        teamManagerMock.Setup(m => m.GetByIdWithEverythingAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync((Team)null);

        // Act
        var result = await behavior.Handle(requestMock.Object, nextMock.Object, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Status.ShouldBe(ResultStatus.NotFound);
        result.Info.ShouldContain(teamId.ToString());
    }

    //------------------------------------//

    // Test with GenResult<Team>
    public interface IGenericTestRequest : IIdUserAndTeamAwareRequest<AppUser>, IRequest<GenResult<Team>> { }
    [Fact]
    public async Task Handle_WithGenericResult_Should_ReturnNotFound_WhenTeamIsNotFound()
    {
        // Setup
        var teamManagerMock = new Mock<IIdentityTeamManager<AppUser>>();
        var requestMock = new Mock<IGenericTestRequest>();
        var nextMock = new Mock<RequestHandlerDelegate<GenResult<Team>>>();
        var behavior = new IdTeamAwarePipelineBehavior<IGenericTestRequest, GenResult<Team>>(teamManagerMock.Object);

        var teamId = Guid.NewGuid();
        requestMock.Setup(r => r.PrincipalTeamId).Returns(teamId);
        teamManagerMock.Setup(m => m.GetByIdWithEverythingAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync((Team)null);

        // Act
        var result = await behavior.Handle(requestMock.Object, nextMock.Object, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Status.ShouldBe(ResultStatus.NotFound);
        result.Info.ShouldContain(teamId.ToString());
    }

    //------------------------------------//


    // Helper class for test responses
    public class TestResponse : BasicResult
    {
        public TestResponse() : base(true) { }
        public TestResponse(bool succeeded) : base(succeeded) { }
    }


}