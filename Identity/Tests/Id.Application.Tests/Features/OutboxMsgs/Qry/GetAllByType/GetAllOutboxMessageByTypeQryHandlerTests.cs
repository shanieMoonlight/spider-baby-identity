using ID.Application.Features.OutboxMessages;
using ID.Application.Features.OutboxMessages.Qry.GetAllByType;
using ID.Domain.Abstractions.Services.Outbox;
using ID.Domain.Entities.OutboxMessages;

namespace ID.Application.Tests.Features.OutboxMsgs.Qry.GetAllByType;

public class GetAllOutboxMessageByTypeQryHandlerTests
{
    private readonly Mock<IIdentityOutboxMsgsService> _mockRepo;
    private readonly GetAllOutboxMessagesFilteredQryHandler _handler;

    //- - - - - - - - - - - - - - - - - - // 

    public GetAllOutboxMessageByTypeQryHandlerTests()
    {
        _mockRepo = new Mock<IIdentityOutboxMsgsService>();
        _handler = new GetAllOutboxMessagesFilteredQryHandler(_mockRepo.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnOutboxMessageDtos_WhenMatched()
    {
        // Arrange
        var outboxMsgType = "MyOutboxMessage_Type";
        var expectedCount = 5;

        _mockRepo.Setup(r => r.GetAllByTypeAsync(outboxMsgType)).ReturnsAsync(IdOutboxMessageDataFactory.CreateMany(expectedCount));

        // Act
        var result = await _handler.Handle(new GetAllOutboxMessagesByTypeQry(outboxMsgType), CancellationToken.None);

        // Assert
        result.Value.ShouldBeAssignableTo<IEnumerable<IdOutboxMessageDto>>();
        result.Value.ShouldNotBeNull();
        result.Value.Count().ShouldBe(expectedCount);
        _mockRepo.Verify(r => r.GetAllByTypeAsync(outboxMsgType), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnEmptyEnumerable_WhenNoMatched()
    {
        // Arrange
        var outboxMsgType = "MyOutboxMessage_Type";

        _mockRepo.Setup(r => r.GetAllByTypeAsync(outboxMsgType)).ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(new GetAllOutboxMessagesByTypeQry(outboxMsgType), CancellationToken.None);

        // Assert
        result.Value.ShouldNotBeNull();
        result.Value.Count().ShouldBe(0);
        result.Value.ShouldBeAssignableTo<IEnumerable<IdOutboxMessageDto>>();
        _mockRepo.Verify(r => r.GetAllByTypeAsync(outboxMsgType), Times.Once);
    }


}//Cls
