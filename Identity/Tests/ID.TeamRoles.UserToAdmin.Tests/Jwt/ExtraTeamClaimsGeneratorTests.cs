using ID.TeamRoles.UserToAdmin.Claims;
using ID.TeamRoles.UserToAdmin.Data;
using ID.TeamRoles.UserToAdmin.Jwt;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.TeamRoles.UserToAdmin.Tests.Jwt;

public class ExtraTeamClaimsGeneratorTests
{
    private readonly TeamRole_User_to_Mgr_ClaimsGenerator _generator;

    public ExtraTeamClaimsGeneratorTests()
    {
        _generator = new TeamRole_User_to_Mgr_ClaimsGenerator();
    }

    //------------------------------//

    [Fact]
    public void Generate_ShouldAddUserClaim_WhenUserPositionIsLessThanMinPosition()
    {
        // Arrange
        var user = AppUserDataFactory.Create(teamPosition: 0);
        var team = TeamDataFactory.Create(minPosition: 1);

        // Act
        var claims = _generator.Generate(user, team);

        // Assert
        claims.ShouldContain(c => c == IdTeamRoleClaims.USER);
    }

    //------------------------------//

    [Fact]
    public void Generate_ShouldAddAdminClaim_WhenUserPositionIsGreaterThanMaxPosition()
    {
        // Arrange
        var user = AppUserDataFactory.Create(teamPosition: 6);
        var team = TeamDataFactory.Create(maxPosition: 5);

        // Act
        var claims = _generator.Generate(user, team);

        // Assert
        claims.ShouldContain(c => c == IdTeamRoleClaims.ADMIN);
    }

    //------------------------------//

    [Fact]
    public void Generate_ShouldAddUserClaim_WhenUserPositionIsEqualToUserPosition()
    {
        // Arrange
        var user = AppUserDataFactory.Create(teamPosition: (int)TeamPositionRoles.User);
        var team = TeamDataFactory.Create(minPosition: 0, maxPosition: 5);

        // Act
        var claims = _generator.Generate(user, team);

        // Assert
        claims.ShouldContain(c => c == IdTeamRoleClaims.USER);
    }

    //------------------------------//

    [Fact]
    public void Generate_ShouldAddManagerClaim_WhenUserPositionIsEqualToManagerPosition()
    {
        // Arrange
        var user = AppUserDataFactory.Create(teamPosition: (int)TeamPositionRoles.Manager);
        var team = TeamDataFactory.Create(minPosition: 0, maxPosition: 5);

        // Act
        var claims = _generator.Generate(user, team);

        // Assert
        claims.ShouldContain(c => c == IdTeamRoleClaims.MANAGER);
    }

    //------------------------------//

    [Fact]
    public void Generate_ShouldAddAdminClaim_WhenUserPositionIsEqualToAdminPosition()
    {
        // Arrange
        var user = AppUserDataFactory.Create(teamPosition: (int)TeamPositionRoles.Admin);
        var team = TeamDataFactory.Create(minPosition: 0, maxPosition: 5);

        // Act
        var claims = _generator.Generate(user, team);

        // Assert
        claims.ShouldContain(c => c == IdTeamRoleClaims.ADMIN);
    }

}//Cls
