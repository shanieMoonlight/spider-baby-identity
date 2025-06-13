using ID.Application.Features.OutboxMessages.Qry.GetById;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Tests.Data.Factories;
using MassTransit;
using MyResults;
using NSubstitute;
using Shouldly;
using ID.Application.Features.OutboxMessages;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.OutboxMessages;
using ID.Domain.Abstractions.Services.Outbox;

namespace ID.Application.Tests.Features.OutboxMsgs.Qry.GetById;

public class GetOutboxMessageByIdQryHandlerTests
{
    private readonly IIdentityOutboxMsgsService _mockRepo;

    //- - - - - - - - - - - - - - - - - - // 

    public GetOutboxMessageByIdQryHandlerTests() =>
        _mockRepo = Substitute.For<IIdentityOutboxMsgsService>();

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnOutboxMessageDto_WhenExists()
    {
        // Arrange
        var outboxMsgId = NewId.NextGuid();
        var expectedOutboxMessage = OutboxMessageDataFactory.Create(id: outboxMsgId);
        _mockRepo.GetByIdAsync(outboxMsgId).Returns(expectedOutboxMessage);
        var handler = new GetOutboxMessageByIdQryHandler(_mockRepo);

        // Act
        var result = await handler.Handle(new GetOutboxMessageByIdQry(outboxMsgId), CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<IdOutboxMessageDto>>();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(outboxMsgId); // Assuming Id is mapped to Dto
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenOutboxMessageDoesNotExist()
    {
        // Arrange
        var expectedOutboxMessage = OutboxMessageDataFactory.Create();
        var outboxMsgId = expectedOutboxMessage.Id;
        _mockRepo.GetByIdAsync(outboxMsgId).Returns((IdOutboxMessage?)null);
        var handler = new GetOutboxMessageByIdQryHandler(_mockRepo);

        // Act
        var result = await handler.Handle(new GetOutboxMessageByIdQry(outboxMsgId), CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<IdOutboxMessageDto>>();
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.NotFound<IdOutboxMessage>(outboxMsgId));
    }

    //------------------------------------//

}//Cls
