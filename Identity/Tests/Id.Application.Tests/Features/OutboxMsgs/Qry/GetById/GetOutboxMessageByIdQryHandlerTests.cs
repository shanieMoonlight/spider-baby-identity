using ID.Application.Features.OutboxMessages;
using ID.Application.Features.OutboxMessages.Qry.GetById;
using ID.Domain.Abstractions.Services.Outbox;
using ID.Domain.Entities.OutboxMessages;
using ID.Domain.Utility.Messages;


namespace ID.Application.Tests.Features.OutboxMsgs.Qry.GetById;

public class GetOutboxMessageByIdQryHandlerTests
{
    private readonly Mock<IIdentityOutboxMsgsService> _mockRepo;

    //- - - - - - - - - - - - - - - - - - // 

    public GetOutboxMessageByIdQryHandlerTests() =>
        _mockRepo = new Mock<IIdentityOutboxMsgsService>();

    //--------------------------//

    [Fact]
    public async Task Handle_ShouldReturnOutboxMessageDto_WhenExists()
    {
        // Arrange
        var outboxMsgId = Guid.NewGuid();
        var expectedOutboxMessage = IdOutboxMessageDataFactory.Create(id: outboxMsgId);

        _mockRepo.Setup(x => x.GetByIdAsync(outboxMsgId))
          .ReturnsAsync(expectedOutboxMessage);

        var handler = new GetOutboxMessageByIdQryHandler(_mockRepo.Object);

        // Act
        var result = await handler.Handle(new GetOutboxMessageByIdQry(outboxMsgId), CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<IdOutboxMessageDto>>();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(outboxMsgId); // Assuming Id is mapped to Dto
    }

    //--------------------------//

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenOutboxMessageDoesNotExist()
    {
        // Arrange
        var expectedOutboxMessage = IdOutboxMessageDataFactory.Create();
        var outboxMsgId = expectedOutboxMessage.Id;
        _mockRepo.Setup(x => x.GetByIdAsync(outboxMsgId))
          .ReturnsAsync((IdOutboxMessage?)null);
        var handler = new GetOutboxMessageByIdQryHandler(_mockRepo.Object);

        // Act
        var result = await handler.Handle(new GetOutboxMessageByIdQry(outboxMsgId), CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<IdOutboxMessageDto>>();
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.NotFound<IdOutboxMessage>(outboxMsgId));
    }


}//Cls
