using ID.Application.Features.FeatureFlags;
using ID.Application.Features.FeatureFlags.Qry.GetById;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;

namespace ID.Application.Tests.Features.FeatureFlags.Qry.GetById;

public class GetFeatureFlagByIdQryHandlerTests
{
    private readonly Mock<IIdentityFeatureFlagService> _mockRepo;

    //- - - - - - - - - - - - - - - - - - //

    public GetFeatureFlagByIdQryHandlerTests()
    {
        _mockRepo = new Mock<IIdentityFeatureFlagService>();
    }

    //--------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFeatureFlagDto_WhenExists()
    {
        // Arrange
        var featureFlagId = Guid.NewGuid();
        var expectedFeatureFlag = FeatureFlagDataFactory.Create(featureFlagId);
        _mockRepo.Setup(x => x.GetByIdAsync(featureFlagId, It.IsAny<CancellationToken>())).ReturnsAsync(expectedFeatureFlag);
        var handler = new GetFeatureFlagByIdQryHandler(_mockRepo.Object);

        // Act
        var result = await handler.Handle(new GetFeatureFlagByIdQry(featureFlagId), CancellationToken.None);

        // Assert
        Assert.IsType<GenResult<FeatureFlagDto>>(result);
        Assert.NotNull(result.Value);
        Assert.Equal(featureFlagId, result.Value.Id); // Assuming Id is mapped to Dto
    }

    //--------------------------//

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenFeatureFlagDoesNotExist()
    {
        // Arrange
        var expectedFeatureFlag = FeatureFlagDataFactory.Create();
        var featureFlagId = expectedFeatureFlag.Id;
        _mockRepo.Setup(x => x.GetByIdAsync(featureFlagId, It.IsAny<CancellationToken>())).ReturnsAsync((FeatureFlag?)null);
        var handler = new GetFeatureFlagByIdQryHandler(_mockRepo.Object);


        // Act
        var result = await handler.Handle(new GetFeatureFlagByIdQry(featureFlagId), CancellationToken.None);

        // Assert
        Assert.IsType<GenResult<FeatureFlagDto>>(result);
        Assert.False(result.Succeeded);
        Assert.True(result.NotFound);
        Assert.Equal(IDMsgs.Error.NotFound<FeatureFlag>(featureFlagId), result.Info);
    }

}//Cls