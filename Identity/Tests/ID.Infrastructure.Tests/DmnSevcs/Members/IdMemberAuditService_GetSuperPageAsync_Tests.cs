namespace ID.Infrastructure.Tests.DmnSevcs.Members;

using System.Threading.Tasks;
using Moq;
using Shouldly;
using Pagination;
using ID.Infrastructure.DomainServices.Members;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Tests.Data.Factories;
using ID.Domain.Entities.AppUsers;

public class IdMemberAuditService_GetSuperPageAsync_Tests
{
    //------------------------------------//

    [Fact]
    public async Task GetSuperPageAsync_WithPagedRequest_ShouldReturnPageOfSuperMembers()
    {
        // Arrange
        var pagedRequest = new PagedRequest { PageNumber = 1, PageSize = 10 };
        int maxPosition = 1000;

        var expectedMembers = AppUserDataFactory.CreateMany(10);
        var expectedPage = new Page<AppUser>(expectedMembers, pagedRequest.PageNumber, pagedRequest.PageSize);

        var repoMock = new Mock<IIdentityMemberAuditRepo<AppUser>>();
        repoMock.Setup(repo => repo.GetSuperPageAsync(pagedRequest, maxPosition))
                .ReturnsAsync(expectedPage);

        var service = new IdMemberAuditService<AppUser>(repoMock.Object);

        // Act
        var result = await service.GetSuperPageAsync(pagedRequest, maxPosition);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(expectedPage);
    }

    //------------------------------------//

    [Fact]
    public async Task GetSuperPageAsync_WithParameters_ShouldReturnPageOfSuperMembers()
    {
        // Arrange
        int maxPosition = 1000;
        int pageNumber = 1;
        int pageSize = 10;
        var sortList = new List<SortRequest>();
        IEnumerable<FilterRequest>? filterList = null;

        var expectedMembers = AppUserDataFactory.CreateMany(10);
        var expectedPage = new Page<AppUser>(expectedMembers, pageNumber, pageSize);

        var repoMock = new Mock<IIdentityMemberAuditRepo<AppUser>>();
        repoMock.Setup(repo => repo.GetSuperPageAsync(maxPosition, pageNumber, pageSize, sortList, filterList))
                .ReturnsAsync(expectedPage);

        var service = new IdMemberAuditService<AppUser>(repoMock.Object);

        // Act
        var result = await service.GetSuperPageAsync(maxPosition, pageNumber, pageSize, sortList, filterList);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(expectedPage);
    }

    //------------------------------------//
}
