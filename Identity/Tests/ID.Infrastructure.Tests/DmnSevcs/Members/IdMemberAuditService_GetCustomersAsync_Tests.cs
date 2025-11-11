using ID.Infrastructure.DomainServices.Members;

namespace ID.Infrastructure.Tests.DmnSevcs.Members;

public class IdMemberAuditService_GetCustomersAsync_Tests
{

    [Fact]
    public async Task GetCustomersAsync_ShouldReturnListOfCustomers()
    {
        // Arrange
        var expectedCustomers = AppUserDataFactory.CreateMany(5);

        var repoMock = new Mock<IIdentityMemberAuditRepo<AppUser>>();
        repoMock.Setup(repo => repo.GetAllCustomersAsync())
                .ReturnsAsync(expectedCustomers);

        var service = new IdMemberAuditService<AppUser>(repoMock.Object);

        // Act
        var result = await service.GetCustomersAsync();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(expectedCustomers);
    }

}
