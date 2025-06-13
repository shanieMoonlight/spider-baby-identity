using ID.Tests.Data.Factories;
using NSubstitute;
using Shouldly;
using ID.Application.Features.OutboxMessages;
using ID.Application.Features.OutboxMessages.Qry.GetAllByType;
using ID.Domain.Abstractions.Services.Outbox;

namespace ID.Application.Tests.Features.OutboxMsgs.Qry.GetAllByType;
public class GetAllOutboxMessageByTypeQryHandlerTests
{
    private readonly IIdentityOutboxMsgsService _mockRepo;
    private readonly GetAllOutboxMessagesFilteredQryHandler _handler;

    //- - - - - - - - - - - - - - - - - - // 

    public GetAllOutboxMessageByTypeQryHandlerTests()
    {
        _mockRepo = Substitute.For<IIdentityOutboxMsgsService>();
        _handler = new GetAllOutboxMessagesFilteredQryHandler(_mockRepo);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnOutboxMessageDtos_WhenMatched()
    {
        // Arrange
        var outboxMsgType = "MyOutboxMessage_Type";
        var expectedCount = 5;

        _mockRepo.GetAllByTypeAsync(outboxMsgType).Returns(OutboxMessageDataFactory.CreateMany(expectedCount));

        // Act
        var result = await _handler.Handle(new GetAllOutboxMessagesByTypeQry(outboxMsgType), CancellationToken.None);

        // Assert
        result.Value.ShouldBeAssignableTo<IEnumerable<IdOutboxMessageDto>>();
        result.Value.ShouldNotBeNull();
        result.Value.Count().ShouldBe(expectedCount);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnEmptyEnumerable_WhenNoMatched()
    {
        // Arrange
        var outboxMsgType = "MyOutboxMessage_Type";

        _mockRepo.GetAllByTypeAsync(outboxMsgType).Returns([]);

        // Act
        var result = await _handler.Handle(new GetAllOutboxMessagesByTypeQry(outboxMsgType), CancellationToken.None);

        // Assert
        result.Value.ShouldNotBeNull();
        result.Value.Count().ShouldBe(0);
        result.Value.ShouldBeAssignableTo<IEnumerable<IdOutboxMessageDto>>();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        var request = new GetAllOutboxMessagesByTypeQry("");

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Value.ShouldBeAssignableTo<IEnumerable<IdOutboxMessageDto>>();
        result.Value?.Count().ShouldBe(0);
        result.Succeeded.ShouldBeTrue();
        result.NotFound.ShouldBeFalse();
    }

    //------------------------------------//


}
