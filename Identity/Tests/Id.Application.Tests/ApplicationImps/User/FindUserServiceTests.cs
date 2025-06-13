using ID.Application.AppImps.User;
using ID.Domain.Abstractions.Services.Members;
using ID.Domain.Entities.AppUsers;
using ID.Tests.Data.Factories;
using Moq;
using Shouldly;

namespace ID.Application.Tests.ApplicationImps.Services;
public class FindUserServiceTests
{


    [Fact]
    public async Task Should_ReturnNull_When_NoDataSuppliedAsync()
    {
        //Arrange
        var _mockUserMgmt = new Mock<IIdentityMemberAuditService<AppUser>>();
        _mockUserMgmt.Setup(r => r.FirstByEmailWithTeamDetailsAsync(It.IsAny<string?>()))
            .ReturnsAsync(AppUserDataFactory.Create());
        _mockUserMgmt.Setup(r => r.FirstByUsernameWithTeamDetailsAsync(It.IsAny<string?>()))
            .ReturnsAsync(AppUserDataFactory.Create());
        _mockUserMgmt.Setup(r => r.FirstByIdWithTeamDetailsAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(AppUserDataFactory.Create());

        var findService = new FindUserService<AppUser>(_mockUserMgmt.Object);


        //Act
        var result = await findService.FindUserWithTeamDetailsAsync(null, null, null);


        //Assert
        result.ShouldBeNull();

    }

    //------------------------------------//


    [Fact]
    public async Task Should_CallFindByNameAsyncWhenFindByEmailFails()
    {
        //Arrange
        var _mockUserMgmt = new Mock<IIdentityMemberAuditService<AppUser>>();
        _mockUserMgmt.Setup(r => r.FirstByEmailWithTeamDetailsAsync(It.IsAny<string?>()))
            .ReturnsAsync((string? name) => null);
        _mockUserMgmt.Setup(r => r.FirstByUsernameWithTeamDetailsAsync(It.IsAny<string?>()))
            .ReturnsAsync(AppUserDataFactory.Create());
        _mockUserMgmt.Setup(r => r.FirstByIdWithTeamDetailsAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(AppUserDataFactory.Create());

        var findService = new FindUserService<AppUser>(_mockUserMgmt.Object);


        //Act
        var result = await findService.FindUserWithTeamDetailsAsync("notnull", "notnull", null);


        //Assert
        _mockUserMgmt.Verify(x =>
                   x.FirstByUsernameWithTeamDetailsAsync(It.IsAny<string?>()),
                   Times.Once
               );

    }

    //------------------------------------//


    [Fact]
    public async Task Should_AssignFindByNameValueToUser()
    {
        //Arrange

        var testUser = AppUserDataFactory.Create();

        var _mockUserMgmt = new Mock<IIdentityMemberAuditService<AppUser>>();
        _mockUserMgmt.Setup(r => r.FirstByEmailWithTeamDetailsAsync(It.IsAny<string?>()))
            .ReturnsAsync((string? name) => null);
        _mockUserMgmt.Setup(r => r.FirstByUsernameWithTeamDetailsAsync(It.IsAny<string?>()))
            .ReturnsAsync(testUser);
        _mockUserMgmt.Setup(r => r.FirstByIdWithTeamDetailsAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(testUser);

        var findService = new FindUserService<AppUser>(_mockUserMgmt.Object);


        //Act
        var result = await findService.FindUserWithTeamDetailsAsync("notnull", "notnull", null);


        //Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(testUser.Id);
    }

    //------------------------------------//


    [Fact]
    public async Task Should_CallFindByIdWithDetailsAsyncWhenFindByEmailFailsAndFindByNameFailsAndIdWasSupplied()
    {
        //Arrange
        var testUser = AppUserDataFactory.Create();

        var _mockUserMgmt = new Mock<IIdentityMemberAuditService<AppUser>>();
        _mockUserMgmt.Setup(r => r.FirstByEmailWithTeamDetailsAsync(It.IsAny<string?>()))
            .ReturnsAsync((string? name) => null);
        _mockUserMgmt.Setup(r => r.FirstByUsernameWithTeamDetailsAsync(It.IsAny<string?>()))
            .ReturnsAsync((string? name) => null);
        _mockUserMgmt.Setup(r => r.FirstByIdWithTeamDetailsAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(testUser);

        var findService = new FindUserService<AppUser>(_mockUserMgmt.Object);


        //Act
        var result = await findService.FindUserWithTeamDetailsAsync("notnull", "notnull", new Guid());


        //Assert
        _mockUserMgmt.Verify(x =>
                   x.FirstByIdWithTeamDetailsAsync(It.IsAny<Guid?>()),
                   Times.Once
               );

    }

    //------------------------------------//


    [Fact]
    public async Task Should_AssignFindByIdValueToUserWhenFindByEmailFailsAndFindByNamelFailsAndIdWasSupplied()
    {
        //Arrange
        var testUser = AppUserDataFactory.Create();

        var _mockUserMgmt = new Mock<IIdentityMemberAuditService<AppUser>>();
        _mockUserMgmt.Setup(r => r.FirstByEmailWithTeamDetailsAsync(It.IsAny<string?>()))
            .ReturnsAsync((string? name) => null);
        _mockUserMgmt.Setup(r => r.FirstByUsernameWithTeamDetailsAsync(It.IsAny<string?>()))
            .ReturnsAsync((string? name) => null);
        _mockUserMgmt.Setup(r => r.FirstByIdWithTeamDetailsAsync(It.IsAny<Guid?>()))
            .ReturnsAsync((Guid? id) => testUser);

        var findService = new FindUserService<AppUser>(_mockUserMgmt.Object);


        //Act
        var result = await findService.FindUserWithTeamDetailsAsync("notnull", "notnull", new Guid());


        //Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(testUser.Id);

    }

    //------------------------------------//
}
