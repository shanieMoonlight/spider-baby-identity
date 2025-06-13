using ID.Tests.Data.Factories;
using MassTransit;
using Moq;
using MyResults;
using NSubstitute;
using ID.Application.Features.FeatureFlags;
using ID.Application.Features.FeatureFlags.Qry.GetByName;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Application.Features.FeatureFlags.Qry.GetAllByName;

namespace ID.Application.Tests.Features.FeatureFlags.Qry.GetByName;
public class GetFeatureFlagByNameQryHandlerTests
{
    private readonly IIdentityFeatureFlagService _mockRepo;
    private readonly GetFeatureFlagByNameQryHandler _handler;

    //- - - - - - - - - - - - - - - - - - //

    public GetFeatureFlagByNameQryHandlerTests()
    {
        _mockRepo = Substitute.For<IIdentityFeatureFlagService>();
        _handler = new GetFeatureFlagByNameQryHandler(_mockRepo);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFeatureFlagDto_WhenExists()
    {
        // Arrange
        var featureFlagId = NewId.NextSequentialGuid();
        var featureFlagName = "MyFeatureFlag";
        var featureFlagNameDescription = "MyFeatureFlag_Description";
        var expectedFeatureFlag = FeatureFlagDataFactory.Create(
            featureFlagId,
            featureFlagName,
            featureFlagNameDescription);

        _mockRepo.GetByNameAsync(featureFlagName, It.IsAny<CancellationToken>()).Returns(expectedFeatureFlag);

        // Act
        var result = await _handler.Handle(new GetFeatureFlagByNameQry(featureFlagName), CancellationToken.None);

        // Assert
        Assert.IsType<GenResult<FeatureFlagDto>>(result);
        Assert.NotNull(result.Value);
        Assert.Equal(featureFlagId, result.Value.Id);  // Assuming Id is mapped to Dto
        Assert.Equal(featureFlagName, result.Value.Name);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenFeatureFlagDoesNotExist()
    {
        // Arrange
        var featureFlagName = "NonExistentFeature";
        _mockRepo.GetByNameAsync(featureFlagName, It.IsAny<CancellationToken>())
            .Returns((FeatureFlag?)null);

        // Act
        var result = await _handler.Handle(new GetFeatureFlagByNameQry(featureFlagName), CancellationToken.None);

        // Assert
        Assert.IsType<GenResult<FeatureFlagDto>>(result);
        Assert.False(result.Succeeded);
        Assert.True(result.NotFound);
        Assert.Equal(IDMsgs.Error.NotFound<FeatureFlag>(featureFlagName), result.Info);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        var request = new GetFeatureFlagByNameQry("");

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.IsType<GenResult<FeatureFlagDto>>(result);
        Assert.False(result.Succeeded);
        Assert.True(result.NotFound);
    }

    //------------------------------------//

}
