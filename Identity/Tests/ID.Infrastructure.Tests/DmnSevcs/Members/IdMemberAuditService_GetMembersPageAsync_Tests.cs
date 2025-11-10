using ID.Infrastructure.DomainServices.Members;

namespace ID.Infrastructure.Tests.DmnSevcs.Members;

public class IdMemberAuditService_GetMembersPageAsync_Tests
{
    //------------------------------------//

    [Fact]
    public async Task GetMembersPageAsync_WithParameters_ShouldReturnPageOfMembers()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        int maxPosition = 10;
        int pageNumber = 1;
        int pageSize = 10;
        var sortList = new List<SortRequest>();
        var filterList = new List<FilterRequest>();

        var expectedMembers = AppUserDataFactory.CreateMany(10);
        var expectedPage = new Page<AppUser>(expectedMembers, pageNumber, pageSize);

        var repoMock = new Mock<IIdentityMemberAuditRepo<AppUser>>();
        repoMock.Setup(repo => repo.GetMembersPageAsync(teamId, maxPosition, pageNumber, pageSize, sortList, filterList))
                .ReturnsAsync(expectedPage);

        var service = new IdMemberAuditService<AppUser>(repoMock.Object);

        // Act
        var result = await service.GetMembersPageAsync(teamId, maxPosition, pageNumber, pageSize, sortList, filterList);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(expectedPage);
    }

    //------------------------------------//

    [Fact]
    public async Task GetMembersPageAsync_WithNullFilterList_ShouldReturnPageOfMembers()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        int maxPosition = 10;
        int pageNumber = 1;
        int pageSize = 10;
        var sortList = new List<SortRequest>();
        IEnumerable<FilterRequest>? filterList = null;

        var expectedMembers = AppUserDataFactory.CreateMany(10);
        var expectedPage = new Page<AppUser>(expectedMembers, pageNumber, pageSize);

        var repoMock = new Mock<IIdentityMemberAuditRepo<AppUser>>();
        repoMock.Setup(repo => repo.GetMembersPageAsync(teamId, maxPosition, pageNumber, pageSize, sortList, filterList))
                .ReturnsAsync(expectedPage);

        var service = new IdMemberAuditService<AppUser>(repoMock.Object);

        // Act
        var result = await service.GetMembersPageAsync(teamId, maxPosition, pageNumber, pageSize, sortList, filterList);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(expectedPage);
    }

    //------------------------------------//
}
