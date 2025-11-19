using ID.Application.Features.FeatureFlags;
using ID.Application.Features.FeatureFlags.Qry.GetAll;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using Moq;
using System.Collections.Generic;
using System.Linq;
using MediatR;

namespace ID.Application.Tests.Features.FeatureFlags.Qry.GetAll;

public class GetAllFeatureFlagsQryHandlerTests
{
    private readonly Mock<IIdentityFeatureFlagService> _repoMock;
    private readonly Mock<IMediator> _mediatorMock;

    //--------------------------//

    public GetAllFeatureFlagsQryHandlerTests()
    {
        _repoMock = new Mock<IIdentityFeatureFlagService>();
        _mediatorMock = new Mock<IMediator>();
    }

    //--------------------------//

    [Fact]
    public async Task Handle_ShouldReturnAllFeatureFlags_WhenSuccessful()
    {
        // Arrange
        var mdls = FeatureFlagDataFactory.CreateMany();
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync((IReadOnlyList<FeatureFlag>)mdls);

        var handler = new GetAllFeatureFlagsQryHandler(_repoMock.Object);
        var request = new GetAllFeatureFlagsQry();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<GenResult<IEnumerable<FeatureFlagDto>>>();
        result.Value?.Count().ShouldBe(mdls.Count);
        _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    //--------------------------//

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoFeatureFlagsExist()
    {
        // Arrange
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync((IReadOnlyList<FeatureFlag>)[]);

        var handler = new GetAllFeatureFlagsQryHandler(_repoMock.Object);
        var request = new GetAllFeatureFlagsQry();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<GenResult<IEnumerable<FeatureFlagDto>>>();
        result.Value.ShouldBeEmpty();
        _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    //--------------------------//

}//Cls