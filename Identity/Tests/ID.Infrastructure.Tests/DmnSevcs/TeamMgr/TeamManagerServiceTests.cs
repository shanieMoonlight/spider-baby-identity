using ID.Domain.Abstractions.Services.Teams.Dvcs;
using ID.Domain.Abstractions.Services.Teams.Subs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Infrastructure.DomainServices.Teams;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Tests.Data.Factories;
using Microsoft.AspNetCore.Identity;
using Moq;
using Shouldly;

namespace ID.Infrastructure.Tests.DmnSevcs.TeamMgr;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
public class TeamManagerServiceTests
{
    private readonly Mock<IIdentityTeamRepo> _teamRepoMock;
    private readonly Mock<IIdUnitOfWork> _uowMock;
    private readonly Mock<UserManager<AppUser>> _userMgrMock;
    private readonly Mock<ITeamSubscriptionServiceFactory> _subsServiceFactoryMock;
    private readonly Mock<ITeamDeviceServiceFactory> _dvcServiceFactoryMock;
    private readonly TeamManagerService<AppUser> _teamManagerService;

    public TeamManagerServiceTests()
    {
        _teamRepoMock = new Mock<IIdentityTeamRepo>();
        _uowMock = new Mock<IIdUnitOfWork>();
        _userMgrMock = new Mock<UserManager<AppUser>>(
            Mock.Of<IUserStore<AppUser>>(),
            null, null, null, null, null, null, null, null
        );
        _subsServiceFactoryMock = new Mock<ITeamSubscriptionServiceFactory>();
        _dvcServiceFactoryMock = new Mock<ITeamDeviceServiceFactory>();

        _uowMock.Setup(u => u.TeamRepo).Returns(_teamRepoMock.Object);

        _teamManagerService = new TeamManagerService<AppUser>(
            _uowMock.Object,
            _userMgrMock.Object,
            _subsServiceFactoryMock.Object,
            _dvcServiceFactoryMock.Object
        );
    }

    //------------------------------------//

    [Fact]
    public async Task GetAllTeams_ShouldReturnAllTeams_WhenIncludeMntcAndIncludeSuperAreTrue()
    {
        // Arrange
        var teams = new List<Team>
        {
           TeamDataFactory.Create(teamType: TeamType.Super),
           TeamDataFactory.Create(teamType: TeamType.Maintenance),
           TeamDataFactory.Create(teamType: TeamType.Customer),
        };
        _teamRepoMock.Setup(repo => repo.ListAllAsync(It.IsAny<AllTeamsSpec>(), CancellationToken.None)).ReturnsAsync(teams);

        // Act
        var result = await _teamManagerService.GetAllTeams(true, true);

        // Assert
        result.ShouldBe(teams);
    }

    //------------------------------------//

    [Fact]
    public async Task GetAllTeams_ShouldReturnOnlyCustomerTeams_WhenIncludeMntcAndIncludeSuperAreFalse()
    {
        // Arrange
        var teams = new List<Team>
        {
           TeamDataFactory.Create(teamType: TeamType.Super),
           TeamDataFactory.Create(teamType: TeamType.Maintenance),
           TeamDataFactory.Create(teamType: TeamType.Customer),
        };
        _teamRepoMock.Setup(repo => repo.ListAllAsync(It.IsAny<AllTeamsSpec>(), CancellationToken.None)).ReturnsAsync(teams);

        // Act
        var result = await _teamManagerService.GetAllTeams(false, false);

        // Assert
        result.ShouldBe([teams[2]]);
    }

    //------------------------------------//

    [Fact]
    public async Task GetAllTeams_ShouldReturnAllExceptSuper_WhenSuperIsFalse()
    {
        // Arrange
        var cusTeam = TeamDataFactory.Create(teamType: TeamType.Customer);
        var mntcTeam = TeamDataFactory.Create(teamType: TeamType.Maintenance);
        var superTeam = TeamDataFactory.Create(teamType: TeamType.Super);
        var teams = new List<Team>
        {
            cusTeam,
            mntcTeam,
            superTeam,
        };
        _teamRepoMock.Setup(repo => repo.ListAllAsync(It.IsAny<AllTeamsSpec>(), CancellationToken.None)).ReturnsAsync(teams);

        // Act
        var result = await _teamManagerService.GetAllTeams(true, false);

        // Assert
        result.Contains(cusTeam).ShouldBeTrue();
        result.Contains(mntcTeam).ShouldBeTrue();
        result.Contains(superTeam).ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public async Task GetAllTeams_ShouldReturnAllExceptMntc_WhenMntcIsFalse()
    {
        // Arrange
        var cusTeam = TeamDataFactory.Create(teamType: TeamType.Customer);
        var mntcTeam = TeamDataFactory.Create(teamType: TeamType.Maintenance);
        var superTeam = TeamDataFactory.Create(teamType: TeamType.Super);
        var teams = new List<Team>
        {
            cusTeam,
            mntcTeam,
            superTeam,
        };
        _teamRepoMock.Setup(repo => repo.ListAllAsync(It.IsAny<AllTeamsSpec>(), CancellationToken.None)).ReturnsAsync(teams);

        // Act
        var result = await _teamManagerService.GetAllTeams(false, true);

        // Assert
        result.Contains(cusTeam).ShouldBeTrue();
        result.Contains(mntcTeam).ShouldBeFalse();
        result.Contains(superTeam).ShouldBeTrue();
    }

    //------------------------------------//

}
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.