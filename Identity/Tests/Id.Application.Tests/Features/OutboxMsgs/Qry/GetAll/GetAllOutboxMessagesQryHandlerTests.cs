using ID.Application.Features.OutboxMessages.Qry.GetAll;
using ID.Tests.Data.Factories;
using MyResults;
using NSubstitute;
using Shouldly;
using ID.Application.Features.OutboxMessages;
using ID.Domain.Abstractions.Services.Outbox;

namespace ID.Application.Tests.Features.OutboxMsgs.Qry.GetAll;

public class GetAllOutboxMessagesQryHandlerTests
{
    private readonly IIdentityOutboxMsgsService _repoMock;

    //------------------------------------//

    public GetAllOutboxMessagesQryHandlerTests() => 
        _repoMock = Substitute.For<IIdentityOutboxMsgsService>();

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnAllOutboxMessages_WhenSuccessful()
    {
        // Arrange
        var mdls = OutboxMessageDataFactory.CreateMany();
        _repoMock.GetAllAsync().Returns(mdls);

        var handler = new GetAllOutboxMessagesQryHandler(_repoMock);
        var request = new GetAllOutboxMessagesQry();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<GenResult<IEnumerable<IdOutboxMessageDto>>>();
        result.Value?.Count().ShouldBe(mdls.Count);
        await _repoMock.Received().GetAllAsync();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoOutboxMessagesExist()
    {
        // Arrange
        _repoMock.GetAllAsync().Returns([]);

        var handler = new GetAllOutboxMessagesQryHandler(_repoMock);
        var request = new GetAllOutboxMessagesQry();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<GenResult<IEnumerable<IdOutboxMessageDto>>>();
        result.Value.ShouldBeEmpty();
        await _repoMock.Received().GetAllAsync();
    }

    //------------------------------------//

}//Cls