using ID.Infrastructure.DomainServices.Members;

namespace ID.Infrastructure.Tests.DmnSevcs.Members;

public class IdMemberAuditService_GetSuperMembersAsync_Tests
{

    [Fact]
    public async Task GetSuperMembersAsync_ShouldReturnListOfSuperMembers()
    {
        // Arrange
        int maxPosition = 1000;
        var expectedMembers = AppUserDataFactory.CreateMany(5);

        var repoMock = new Mock<IIdentityMemberAuditRepo<AppUser>>();
        repoMock.Setup(repo => repo.GetAllSuperMembersAsync(maxPosition))
                .ReturnsAsync(expectedMembers);

        var service = new IdMemberAuditService<AppUser>(repoMock.Object);

        // Act
        var result = await service.GetSuperMembersAsync(maxPosition);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(expectedMembers);
    }

}
