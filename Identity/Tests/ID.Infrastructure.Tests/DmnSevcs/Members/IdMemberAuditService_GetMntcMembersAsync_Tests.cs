namespace ID.Infrastructure.Tests.DmnSevcs.Members;

using ID.Domain.Entities.AppUsers;
using ID.Infrastructure.DomainServices.Members;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Tests.Data.Factories;
using Moq;
using Shouldly;
using System.Threading.Tasks;

public class IdMemberAuditService_GetMntcMembersAsync_Tests
{
    //------------------------------------//

    [Fact]
    public async Task GetMntcMembersAsync_ShouldReturnListOfMntcMembers()
    {
        // Arrange
        int maxPosition = 1000;
        var expectedMembers = AppUserDataFactory.CreateMany(5);

        var repoMock = new Mock<IIdentityMemberAuditRepo<AppUser>>();
        repoMock.Setup(repo => repo.GetAllMntcMembersAsync(maxPosition))
                .ReturnsAsync(expectedMembers);

        var service = new IdMemberAuditService<AppUser>(repoMock.Object);

        // Act
        var result = await service.GetMntcMembersAsync(maxPosition);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(expectedMembers);
    }

    //------------------------------------//

}
