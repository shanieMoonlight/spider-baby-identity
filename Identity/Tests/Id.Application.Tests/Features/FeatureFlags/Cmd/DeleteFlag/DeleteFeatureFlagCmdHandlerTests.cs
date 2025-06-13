using ID.Application.Features.FeatureFlags.Cmd.Delete;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using ID.Domain.Utility.Messages;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Features.FeatureFlags.Cmd.DeleteFlag;

public class DeleteFeatureFlagCmdHandlerTests
{
    private readonly Mock<IIdentityFeatureFlagService> _mockRepo;
    private readonly DeleteFeatureFlagCmdHandler _handler;

    //- - - - - - - - - - - - - - - - - - //

    public DeleteFeatureFlagCmdHandlerTests()
    {
        _mockRepo = new Mock<IIdentityFeatureFlagService>();
        _handler = new DeleteFeatureFlagCmdHandler(_mockRepo.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenDeletesSuccessfully()
    {
        // Arrange
        var featureFlagId = Guid.NewGuid();
        _mockRepo.Setup(repo => repo.DeleteAsync(It.IsAny<Guid>()))
                 .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(new DeleteFeatureFlagCmd(featureFlagId), CancellationToken.None);

        // Assert
        result.ShouldBeOfType<BasicResult>();
        result.Succeeded.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Info.Deleted<FeatureFlag>(featureFlagId));
    }

    //------------------------------------//

    //[Fact]
    //public async Task Handle_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    //{
    //    // Arrange
    //    var featureFlagId = Guid.NewGuid();
    //    _mockRepo.Setup(repo => repo.DeleteAsync(It.IsAny<Guid>()))
    //             .ThrowsAsync(new Exception("Error deleting feature flag"));

    //    // Act
    //    var result = await _handler.Handle(new DeleteFeatureFlagCmd(featureFlagId), CancellationToken.None);

    //    // Assert
    //    result.ShouldBeOfType<BasicResult>();
    //    result.Succeeded.ShouldBeFalse();
    //    result.StatusCode.ShouldBe(StatusCodes.InternalServerError);
    //}

    //------------------------------------//

}