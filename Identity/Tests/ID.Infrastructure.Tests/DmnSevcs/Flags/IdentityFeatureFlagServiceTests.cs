using ClArch.SimpleSpecification;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using ID.Infrastructure.DomainServices.SubPlans;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.Abstractions.Repos.Specs;
using ID.Tests.Data.Factories;
using Moq;
using Pagination;
using Shouldly;

namespace ID.Infrastructure.Tests.DmnSevcs.Flags;

public class IdentityFeatureFlagServiceTests
{
    private readonly Mock<IIdentityFeatureFlagRepo> _mockRepo;
    private readonly IdentityFeatureFlagService _service;

    public IdentityFeatureFlagServiceTests()
    {
        _mockRepo = new Mock<IIdentityFeatureFlagRepo>();
        _service = new IdentityFeatureFlagService(_mockRepo.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldCallRepoAddAsync()
    {
        var featureFlag = FeatureFlagDataFactory.Create();
        await _service.AddAsync(featureFlag);
        _mockRepo.Verify(repo => repo.AddAsync(featureFlag, It.IsAny<CancellationToken>()), Times.Once);
    }

    //- - - - - - - - - - - - - - - - - - //

    [Fact]
    public async Task AddRangeAsync_ShouldCallRepoAddRangeAsync()
    {
        var featureFlags = FeatureFlagDataFactory.CreateMany(2);
        await _service.AddRangeAsync(featureFlags);
        _mockRepo.Verify(repo => repo.AddRangeAsync(featureFlags, It.IsAny<CancellationToken>()), Times.Once);
    }

    //- - - - - - - - - - - - - - - - - - //

    [Fact]
    public async Task Count_ShouldCallRepoCount()
    {
        _mockRepo.Setup(repo => repo.CountAsync()).ReturnsAsync(5);
        var count = await _service.CountAsync();
        count.ShouldBe(5);
    }

    //- - - - - - - - - - - - - - - - - - //

    [Fact]
    public async Task DeleteAsync_ByEntity_ShouldCallRepoDeleteAsync()
    {
        var featureFlag = FeatureFlagDataFactory.Create();
        await _service.DeleteAsync(featureFlag);
        _mockRepo.Verify(repo => repo.DeleteAsync(featureFlag), Times.Once);
    }

    //- - - - - - - - - - - - - - - - - - //

    [Fact]
    public async Task DeleteAsync_ById_ShouldCallRepoDeleteAsync()
    {
        var id = Guid.NewGuid();
        await _service.DeleteAsync(id);
        _mockRepo.Verify(repo => repo.DeleteAsync(id), Times.Once);
    }
    //- - - - - - - - - - - - - - - - - - //

    [Fact]
    public async Task ExistsAsync_ShouldCallRepoExistsAsync()
    {
        var id = Guid.NewGuid();
        _mockRepo.Setup(repo => repo.ExistsAsync(id)).ReturnsAsync(true);
        var exists = await _service.ExistsAsync(id);
        exists.ShouldBeTrue();
    }

    //- - - - - - - - - - - - - - - - - - //

    [Fact]
    public async Task GetAllAsync_ShouldCallRepoGetAllAsync()
    {
        var featureFlags = FeatureFlagDataFactory.CreateMany(2);
        _mockRepo.Setup(repo => repo.ListAllAsync()).ReturnsAsync(featureFlags);
        var result = await _service.GetAllAsync();
        result.ShouldBe(featureFlags);
    }

    //- - - - - - - - - - - - - - - - - - //


    [Fact]
    public async Task GetByIdAsync_ShouldCallRepoGetByIdAsync()
    {
        var id = Guid.NewGuid();
        var featureFlag = FeatureFlagDataFactory.Create(id: id);
        //_mockRepo.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(featureFlag); //new GetByIdSpec<FeatureFlag>(id)
        _mockRepo.Setup(repo => repo.FirstOrDefaultAsync(
         It.Is<GetByIdSpec<FeatureFlag>>(spec => spec.TESTING_GetCriteria().Compile().Invoke(featureFlag)),
        It.IsAny<CancellationToken>()
    )).ReturnsAsync(featureFlag);
        var result = await _service.GetByIdAsync(id, default);
        result.ShouldBe(featureFlag);
    }
    //- - - - - - - - - - - - - - - - - - //


    [Fact]
    public async Task GetByNameAsync_ShouldCallRepoGetByNameAsync()
    {
        var name = "FeatureFlagName";
        var featureFlag = FeatureFlagDataFactory.Create();
        _mockRepo.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<FlagByNameSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(featureFlag);
        var result = await _service.GetByNameAsync(name, default);
        result.ShouldBe(featureFlag);
    }
    //- - - - - - - - - - - - - - - - - - //


    [Fact]
    public async Task GetPageAsync_ByRequest_ShouldCallRepoGetPageAsync()
    {
        var request = new PagedRequest();
        var page = new Page<FeatureFlag>();
        _mockRepo.Setup(repo => repo.PageAsync(request)).ReturnsAsync(page);
        var result = await _service.GetPageAsync(request);
        result.ShouldBe(page);
    }
    //- - - - - - - - - - - - - - - - - - //

    [Fact]
    public async Task GetPageAsync_ByParams_ShouldCallRepoGetPageAsync()
    {
        var pageNumber = 1;
        var pageSize = 10;
        var sortList = new List<SortRequest> { new() };
        var filterList = new List<FilterRequest> { new() };
        var page = new Page<FeatureFlag>();
        _mockRepo.Setup(repo => repo.PageAsync(pageNumber, pageSize, sortList, filterList)).ReturnsAsync(page);
        var result = await _service.GetPageAsync(pageNumber, pageSize, sortList, filterList);
        result.ShouldBe(page);
    }
    //- - - - - - - - - - - - - - - - - - //

    [Fact]
    public async Task UpdateAsync_ShouldCallRepoUpdateAsync()
    {
        var featureFlag = FeatureFlagDataFactory.Create();
        await _service.UpdateAsync(featureFlag);
        _mockRepo.Verify(repo => repo.UpdateAsync(featureFlag), Times.Once);
    }
    //- - - - - - - - - - - - - - - - - - //

    [Fact]
    public async Task UpdateRangeAsync_ShouldCallRepoUpdateRangeAsync()
    {
        var featureFlags = FeatureFlagDataFactory.CreateMany(2);
        await _service.UpdateRangeAsync(featureFlags);
        _mockRepo.Verify(repo => repo.UpdateRangeAsync(featureFlags), Times.Once);
    }

}//Cls
