using ID.Domain.Entities.AppUsers;
using ID.Infrastructure.DomainServices.Members;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Tests.Data.Factories;
using Moq;
using Shouldly;

namespace ID.Infrastructure.Tests.DmnSevcs.Members;

public class IdMemberAuditService_GetMemberAsync_Tests
{
    //------------------------------------//

    [Fact]
    public async Task GetMemberAsync_ShouldReturnMember_WhenMemberExists()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var expectedMember = AppUserDataFactory.Create(teamId, memberId);

        var repoMock = new Mock<IIdentityMemberAuditRepo<AppUser>>();
        repoMock.Setup(repo => repo.GetMemberAsync(teamId, memberId))
                .ReturnsAsync(expectedMember);

        var service = new IdMemberAuditService<AppUser>(repoMock.Object);

        // Act
        var result = await service.GetMemberAsync(teamId, memberId);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(expectedMember);
    }

    //------------------------------------//

    [Fact]
    public async Task GetMemberAsync_ShouldReturnNull_WhenMemberDoesNotExist()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        var repoMock = new Mock<IIdentityMemberAuditRepo<AppUser>>();
        repoMock.Setup(repo => repo.GetMemberAsync(teamId, memberId))
                .ReturnsAsync((AppUser?)null);

        var service = new IdMemberAuditService<AppUser>(repoMock.Object);

        // Act
        var result = await service.GetMemberAsync(teamId, memberId);

        // Assert
        result.ShouldBeNull();
    }

    //------------------------------------//
}