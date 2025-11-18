using ID.Application.Features.SubscriptionPlans;
using ID.Application.Features.SubscriptionPlans.Qry.GetAll;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Entities.SubscriptionPlans;
using Moq;
using System.Collections.Generic;

namespace ID.Application.Tests.Features.SubscriptionPlans.Qry.GetAll;

public class GetAllSubscriptionPlansQryHandlerTests
{
    private readonly Mock<IIdentitySubscriptionPlanService> _repoMock;

    //- - - - - - - - - - - - - - - - - - //

    public GetAllSubscriptionPlansQryHandlerTests()
    {
        _repoMock = new Mock<IIdentitySubscriptionPlanService>();
    }

    //--------------------------//

    [Fact]
    public async Task Handle_ShouldReturnAllSubscriptionPlans_WhenSuccessful()
    {
        // Arrange
        var mdls = SubscriptionPlanDataFactory.CreateMany();
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync((IReadOnlyList<SubscriptionPlan>)mdls);

        var handler = new GetAllSubscriptionPlansQryHandler(_repoMock.Object);
        var request = new GetAllSubscriptionPlansQry();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<GenResult<IEnumerable<SubscriptionPlanDto>>>();
        result.Value?.Count().ShouldBe(mdls.Count);
        _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    //--------------------------//

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoSubscriptionPlansExist()
    {
        // Arrange
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync((IReadOnlyList<SubscriptionPlan>)[]);

        var handler = new GetAllSubscriptionPlansQryHandler(_repoMock.Object);
        var request = new GetAllSubscriptionPlansQry();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<GenResult<IEnumerable<SubscriptionPlanDto>>>();
        result.Value.ShouldBeEmpty();
        _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    //--------------------------//

}//Cls