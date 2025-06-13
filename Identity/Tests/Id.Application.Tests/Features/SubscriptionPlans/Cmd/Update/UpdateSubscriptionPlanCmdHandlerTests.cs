using ID.Application.Features.SubscriptionPlans;
using ID.Application.Features.SubscriptionPlans.Cmd.Update;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Utility.Messages;
using ID.Tests.Data.Factories;
using ID.Tests.Data.Factories.Dtos;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Features.SubscriptionPlans.Cmd.Update;

public class Update__modelClassName__CmdHandlerTests
{
    private readonly Mock<IIdentitySubscriptionPlanService> _mockRepo;
    private readonly UpdateSubscriptionPlanCmdHandler _handler;

    //- - - - - - - - - - - - - - - - - - //

    public Update__modelClassName__CmdHandlerTests()
    {
        _mockRepo = new Mock<IIdentitySubscriptionPlanService>();
        _handler = new UpdateSubscriptionPlanCmdHandler(_mockRepo.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenUpdateSucceeds()
    {
        // Arrange

        var Id = Guid.NewGuid();

        var original = SubscriptionPlanDataFactory.Create(
           Id,
           "SubscriptionPlanName_ORIGINAL",
           "SubscriptionPlanDescription_ORIGINAL");


        var requestDto = SubscriptionPlanDtoDataFactory.Create(
           Id,
           "SubscriptionPlanName_NEWNEWNEW",
           "SubscriptionPlanDescription_NEWNEWNEW");

        var model = requestDto.ToModel();

        _mockRepo.Setup(repo => repo.GetByIdWithFeatureFlagsAsync(Id))
                 .ReturnsAsync(original);

        _mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(model);

        // Act
        var result = await _handler.Handle(new UpdateSubscriptionPlanCmd(requestDto), CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<SubscriptionPlanDto>>();
        result.Value!.Name.ShouldBeEquivalentTo(requestDto.Name);
        result.Value!.Description.ShouldBeEquivalentTo(requestDto.Description);
    }

    //------------------------------------//
    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenSubscriptionPlanNotFound()
    {
        // Arrange
        var Dto = new SubscriptionPlanDto { Id = Guid.NewGuid() };

        _mockRepo.Setup(repo => repo.GetByIdWithFeatureFlagsAsync(Dto.Id))
                 .ReturnsAsync((SubscriptionPlan?)null);

        // Act
        var result = await _handler.Handle(new UpdateSubscriptionPlanCmd(Dto), CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<SubscriptionPlanDto>>();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(IDMsgs.Error.NotFound<SubscriptionPlan>(Dto.Id));
    }

    //------------------------------------//

    // Add a test case for any specific error handling you have in the Update method.
}