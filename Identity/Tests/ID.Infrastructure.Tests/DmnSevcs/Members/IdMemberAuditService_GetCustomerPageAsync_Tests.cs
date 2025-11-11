using ID.Infrastructure.DomainServices.Members;

namespace ID.Infrastructure.Tests.DmnSevcs.Members;


public class IdMemberAuditService_GetCustomerPageAsync_Tests
{

    [Fact]
    public async Task GetCustomerPageAsync_WithPagedRequest_ShouldReturnPageOfCustomers()
    {
        // Arrange
        var pagedRequest = new PagedRequest { PageNumber = 1, PageSize = 10 };
        var expectedCustomers = AppUserDataFactory.CreateMany(10);
        var expectedPage = new Page<AppUser>(expectedCustomers, 1, 10);

        var repoMock = new Mock<IIdentityMemberAuditRepo<AppUser>>();
        repoMock.Setup(repo => repo.GetCustomerPageAsync(pagedRequest))
                .ReturnsAsync(expectedPage);

        var service = new IdMemberAuditService<AppUser>(repoMock.Object);

        // Act
        var result = await service.GetCustomerPageAsync(pagedRequest);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(expectedPage);
    }


}//Cls
