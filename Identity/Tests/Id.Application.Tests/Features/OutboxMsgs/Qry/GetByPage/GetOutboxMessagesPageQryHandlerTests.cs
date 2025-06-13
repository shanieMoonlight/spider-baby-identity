using ID.Tests.Data.Factories;
using MyResults;
using NSubstitute;
using Pagination;
using Shouldly;
using ID.Application.Features.OutboxMessages;
using ID.Application.Features.OutboxMessages.Qry.GetPage;
using ID.Domain.Entities.OutboxMessages;
using ID.Domain.Abstractions.Services.Outbox;

namespace ID.Application.Tests.Features.OutboxMsgs.Qry.GetByPage;

public class GetOutboxMessagesPageQryHandlerTests
{
    private readonly IIdentityOutboxMsgsService _mockSvc;
    private readonly GetOutboxMessagePageQryHandler _handler;

    public GetOutboxMessagesPageQryHandlerTests()
    {
        _mockSvc = Substitute.For<IIdentityOutboxMsgsService>();
        _handler = new GetOutboxMessagePageQryHandler(_mockSvc);
    }

    //- - - - - - - - - - - - - - - - - - //

    [Fact]
    public async Task Handle_ShouldReturnPagedOutboxMessages_WhenRequestIsSuccessful()
    {
        // Arrange
        var pgReq = PagedRequest.Empty();
        var mdls = OutboxMessageDataFactory.CreateMany(pgReq.PageSize);
        var mdlsPage = new Page<IdOutboxMessage>(mdls, pgReq.PageNumber, pgReq.PageSize);
        var dtosPage = mdlsPage.Transform((d) => d.ToDto());

        var qry = new GetOutboxMessagePageQry(pgReq);

        PagedResponse<IdOutboxMessageDto> pagedResponse = new(dtosPage, pgReq);

        _mockSvc.GetPageAsync(pgReq.PageNumber, pgReq.PageSize, pgReq.SortList, pgReq.FilterList)
            .Returns(mdlsPage);

        // Act
        var result = await _handler.Handle(new GetOutboxMessagePageQry(pgReq), CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<PagedResponse<IdOutboxMessageDto>>>();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeEquivalentTo(pagedResponse);

    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnEmptyPagedResponse_WhenNoFeaturesExist()
    {
        // Arrange
        var pagedRequest = PagedRequest.Empty();
        var mdlsEmptyPage = new Page<IdOutboxMessage>([], pagedRequest.PageNumber, pagedRequest.PageSize);
        var qry = new GetOutboxMessagePageQry(pagedRequest);

        _mockSvc.GetPageAsync(pagedRequest.PageNumber, pagedRequest.PageSize, pagedRequest.SortList, pagedRequest.FilterList)
            .Returns(mdlsEmptyPage);

        // Act
        var result = await _handler.Handle(new GetOutboxMessagePageQry(pagedRequest), CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<PagedResponse<IdOutboxMessageDto>>>();
        result.Value.ShouldNotBeNull();
        result.Value?.Data.ShouldBeEmpty();
        result.Value?.Data.Count().ShouldBe(0);
        result.Value?.PageNumber.ShouldBe(1);
        result.Value?.PageSize.ShouldBe(pagedRequest.PageSize);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldUseDefaultPagedRequest_WhenNotProvided()
    {
        // Arrange
        var defaultPagedRequest = PagedRequest.Empty();
        var mdls = OutboxMessageDataFactory.CreateMany(defaultPagedRequest.PageSize);
        var mdlsPage = new Page<IdOutboxMessage>(mdls, defaultPagedRequest.PageNumber, defaultPagedRequest.PageSize);

        _mockSvc.GetPageAsync(defaultPagedRequest.PageNumber, defaultPagedRequest.PageSize, defaultPagedRequest.SortList, defaultPagedRequest.FilterList)
            .Returns(mdlsPage);


        // Act
        var result = await _handler.Handle(new GetOutboxMessagePageQry(null), CancellationToken.None);


        // Assert
        result.ShouldBeOfType<GenResult<PagedResponse<IdOutboxMessageDto>>>();
        result.Value.ShouldNotBeNull();
        result.Value?.Data.Count().ShouldBe(defaultPagedRequest.PageSize);
        result.Value?.Data.Count().ShouldBe(defaultPagedRequest.PageSize);
        result.Value?.PageNumber.ShouldBe(1);
        result.Value?.PageSize.ShouldBe(defaultPagedRequest.PageSize);
    }

    //------------------------------------//

}//Cls
