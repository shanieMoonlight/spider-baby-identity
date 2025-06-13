using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;
using ID.Application.Features.Teams;
using ID.Application.Features.Teams.Qry.GetById;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Application.Tests.Features.Teams.Qry.GetById;

public class GetTeamByIdQryHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly GetTeamByIdQryHandler _handler;

    public GetTeamByIdQryHandlerTests()
    {
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _handler = new GetTeamByIdQryHandler(_mockTeamManager.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_TeamNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var request = new GetTeamByIdQry(Guid.NewGuid());
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        _mockTeamManager.Setup(m => m.GetByIdWithEverythingAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync((Team)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<TeamDto>>();
        result.NotFound.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_TeamIsSuperAndUserNotSuper_ReturnsForbiddenResult()
    {
        // Arrange
        var team = TeamDataFactory.Create(teamType: TeamType.Super);
        var request = new GetTeamByIdQry(team.Id) { IsSuper = false };
        _mockTeamManager.Setup(m => m.GetByIdWithEverythingAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(team);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<TeamDto>>();
        result.Forbidden.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ValidRequest_ReturnsTeamDto()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var request = new GetTeamByIdQry(team.Id);
        _mockTeamManager.Setup(m => m.GetByIdWithEverythingAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(team);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<TeamDto>>();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(team.Id);
        result.Value.Name.ShouldBe(team.Name);
        result.Value.Members.Count.ShouldBe(team.Members.Count);
    }

    //------------------------------------//

}//Cls
