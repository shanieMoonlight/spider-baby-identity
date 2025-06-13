using ID.Infrastructure.Persistance.Abstractions.Repos;
using Moq;

namespace ID.Infrastructure.Tests.DmnSevcs.SubsService;
public abstract class BaseSubServiceTests
{
    internal readonly Mock<IIdentityTeamRepo> _teamRepoMock;
    internal readonly Mock<IIdUnitOfWork> _uowMock;
    internal readonly Mock<IIdentitySubscriptionPlanRepo> _subPlanRepoMock;

    //- - - - - - - - - - - - - - - - - - //

    public BaseSubServiceTests()
    {
        _uowMock = new Mock<IIdUnitOfWork>();
        _teamRepoMock = new Mock<IIdentityTeamRepo>();
        _subPlanRepoMock = new Mock<IIdentitySubscriptionPlanRepo>();
        _uowMock.Setup(uow => uow.TeamRepo).Returns(_teamRepoMock.Object);
        _uowMock.Setup(uow => uow.SubscriptionPlanRepo).Returns(_subPlanRepoMock.Object);
        _uowMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).Verifiable();
    }

    //------------------------------------//
}
