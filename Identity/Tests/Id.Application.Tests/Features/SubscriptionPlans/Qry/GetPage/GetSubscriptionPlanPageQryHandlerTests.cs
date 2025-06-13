using ID.Application.Features.SubscriptionPlans;
using ID.Application.Features.SubscriptionPlans.Qry.GetPage;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Entities.SubscriptionPlans;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Pagination;
using Shouldly;

namespace ID.Application.Tests.Features.SubscriptionPlans.Qry.GetPage;

public class GetSubscriptionPlansPageQryHandlerTests
{
    private readonly Mock<IIdentitySubscriptionPlanService> _mockRepo;
    private readonly GetSubscriptionPlansPageQryHandler _handler;

    public GetSubscriptionPlansPageQryHandlerTests()
    {
        _mockRepo = new Mock<IIdentitySubscriptionPlanService>();
        _handler = new GetSubscriptionPlansPageQryHandler(_mockRepo.Object);
    }

    //- - - - - - - - - - - - - - - - - - //

    [Fact]
    public async Task Handle_ShouldReturnPagedSubscriptionPlans_WhenRequestIsSuccessful()
    {
        // Arrange
        var pgReq = PagedRequest.Empty();
        var mdls = SubscriptionPlanDataFactory.CreateMany(pgReq.PageSize);
        var mdlsPage = new Page<SubscriptionPlan>(mdls, pgReq.PageNumber, pgReq.PageSize);
        var dtosPage = mdlsPage.Transform((d) => d.ToDto());

        var qry = new GetSubscriptionPlansPageQry(pgReq);

        PagedResponse<SubscriptionPlanDto> pagedResponse = new(dtosPage, pgReq);

        _mockRepo.Setup(repo => repo.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IEnumerable<SortRequest>>(), It.IsAny<IEnumerable<FilterRequest>?>()))
            .ReturnsAsync(mdlsPage);

        // Act
        var result = await _handler.Handle(new GetSubscriptionPlansPageQry(pgReq), CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<PagedResponse<SubscriptionPlanDto>>>();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeEquivalentTo(pagedResponse);

    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnEmptyPagedResponse_WhenNoPlansExist()
    {
        // Arrange
        var pagedRequest = PagedRequest.Empty();
        var mdlsEmptyPage = new Page<SubscriptionPlan>([], pagedRequest.PageNumber, pagedRequest.PageSize);
        var qry = new GetSubscriptionPlansPageQry(pagedRequest);

        _mockRepo.Setup(repo => repo.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IEnumerable<SortRequest>>(), It.IsAny<IEnumerable<FilterRequest>?>()))
            .ReturnsAsync(mdlsEmptyPage);


        // Act
        var result = await _handler.Handle(new GetSubscriptionPlansPageQry(pagedRequest), CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<PagedResponse<SubscriptionPlanDto>>>();
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
        var emptyPagedRequest = PagedRequest.Empty();
        var emptyResponsePage = new Page<SubscriptionPlan>([], emptyPagedRequest.PageNumber, emptyPagedRequest.PageSize);

        _mockRepo.Setup(repo => repo.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IEnumerable<SortRequest>>(), It.IsAny<IEnumerable<FilterRequest>?>()))
            .ReturnsAsync(emptyResponsePage);

        // Act
        var result = await _handler.Handle(new GetSubscriptionPlansPageQry(null), CancellationToken.None);

        // Assert
        _mockRepo.Verify(m => m.GetPageAsync(emptyPagedRequest.PageNumber, emptyPagedRequest.PageSize, emptyPagedRequest.SortList, emptyPagedRequest.FilterList), Times.Once);
    }

    //------------------------------------//

}//Cls
