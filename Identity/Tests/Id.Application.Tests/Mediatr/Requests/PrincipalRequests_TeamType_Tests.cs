using ID.Application.Mediatr.Cqrslmps;
using ID.Domain.Entities.Teams;

namespace ID.Application.Tests.Mediatr.Requests;

public class PrincipalRequests_TeamType_Tests
{
    [Fact]
    public void TeamType_ShouldReturnMaintenance_WhenIsMntcIsTrue()
    {
        // Arrange
        var request = new APrincipalInfoRequest { IsMntc = true };

        // Act
        var teamType = request.TeamType;

        // Assert
        Assert.Equal(TeamType.Maintenance, teamType);
    }

    //- - - - - - - - - - - - - - - - - - - - - - -//

    [Fact]
    public void TeamType_ShouldReturnSuper_WhenIsSuperIsTrue()
    {
        // Arrange
        var request = new APrincipalInfoRequest { IsSuper = true };

        // Act
        var teamType = request.TeamType;

        // Assert
        Assert.Equal(TeamType.Super, teamType);
    }

    //- - - - - - - - - - - - - - - - - - - - - - -//

    [Fact]
    public void TeamType_ShouldReturnCustomer_WhenNeitherIsMntcNorIsSuperIsTrue()
    {
        // Arrange
        var request = new APrincipalInfoRequest { IsMntc = false, IsSuper = false };

        // Act
        var teamType = request.TeamType;

        // Assert
        Assert.Equal(TeamType.Customer, teamType);
    }

    //---------------------------------------------//

}//Cls