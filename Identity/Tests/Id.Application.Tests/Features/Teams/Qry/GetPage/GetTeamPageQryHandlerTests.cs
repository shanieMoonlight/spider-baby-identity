using ID.Tests.Data.Factories;
using Moq;
using Pagination;
using Shouldly;
using ID.Application.Features.Teams.Qry.GetPage;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Application.Tests.Features.Teams.Qry.GetPage;

public class GetTeamPageQryHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _mbrServiceMock;
    private readonly GetTeamsPageQryHandler _handler;

    public GetTeamPageQryHandlerTests()
    {
        _mbrServiceMock = new Mock<IIdentityTeamManager<AppUser>>();
        _handler = new GetTeamsPageQryHandler(_mbrServiceMock.Object);
    }


    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnPagedResponseOfTeamDto()
    {
        // Arrange
        var pagedRequest = new PagedRequest(1, 10);
        var request = new GetTeamsPageQry(pagedRequest);
        var Teams = TeamDataFactory.CreateMany();
        var page = new Page<Team>(Teams, 1, 10);
        _mbrServiceMock.Setup(mgr => mgr.GetPageAsync(It.IsAny<PagedRequest>()))
            .ReturnsAsync(page);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);


        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Data.ShouldNotBeEmpty();
        result.Value.Data.Count().ShouldNotBe(Teams.Count);
        _mbrServiceMock.Verify(mgr => mgr.GetPageAsync(It.IsAny<PagedRequest>()));
    }

    //------------------------------------//
}//Cls
