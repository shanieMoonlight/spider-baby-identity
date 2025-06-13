using ID.Application.Features.FeatureFlags;
using ID.Application.Features.FeatureFlags.Qry.GetById;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using ID.Domain.Utility.Messages;
using ID.Tests.Data.Factories;
using MassTransit;
using MediatR;
using Moq;
using MyResults;
using NSubstitute;

namespace ID.Application.Tests.Features.FeatureFlags.Qry.GetById;

public class GetFeatureFlagByIdQryHandlerTests
{
    private readonly IIdentityFeatureFlagService _mockRepo;
    private readonly IMediator _mockMediator;
    private readonly GetFeatureFlagByIdQryHandler _handler;

    //- - - - - - - - - - - - - - - - - - //

    public GetFeatureFlagByIdQryHandlerTests()
    {
        _mockRepo = Substitute.For<IIdentityFeatureFlagService>();
        _mockMediator = Substitute.For<IMediator>();
        _handler = new GetFeatureFlagByIdQryHandler(_mockRepo);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFeatureFlagDto_WhenExists()
    {
        // Arrange
        var featureFlagId = NewId.NextSequentialGuid();
        var expectedFeatureFlag = FeatureFlagDataFactory.Create(featureFlagId);
        _mockRepo.GetByIdAsync(featureFlagId, It.IsAny<CancellationToken>()).Returns(expectedFeatureFlag);
        var handler = new GetFeatureFlagByIdQryHandler(_mockRepo);

        // Act
        var result = await handler.Handle(new GetFeatureFlagByIdQry(featureFlagId), CancellationToken.None);

        // Assert
        Assert.IsType<GenResult<FeatureFlagDto>>(result);
        Assert.NotNull(result.Value);
        Assert.Equal(featureFlagId, result.Value.Id); // Assuming Id is mapped to Dto
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenFeatureFlagDoesNotExist()
    {
        // Arrange
        var expectedFeatureFlag = FeatureFlagDataFactory.Create();
        var featureFlagId = expectedFeatureFlag.Id;
        _mockRepo.GetByIdAsync(featureFlagId).Returns((FeatureFlag?)null);
        var handler = new GetFeatureFlagByIdQryHandler(_mockRepo);


        // Act
        var result = await handler.Handle(new GetFeatureFlagByIdQry(featureFlagId), CancellationToken.None);

        // Assert
        Assert.IsType<GenResult<FeatureFlagDto>>(result);
        Assert.False(result.Succeeded);
        Assert.True(result.NotFound);
        Assert.Equal(IDMsgs.Error.NotFound<FeatureFlag>(featureFlagId), result.Info);
    }

    //------------------------------------//

}//Cls