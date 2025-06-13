using ID.Application.Features.Teams.Qry.GetMntcMembers;
using ID.Tests.Data.Factories;
using Moq;
using Pagination;
using Shouldly;
using ID.Application.Features.Teams.Qry.GetMntcMembersPage;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Members;

namespace ID.Application.Tests.Features.Teams.Qry.GetMntcMembersPage;

public class GetMntcMembersPageQryHandlerTests
{
    private readonly Mock<IIdentityMemberAuditService<AppUser>> _mbrServiceMock;
    private readonly GetMntcMembersPageQryHandler _handler;

    public GetMntcMembersPageQryHandlerTests()
    {
        _mbrServiceMock = new Mock<IIdentityMemberAuditService<AppUser>>();
        _handler = new GetMntcMembersPageQryHandler(_mbrServiceMock.Object);
    }


    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnPagedResponseOfAppUserDto()
    {
        // Arrange
        var pagedRequest = new PagedRequest(1, 10);
        var request = new GetMntcMembersPageQry(pagedRequest);
        var appUsers = AppUserDataFactory.CreateMany();
        var page = new Page<AppUser>(appUsers, 1, 10);
        _mbrServiceMock.Setup(mgr => mgr.GetMntcPageAsync(It.IsAny<PagedRequest>(), It.IsAny<int>()))
            .ReturnsAsync(page);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);


        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Data.ShouldNotBeEmpty();
        result.Value.Data.Count().ShouldNotBe(appUsers.Count);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShoulCallGetMntcPageAsync_With10000_ifIsSuper()
    {
        // Arrange
        var teamPosition = 5;
        var pagedRequest = new PagedRequest(1, 10);
        var appUsers = AppUserDataFactory.CreateMany();
        var page = new Page<AppUser>(appUsers, 1, 10);
        var request = new GetMntcMembersPageQry(pagedRequest)
        {
            IsSuper = true,
            PrincipalTeamPosition = teamPosition
        };
        _mbrServiceMock.Setup(mgr => mgr.GetMntcPageAsync(It.IsAny<PagedRequest>(), It.IsAny<int>()))
            .ReturnsAsync(page);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        _mbrServiceMock.Verify(mgr => mgr.GetMntcPageAsync(It.IsAny<PagedRequest>(), 10000));
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShoulCallGetMntcPageAsync_WithPrincipalTeamPosition_IfIsMntc()
    {
        // Arrange
        var teamPosition = 5;
        var pagedRequest = new PagedRequest(1, 10);
        var appUsers = AppUserDataFactory.CreateMany();
        var page = new Page<AppUser>(appUsers, 1, 10);
        var request = new GetMntcMembersPageQry(pagedRequest)
        {
            IsSuper = false,
            PrincipalTeamPosition = teamPosition
        };
        _mbrServiceMock.Setup(mgr => mgr.GetMntcPageAsync(It.IsAny<PagedRequest>(), It.IsAny<int>()))
            .ReturnsAsync(page);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        _mbrServiceMock.Verify(mgr => mgr.GetMntcPageAsync(It.IsAny<PagedRequest>(), teamPosition));
    }

    //------------------------------------//

}//Cls
