using ID.Application.Features.Teams.Qry.GetSprMembers;
using ID.Tests.Data.Factories;
using Moq;
using Pagination;
using Shouldly;
using ID.Application.Features.Teams.Qry.GetSprMembersPage;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Members;

namespace ID.Application.Tests.Features.Teams.Qry.GetSprMembersPage;

public class GetSprMembersPageQryHandlerTests
{
    private readonly Mock<IIdentityMemberAuditService<AppUser>> _mbrServiceMock;
    private readonly GetSprMembersPageQryHandler _handler;

    public GetSprMembersPageQryHandlerTests()
    {
        _mbrServiceMock = new Mock<IIdentityMemberAuditService<AppUser>>();
        _handler = new GetSprMembersPageQryHandler(_mbrServiceMock.Object);
    }


    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnPagedResponseOfAppUserDto()
    {
        // Arrange
        var pagedRequest = new PagedRequest(1, 10);
        var request = new GetSprMembersPageQry(pagedRequest);
        var appUsers = AppUserDataFactory.CreateMany();
        var page = new Page<AppUser>(appUsers, 1, 10);
        _mbrServiceMock.Setup(mgr => mgr.GetSuperPageAsync(It.IsAny<PagedRequest>(), It.IsAny<int>()))
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
    public async Task Handle_ShoulCallGetSuperPageAsync_WithPrincipalTeamPosition()
    {
        // Arrange
        var teamPosition = 5;
        var pagedRequest = new PagedRequest(1, 10);
        var appUsers = AppUserDataFactory.CreateMany();
        var page = new Page<AppUser>(appUsers, 1, 10);
        var request = new GetSprMembersPageQry(pagedRequest)
        {
            IsSuper = false,
            PrincipalTeamPosition = teamPosition
        };
        _mbrServiceMock.Setup(mgr => mgr.GetSuperPageAsync(It.IsAny<PagedRequest>(), It.IsAny<int>()))
            .ReturnsAsync(page);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        _mbrServiceMock.Verify(mgr => mgr.GetSuperPageAsync(It.IsAny<PagedRequest>(), teamPosition));
    }

    //------------------------------------//

}//Cls
