using ID.Application.Features.FeatureFlags;
using ID.Application.Features.FeatureFlags.Qry.GetPage;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using ID.Tests.Data.Factories;
using MyResults;
using NSubstitute;
using Pagination;
using Shouldly;

namespace ID.Application.Tests.Features.FeatureFlags.Qry.GetByPage;

public class GetFeatureFlagsPageQryHandlerTests
{
    private readonly IIdentityFeatureFlagService _mockRepo;
    private readonly GetFeatureFlagsPageQryHandler _handler;

    public GetFeatureFlagsPageQryHandlerTests()
    {
        _mockRepo = Substitute.For<IIdentityFeatureFlagService>();
        _handler = new GetFeatureFlagsPageQryHandler(_mockRepo);
    }

    //- - - - - - - - - - - - - - - - - - //

    [Fact]
    public async Task Handle_ShouldReturnPagedFeatureFlags_WhenRequestIsSuccessful()
    {
        // Arrange
        var pgReq = PagedRequest.Empty();
        var mdls = FeatureFlagDataFactory.CreateMany(pgReq.PageSize);
        var mdlsPage = new Page<FeatureFlag>(mdls, pgReq.PageNumber, pgReq.PageSize);
        var dtosPage = mdlsPage.Transform((d) => d.ToDto());

        var qry = new GetFeatureFlagsPageQry(pgReq);

        PagedResponse<FeatureFlagDto> pagedResponse = new(dtosPage, pgReq);

        _mockRepo.GetPageAsync(pgReq.PageNumber, pgReq.PageSize, pgReq.SortList, pgReq.FilterList)
            .Returns(mdlsPage);

        // Act
        var result = await _handler.Handle(new GetFeatureFlagsPageQry(pgReq), CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<PagedResponse<FeatureFlagDto>>>();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeEquivalentTo(pagedResponse);

    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnEmptyPagedResponse_WhenNoFeaturesExist()
    {
        // Arrange
        var pagedRequest = PagedRequest.Empty();
        var mdlsEmptyPage = new Page<FeatureFlag>([], pagedRequest.PageNumber, pagedRequest.PageSize);
        

        _mockRepo.GetPageAsync(pagedRequest.PageNumber, pagedRequest.PageSize, pagedRequest.SortList, pagedRequest.FilterList)
            .Returns(mdlsEmptyPage);

        // Act
        var result = await _handler.Handle(new GetFeatureFlagsPageQry(pagedRequest), CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<PagedResponse<FeatureFlagDto>>>();
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
        var mdls = FeatureFlagDataFactory.CreateMany(defaultPagedRequest.PageSize);
        var mdlsPage = new Page<FeatureFlag>(mdls, defaultPagedRequest.PageNumber, defaultPagedRequest.PageSize);

        _mockRepo.GetPageAsync(defaultPagedRequest.PageNumber, defaultPagedRequest.PageSize, defaultPagedRequest.SortList, defaultPagedRequest.FilterList)
            .Returns(mdlsPage);

        // Act
        var result = await _handler.Handle(new GetFeatureFlagsPageQry(null), CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<PagedResponse<FeatureFlagDto>>>();
        result.Value.ShouldNotBeNull();
        result.Value?.Data.Count().ShouldBe(defaultPagedRequest.PageSize);
        result.Value?.Data.Count().ShouldBe(defaultPagedRequest.PageSize);
        result.Value?.PageNumber.ShouldBe(1);
        result.Value?.PageSize.ShouldBe(defaultPagedRequest.PageSize);
    }

    //------------------------------------//

}//Cls
