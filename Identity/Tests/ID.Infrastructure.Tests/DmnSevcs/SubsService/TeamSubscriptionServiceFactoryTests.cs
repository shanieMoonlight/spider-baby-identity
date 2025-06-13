using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.Infrastructure.DomainServices.Teams.Subs;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using Moq;

namespace ID.Infrastructure.Tests.DmnSevcs.SubsService;

public class TeamSubscriptionServiceFactoryTests
{
    private readonly Mock<IIdUnitOfWork> _uowMock;

    //- - - - - - - - - - - - - - - - - - //

    public TeamSubscriptionServiceFactoryTests() => _uowMock = new Mock<IIdUnitOfWork>();

    //------------------------------------//

    [Fact]
    public async Task GetServiceAsync_ShouldReturnSuccess_WhenTeamExists()
    {
        // Arrange
        //var mockUow = new Mock<IIdUnitOfWork>();
        var teamId = Guid.NewGuid();
        var dbTeam = TeamDataFactory.Create(teamId);
        _uowMock.Setup(u => u.TeamRepo.FirstOrDefaultAsync(
            It.IsAny<TeamByIdWithSubscriptionsSpec>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbTeam);

        var factory = new TeamSubsriptionServiceFactory(_uowMock.Object);

        // Act
        var result = await factory.GetServiceAsync(teamId);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
    }

    //------------------------------------//

    [Fact]
    public async Task GetServiceAsync_TeamNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _uowMock.Setup(u => u.TeamRepo.FirstOrDefaultAsync(new TeamByIdWithSubscriptionsSpec(It.IsAny<Guid?>()), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Team?)null);

        //_uowMock.Setup(uow => uow.TeamRepo.GetByIdWithSubscriptionsAsync(It.IsAny<Guid?>()))
        //    .ReturnsAsync((Team?)null);
        var factory = new TeamSubsriptionServiceFactory(_uowMock.Object);
        var nonExistantTeamId = Guid.NewGuid();

        // Act
        var result = await factory.GetServiceAsync(nonExistantTeamId);

        // Assert
        result.NotFound.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.NotFound<Team>(nonExistantTeamId));
    }

    //------------------------------------//

}//Cls
