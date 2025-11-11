using ID.Application.Features.OutboxMessages;
using ID.Application.Features.OutboxMessages.Qry.GetAll;
using ID.Domain.Abstractions.Services.Outbox;
using ID.Domain.Entities.OutboxMessages;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace ID.Application.Tests.Features.OutboxMsgs.Qry.GetAll;

public class GetAllOutboxMessagesQryHandlerTests
{
    private readonly Mock<IIdentityOutboxMsgsService> _repoMock;

    //------------------------------------//

    public GetAllOutboxMessagesQryHandlerTests() => 
        _repoMock = new Mock<IIdentityOutboxMsgsService>();

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnAllOutboxMessages_WhenSuccessful()
    {
        // Arrange
        var mdls = IdOutboxMessageDataFactory.CreateMany();
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(mdls);

        var handler = new GetAllOutboxMessagesQryHandler(_repoMock.Object);
        var request = new GetAllOutboxMessagesQry();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<GenResult<IEnumerable<IdOutboxMessageDto>>>();
        result.Value?.Count().ShouldBe(mdls.Count);
        _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoOutboxMessagesExist()
    {
        // Arrange
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync((IReadOnlyList<IdOutboxMessage>)[]);

        var handler = new GetAllOutboxMessagesQryHandler(_repoMock.Object);
        var request = new GetAllOutboxMessagesQry();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<GenResult<IEnumerable<IdOutboxMessageDto>>>();
        result.Value.ShouldBeEmpty();
        _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    //------------------------------------//

}//Cls