namespace ID.Infrastructure.Tests.DmnSevcs.Members;

using ID.Domain.Abstractions.Services.Members;
using ID.Domain.Entities.AppUsers;
using ID.Infrastructure.DomainServices.Members;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.EF.Repos.Specs.Members.WithEverything;
using ID.Tests.Data.Factories;
using Moq;
using Shouldly;
using System.Threading.Tasks;

public class IdMemberAuditService_FirstByIdAsync_Tests
{
    private readonly Mock<IIdentityMemberAuditRepo<AppUser>> _repoMock;
    private readonly IIdentityMemberAuditService<AppUser> _service;

    public IdMemberAuditService_FirstByIdAsync_Tests()
    {
        _repoMock = new Mock<IIdentityMemberAuditRepo<AppUser>>();
        _service = new IdMemberAuditService<AppUser>(_repoMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task FirstByUsernameWithDetailsAsync_ShouldReturnAppUser_WhenUserExists()
    {
        // Arrange
        var username = "testuser";
        var expectedUser = AppUserDataFactory.Create(teamId: Guid.NewGuid(), userName: username);
        _repoMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<MemberByUsernameWithEverythingSpec<AppUser>>(), default))
                 .ReturnsAsync(expectedUser);

        // Act
        var result = await _service.FirstByUsernameWithTeamDetailsAsync(username);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(expectedUser);
    }

    //------------------------------------//

    [Fact]
    public async Task FirstByUsernameWithDetailsAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var username = "testuser";
        _repoMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<MemberByUsernameWithEverythingSpec<AppUser>>(), default))
                 .ReturnsAsync((AppUser?)null);

        // Act
        var result = await _service.FirstByUsernameWithTeamDetailsAsync(username);

        // Assert
        result.ShouldBeNull();
    }

    //------------------------------------//

}//Cls
