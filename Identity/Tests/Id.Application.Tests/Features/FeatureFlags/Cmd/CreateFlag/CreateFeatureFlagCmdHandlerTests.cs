using ID.Application.Features.FeatureFlags;
using ID.Application.Features.FeatureFlags.Cmd.Create;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using ID.Tests.Data.Factories.Dtos;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Features.FeatureFlags.Cmd.CreateFlag;

public class CreateFeatureFlagCmdHandlerTests
{
    private readonly Mock<IIdentityFeatureFlagService> _mockRepo;
    private readonly CreateFeatureFlagCmdHandler _handler;

    //- - - - - - - - - - - - - - - - - - //

    public CreateFeatureFlagCmdHandlerTests()
    {
        _mockRepo = new Mock<IIdentityFeatureFlagService>();
        _handler = new CreateFeatureFlagCmdHandler(_mockRepo.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldCreateFeatureFlag_WhenDtoIsValid()
    {
        // Arrange
        var requestDto = FeatureFlagDtoDataFactory.Create(
            null,
            "FeatureFlagName",
            "FeatureFlagDescription");
        var model = requestDto.ToModel();



        _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<FeatureFlag>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((FeatureFlag mdl, CancellationToken cancellationToken) => model);

        // Act
        var result = await _handler.Handle(new CreateFeatureFlagCmd(requestDto), CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<FeatureFlagDto>>();
        result.Value!.Name.ShouldBeEquivalentTo(requestDto.Name);
        result.Value!.Description.ShouldBeEquivalentTo(requestDto.Description);
    }

    //------------------------------------//

}//Cls