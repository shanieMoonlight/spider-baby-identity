using Moq;
using MyResults;
using Shouldly;
using ID.Application.Features.SubscriptionPlans.Cmd.Delete;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Abstractions.Services.SubPlans;

namespace ID.Application.Tests.Features.SubscriptionPlans.Cmd.Delete;

public class DeleteSubscriptionPlanCmdHandlerTests
{
    private readonly Mock<IIdentitySubscriptionPlanService> _mockRepo;
    private readonly DeleteSubscriptionPlanCmdHandler _handler;

    //- - - - - - - - - - - - - - - - - - //

    public DeleteSubscriptionPlanCmdHandlerTests()
    {
        _mockRepo = new Mock<IIdentitySubscriptionPlanService>();
        _handler = new DeleteSubscriptionPlanCmdHandler(_mockRepo.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenDeletesSuccessfully()
    {
        // Arrange
        var Id = Guid.NewGuid();
        _mockRepo.Setup(repo => repo.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(new DeleteSubscriptionPlanCmd(Id), CancellationToken.None);

        // Assert
        result.ShouldBeOfType<BasicResult>();
        result.Succeeded.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Info.Deleted<SubscriptionPlan>(Id));
    }

    //------------------------------------//

    //[Fact]
    //public async Task Handle_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    //{
    //    // Arrange
    //    var Id = Guid.NewGuid();
    //    _mockRepo.Setup(repo => repo.DeleteAsync(It.IsAny<Guid>()))
    //             .ThrowsAsync(new Exception("Error deleting feature flag"));

    //    // Act
    //    var result = await _handler.Handle(new DeleteSubscriptionPlanCmd(Id), CancellationToken.None);

    //    // Assert
    //    result.ShouldBeOfType<BasicResult>();
    //    result.Succeeded.ShouldBeFalse();
    //    result.StatusCode.ShouldBe(StatusCodes.InternalServerError);
    //}

    //------------------------------------//

}