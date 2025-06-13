//using ID.Domain.Entities.Teams;
//using ID.Domain.Utility;
//using ID.Infrastructure.DomainServices.Teams.Dvcs;
//using ID.Infrastructure.Persistance.Abstractions.Repos;
//using ID.Tests.Data.Factories;
//using Moq;
//using Shouldly;

//namespace ID.Infrastructure.Tests.DmnSevcs.DvcsService;

//public class TeamDeviceServiceFactoryTests
//{
//    private readonly Mock<IIdUnitOfWork> _uowMock;

//    //- - - - - - - - - - - - - - - - - - //

//    public TeamDeviceServiceFactoryTests() => _uowMock = new Mock<IIdUnitOfWork>();

//    //------------------------------------//

//    [Fact]
//    public async Task GetServiceAsync_TeamNotFound_ShouldReturnNotFound()
//    {
//        // Arrange
//        _uowMock.Setup(uow => uow.TeamRepo.GetByIdWithSubscriptionsAsync(It.IsAny<Guid?>()))
//            .ReturnsAsync((Team?)null);
//        var factory = new TeamDeviceServiceFactory(_uowMock.Object);
//        var nonExistantTeamId = Guid.NewGuid();

//        // Act
//        var result = await factory.GetServiceAsync(nonExistantTeamId, Guid.NewGuid());

//        // Assert
//        result.NotFound.ShouldBeTrue();
//        result.Info.ShouldBe(IDMsgs.Error.NotFound<Team>(nonExistantTeamId));
//    }

//    //------------------------------------//

//    [Fact]
//    public async Task GetServiceAsync_SubscriptionNotFound_ShouldReturnNotFound()
//    {
//        // Arrange
//        var team = TeamDataFactory.Create();
//        _uowMock.Setup(uow => uow.TeamRepo.GetByIdWithSubscriptionsAsync(It.IsAny<Guid?>()))
//            .ReturnsAsync(team);
//        var factory = new TeamDeviceServiceFactory(_uowMock.Object);
//        var nonExistantSubId = Guid.NewGuid();

//        // Act
//        var result = await factory.GetServiceAsync(Guid.NewGuid(), nonExistantSubId);

//        // Assert
//        result.NotFound.ShouldBeTrue();
//        result.Info.ShouldBe(IDMsgs.Error.NotFound<TeamSubscription>(nonExistantSubId));

//    }

//    //------------------------------------//

//    [Fact]
//    public async Task GetServiceAsync_ValidInput_ShouldReturnService()
//    {
//        // Arrange
//        var subId = Guid.NewGuid();
//        var teamId = Guid.NewGuid();
//        var subscription = SubscriptionDataFactory.Create(subId, null, null, null, null, null, 0);
//        var team = TeamDataFactory.Create(teamId, null, null, [subscription]);

//        _uowMock.Setup(uow => uow.TeamRepo.GetByIdWithSubscriptionsAsync(It.IsAny<Guid?>()))
//            .ReturnsAsync(team);
//        var factory = new TeamDeviceServiceFactory(_uowMock.Object);

//        // Act
//        var result = await factory.GetServiceAsync(Guid.NewGuid(), subId);

//        // Assert
//        result.Succeeded.ShouldBeTrue();
//        result.Value.ShouldBeOfType<TeamDeviceService>();
//    }

//    //------------------------------------//

//}//Cls
