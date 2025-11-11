using ID.Infrastructure.DomainServices.Members;

namespace ID.Infrastructure.Tests.DmnSevcs.Members;

public class IdMemberAuditService_GetMembersPageAsync_Overload_Tests
{
    //------------------------------------//

    [Fact]
    public async Task GetMembersPageAsync_WithPagedRequest_ShouldReturnPageOfMembers()
    {
        // Arrange
        var pagedRequest = new PagedRequest { PageNumber = 1, PageSize = 10 };
        var teamId = Guid.NewGuid();
        int maxPosition = 10;

        var expectedMembers = AppUserDataFactory.CreateMany(10);
        var expectedPage = new Page<AppUser>(expectedMembers, pagedRequest.PageNumber, pagedRequest.PageSize);

        var repoMock = new Mock<IIdentityMemberAuditRepo<AppUser>>();
        repoMock.Setup(repo => repo.GetMembersPageAsync(pagedRequest, teamId, maxPosition))
                .ReturnsAsync(expectedPage);

        var service = new IdMemberAuditService<AppUser>(repoMock.Object);

        // Act
        var result = await service.GetMembersPageAsync(pagedRequest, teamId, maxPosition);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(expectedPage);
    }

}
