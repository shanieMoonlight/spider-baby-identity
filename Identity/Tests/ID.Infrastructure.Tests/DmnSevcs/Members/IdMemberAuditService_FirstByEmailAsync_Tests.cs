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

public class IdMemberAuditService_FirstByEmailAsync_Tests
{
    private readonly Mock<IIdentityMemberAuditRepo<AppUser>> _repoMock;
    private readonly IIdentityMemberAuditService<AppUser> _service;

    public IdMemberAuditService_FirstByEmailAsync_Tests()
    {
        _repoMock = new Mock<IIdentityMemberAuditRepo<AppUser>>();
        _service = new IdMemberAuditService<AppUser>(_repoMock.Object);
    }

    //------------------------------------//


    [Fact]
    public async Task FirstByEmailWithDetailsAsync_ShouldReturnAppUser_WhenUserExists()
    {
        // Arrange
        var email = "test@example.com";
        var expectedUser = AppUserDataFactory.Create(teamId: Guid.NewGuid(), email: email);
        _repoMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<MemberByEmailWithEverythingSpec<AppUser>>(), default))
                 .ReturnsAsync(expectedUser);

        // Act
        var result = await _service.FirstByEmailWithTeamDetailsAsync(email);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(expectedUser);
    }

    //------------------------------------//

    [Fact]
    public async Task FirstByEmailWithDetailsAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var email = "test@example.com";
        _repoMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<MemberByEmailWithEverythingSpec<AppUser>>(), default))
                 .ReturnsAsync((AppUser?)null);

        // Act
        var result = await _service.FirstByEmailWithTeamDetailsAsync(email);

        // Assert
        result.ShouldBeNull();
    }

    //------------------------------------//

}//Cls
