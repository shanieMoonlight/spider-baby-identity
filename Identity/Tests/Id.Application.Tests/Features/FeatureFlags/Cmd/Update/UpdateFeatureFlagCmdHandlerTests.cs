using ID.Application.Features.FeatureFlags;
using ID.Application.Features.FeatureFlags.Cmd.Update;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using ID.Domain.Utility.Messages;
using ID.Tests.Data.Factories;
using ID.Tests.Data.Factories.Dtos;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Features.FeatureFlags.Cmd.Update;

public class UpdateFeatureFlagCmdHandlerTests
{
    private readonly Mock<IIdentityFeatureFlagService> _mockRepo;
    private readonly UpdateFeatureFlagCmdHandler _handler;

    //- - - - - - - - - - - - - - - - - - //

    public UpdateFeatureFlagCmdHandlerTests()
    {
        _mockRepo = new Mock<IIdentityFeatureFlagService>();
        _handler = new UpdateFeatureFlagCmdHandler(_mockRepo.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenUpdateSucceeds()
    {
        // Arrange

        var featureFlagId = Guid.NewGuid();

        var original = FeatureFlagDataFactory.Create(
           featureFlagId,
           "FeatureFlagName_ORIGINAL",
           "FeatureFlagDescription_ORIGINAL");


        var requestDto = FeatureFlagDtoDataFactory.Create(
           featureFlagId,
           "FeatureFlagName_NEWNEWNEW",
           "FeatureFlagDescription_NEWNEWNEW");

        var model = requestDto.ToModel();
        var featureFlagModel = requestDto.ToModel();

        _mockRepo.Setup(repo => repo.GetByIdAsync(featureFlagId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(original);

        _mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<FeatureFlag>()))
                 .ReturnsAsync((FeatureFlag entity) => entity);

        // Act
        var result = await _handler.Handle(new UpdateFeatureFlagCmd(requestDto), CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<FeatureFlagDto>>();
        result.Value!.Name.ShouldBeEquivalentTo(requestDto.Name);
        result.Value!.Description.ShouldBeEquivalentTo(requestDto.Description);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenDtoIsNull()
    {
        // Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var request = new UpdateFeatureFlagCmd(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<FeatureFlagDto>>();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(IDMsgs.Error.NO_DATA_SUPPLIED);
        result.BadRequest.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenFeatureFlagNotFound()
    {
        // Arrange
        var featureFlagDto = new FeatureFlagDto { Id = Guid.NewGuid() };

        _mockRepo.Setup(repo => repo.GetByIdAsync(featureFlagDto.Id, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((FeatureFlag?)null);

        // Act
        var result = await _handler.Handle(new UpdateFeatureFlagCmd(featureFlagDto), CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<FeatureFlagDto>>();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(IDMsgs.Error.NotFound<FeatureFlag>(featureFlagDto.Id));
    }

    //------------------------------------//

    // Add a test case for any specific error handling you have in the Update method.
}