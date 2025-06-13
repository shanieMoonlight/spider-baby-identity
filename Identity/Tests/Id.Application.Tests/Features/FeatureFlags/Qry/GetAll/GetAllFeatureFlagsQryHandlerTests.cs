using ID.Application.Features.FeatureFlags;
using ID.Application.Features.FeatureFlags.Qry.GetAll;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Tests.Data.Factories;
using MediatR;
using MyResults;
using NSubstitute;
using Shouldly;

namespace ID.Application.Tests.Features.FeatureFlags.Qry.GetAll;

public class GetAllFeatureFlagsQryHandlerTests
{
    private readonly IIdentityFeatureFlagService _repoMock;
    private readonly IMediator _mediatorMock;

    //------------------------------------//

    public GetAllFeatureFlagsQryHandlerTests()
    {
        _repoMock = Substitute.For<IIdentityFeatureFlagService>();
        _mediatorMock = Substitute.For<IMediator>();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnAllFeatureFlags_WhenSuccessful()
    {
        // Arrange
        var mdls = FeatureFlagDataFactory.CreateMany();
        _repoMock.GetAllAsync().Returns(mdls);

        var handler = new GetAllFeatureFlagsQryHandler(_repoMock);
        var request = new GetAllFeatureFlagsQry();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<GenResult<IEnumerable<FeatureFlagDto>>>();
        result.Value?.Count().ShouldBe(mdls.Count);
        await _repoMock.Received().GetAllAsync();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoFeatureFlagsExist()
    {
        // Arrange
        _repoMock.GetAllAsync().Returns([]);

        var handler = new GetAllFeatureFlagsQryHandler(_repoMock);
        var request = new GetAllFeatureFlagsQry();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<GenResult<IEnumerable<FeatureFlagDto>>>();
        result.Value.ShouldBeEmpty();
        await _repoMock.Received().GetAllAsync();
    }

    //------------------------------------//

}//Cls