namespace ID.Infrastructure.Tests.DmnSevcs.Members;

using System.Threading.Tasks;
using Moq;
using Shouldly;
using System.Collections.Generic;
using ID.Infrastructure.DomainServices.Members;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Tests.Data.Factories;
using ID.Domain.Entities.AppUsers;

public class IdMemberAuditService_GetSuperMembersAsync_Tests
{
    //------------------------------------//

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

    //------------------------------------//
}
