using ID.Application.Features.SubscriptionPlans.Qry.GetAll;
using ID.Tests.Data.Factories;
using MyResults;
using NSubstitute;
using Shouldly;
using ID.Application.Features.SubscriptionPlans;
using ID.Domain.Abstractions.Services.SubPlans;

namespace ID.Application.Tests.Features.SubscriptionPlans.Qry.GetAll;

public class GetAllSubscriptionPlansQryHandlerTests
{
    private readonly IIdentitySubscriptionPlanService _repoMock;

    //- - - - - - - - - - - - - - - - - - //

    public GetAllSubscriptionPlansQryHandlerTests()
    {
        _repoMock = Substitute.For<IIdentitySubscriptionPlanService>();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnAllSubscriptionPlans_WhenSuccessful()
    {
        // Arrange
        var mdls = SubscriptionPlanDataFactory.CreateMany();
        _repoMock.GetAllAsync().Returns(mdls);

        var handler = new GetAllSubscriptionPlansQryHandler(_repoMock);
        var request = new GetAllSubscriptionPlansQry();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<GenResult<IEnumerable<SubscriptionPlanDto>>>();
        result.Value?.Count().ShouldBe(mdls.Count);
        await _repoMock.Received().GetAllAsync();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoSubscriptionPlansExist()
    {
        // Arrange
        _repoMock.GetAllAsync().Returns([]);

        var handler = new GetAllSubscriptionPlansQryHandler(_repoMock);
        var request = new GetAllSubscriptionPlansQry();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<GenResult<IEnumerable<SubscriptionPlanDto>>>();
        result.Value.ShouldBeEmpty();
        await _repoMock.Received().GetAllAsync();
    }

    //------------------------------------//

}//Cls